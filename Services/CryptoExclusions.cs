using System.Text.Json;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    // Crypto transactions the user has excluded from profit & loss (airdropped spam, transfers between
    // their own wallets, mis-priced entries...). Stored by a key built from the transaction itself, so the
    // choice survives wallet refreshes that re-download the history.
    public static class CryptoExclusions
    {
        private static HashSet<string> _keys;
        private static string PathOnDisk => Path.Combine(MockDataService.DataDir, "crypto_excluded.json");

        private static HashSet<string> Keys
        {
            get
            {
                if (_keys != null) return _keys;
                try
                {
                    if (File.Exists(PathOnDisk))
                        _keys = JsonSerializer.Deserialize<HashSet<string>>(SecureFile.ReadAllText(PathOnDisk));
                }
                catch { }
                return _keys ??= new HashSet<string>();
            }
        }

        public static string KeyFor(MockCryptoTransaction t) =>
            !string.IsNullOrWhiteSpace(t.Hash)
                ? $"{t.Symbol}|{t.Hash}|{t.Type}|{t.Quantity}"
                : $"{t.Symbol}|{t.Date:yyyyMMddHHmmss}|{t.Type}|{t.Quantity}";

        public static bool IsExcluded(MockCryptoTransaction t) => t != null && Keys.Contains(KeyFor(t));

        public static int Count => Keys.Count;

        public static void Set(MockCryptoTransaction t, bool excluded)
        {
            if (t == null) return;
            if (excluded) Keys.Add(KeyFor(t)); else Keys.Remove(KeyFor(t));
            try { SecureFile.WriteAllText(PathOnDisk, JsonSerializer.Serialize(Keys)); } catch { }
        }

        // Transactions that count towards P&L.
        public static List<MockCryptoTransaction> Counted(IEnumerable<MockCryptoTransaction> txs) =>
            (txs ?? Enumerable.Empty<MockCryptoTransaction>()).Where(t => !IsExcluded(t)).ToList();
    }
}
