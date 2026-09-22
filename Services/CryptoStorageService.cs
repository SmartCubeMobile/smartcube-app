using Newtonsoft.Json;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    public class SavedConnection
    {
        public string Type { get; set; }
        public string Symbol { get; set; }
        public string Address { get; set; }
        public string Label { get; set; }
        public string ExchangeName { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
    }

    public static class CryptoStorageService
    {
        private static readonly string _dataDir = FileSystem.AppDataDirectory;
        private static readonly string _connectionsFile = Path.Combine(FileSystem.AppDataDirectory, "crypto_connections.json");
        private static readonly string _cacheFile = Path.Combine(FileSystem.AppDataDirectory, "crypto_cache.json");

        public static void SaveConnection(SavedConnection conn)
        {
            var connections = LoadConnections();

            var existing = connections.FindIndex(c =>
                c.Type == conn.Type &&
                ((c.Type == "wallet" && c.Symbol == conn.Symbol && c.Address == conn.Address) ||
                 (c.Type == "exchange" && c.ExchangeName == conn.ExchangeName && c.ApiKey == conn.ApiKey)));

            if (existing >= 0)
                connections[existing] = conn;
            else
                connections.Add(conn);

            File.WriteAllText(_connectionsFile, JsonConvert.SerializeObject(connections, Formatting.Indented));
        }

        public static List<SavedConnection> LoadConnections()
        {
            try
            {
                if (File.Exists(_connectionsFile))
                    return JsonConvert.DeserializeObject<List<SavedConnection>>(File.ReadAllText(_connectionsFile)) ?? new();
            }
            catch { }
            return new();
        }

        public static void RemoveConnection(SavedConnection conn)
        {
            var connections = LoadConnections();
            connections.RemoveAll(c =>
                c.Type == conn.Type &&
                ((c.Type == "wallet" && c.Symbol == conn.Symbol && c.Address == conn.Address) ||
                 (c.Type == "exchange" && c.ExchangeName == conn.ExchangeName && c.ApiKey == conn.ApiKey)));
            File.WriteAllText(_connectionsFile, JsonConvert.SerializeObject(connections, Formatting.Indented));
        }

        public static void SaveHoldingsCache(List<MockCryptoHolding> holdings)
        {
            try
            {
                var json = JsonConvert.SerializeObject(holdings, Formatting.Indented);
                File.WriteAllText(_cacheFile, json);
            }
            catch { }
        }

        public static List<MockCryptoHolding> LoadHoldingsCache()
        {
            try
            {
                if (File.Exists(_cacheFile))
                    return JsonConvert.DeserializeObject<List<MockCryptoHolding>>(File.ReadAllText(_cacheFile)) ?? new();
            }
            catch { }
            return new();
        }
    }
}
