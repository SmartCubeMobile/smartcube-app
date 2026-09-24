using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace SmartCubeMobile.MockData
{
    public class MockAccount
    {
        public string Institution { get; set; }
        public string AccountName { get; set; }
        public string SortCode { get; set; }
        public string AccountNumber { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string AccountType { get; set; }
        public string CurrencyCode { get; set; } = "GBP";
    }

    public class MockTransaction
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string AccountName { get; set; }
        public string Type { get; set; }
    }

    public class MockUtilityBill
    {
        public DateTime BillDate { get; set; }
        public string Supplier { get; set; }
        public string FuelType { get; set; }
        public decimal Amount { get; set; }
        public decimal UnitsUsed { get; set; }
        public string UoM { get; set; } = "kWh";
        public string Period { get; set; }
        public string Address { get; set; }
        public decimal UnitRatePence { get; set; }
        public decimal StandingChargePence { get; set; }
        public decimal VatRate { get; set; } = 5m;
        public bool IsEstimated { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string PaymentMethod { get; set; } = "Direct Debit";
        public bool IsFromPdf { get; set; }
        public bool HasSolar { get; set; }
        public decimal ExportKwh { get; set; }
        public decimal ExportRatePence { get; set; }
        public decimal ExportPayment { get; set; }
        public decimal GenerationKwh { get; set; }
        public string ExportTariffType { get; set; }
        public bool IsDeemedExport { get; set; }
        public decimal MeterReadingStart { get; set; }
        public decimal MeterReadingEnd { get; set; }
    }

    public class MockMeterReading
    {
        public DateTime ReadingDate { get; set; }
        public string MeterType { get; set; }
        public decimal Reading { get; set; }
        public decimal UnitsUsed { get; set; }
        public string Source { get; set; }
    }

    public class PropertyAddress
    {
        public string Address { get; set; }
        public string Postcode { get; set; }
    }

    public class MockCryptoHolding
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceGBP { get; set; }
        public decimal PriceUSD { get; set; }
        public decimal Change24h { get; set; }
        public string WalletLabel { get; set; }
        public string WalletAddress { get; set; }
        public string Network { get; set; }
        public decimal AvgCostBasis { get; set; }
        public decimal[] PriceHistory7d { get; set; }
        public List<MockCryptoTransaction> Transactions { get; set; } = new();
    }

    public class MockCryptoTransaction
    {
        public DateTime Date { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
        public string Hash { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string SwapFor { get; set; }
    }

    public class CryptoPriceAlert
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal TargetPrice { get; set; }
        public string Direction { get; set; }
        public bool IsTriggered { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MockExchangeRate
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal Rate { get; set; }
        public decimal PreviousRate { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; } = "ECB";
    }

    public class MockSupplier
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        public string Tariff { get; set; }
        public string TariffDetail { get; set; }
        public decimal MonthlyCost { get; set; }
        public string AccountRef { get; set; }
        public string BrandColor { get; set; }
        public string BrandAbbrev { get; set; }
    }

    public static class SupplierBranding
    {
        private static readonly Dictionary<string, (string Color, string Abbrev)> Brands = new(StringComparer.OrdinalIgnoreCase)
        {
            ["British Gas"] = ("#0072CE", "BG"),
            ["EDF"] = ("#FF6600", "EDF"),
            ["Octopus Energy"] = ("#8B2F97", "OCT"),
            ["OVO Energy"] = ("#00C853", "OVO"),
            ["E.ON"] = ("#EA1B2C", "E.ON"),
            ["Scottish Power"] = ("#003DA5", "SP"),
            ["Shell Energy"] = ("#DD1D21", "SHL"),
            ["Bulb"] = ("#FF69B4", "BLB"),
            ["SSE"] = ("#003B71", "SSE"),
            ["So Energy"] = ("#00BFA5", "SO"),
            ["Good Energy"] = ("#6DBE45", "GE"),
            ["Utility Warehouse"] = ("#E91E63", "UW"),
            ["Severn Trent"] = ("#00A651", "ST"),
            ["Thames Water"] = ("#0077C8", "TW"),
            ["United Utilities"] = ("#00529B", "UU"),
            ["Sky"] = ("#0072C6", "SKY"),
            ["BT"] = ("#6400AA", "BT"),
            ["Virgin Media"] = ("#C8102E", "VM"),
            ["Plusnet"] = ("#F47B20", "PN"),
            ["TalkTalk"] = ("#6E2C91", "TT"),
            ["EE"] = ("#009FDA", "EE"),
            ["Three"] = ("#000000", "3"),
            ["O2"] = ("#0019A5", "O2"),
            ["Vodafone"] = ("#E60000", "VF"),
        };

        public static (string Color, string Abbrev) Get(string supplierName)
        {
            if (Brands.TryGetValue(supplierName, out var brand))
                return brand;
            var abbrev = supplierName.Length <= 3
                ? supplierName.ToUpper()
                : string.Concat(supplierName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => w[0])).ToUpper();
            if (abbrev.Length > 3) abbrev = abbrev[..3];
            return ("#64748B", abbrev);
        }
    }

    public class MockChartPoint
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Category { get; set; }
    }

    public class MockUserProfile
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Postcode { get; set; }
        public string HouseNumber { get; set; }
        public string Address { get; set; }
        public string CultureCode { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime AccountCreated { get; set; }
        public bool TraceEnabled { get; set; }
        public List<MockCubeFace> CubeFaces { get; set; }
    }

    public class MockCubeFace
    {
        public char Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public int ScreenOrder { get; set; }
    }

    public class MortgageDetails
    {
        public string Lender { get; set; }
        public decimal PropertyValue { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal MonthlyPayment { get; set; }
        public decimal CurrentRate { get; set; }
        public decimal SvrRate { get; set; }
        public string RateType { get; set; }
        public DateTime FixedRateEndDate { get; set; }
        public DateTime MortgageStartDate { get; set; }
        public int TermYears { get; set; }

        public decimal Ltv => PropertyValue > 0 ? Math.Round(OutstandingBalance / PropertyValue * 100, 1) : 0;
        public int MonthsUntilEnd => Math.Max(0, (int)((FixedRateEndDate - DateTime.Now).TotalDays / 30.44));
        public bool InAlertWindow => MonthsUntilEnd <= 6 && MonthsUntilEnd > 0;
        public bool FixExpired => FixedRateEndDate <= DateTime.Now;

        public decimal EstimatedSvrPayment
        {
            get
            {
                if (SvrRate <= 0 || OutstandingBalance <= 0 || TermYears <= 0) return 0;
                var remainingMonths = Math.Max(12, TermYears * 12 - (int)((DateTime.Now - MortgageStartDate).TotalDays / 30.44));
                var monthlyRate = SvrRate / 100m / 12m;
                var payment = OutstandingBalance * (monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), remainingMonths))
                    / ((decimal)Math.Pow((double)(1 + monthlyRate), remainingMonths) - 1);
                return Math.Round(payment, 2);
            }
        }

        public decimal MonthlySvrIncrease => Math.Max(0, EstimatedSvrPayment - MonthlyPayment);
    }

    public static class MockDataService
    {
        private static readonly List<MockAccount> _extraAccounts = new();
        private static readonly List<MockTransaction> _extraTransactions = new();
        private static readonly List<MockCryptoHolding> _extraHoldings = new();
        private static readonly List<MockCryptoTransaction> _cryptoTransactions = new();
        private static readonly List<MockInvestment> _extraInvestments = new();
        private static MortgageDetails _mortgage;
        private static PropertyAddress _property;

        private static string DataDir
        {
            get
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartCube");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static PropertyAddress GetProperty()
        {
            if (_property != null) return _property;
            var path = Path.Combine(DataDir, "property.json");
            if (File.Exists(path))
            {
                try { _property = JsonSerializer.Deserialize<PropertyAddress>(SecureFile.ReadAllText(path)); }
                catch { _property = new PropertyAddress(); }
            }
            return _property ??= new PropertyAddress();
        }

        public static void SetProperty(PropertyAddress property)
        {
            _property = property;
            var path = Path.Combine(DataDir, "property.json");
            SecureFile.WriteAllText(path, JsonSerializer.Serialize(property));
        }

        public static void SaveBills()
        {
            if (_bills == null) return;
            var path = Path.Combine(DataDir, "bills.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            SecureFile.WriteAllText(path, JsonSerializer.Serialize(_bills, options));
        }

        private static void LoadBills()
        {
            var path = Path.Combine(DataDir, "bills.json");
            if (File.Exists(path))
            {
                try { _bills = JsonSerializer.Deserialize<List<MockUtilityBill>>(SecureFile.ReadAllText(path)) ?? new(); }
                catch { _bills = new(); }
            }
            else
            {
                _bills = new();
            }
        }

        private static bool _mortgageLoaded;

        public static MortgageDetails GetMortgage()
        {
            if (_mortgageLoaded) return _mortgage;
            _mortgageLoaded = true;
            var path = Path.Combine(DataDir, "mortgage.json");
            if (File.Exists(path))
            {
                try { _mortgage = JsonSerializer.Deserialize<MortgageDetails>(SecureFile.ReadAllText(path)); }
                catch { _mortgage = null; }
            }
            return _mortgage;
        }

        public static void SetMortgage(MortgageDetails mortgage)
        {
            _mortgage = mortgage;
            _mortgageLoaded = true;
            var path = Path.Combine(DataDir, "mortgage.json");
            try
            {
                if (mortgage == null) { if (File.Exists(path)) File.Delete(path); }
                else SecureFile.WriteAllText(path, JsonSerializer.Serialize(mortgage, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        public static void AddAccounts(List<MockAccount> accounts) => _extraAccounts.AddRange(accounts);
        public static void AddTransactions(List<MockTransaction> transactions) => _extraTransactions.AddRange(transactions);
        public static void AddCryptoHoldings(List<MockCryptoHolding> holdings) => _extraHoldings.AddRange(holdings);
        public static void ClearCryptoHoldings() => _extraHoldings.Clear();
        public static void AddCryptoTransactions(List<MockCryptoTransaction> transactions) => _cryptoTransactions.AddRange(transactions);

        private static bool _investmentsLoaded;

        private static void LoadInvestments()
        {
            if (_investmentsLoaded) return;
            _investmentsLoaded = true;
            var path = Path.Combine(DataDir, "investments.json");
            if (!File.Exists(path)) return;
            try
            {
                var saved = JsonSerializer.Deserialize<List<MockInvestment>>(SecureFile.ReadAllText(path));
                if (saved != null) _extraInvestments.AddRange(saved);
            }
            catch { }
        }

        private static void SaveInvestments()
        {
            try
            {
                SecureFile.WriteAllText(Path.Combine(DataDir, "investments.json"),
                    JsonSerializer.Serialize(_extraInvestments, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        public static void AddInvestment(MockInvestment inv)
        {
            LoadInvestments();
            _extraInvestments.Add(inv);
            SaveInvestments();
        }

        public static List<MockAccount> GetAccounts()
        {
            return new List<MockAccount>(_extraAccounts);
        }

        public static List<MockTransaction> GetTransactions()
        {
            return new List<MockTransaction>(_extraTransactions);
        }

        private static List<MockUtilityBill> _bills;

        public static List<MockUtilityBill> GetUtilityBills()
        {
            if (_bills != null) return _bills;
            LoadBills();
            return _bills;
        }

        public static void RemoveBill(int index)
        {
            GetUtilityBills();
            if (index >= 0 && index < _bills.Count)
            {
                _bills.RemoveAt(index);
                SaveBills();
            }
        }

        public static bool AddBill(MockUtilityBill bill)
        {
            GetUtilityBills();
            var isDupe = _bills.Any(b =>
                b.BillDate == bill.BillDate &&
                b.Supplier == bill.Supplier &&
                b.FuelType == bill.FuelType &&
                b.Amount == bill.Amount);
            if (isDupe) return false;

            var index = _bills.FindIndex(b => b.BillDate < bill.BillDate);
            if (index >= 0)
                _bills.Insert(index, bill);
            else
                _bills.Add(bill);
            SaveBills();
            return true;
        }

        public static List<MockMeterReading> GetMeterReadings()
        {
            return new List<MockMeterReading>();
        }

        private static readonly List<MockCryptoHolding> _cryptoHoldings = new();

        public static List<MockCryptoHolding> GetCryptoHoldings()
        {
            var holdings = new List<MockCryptoHolding>(_cryptoHoldings);
            holdings.AddRange(_extraHoldings);
            return holdings;
        }

        public static List<MockCryptoTransaction> GetCryptoTransactions()
        {
            return new List<MockCryptoTransaction>(_cryptoTransactions);
        }

        public static List<MockExchangeRate> GetExchangeRates()
        {
            var now = DateTime.Now;
            return new List<MockExchangeRate>
            {
                new MockExchangeRate { FromCurrency = "EUR", ToCurrency = "GBP", Rate = 0.8412m, PreviousRate = 0.8398m, Date = now },
                new MockExchangeRate { FromCurrency = "USD", ToCurrency = "GBP", Rate = 0.7865m, PreviousRate = 0.7891m, Date = now },
                new MockExchangeRate { FromCurrency = "JPY", ToCurrency = "GBP", Rate = 0.005234m, PreviousRate = 0.005218m, Date = now },
                new MockExchangeRate { FromCurrency = "CHF", ToCurrency = "GBP", Rate = 0.8821m, PreviousRate = 0.8805m, Date = now },
                new MockExchangeRate { FromCurrency = "AUD", ToCurrency = "GBP", Rate = 0.5124m, PreviousRate = 0.5141m, Date = now },
                new MockExchangeRate { FromCurrency = "CAD", ToCurrency = "GBP", Rate = 0.5732m, PreviousRate = 0.5718m, Date = now },
                new MockExchangeRate { FromCurrency = "SEK", ToCurrency = "GBP", Rate = 0.0742m, PreviousRate = 0.0738m, Date = now },
                new MockExchangeRate { FromCurrency = "NOK", ToCurrency = "GBP", Rate = 0.0718m, PreviousRate = 0.0721m, Date = now },
                new MockExchangeRate { FromCurrency = "DKK", ToCurrency = "GBP", Rate = 0.1128m, PreviousRate = 0.1125m, Date = now },
                new MockExchangeRate { FromCurrency = "NZD", ToCurrency = "GBP", Rate = 0.4698m, PreviousRate = 0.4712m, Date = now },
                new MockExchangeRate { FromCurrency = "ZAR", ToCurrency = "GBP", Rate = 0.0432m, PreviousRate = 0.0428m, Date = now },
                new MockExchangeRate { FromCurrency = "INR", ToCurrency = "GBP", Rate = 0.00932m, PreviousRate = 0.00928m, Date = now },
            };
        }

        public static List<MockChartPoint> GetMonthlySpending()
        {
            var now = DateTime.Now;
            return new List<MockChartPoint>
            {
                new MockChartPoint { Date = now.AddMonths(-5), Value = 2340.50m, Category = "Total" },
                new MockChartPoint { Date = now.AddMonths(-4), Value = 2180.20m, Category = "Total" },
                new MockChartPoint { Date = now.AddMonths(-3), Value = 2510.80m, Category = "Total" },
                new MockChartPoint { Date = now.AddMonths(-2), Value = 1980.30m, Category = "Total" },
                new MockChartPoint { Date = now.AddMonths(-1), Value = 2290.60m, Category = "Total" },
                new MockChartPoint { Date = now, Value = 1450.20m, Category = "Total" },

                new MockChartPoint { Date = now.AddMonths(-5), Value = 520.30m, Category = "Groceries" },
                new MockChartPoint { Date = now.AddMonths(-4), Value = 480.10m, Category = "Groceries" },
                new MockChartPoint { Date = now.AddMonths(-3), Value = 545.60m, Category = "Groceries" },
                new MockChartPoint { Date = now.AddMonths(-2), Value = 498.20m, Category = "Groceries" },
                new MockChartPoint { Date = now.AddMonths(-1), Value = 510.40m, Category = "Groceries" },
                new MockChartPoint { Date = now, Value = 142.45m, Category = "Groceries" },

                new MockChartPoint { Date = now.AddMonths(-5), Value = 380.00m, Category = "Bills" },
                new MockChartPoint { Date = now.AddMonths(-4), Value = 380.00m, Category = "Bills" },
                new MockChartPoint { Date = now.AddMonths(-3), Value = 395.00m, Category = "Bills" },
                new MockChartPoint { Date = now.AddMonths(-2), Value = 380.00m, Category = "Bills" },
                new MockChartPoint { Date = now.AddMonths(-1), Value = 380.00m, Category = "Bills" },
                new MockChartPoint { Date = now, Value = 325.50m, Category = "Bills" },

                new MockChartPoint { Date = now.AddMonths(-5), Value = 210.40m, Category = "Energy" },
                new MockChartPoint { Date = now.AddMonths(-4), Value = 245.80m, Category = "Energy" },
                new MockChartPoint { Date = now.AddMonths(-3), Value = 195.30m, Category = "Energy" },
                new MockChartPoint { Date = now.AddMonths(-2), Value = 168.50m, Category = "Energy" },
                new MockChartPoint { Date = now.AddMonths(-1), Value = 136.80m, Category = "Energy" },
                new MockChartPoint { Date = now, Value = 94.50m, Category = "Energy" },
            };
        }

        public static List<MockChartPoint> GetNetWorthHistory()
        {
            return new List<MockChartPoint>();
        }

        public static MockUserProfile GetUserProfile()
        {
            var session = Services.SessionService.Current;
            return new MockUserProfile
            {
                Username = session?.Username ?? "",
                DisplayName = session?.FullName ?? session?.Username ?? "SmartCube User",
                Email = session?.Email ?? "",
                Postcode = "",
                HouseNumber = "",
                Address = "",
                CultureCode = "en-GB",
                CurrencyCode = "GBP",
                LastLogin = session?.LastLogin?.ToLocalTime() ?? DateTime.Now,
                AccountCreated = DateTime.Now,
                TraceEnabled = true,
                CubeFaces = new List<MockCubeFace>
                {
                    new MockCubeFace { Code = 'B', Name = "Banking", Icon = "\U0001F3E6", IsActive = true, ScreenOrder = 1 },
                    new MockCubeFace { Code = 'U', Name = "Utility", Icon = "⚡", IsActive = true, ScreenOrder = 2 },
                    new MockCubeFace { Code = 'C', Name = "Crypto", Icon = "\U0001FA99", IsActive = true, ScreenOrder = 3 },
                    new MockCubeFace { Code = 'X', Name = "Exchange", Icon = "\U0001F4B1", IsActive = true, ScreenOrder = 4 },
                    new MockCubeFace { Code = 'A', Name = "Charts", Icon = "\U0001F4CA", IsActive = true, ScreenOrder = 5 },
                    new MockCubeFace { Code = 'S', Name = "Subscriptions", Icon = "\U0001F4CB", IsActive = true, ScreenOrder = 6 },
                    new MockCubeFace { Code = 'P', Name = "Profile", Icon = "\U0001F510", IsActive = true, ScreenOrder = 7 },
                }
            };
        }

        public static decimal GetTotalBalance()
        {
            return GetAccounts().Sum(a => a.TotalBalance);
        }

        private static List<MockSupplier> _suppliers;

        private static readonly Dictionary<string, string> _fuelTypeIcons = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Electricity"] = "⚡", ["Gas"] = "🔥", ["Water"] = "💧",
            ["Broadband"] = "📡", ["Mobile"] = "📱", ["Phone"] = "📞",
        };

        public static List<MockSupplier> GetSuppliers()
        {
            if (_suppliers != null) return _suppliers;
            var path = Path.Combine(DataDir, "suppliers.json");
            if (File.Exists(path))
            {
                try { _suppliers = JsonSerializer.Deserialize<List<MockSupplier>>(SecureFile.ReadAllText(path)) ?? new(); }
                catch { _suppliers = new(); }
            }
            else
            {
                _suppliers = new();
            }
            DetectSuppliersFromBills();
            return _suppliers;
        }

        private static void DetectSuppliersFromBills()
        {
            var bills = GetUtilityBills();
            if (bills.Count == 0) return;

            var groups = bills
                .Where(b => !string.IsNullOrEmpty(b.Supplier))
                .GroupBy(b => $"{b.Supplier}|{b.FuelType}");

            bool added = false;
            foreach (var g in groups)
            {
                var latest = g.OrderByDescending(b => b.BillDate).First();
                var exists = _suppliers.Any(s =>
                    s.Name.Equals(latest.Supplier, StringComparison.OrdinalIgnoreCase) &&
                    s.Type.Equals(latest.FuelType, StringComparison.OrdinalIgnoreCase));
                if (exists) continue;

                _fuelTypeIcons.TryGetValue(latest.FuelType, out var icon);
                var tariffDetail = latest.UnitRatePence > 0
                    ? $"{latest.UnitRatePence:F2}p/{latest.UoM ?? "kWh"}"
                    : "";
                if (latest.StandingChargePence > 0)
                    tariffDetail += $" · {latest.StandingChargePence:F2}p/day standing";

                _suppliers.Add(new MockSupplier
                {
                    Name = latest.Supplier,
                    Type = latest.FuelType,
                    Icon = icon ?? "📄",
                    Tariff = latest.PaymentMethod ?? "Standard",
                    TariffDetail = tariffDetail.TrimStart(' ', '·').Trim(),
                    MonthlyCost = latest.Amount,
                    AccountRef = "",
                });
                added = true;
            }
            if (added) SaveSuppliers();
        }

        public static void SaveSuppliers()
        {
            try
            {
                if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
                SecureFile.WriteAllText(Path.Combine(DataDir, "suppliers.json"),
                    JsonSerializer.Serialize(_suppliers, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        private static List<Subscription> _subscriptions;

        internal static readonly Dictionary<string, (string Category, string CancelUrl, string CancelPhone, string CancelNotes)> KnownSubscriptions
            = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Amazon Prime"] = ("Streaming", "https://www.amazon.co.uk/gp/primecentral", "0800 279 7234", "Cancel in Amazon account settings > Prime membership"),
            ["Netflix"] = ("Streaming", "https://www.netflix.com/cancelplan", "Help Centre only", "Cancel in Account > Membership & Billing"),
            ["Spotify"] = ("Music", "https://www.spotify.com/account/subscription/", null, "Cancel in Account > Subscription > Change plan"),
            ["Disney+"] = ("Streaming", "https://www.disneyplus.com/account", null, "Cancel in Account > Billing details"),
            ["Apple TV+"] = ("Streaming", null, "0800 048 0408", "Cancel via Settings > Apple ID > Subscriptions on device"),
            ["Sky TV"] = ("TV & Broadband", "https://www.sky.com/shop/cancel/", "0333 759 1018", "31-day notice period required. Cancel via My Sky or phone"),
            ["Sky Sports"] = ("TV & Broadband", "https://www.sky.com/shop/cancel/", "0333 759 1018", "Cancel via My Sky"),
            ["NOW TV"] = ("Streaming", "https://account.nowtv.com/", "0800 138 8031", "Cancel in My Account > Manage passes"),
            ["BT Sport"] = ("TV & Broadband", "https://www.bt.com/sport/cancel", "0800 800 150", "Cancel in My BT"),
            ["YouTube Premium"] = ("Streaming", "https://www.youtube.com/paid_memberships", null, "Cancel in YouTube > Paid memberships"),
            ["Xbox Game Pass"] = ("Gaming", "https://account.microsoft.com/services", "0800 587 1102", "Cancel in Microsoft Account > Services"),
            ["PlayStation Plus"] = ("Gaming", "https://store.playstation.com/subscriptions/", "0203 538 2665", "Cancel in PS Store > Subscriptions"),
            ["Nintendo Switch Online"] = ("Gaming", "https://accounts.nintendo.com/", "0345 605 0247", "Cancel in Nintendo Account > Shop menu"),
            ["Gym"] = ("Health & Fitness", null, null, "Check contract terms — many require 30 days notice in writing"),
            ["Council Tax"] = ("Bills", null, null, "Cannot cancel — contact your local council to discuss payment plans"),
            ["Virgin Media"] = ("TV & Broadband", "https://www.virginmedia.com/help/cancel", "0345 454 1111", "30-day notice. Disconnect fee may apply in contract"),
            ["BT Broadband"] = ("TV & Broadband", "https://www.bt.com/help/cancel", "0800 800 150", "Cancel in My BT — early termination charge if in contract"),
        };

        public static List<Subscription> GetSubscriptions()
        {
            if (_subscriptions != null) return _subscriptions;
            var path = Path.Combine(DataDir, "subscriptions.json");
            if (File.Exists(path))
            {
                try { _subscriptions = JsonSerializer.Deserialize<List<Subscription>>(SecureFile.ReadAllText(path)) ?? new(); }
                catch { _subscriptions = new(); }
            }
            else
            {
                _subscriptions = new();
            }
            DetectSubscriptionsFromTransactions();
            return _subscriptions;
        }

        private static void DetectSubscriptionsFromTransactions()
        {
            var transactions = GetTransactions();
            DetectFromTransactionList(transactions);
        }

        public static void DetectSubscriptionsFromLiveTransactions(List<MockTransaction> transactions)
        {
            GetSubscriptions();
            DetectFromTransactionList(transactions);
        }

        private static void DetectFromTransactionList(List<MockTransaction> transactions)
        {
            var excludeCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Energy", "Water" };
            var excludeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "British Gas", "Water Bill", "Water Bill - Severn Trent" };

            var candidates = transactions.Where(t =>
                t.Amount < 0
                && !excludeCategories.Contains(t.Category ?? "")
                && !excludeNames.Contains(t.Description?.Trim() ?? "")
                && (t.Type == "DD" || t.Type == "SO" || t.Category == "Subscription"
                    || MatchesKnownSubscription(t.Description?.Trim()))).ToList();

            foreach (var t in candidates)
            {
                var name = CleanDescription(t.Description.Trim());
                if (_subscriptions.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) continue;

                KnownSubscriptions.TryGetValue(name, out var info);

                var sub = new Subscription
                {
                    Id = Guid.NewGuid().ToString("N")[..8],
                    Name = name,
                    MonthlyCost = Math.Abs(t.Amount),
                    Category = info.Category ?? t.Category ?? "Other",
                    NextRenewal = t.Date.AddMonths(1),
                    PaymentMethod = t.Type == "DD" ? "Direct Debit" : t.Type == "SO" ? "Standing Order" : "Card",
                    CancelUrl = info.CancelUrl,
                    CancelPhone = info.CancelPhone,
                    CancelNotes = info.CancelNotes,
                    IsAutoDetected = true,
                };
                _subscriptions.Add(sub);
            }
            SaveSubscriptions();
        }

        private static bool MatchesKnownSubscription(string desc)
        {
            if (string.IsNullOrEmpty(desc)) return false;
            var upper = desc.ToUpperInvariant();
            return KnownSubscriptions.Keys.Any(k => upper.Contains(k.ToUpperInvariant()));
        }

        private static string CleanDescription(string desc)
        {
            foreach (var known in KnownSubscriptions.Keys)
            {
                if (desc.Contains(known, StringComparison.OrdinalIgnoreCase))
                    return known;
            }
            return desc;
        }

        public static void SaveSubscriptions()
        {
            if (_subscriptions == null) return;
            var path = Path.Combine(DataDir, "subscriptions.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            SecureFile.WriteAllText(path, JsonSerializer.Serialize(_subscriptions, options));
        }

        public static void AddSubscription(Subscription sub)
        {
            GetSubscriptions();
            _subscriptions.Add(sub);
            SaveSubscriptions();
        }

        public static void RemoveSubscription(string id)
        {
            GetSubscriptions();
            _subscriptions.RemoveAll(s => s.Id == id);
            SaveSubscriptions();
        }

        public static void UpdateSubscriptionStatus(string id, string status)
        {
            GetSubscriptions();
            var sub = _subscriptions.FirstOrDefault(s => s.Id == id);
            if (sub != null)
            {
                sub.Status = status;
                SaveSubscriptions();
            }
        }

        public static void AddSupplier(MockSupplier supplier)
        {
            GetSuppliers();
            _suppliers.Add(supplier);
            SaveSuppliers();
        }

        public static void UpdateSupplier(string type, string newName, string newTariff, string newDetail, decimal newCost)
        {
            var suppliers = GetSuppliers();
            var existing = suppliers.FirstOrDefault(s => s.Type == type);
            if (existing == null) return;
            existing.Name = newName;
            existing.Tariff = newTariff;
            existing.TariffDetail = newDetail;
            existing.MonthlyCost = newCost;
            SaveSuppliers();
        }

        public static decimal GetTotalCryptoValueGBP()
        {
            return GetCryptoHoldings().Sum(h => h.Quantity * h.PriceGBP);
        }

        public static (int Score, string Label, string Description) GetFearGreedIndex()
        {
            return (72, "Greed", "Market sentiment is bullish — investors are confident but approaching excessive optimism.");
        }

        private static List<CryptoPriceAlert> _priceAlerts = new();

        public static List<CryptoPriceAlert> GetPriceAlerts() => _priceAlerts;

        public static void AddPriceAlert(CryptoPriceAlert alert)
        {
            _priceAlerts.Add(alert);
        }

        public static void RemovePriceAlert(CryptoPriceAlert alert)
        {
            _priceAlerts.Remove(alert);
        }

        public static List<MockInvestment> GetInvestments()
        {
            LoadInvestments();
            return new List<MockInvestment>(_extraInvestments);
        }

        public static decimal GetTotalInvestmentValueGBP()
        {
            return GetInvestments().Sum(i => i.Shares * i.PricePence / 100m);
        }

        public static List<MockPurchase> GetPurchaseHistory(string symbol)
        {
            var rng = new Random(symbol.GetHashCode());
            var now = DateTime.Now;
            var purchases = new List<MockPurchase>();
            var inv = GetInvestments().FirstOrDefault(i => i.Symbol == symbol);
            if (inv == null) return purchases;

            int remaining = inv.Shares;
            int lots = rng.Next(2, 5);
            for (int i = 0; i < lots; i++)
            {
                int qty = i == lots - 1 ? remaining : Math.Max(1, remaining / (lots - i) + rng.Next(-remaining / (lots * 2), remaining / (lots * 2)));
                if (qty <= 0 || remaining <= 0) break;
                qty = Math.Min(qty, remaining);
                int daysAgo = rng.Next(60 + i * 180, 180 + i * 240);
                var costVar = (decimal)(rng.NextDouble() * 0.15 - 0.05);
                purchases.Add(new MockPurchase
                {
                    Date = now.AddDays(-daysAgo),
                    Shares = qty,
                    PricePence = Math.Round(inv.CostBasisPence * (1 + costVar), 0),
                    FeePence = rng.Next(995, 1195),
                });
                remaining -= qty;
            }
            return purchases.OrderBy(p => p.Date).ToList();
        }

        public static List<MockPricePoint> GetPriceHistory(string symbol, string period)
        {
            var rng = new Random(symbol.GetHashCode() + period.GetHashCode());
            var inv = GetInvestments().FirstOrDefault(i => i.Symbol == symbol);
            if (inv == null) return new();

            var now = DateTime.Now;
            int points;
            int totalDays;
            switch (period)
            {
                case "1D":
                    points = 24; totalDays = 1; break;
                case "1W":
                    points = 7; totalDays = 7; break;
                case "1M":
                    points = 30; totalDays = 30; break;
                case "3M":
                    points = 45; totalDays = 90; break;
                case "6M":
                    points = 60; totalDays = 180; break;
                case "1Y":
                    points = 52; totalDays = 365; break;
                default:
                    points = 30; totalDays = 30; break;
            }

            var result = new List<MockPricePoint>();
            decimal price = inv.CostBasisPence * 0.92m;
            decimal target = inv.PricePence;
            decimal drift = (target - price) / points;

            for (int i = 0; i < points; i++)
            {
                var date = now.AddDays(-totalDays + (double)totalDays / points * i);
                price += drift + (decimal)(rng.NextDouble() * (double)(inv.PricePence * 0.03m) - (double)(inv.PricePence * 0.015m));
                if (price < inv.CostBasisPence * 0.7m) price = inv.CostBasisPence * 0.75m;
                result.Add(new MockPricePoint { Date = date, PricePence = Math.Round(price, 0) });
            }
            result.Add(new MockPricePoint { Date = now, PricePence = inv.PricePence });
            return result;
        }
    }

    public class MockInvestment
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Shares { get; set; }
        public decimal PricePence { get; set; }
        public decimal CostBasisPence { get; set; }
        public decimal Change1D { get; set; }
        public decimal DividendYield { get; set; }
        public string Sector { get; set; }
        public string Platform { get; set; }
    }

    public class MockPurchase
    {
        public DateTime Date { get; set; }
        public int Shares { get; set; }
        public decimal PricePence { get; set; }
        public decimal FeePence { get; set; }
    }

    public class MockPricePoint
    {
        public DateTime Date { get; set; }
        public decimal PricePence { get; set; }
    }

    public class Subscription
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal MonthlyCost { get; set; }
        public string Frequency { get; set; } = "Monthly";
        public string Category { get; set; }
        public DateTime? NextRenewal { get; set; }
        public string PaymentMethod { get; set; } = "Direct Debit";
        public string Status { get; set; } = "Active";
        public string CancelUrl { get; set; }
        public string CancelPhone { get; set; }
        public string CancelNotes { get; set; }
        public bool IsAutoDetected { get; set; }
    }
}
