using Newtonsoft.Json;

namespace SmartCubeMobile.Services
{
    public class InsurancePolicy
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string PolicyType { get; set; }
        public string Provider { get; set; }
        public decimal MonthlyPremium { get; set; }
        public decimal AnnualPremium { get; set; }
        public decimal CoverAmount { get; set; }
        public string PolicyNumber { get; set; }
        public decimal Excess { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string DocumentPath { get; set; }
        public string VehicleReg { get; set; }
        public string VehicleMakeModel { get; set; }
        public string PropertyAddress { get; set; }
        public string PetName { get; set; }
        public string PetBreed { get; set; }
        public string NamedInsured { get; set; }
    }

    public static class InsuranceDataService
    {
        private static readonly List<InsurancePolicy> _policies = new();
        private static readonly string _filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SmartCube", "insurance.json");

        static InsuranceDataService()
        {
            Load();
        }

        public static List<InsurancePolicy> GetPolicies() => _policies.ToList();

        public static void AddPolicy(InsurancePolicy policy)
        {
            _policies.Add(policy);
            Save();
        }

        public static void UpdatePolicy(InsurancePolicy policy)
        {
            var idx = _policies.FindIndex(p => p.Id == policy.Id);
            if (idx >= 0)
                _policies[idx] = policy;
            Save();
        }

        public static void RemovePolicy(string id)
        {
            _policies.RemoveAll(p => p.Id == id);
            Save();
        }

        private static void Load()
        {
            try
            {
                if (!File.Exists(_filePath)) return;
                var json = File.ReadAllText(_filePath);
                var policies = JsonConvert.DeserializeObject<List<InsurancePolicy>>(json);
                if (policies != null)
                    _policies.AddRange(policies);
            }
            catch { }
        }

        private static void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(_policies, Formatting.Indented));
            }
            catch { }
        }
    }
}
