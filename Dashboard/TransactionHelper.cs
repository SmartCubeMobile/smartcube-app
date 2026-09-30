using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard
{
    public class AddressInfo
    {
        public string Display { get; set; }
        public string Color { get; set; }
        public bool CanClaim { get; set; }
        public string FullAddress { get; set; }
    }

    public static class TransactionHelper
    {
        private static readonly string[] LinkColors = new[]
        {
            "#F59E0B", "#3B82F6", "#A855F7", "#22C55E", "#EC4899",
            "#06B6D4", "#F97316", "#84CC16", "#E879F9", "#FB923C",
        };

        private static readonly string[] LinkBgColors = new[]
        {
            "#1A2A40", "#1A2040", "#201A40", "#1A3020", "#301A28",
            "#1A2830", "#2A1A10", "#1A2810", "#281A30", "#2A1A18",
        };

        public static Dictionary<string, string> BuildAddressMap()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var connections = CryptoStorageService.LoadConnections();
            foreach (var c in connections)
            {
                if (c.Type == "wallet" && !string.IsNullOrEmpty(c.Address))
                {
                    var label = c.Label ?? $"{c.Symbol} Wallet";
                    map[c.Address] = label;
                }
                else if (c.Type == "exchange")
                {
                    var label = c.Label ?? c.ExchangeName ?? "Exchange";
                    map[$"_exchange_{label}"] = label;
                }
            }
            return map;
        }

        private static readonly HashSet<string> ExchangeResources = new(StringComparer.OrdinalIgnoreCase)
        {
            "coinbase_account", "coinbase", "coinbase_pro", "exchange",
        };

        public static AddressInfo ResolveAddress(string address, Dictionary<string, string> addressMap, string exchangeLabel = null)
        {
            if (string.IsNullOrEmpty(address) || address == "—")
                return new AddressInfo { Display = "—", Color = "#64748B", CanClaim = false, FullAddress = address };

            if (ExchangeResources.Contains(address))
            {
                var exLabel = exchangeLabel ?? "Exchange";
                return new AddressInfo
                {
                    Display = $"{exLabel} NODE",
                    Color = "#F59E0B",
                    CanClaim = false,
                    FullAddress = address,
                };
            }

            if (addressMap.TryGetValue(address, out var label))
                return new AddressInfo
                {
                    Display = label,
                    Color = "#22C55E",
                    CanClaim = false,
                    FullAddress = address,
                };

            var truncated = address.Length > 16 ? address[..8] + "..." + address[^4..] : address;
            return new AddressInfo
            {
                Display = truncated,
                Color = "#94A3B8",
                CanClaim = true,
                FullAddress = address,
            };
        }

        public static AddressInfo ResolveWithSource(string address, string txType, bool isFrom, string sourceLabel, Dictionary<string, string> addressMap, string exchangeLabel = null)
        {
            if (string.IsNullOrEmpty(address) || address == "—")
            {
                var shouldInfer = isFrom
                    ? (txType == "Send" || txType == "Sell" || txType == "Withdraw" || txType == "Swap")
                    : (txType == "Receive" || txType == "Buy");

                if (shouldInfer && !string.IsNullOrEmpty(sourceLabel))
                    return new AddressInfo { Display = sourceLabel, Color = "#22C55E", CanClaim = false, FullAddress = address ?? "—" };
            }
            return ResolveAddress(address, addressMap, exchangeLabel);
        }

        public static Dictionary<MockCryptoTransaction, int> FindLinkedGroups(List<MockCryptoTransaction> txs)
        {
            var groups = new Dictionary<MockCryptoTransaction, int>();
            var sends = txs.Where(t => t.Type == "Send" || t.Type == "Sell" || t.Type == "Withdraw").ToList();
            var receives = txs.Where(t => t.Type == "Receive" || t.Type == "Buy").ToList();
            int groupIndex = 0;

            foreach (var send in sends)
            {
                foreach (var recv in receives)
                {
                    if (recv.Symbol != send.Symbol) continue;
                    if (groups.ContainsKey(recv)) continue;
                    var timeDiff = Math.Abs((send.Date - recv.Date).TotalMinutes);
                    if (timeDiff > 60) continue;
                    var diff = Math.Abs(send.Quantity - recv.Quantity);
                    var tolerance = send.Quantity * 0.05m;
                    if (diff <= tolerance)
                    {
                        groups[send] = groupIndex;
                        groups[recv] = groupIndex;
                        groupIndex++;
                        break;
                    }
                }
            }
            return groups;
        }

        public static string GetLinkColor(int groupIndex) =>
            LinkColors[groupIndex % LinkColors.Length];

        public static string GetLinkBgColor(int groupIndex) =>
            LinkBgColors[groupIndex % LinkBgColors.Length];

        public static string GetLinkStrokeColor(int groupIndex) =>
            GetLinkColor(groupIndex) + "40";

        public static TransactionReportRow ToReportRow(MockCryptoTransaction tx, string source, decimal currentPrice, Dictionary<string, string> addressMap)
        {
            var qtyPrefix = (tx.Type == "Receive" || tx.Type == "Buy") ? "+" : (tx.Type == "Send" || tx.Type == "Sell" || tx.Type == "Withdraw") ? "-" : "";
            var valueAtTime = tx.PriceAtTime > 0 ? tx.Quantity * tx.PriceAtTime : 0;
            var valueNow = tx.Quantity * currentPrice;
            var gainLoss = tx.PriceAtTime > 0 ? valueNow - valueAtTime : 0;
            var glPct = valueAtTime > 0 ? (gainLoss / valueAtTime) * 100 : 0;
            var fromInfo = ResolveWithSource(tx.FromAddress, tx.Type, true, source, addressMap, source);
            var toInfo = ResolveWithSource(tx.ToAddress, tx.Type, false, source, addressMap, source);
            var typeLabel = tx.Type == "Swap" && !string.IsNullOrEmpty(tx.SwapFor) ? $"Swap -> {tx.SwapFor}" : tx.Type;

            return new TransactionReportRow
            {
                Date = tx.Date.ToString("dd MMM yy HH:mm"),
                Type = typeLabel,
                Symbol = tx.Symbol ?? "",
                Source = source ?? "",
                From = fromInfo.Display,
                To = toInfo.Display,
                Quantity = $"{qtyPrefix}{tx.Quantity:G}",
                Price = tx.PriceAtTime > 0 ? CryptoFormatHelper.FormatPrice(tx.PriceAtTime) : "-",
                Value = valueAtTime > 0 ? CryptoFormatHelper.FormatValue(valueAtTime) : "-",
                PnL = tx.PriceAtTime > 0 ? $"{CryptoFormatHelper.FormatSignedValue(gainLoss)} ({glPct:+0.0;-0.0}%)" : "-",
                Hash = tx.Hash ?? "",
            };
        }

        public static string DetectNetwork(string address)
        {
            if (string.IsNullOrEmpty(address)) return null;

            if (address.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && address.Length == 42)
                return "ETH";
            if ((address.StartsWith("1") || address.StartsWith("3") || address.StartsWith("bc1")) && address.Length >= 26 && address.Length <= 62)
                return "BTC";
            if (address.StartsWith("r") && address.Length >= 25 && address.Length <= 35)
                return "XRP";
            if (address.Length >= 32 && address.Length <= 44 && !address.StartsWith("0x") && !address.Contains("."))
                return "SOL";
            if (address.StartsWith("0.0.") && address.Length >= 5)
                return "HBAR";
            if (address.StartsWith("addr1") || address.StartsWith("stake1"))
                return "ADA";
            if (address.StartsWith("G") && address.Length == 56)
                return "XLM";

            return null;
        }

        public static async Task ClaimAddress(string fullAddress, Page page)
        {
            if (string.IsNullOrEmpty(fullAddress)) return;

            var connections = CryptoStorageService.LoadConnections();
            var walletLabels = connections
                .Where(c => c.Type == "wallet")
                .Select(c => c.Label ?? $"{c.Symbol} Wallet")
                .Distinct()
                .ToArray();

            var exchangeLabels = connections
                .Where(c => c.Type == "exchange")
                .Select(c => c.Label ?? c.ExchangeName ?? "Exchange")
                .Distinct()
                .ToArray();

            var options = walletLabels.Concat(exchangeLabels).Append("+ New Wallet").Append("Label Address").ToArray();

            var displayAddr = fullAddress.Length > 20 ? fullAddress[..10] + "..." + fullAddress[^6..] : fullAddress;
            var selected = await page.DisplayActionSheet(
                $"Claim address\n{displayAddr}",
                "Cancel", null, options);

            if (string.IsNullOrEmpty(selected) || selected == "Cancel") return;

            if (selected == "Label Address")
            {
                var label = await page.DisplayPromptAsync("Label Address", $"Enter a label for:\n{displayAddr}", initialValue: "");
                if (string.IsNullOrWhiteSpace(label)) return;

                var newConn = new SavedConnection
                {
                    Type = "wallet",
                    Symbol = "LABEL",
                    Address = fullAddress,
                    Label = label.Trim(),
                };
                CryptoStorageService.SaveConnection(newConn);
                await page.DisplayAlert("Address Labelled", $"\"{label.Trim()}\" saved for {displayAddr}", "OK");
                return;
            }

            if (selected == "+ New Wallet")
            {
                var detected = DetectNetwork(fullAddress);
                var networks = new[] { "BTC", "ETH", "XRP", "SOL", "HBAR", "ADA", "MATIC", "XLM" };

                string symbol;
                if (detected != null)
                {
                    var confirm = await page.DisplayActionSheet(
                        $"Detected: {detected} network", "Cancel", null,
                        new[] { $"{detected} (detected)" }.Concat(networks.Where(n => n != detected)).ToArray());
                    if (string.IsNullOrEmpty(confirm) || confirm == "Cancel") return;
                    symbol = confirm.Replace(" (detected)", "");
                }
                else
                {
                    symbol = await page.DisplayActionSheet(
                        "Select network for this address", "Cancel", null, networks);
                    if (string.IsNullOrEmpty(symbol) || symbol == "Cancel") return;
                }

                var label = await page.DisplayPromptAsync("Wallet Label", "Enter a label (optional):", initialValue: $"{symbol} Wallet");
                if (label == null) return;

                var newConn = new SavedConnection
                {
                    Type = "wallet",
                    Symbol = symbol,
                    Address = fullAddress,
                    Label = string.IsNullOrWhiteSpace(label) ? null : label.Trim(),
                };
                CryptoStorageService.SaveConnection(newConn);
                await page.DisplayAlert("Wallet Created",
                    $"{label?.Trim() ?? $"{symbol} Wallet"} added with address {displayAddr}\n\nRefresh to fetch assets.", "OK");
                return;
            }

            var conn = connections.FirstOrDefault(c =>
                (c.Type == "wallet" && (c.Label ?? $"{c.Symbol} Wallet") == selected) ||
                (c.Type == "exchange" && (c.Label ?? c.ExchangeName ?? "Exchange") == selected));

            if (conn == null) return;

            if (conn.Type == "wallet")
            {
                conn.Address = fullAddress;
                CryptoStorageService.SaveConnection(conn);
                await page.DisplayAlert("Address Claimed", $"Address linked to {selected}", "OK");
            }
            else
            {
                await page.DisplayAlert("Address Claimed", $"Address recognised as {selected} NODE", "OK");
            }
        }
    }
}
