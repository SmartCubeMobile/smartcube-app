using Newtonsoft.Json;

namespace SmartCubeMobile.Services
{
    // Saved supplier-website logins, opt-in per supplier. Stored DPAPI-encrypted (SecureFile) on this PC only.
    public static class SupplierCredentialStore
    {
        public class SavedLogin
        {
            public string User { get; set; }
            public string Pass { get; set; }
            public DateTime SavedAt { get; set; }
        }

        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartCube", "supplier_logins.json");
        private static Dictionary<string, SavedLogin> _cache;

        private static Dictionary<string, SavedLogin> Load()
        {
            if (_cache != null) return _cache;
            try
            {
                _cache = File.Exists(FilePath)
                    ? JsonConvert.DeserializeObject<Dictionary<string, SavedLogin>>(SecureFile.ReadAllText(FilePath)) ?? new()
                    : new();
            }
            catch { _cache = new(); }
            return _cache;
        }

        private static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            SecureFile.WriteAllText(FilePath, JsonConvert.SerializeObject(_cache, Formatting.Indented));
        }

        public static SavedLogin Get(string supplier) =>
            !string.IsNullOrEmpty(supplier) && Load().TryGetValue(supplier, out var l) ? l : null;

        public static bool Has(string supplier) => Get(supplier) != null;

        public static void Set(string supplier, string user, string pass)
        {
            Load()[supplier] = new SavedLogin { User = user, Pass = pass, SavedAt = DateTime.Now };
            Save();
        }

        public static void Forget(string supplier)
        {
            if (Load().Remove(supplier)) Save();
        }
    }
}
