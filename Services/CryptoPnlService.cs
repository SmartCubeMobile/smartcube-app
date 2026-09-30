using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    // Profit & loss for the crypto portfolio as a whole.
    //
    // Moving coins between your own wallets and exchanges (e.g. buy on Coinbase, send to a hardware
    // wallet) is not a sale or a purchase, so both legs of such a transfer are left out of P&L. That's
    // also why P&L is only shown for the portfolio (per coin and in total), never per wallet: a wallet
    // that only ever received coins from your exchange has no cost of its own.
    //
    //   cost      = value at the time of every Buy / Receive from outside your portfolio
    //   proceeds  = value at the time of every Sell / Send / Withdraw to outside your portfolio
    //   P&L       = value held now + proceeds - cost
    // Transactions the user has excluded (CryptoExclusions) don't count either.
    public static class CryptoPnlService
    {
        public class TransferLeg
        {
            public string OtherSide { get; set; }   // wallet/exchange label (or "your address") at the other end
            public bool Outgoing { get; set; }
        }

        public class PnlResult
        {
            public decimal CurrentValue { get; set; }
            public decimal Cost { get; set; }
            public decimal Proceeds { get; set; }
            public decimal AcquiredQty { get; set; }
            public decimal Pnl => CurrentValue + Proceeds - Cost;
            public decimal PnlPct => Cost > 0 ? Pnl / Cost * 100 : 0;
            public decimal AvgCost => AcquiredQty > 0 ? Cost / AcquiredQty : 0;
            public bool HasCost => Cost > 0;
            public int TransferCount { get; set; }
        }

        private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["WETH"] = "ETH", ["WBTC"] = "BTC",
        };
        private static string Norm(string s) => Aliases.TryGetValue(s ?? "", out var a) ? a : (s ?? "").ToUpperInvariant();

        public static bool IsOut(MockCryptoTransaction t) => t.Type is "Send" or "Withdraw";
        public static bool IsIn(MockCryptoTransaction t) => t.Type == "Receive";

        private static string SourceOf(MockCryptoHolding h) => h.WalletLabel ?? $"{h.Symbol} Wallet";

        // Finds both legs of transfers between the user's own wallets and exchanges.
        //   1. A send from one source matched to a receive of the same coin in another source: same hash,
        //      or received within 72 hours for 95-100% of the amount (the rest is network fees).
        //   2. A send to, or a receive from, one of the user's own wallet addresses, even when the other
        //      side's history isn't loaded.
        public static Dictionary<MockCryptoTransaction, TransferLeg> FindTransfers(IEnumerable<MockCryptoHolding> holdings)
        {
            var legs = new Dictionary<MockCryptoTransaction, TransferLeg>(ReferenceEqualityComparer.Instance);
            var all = (holdings ?? Enumerable.Empty<MockCryptoHolding>())
                .Where(h => h.Transactions != null)
                .SelectMany(h => h.Transactions.Select(t => (Tx: t, Source: SourceOf(h))))
                .Where(x => !CryptoExclusions.IsExcluded(x.Tx))
                .ToList();

            var own = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (var c in CryptoStorageService.LoadConnections())
                    if (c.Type == "wallet" && c.Symbol != "LABEL" && !string.IsNullOrEmpty(c.Address))
                        own[c.Address] = c.Label ?? $"{c.Symbol} Wallet";
            }
            catch { }

            var ins = all.Where(x => IsIn(x.Tx)).ToList();
            foreach (var send in all.Where(x => IsOut(x.Tx)).OrderBy(x => x.Tx.Date))
            {
                var sym = Norm(send.Tx.Symbol);
                var match = ins
                    .Where(r => !legs.ContainsKey(r.Tx) && r.Source != send.Source && Norm(r.Tx.Symbol) == sym)
                    .Where(r => SameHash(send.Tx, r.Tx) || (
                        (r.Tx.Date - send.Tx.Date).TotalHours is >= -2 and <= 72
                        && r.Tx.Quantity <= send.Tx.Quantity * 1.001m
                        && r.Tx.Quantity >= send.Tx.Quantity * 0.95m))
                    .OrderByDescending(r => SameHash(send.Tx, r.Tx))
                    .ThenBy(r => Math.Abs((r.Tx.Date - send.Tx.Date).TotalMinutes))
                    .FirstOrDefault();

                if (match.Tx != null)
                {
                    legs[send.Tx] = new TransferLeg { OtherSide = match.Source, Outgoing = true };
                    legs[match.Tx] = new TransferLeg { OtherSide = send.Source, Outgoing = false };
                }
                else if (!string.IsNullOrEmpty(send.Tx.ToAddress) && own.TryGetValue(send.Tx.ToAddress, out var toLabel) && toLabel != send.Source)
                {
                    legs[send.Tx] = new TransferLeg { OtherSide = toLabel, Outgoing = true };
                }
            }

            foreach (var r in ins.Where(r => !legs.ContainsKey(r.Tx)))
                if (!string.IsNullOrEmpty(r.Tx.FromAddress) && own.TryGetValue(r.Tx.FromAddress, out var fromLabel) && fromLabel != r.Source)
                    legs[r.Tx] = new TransferLeg { OtherSide = fromLabel, Outgoing = false };

            return legs;
        }

        private static bool SameHash(MockCryptoTransaction a, MockCryptoTransaction b)
        {
            if (string.IsNullOrEmpty(a.Hash) || string.IsNullOrEmpty(b.Hash)) return false;
            // Wallet hashes are stored shortened ("0x12ab34cd...ef01"); exchanges keep the full hash.
            static string Ends(string h) => h.Replace("...", "").ToLowerInvariant();
            var x = Ends(a.Hash); var y = Ends(b.Hash);
            if (x == y) return true;
            return x.Length >= 12 && y.Length >= 12 && x[..8] == y[..8] && x[^4..] == y[^4..];
        }

        // P&L for a set of holdings (one coin across every source, or the whole portfolio).
        // transfers should come from FindTransfers over the whole portfolio.
        public static PnlResult Calculate(IEnumerable<MockCryptoHolding> holdings, Dictionary<MockCryptoTransaction, TransferLeg> transfers)
        {
            var result = new PnlResult();
            foreach (var h in holdings ?? Enumerable.Empty<MockCryptoHolding>())
            {
                result.CurrentValue += h.Quantity * h.PriceGBP;
                var txs = h.Transactions ?? new List<MockCryptoTransaction>();
                if (txs.Count == 0)
                {
                    // No history (added by hand): fall back to the average cost entered for it.
                    result.Cost += h.Quantity * h.AvgCostBasis;
                    result.AcquiredQty += h.AvgCostBasis > 0 ? h.Quantity : 0;
                    continue;
                }
                foreach (var t in txs)
                {
                    if (CryptoExclusions.IsExcluded(t)) continue;
                    if (transfers != null && transfers.ContainsKey(t)) { result.TransferCount++; continue; }
                    var value = t.PriceAtTime > 0 ? t.Quantity * t.PriceAtTime : 0;
                    if (t.Type is "Buy" or "Receive") { result.Cost += value; if (value > 0) result.AcquiredQty += t.Quantity; }
                    else if (t.Type is "Sell" or "Send" or "Withdraw") result.Proceeds += value;
                }
            }
            return result;
        }

        public static PnlResult Portfolio(IEnumerable<MockCryptoHolding> holdings)
        {
            var list = holdings?.ToList() ?? new();
            return Calculate(list, FindTransfers(list));
        }
    }
}
