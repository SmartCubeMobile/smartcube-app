using Newtonsoft.Json;

namespace SmartCubeMobile.Services
{
    public class UserProfileData
    {
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string NiNumber { get; set; }
        public string EmploymentStatus { get; set; }
        public string Employer { get; set; }
        public decimal AnnualIncome { get; set; }
        public string TaxCode { get; set; }
        public int Dependants { get; set; }
        public string MaritalStatus { get; set; }
        public string CreditScore { get; set; }
        public string CreditScoreProvider { get; set; }
        public string CreditScoreDate { get; set; }
        public List<CreditScoreEntry> CreditScoreHistory { get; set; } = new();
        public string Gender { get; set; }
        public string DrivingLicenceNumber { get; set; }
        public string SmartScanLicence { get; set; }
        public bool SmartScanPromptShown { get; set; }
        public string LicenceIssueDate { get; set; }
        public string LicenceExpiryDate { get; set; }
        public string DrivingCategories { get; set; }
        public string PassportNumber { get; set; }
        public string PassportExpiry { get; set; }
        public string Nationality { get; set; }
        public string PlaceOfBirth { get; set; }
        public List<PropertyInfo> Properties { get; set; } = new();
        public List<VehicleInfo> Vehicles { get; set; } = new();
    }

    public class CreditScoreEntry
    {
        public int Score { get; set; }
        public string Provider { get; set; }
        public string Date { get; set; }
    }

    public class PropertyInfo
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string PropertyType { get; set; }
        public int Bedrooms { get; set; }
        public string Tenure { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal PurchasePrice { get; set; }
        public string PurchaseDate { get; set; }
        public string EpcRating { get; set; }
        public string CouncilTaxBand { get; set; }
        public bool IsPrimary { get; set; }
        public string CertificateUrl { get; set; }
        public string FloorArea { get; set; }
        public string WallsType { get; set; }
        public string RoofType { get; set; }
        public string HeatingType { get; set; }
        public string WindowsType { get; set; }
        public string HotWater { get; set; }
        public string Lighting { get; set; }
        public string EpcExpiry { get; set; }
        public string LastSaleDate { get; set; }
        public decimal LastSalePrice { get; set; }
        public List<PropertyMaintenanceEntry> MaintenanceHistory { get; set; } = new();
        public List<ContractorInfo> Contractors { get; set; } = new();
    }

    public class PropertyMaintenanceEntry
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string Category { get; set; }
    }

    public class MileageEntry
    {
        public string Date { get; set; }
        public decimal Miles { get; set; }
        public string Notes { get; set; }
    }

    public class ContractorInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Trade { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class GarageInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
    }

    public class VehicleInfo
    {
        public string Id { get; set; }
        public string Registration { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string FuelType { get; set; }
        public string Colour { get; set; }
        public decimal Mileage { get; set; }
        public string MotExpiry { get; set; }
        public string MotTestDate { get; set; }
        public string MotResult { get; set; }
        public List<string> MotAdvisories { get; set; } = new();
        public List<MileageEntry> MileageHistory { get; set; } = new();
        public string MotReminderDate { get; set; }
        public List<GarageInfo> PreferredGarages { get; set; } = new();
        public string TaxExpiry { get; set; }
        public decimal EstimatedValue { get; set; }
    }

    public static class UserProfileDataService
    {
        private static UserProfileData _profile;
        private static readonly string _filePath = Path.Combine(
            SmartCubeMobile.Services.AppPaths.Local,
            "SmartCube", "userprofile.json");

        static UserProfileDataService()
        {
            Load();
        }

        public static UserProfileData GetProfile()
        {
            return _profile ??= new UserProfileData();
        }

        public static void SaveProfile()
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                SecureFile.WriteAllText(_filePath, JsonConvert.SerializeObject(_profile, Formatting.Indented));
            }
            catch { }
        }

        public static void AddProperty(PropertyInfo property)
        {
            _profile ??= new UserProfileData();
            _profile.Properties.Add(property);
            SaveProfile();
        }

        public static void RemoveProperty(string id)
        {
            _profile?.Properties.RemoveAll(p => p.Id == id);
            SaveProfile();
        }

        public static void AddVehicle(VehicleInfo vehicle)
        {
            _profile ??= new UserProfileData();
            _profile.Vehicles.Add(vehicle);
            SaveProfile();
        }

        public static void RemoveVehicle(string id)
        {
            _profile?.Vehicles.RemoveAll(v => v.Id == id);
            SaveProfile();
        }

        private static void Load()
        {
            try
            {
                if (!File.Exists(_filePath)) return;
                var json = SecureFile.ReadAllText(_filePath);
                _profile = JsonConvert.DeserializeObject<UserProfileData>(json);
            }
            catch { _profile = new UserProfileData(); }
        }
    }
}
