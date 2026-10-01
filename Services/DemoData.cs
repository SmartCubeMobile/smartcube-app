using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    // Made-up data for demo mode (start the app with --demo): a fictional person, "Alex Morgan", with
    // bank accounts, bills, subscriptions, insurance, crypto and investments. Everything here is
    // invented; it is written to the separate demo folder (AppPaths) each time the demo starts, so the
    // real data is never touched and screenshots contain nothing personal.
    public static class DemoData
    {
        public const string Username = "alexmorgan";
        public const string FullName = "Alex Morgan";
        public const string Email = "alex.morgan@example.com";
        private const string Address = "14 Willow Lane, Leeds";
        private const string Postcode = "LS6 2AB";

        private const string Current = "Barclays Current Account";
        private const string Saver = "Everyday Saver";
        private const string Joint = "Nationwide FlexDirect";
        private const string Card = "Barclaycard Platinum";

        public static void Seed()
        {
            if (!AppPaths.IsDemo) return;
            var root = AppPaths.Local;
            if (!root.EndsWith("SmartCubeDemo", StringComparison.OrdinalIgnoreCase)) return;   // never the real folder

            try { if (Directory.Exists(root)) Directory.Delete(root, true); } catch { }
            Directory.CreateDirectory(AppPaths.Data);
            Directory.CreateDirectory(AppPaths.AppData);

            var bills = BuildBills();
            var (accounts, transactions) = BuildBanking(bills);

            WriteNewtonsoft("accounts_cache.json", accounts);
            WriteNewtonsoft("transactions_cache.json", transactions.OrderByDescending(t => t.Date).ToList());
            MockDataService.AddAccounts(accounts);
            MockDataService.AddTransactions(transactions.OrderByDescending(t => t.Date).ToList());

            WriteSystemJson("bills.json", bills.OrderByDescending(b => b.BillDate).ToList());
            WriteSystemJson("suppliers.json", new List<MockSupplier>
            {
                new() { Name = "Octopus Energy", Type = "Electricity", Icon = "⚡", Tariff = "Flexible Octopus", TariffDetail = "24.50p/kWh · 60.10p/day standing", MonthlyCost = 86.40m, AccountRef = "A-4F21C9D0" },
                new() { Name = "Octopus Energy", Type = "Gas", Icon = "🔥", Tariff = "Flexible Octopus", TariffDetail = "6.24p/kWh · 31.43p/day standing", MonthlyCost = 58.20m, AccountRef = "A-4F21C9D0" },
                new() { Name = "Yorkshire Water", Type = "Water", Icon = "💧", Tariff = "Metered", TariffDetail = "Billed every 6 months", MonthlyCost = 42.50m, AccountRef = "7701 2284 0193" },
                new() { Name = "Sky", Type = "Broadband", Icon = "🌐", Tariff = "Superfast 61Mb", TariffDetail = "18-month contract", MonthlyCost = 38.00m, AccountRef = "SKY-620118845" },
                new() { Name = "EE", Type = "Mobile", Icon = "📱", Tariff = "SIM only 25GB", TariffDetail = "12-month contract", MonthlyCost = 24.00m, AccountRef = "EE-0048812" },
            });
            WriteSystemJson("property.json", new PropertyAddress { Address = Address, Postcode = Postcode });
            WriteSystemJson("mortgage.json", new MortgageDetails
            {
                Lender = "Nationwide", PropertyValue = 285000, OutstandingBalance = 168400, MonthlyPayment = 845,
                CurrentRate = 4.29m, SvrRate = 7.49m, RateType = "Fixed",
                FixedRateEndDate = DateTime.Today.AddMonths(14), MortgageStartDate = new DateTime(2019, 6, 1), TermYears = 25,
            });
            WriteSystemJson("investments.json", BuildInvestments());
            WriteNewtonsoft("insurance.json", BuildInsurance());
            WriteNewtonsoft("userprofile.json", BuildProfile());

            File.WriteAllText(Path.Combine(AppPaths.AppData, "crypto_cache.json"),
                Newtonsoft.Json.JsonConvert.SerializeObject(BuildCrypto(), Newtonsoft.Json.Formatting.Indented));
        }

        #region Support

        // Ids far above real ticket numbers so "seen" markers never clash with the real account's tickets.
        public const int ChatTicketId = 900001;
        public const string ChatSubject = "Bank transactions not refreshing";

        // "--chat=<url>|<reference>" points the demo ticket at a real chat page (used for screenshots).
        private static readonly string[] ChatArg = (Environment.GetCommandLineArgs()
            .FirstOrDefault(a => a.StartsWith("--chat=", StringComparison.OrdinalIgnoreCase))?[7..] ?? "").Split('|');
        public static string ChatUrl => ChatArg[0].Length > 0 ? ChatArg[0] : null;
        public static string ChatReference => ChatArg.Length > 1 ? ChatArg[1] : "SC-00042";

        private const string ChatMessage = "Since yesterday my Barclays account shows the same balance and no new transactions when I press refresh.";

        public static List<SupportTicket> SupportTickets()
        {
            var now = DateTime.Now;
            return new()
            {
                new()
                {
                    Id = ChatTicketId, Reference = ChatReference, Area = "Bank connections", Subject = ChatSubject, Message = ChatMessage,
                    Status = "In progress", Channel = "chat", CreatedAt = now.AddHours(-3), UpdatedAt = now.AddMinutes(-12), Replies = 2,
                    LastStaffReplyAt = now.AddMinutes(-12), ChatUrl = ChatUrl ?? "https://api.smartcubemobile.com/site/chat.html",
                },
                new()
                {
                    Id = ChatTicketId + 1, Reference = "SC-00037", Area = "Utilities & energy bills", Subject = "Octopus bill imported twice",
                    Message = "My September electricity bill appears twice in Bill History.", Status = "Resolved", Channel = "email",
                    CreatedAt = now.AddDays(-9), UpdatedAt = now.AddDays(-8), Replies = 1, LastStaffReplyAt = now.AddDays(-8),
                },
            };
        }

        public static (bool Ok, string Error, SupportTicket Ticket, List<SupportMessage> Messages) SupportThread(int id)
        {
            var t = SupportTickets().FirstOrDefault(x => x.Id == id);
            if (t == null) return (false, "Ticket not found.", null, new());
            var msgs = new List<SupportMessage> { new() { Author = "You", IsStaff = false, Message = t.Message, CreatedAt = t.CreatedAt } };
            if (id == ChatTicketId)
            {
                msgs.Add(new() { Author = "SmartCube Support", IsStaff = true, CreatedAt = t.CreatedAt.AddMinutes(40),
                    Message = "Hi Alex, thanks for getting in touch. Your bank asks you to renew its permission every 90 days and yours has just run out, which is why nothing new is coming through." });
                msgs.Add(new() { Author = "SmartCube Support", IsStaff = true, CreatedAt = t.LastStaffReplyAt ?? DateTime.Now,
                    Message = "I'm online now if you'd like to go through it together: press Join live chat and I'll walk you through reconnecting. It takes about a minute." });
            }
            else
            {
                msgs.Add(new() { Author = "SmartCube Support", IsStaff = true, CreatedAt = t.CreatedAt.AddHours(5),
                    Message = "Thanks Alex. That was caused by the same PDF being dropped in twice; select the extra row in Bill History and choose Remove Bill. The next update stops duplicates being added." });
            }
            return (true, null, t, msgs);
        }

        #endregion

        // Twelve months of made-up net worth for the Charts section, ending near today's demo balances.
        public static List<MockChartPoint> NetWorthHistory()
        {
            decimal[] values = { 13900, 14350, 14120, 15040, 15680, 15420, 16390, 17150, 17820, 18460, 19310, 20243 };
            var thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return values.Select((v, i) => new MockChartPoint { Date = thisMonth.AddMonths(i - 11), Value = v, Category = "Total" }).ToList();
        }

        private static void WriteNewtonsoft(string file, object data) =>
            File.WriteAllText(Path.Combine(AppPaths.Data, file), Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented));

        private static void WriteSystemJson(string file, object data) =>
            File.WriteAllText(Path.Combine(AppPaths.Data, file), System.Text.Json.JsonSerializer.Serialize(data, data.GetType(),
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

        #region Bills

        private static readonly int[] ElecKwh = { 0, 340, 310, 290, 250, 230, 215, 210, 215, 235, 270, 310, 345 };
        private static readonly int[] GasKwh = { 0, 1450, 1250, 1050, 650, 380, 220, 180, 180, 300, 650, 1050, 1400 };

        private static decimal EnergyAmount(int kwh, decimal ratePence, decimal standingPence, int days) =>
            Math.Round((kwh * ratePence + days * standingPence) / 100m * 1.05m, 2);

        private static List<MockUtilityBill> BuildBills()
        {
            var bills = new List<MockUtilityBill>();
            var thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            decimal elecMeter = 41250, gasMeter = 8830;

            for (int back = 12; back >= 1; back--)
            {
                var start = thisMonth.AddMonths(-back);
                var end = start.AddMonths(1).AddDays(-1);
                int days = end.Day;
                var billDate = end.AddDays(3);
                if (billDate > DateTime.Today) continue;
                var period = $"{start:dd MMM yyyy} - {end:dd MMM yyyy}";

                int e = ElecKwh[start.Month], g = GasKwh[start.Month];
                bills.Add(new MockUtilityBill
                {
                    BillDate = billDate, Supplier = "Octopus Energy", FuelType = "Electricity", UoM = "kWh",
                    UnitsUsed = e, UnitRatePence = 24.50m, StandingChargePence = 60.10m,
                    Amount = EnergyAmount(e, 24.50m, 60.10m, days), Period = period, PeriodStart = start, PeriodEnd = end,
                    Address = $"{Address}, {Postcode}", IsFromPdf = true,
                    MeterReadingStart = elecMeter, MeterReadingEnd = elecMeter + e,
                });
                elecMeter += e;
                bills.Add(new MockUtilityBill
                {
                    BillDate = billDate, Supplier = "Octopus Energy", FuelType = "Gas", UoM = "kWh",
                    UnitsUsed = g, UnitRatePence = 6.24m, StandingChargePence = 31.43m,
                    Amount = EnergyAmount(g, 6.24m, 31.43m, days), Period = period, PeriodStart = start, PeriodEnd = end,
                    Address = $"{Address}, {Postcode}", IsFromPdf = true,
                    MeterReadingStart = gasMeter, MeterReadingEnd = gasMeter + Math.Round(g / 11.2m),
                });
                gasMeter += Math.Round(g / 11.2m);

                if (back <= 6)
                {
                    bills.Add(new MockUtilityBill
                    {
                        BillDate = start.AddDays(11), Supplier = "Sky", FuelType = "Broadband", UoM = "", Amount = 38.00m,
                        Period = $"{start:dd MMM yyyy} - {end:dd MMM yyyy}", PeriodStart = start, PeriodEnd = end,
                        Address = $"{Address}, {Postcode}", IsFromPdf = true, VatRate = 20,
                    });
                    bills.Add(new MockUtilityBill
                    {
                        BillDate = start.AddDays(15), Supplier = "EE", FuelType = "Mobile", UoM = "", Amount = 24.00m,
                        Period = $"{start:dd MMM yyyy} - {end:dd MMM yyyy}", PeriodStart = start, PeriodEnd = end,
                        Address = $"{Address}, {Postcode}", IsFromPdf = true, VatRate = 20,
                    });
                }
            }

            // Water: billed every six months, paid monthly.
            foreach (var month in new[] { 4, 10 })
            {
                var d = new DateTime(DateTime.Today.Year, month, 1);
                if (d > DateTime.Today) d = d.AddYears(-1);
                bills.Add(new MockUtilityBill
                {
                    BillDate = d, Supplier = "Yorkshire Water", FuelType = "Water", UoM = "m³", UnitsUsed = 48,
                    UnitRatePence = 361.5m, StandingChargePence = 22.4m, Amount = 255.00m, VatRate = 0,
                    Period = $"{d.AddMonths(-6):dd MMM yyyy} - {d.AddDays(-1):dd MMM yyyy}", PeriodStart = d.AddMonths(-6), PeriodEnd = d.AddDays(-1),
                    Address = $"{Address}, {Postcode}", IsFromPdf = true,
                });
            }
            return bills;
        }

        #endregion

        #region Banking

        private static (List<MockAccount>, List<MockTransaction>) BuildBanking(List<MockUtilityBill> bills)
        {
            var today = DateTime.Today;
            var rng = new Random(20261001);
            var tx = new List<MockTransaction>();

            void Add(DateTime d, string desc, decimal amount, string type, string category, string account = Current)
            {
                if (d.Date > today) return;
                tx.Add(new MockTransaction { Date = d.Date.AddHours(9 + rng.Next(0, 9)), Description = desc, Amount = amount, Type = type, Category = category, AccountName = account });
            }
            decimal Between(double lo, double hi) => Math.Round((decimal)(lo + rng.NextDouble() * (hi - lo)), 2);

            var firstMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-6);
            for (int m = 0; m <= 6; m++)
            {
                var ms = firstMonth.AddMonths(m);
                int dim = DateTime.DaysInMonth(ms.Year, ms.Month);
                DateTime D(int day) => ms.AddDays(Math.Min(day, dim) - 1);

                // Income and the big monthly bills.
                Add(D(28), "ACME DIGITAL LTD SALARY", 2850.00m, "BACS", "Income");
                Add(D(1), "NATIONWIDE BS MORTGAGE", -845.00m, "DD", "Bills");
                Add(D(1), "LEEDS CITY COUNCIL CTAX", -168.00m, "DD", "Bills");
                Add(D(2), "VANGUARD ASSET MGMT", -200.00m, "DD", "Investments");

                // Energy: pays last month's electricity + gas bill.
                var prevStart = ms.AddMonths(-1);
                var energy = bills.Where(b => b.Supplier == "Octopus Energy" && b.PeriodStart == prevStart).Sum(b => b.Amount);
                if (energy > 0) Add(D(8), "OCTOPUS ENERGY", -energy, "DD", "Energy");

                Add(D(5), "YORKSHIRE WATER", -42.50m, "DD", "Water");
                Add(D(14), "SKY DIGITAL", -38.00m, "DD", "Bills");
                Add(D(18), "EE LIMITED", -24.00m, "DD", "Bills");

                // Insurance.
                Add(D(3), "AVIVA INSURANCE", -41.20m, "DD", "Insurance");
                Add(D(3), "DIRECT LINE HOME INS", -18.75m, "DD", "Insurance");
                Add(D(6), "LEGAL AND GENERAL", -22.40m, "DD", "Insurance");
                Add(D(10), "PETPLAN", -19.80m, "DD", "Insurance");

                // Subscriptions.
                Add(D(5), "NETFLIX.COM", -10.99m, "Card", "Subscription");
                Add(D(9), "SPOTIFY UK", -11.99m, "Card", "Subscription");
                Add(D(12), "DISNEY PLUS", -7.99m, "Card", "Subscription");
                Add(D(15), "PUREGYM LTD", -24.99m, "DD", "Subscription");
                Add(D(20), "AMAZON PRIME", -8.99m, "Card", "Subscription");

                // Credit card repayment (both sides), crypto, savings.
                var cardPay = Between(260, 460);
                Add(D(22), "BARCLAYCARD", -cardPay, "DD", "Bills");
                Add(D(22), "PAYMENT RECEIVED - THANK YOU", cardPay, "BACS", "Payment", Card);
                Add(D(16), "COINBASE", -100.00m, "Card", "Crypto");
                Add(D(29), "TRANSFER TO EVERYDAY SAVER", -300.00m, "FP", "Transfer");
                Add(D(29), "TRANSFER FROM CURRENT ACCOUNT", 300.00m, "FP", "Transfer", Saver);
                Add(D(dim), "INTEREST PAID", Between(21, 27), "BACS", "Income", Saver);

                // Joint account.
                Add(D(1), "J MORGAN", 650.00m, "FP", "Income", Joint);
                Add(D(4), "LITTLE ACORNS NURSERY", -420.00m, "SO", "Bills", Joint);
                Add(D(7), "TV LICENSING", -14.54m, "DD", "Bills", Joint);

                // Everyday spending.
                string[] supermarkets = { "TESCO STORES 3214", "SAINSBURYS S/MKTS", "ALDI STORES", "MORRISONS" };
                string[] eating = { "NANDOS LEEDS", "COSTA COFFEE", "PRET A MANGER", "DELIVEROO", "WAGAMAMA", "GREGGS", "MCDONALDS" };
                string[] shops = { "AMAZON.CO.UK", "ARGOS", "B&Q LEEDS", "NEXT RETAIL", "CURRYS ONLINE", "BOOTS" };
                for (int day = 1; day <= dim; day++)
                {
                    var d = D(day);
                    if (d.DayOfWeek == DayOfWeek.Saturday)
                        Add(d, supermarkets[rng.Next(supermarkets.Length)], -Between(48, 96), "Card", "Groceries");
                    if (d.DayOfWeek == DayOfWeek.Wednesday)
                        Add(d, "CO-OP FOOD", -Between(7, 24), "Card", "Groceries", rng.Next(3) == 0 ? Joint : Current);
                    if (d.DayOfWeek == DayOfWeek.Friday && (d.Day / 7) % 2 == 0)
                        Add(d, rng.Next(2) == 0 ? "SHELL HEADINGLEY" : "BP KIRKSTALL ROAD", -Between(46, 68), "Card", "Transport");
                }
                for (int i = 0; i < 5; i++)
                    Add(D(rng.Next(1, dim + 1)), eating[rng.Next(eating.Length)], -Between(3.4, 42), "Card", "Eating Out");
                for (int i = 0; i < 3; i++)
                    Add(D(rng.Next(1, dim + 1)), shops[rng.Next(shops.Length)], -Between(11, 118), "Card", "Shopping");
                Add(D(rng.Next(2, 27)), "TRAINLINE", -Between(18, 54), "Card", "Transport");
                Add(D(rng.Next(2, 27)), "CASH WITHDRAWAL LINK ATM", -(rng.Next(2) == 0 ? 40m : 60m), "ATM", "Cash");

                // Credit card purchases.
                string[] cardShops = { "JOHN LEWIS", "IKEA LEEDS", "BRITISH AIRWAYS", "TK MAXX", "M&S SIMPLY FOOD", "WATERSTONES" };
                for (int i = 0; i < 4; i++)
                    Add(D(rng.Next(1, dim + 1)), cardShops[rng.Next(cardShops.Length)], -Between(14, 140), "Card", "Shopping", Card);
            }

            var accounts = new List<MockAccount>
            {
                new() { Institution = "Barclays", AccountName = Current, SortCode = "20-45-12", AccountNumber = "****4821", AccountType = "Current", TotalBalance = 2846.37m, AvailableBalance = 2846.37m },
                new() { Institution = "Barclays", AccountName = Saver, SortCode = "20-45-12", AccountNumber = "****7730", AccountType = "Savings", TotalBalance = 8250.00m, AvailableBalance = 8250.00m },
                new() { Institution = "Nationwide", AccountName = Joint, SortCode = "07-01-16", AccountNumber = "****1946", AccountType = "Current", TotalBalance = 1204.18m, AvailableBalance = 1204.18m },
                new() { Institution = "Barclaycard", AccountName = Card, SortCode = "", AccountNumber = "****5512", AccountType = "Credit Card", TotalBalance = -412.60m, AvailableBalance = 4587.40m },
            };
            return (accounts, tx);
        }

        #endregion

        private static List<MockInvestment> BuildInvestments() => new()
        {
            new() { Symbol = "VWRL", Name = "Vanguard FTSE All-World ETF", Type = "ETF", Shares = 62, PricePence = 10420, CostBasisPence = 8760, Change1D = 0.42m, DividendYield = 1.6m, Sector = "Global Equity", Platform = "Vanguard" },
            new() { Symbol = "VUSA", Name = "Vanguard S&P 500 ETF", Type = "ETF", Shares = 40, PricePence = 8315, CostBasisPence = 6540, Change1D = 0.61m, DividendYield = 1.1m, Sector = "US Equity", Platform = "Vanguard" },
            new() { Symbol = "LLOY", Name = "Lloyds Banking Group", Type = "Share", Shares = 2400, PricePence = 71.2m, CostBasisPence = 48.5m, Change1D = -0.35m, DividendYield = 4.4m, Sector = "Financials", Platform = "Hargreaves Lansdown" },
            new() { Symbol = "SHEL", Name = "Shell plc", Type = "Share", Shares = 55, PricePence = 2684, CostBasisPence = 2310, Change1D = 0.18m, DividendYield = 3.9m, Sector = "Energy", Platform = "Hargreaves Lansdown" },
            new() { Symbol = "AZN", Name = "AstraZeneca", Type = "Share", Shares = 12, PricePence = 11850, CostBasisPence = 10240, Change1D = -0.22m, DividendYield = 2.0m, Sector = "Healthcare", Platform = "Hargreaves Lansdown" },
        };

        private static List<InsurancePolicy> BuildInsurance()
        {
            var t = DateTime.Today;
            return new()
            {
                new() { Id = "demo-car", Category = "Car", PolicyType = "Comprehensive", Provider = "Aviva", MonthlyPremium = 41.20m, AnnualPremium = 494.40m, CoverAmount = 0, PolicyNumber = "AV-48213907", Excess = 350, StartDate = t.AddMonths(-7), RenewalDate = t.AddMonths(5), VehicleReg = "AM21 XYZ", VehicleMakeModel = "Volkswagen Golf 1.5 TSI", NamedInsured = FullName },
                new() { Id = "demo-home", Category = "House", PolicyType = "Buildings & Contents", Provider = "Direct Line", MonthlyPremium = 18.75m, AnnualPremium = 225.00m, CoverAmount = 500000, PolicyNumber = "DL-HM-7730218", Excess = 250, StartDate = t.AddMonths(-10), RenewalDate = t.AddMonths(2), PropertyAddress = $"{Address}, {Postcode}", NamedInsured = FullName },
                new() { Id = "demo-life", Category = "Life", PolicyType = "Level Term (25 years)", Provider = "Legal & General", MonthlyPremium = 22.40m, AnnualPremium = 268.80m, CoverAmount = 250000, PolicyNumber = "LG-0094-55821", Excess = 0, StartDate = new DateTime(2019, 6, 14), RenewalDate = null, NamedInsured = FullName },
                new() { Id = "demo-pet", Category = "Pet", PolicyType = "Lifetime Cover", Provider = "Petplan", MonthlyPremium = 19.80m, AnnualPremium = 237.60m, CoverAmount = 7000, PolicyNumber = "PP-3302117", Excess = 110, StartDate = t.AddMonths(-4), RenewalDate = t.AddMonths(8), PetName = "Bella", PetBreed = "Labrador Retriever", NamedInsured = FullName },
            };
        }

        private static UserProfileData BuildProfile()
        {
            var t = DateTime.Today;
            return new UserProfileData
            {
                FullName = FullName, DateOfBirth = "14/03/1988", Email = Email, Phone = "07700 900123",
                NiNumber = "QQ 12 34 56 C", EmploymentStatus = "Employed", Employer = "Acme Digital Ltd", AnnualIncome = 46000,
                TaxCode = "1257L", Dependants = 1, MaritalStatus = "Married", Gender = "—",
                CreditScore = "742", CreditScoreProvider = "Experian", CreditScoreDate = t.AddDays(-12).ToString("dd/MM/yyyy"),
                CreditScoreHistory = new()
                {
                    new() { Score = 698, Provider = "Experian", Date = t.AddMonths(-9).ToString("dd/MM/yyyy") },
                    new() { Score = 715, Provider = "Experian", Date = t.AddMonths(-6).ToString("dd/MM/yyyy") },
                    new() { Score = 731, Provider = "Experian", Date = t.AddMonths(-3).ToString("dd/MM/yyyy") },
                    new() { Score = 742, Provider = "Experian", Date = t.AddDays(-12).ToString("dd/MM/yyyy") },
                },
                DrivingLicenceNumber = "MORGA803148A99XX", LicenceIssueDate = "02/09/2019", LicenceExpiryDate = "01/09/2029", DrivingCategories = "AM, B, BE",
                PassportNumber = "999228847", PassportExpiry = "18/05/2031", Nationality = "British", PlaceOfBirth = "Leeds",
                SmartScanLicence = "DEMO-0000-0000", SmartScanPromptShown = true,
                Properties = new()
                {
                    new()
                    {
                        Id = "demo-home", Address = Address, Postcode = Postcode, PropertyType = "Semi-detached house", Bedrooms = 3, Tenure = "Freehold",
                        EstimatedValue = 285000, PurchasePrice = 212000, PurchaseDate = "14/06/2019", EpcRating = "C", CouncilTaxBand = "C", IsPrimary = true,
                        FloorArea = "94 m²", WallsType = "Cavity wall, filled", RoofType = "Pitched, 270 mm loft insulation", HeatingType = "Gas boiler and radiators",
                        WindowsType = "Double glazing", HotWater = "From main system", Lighting = "Low energy lighting in all fixed outlets", EpcExpiry = "22/04/2029",
                        LastSaleDate = "14/06/2019", LastSalePrice = 212000,
                        MaintenanceHistory = new()
                        {
                            new() { Id = "m1", Date = t.AddMonths(-5).ToString("dd/MM/yyyy"), Description = "Boiler service", Cost = 85, Category = "Heating" },
                            new() { Id = "m2", Date = t.AddMonths(-14).ToString("dd/MM/yyyy"), Description = "Replaced guttering at the rear", Cost = 420, Category = "Exterior" },
                        },
                        Contractors = new() { new() { Id = "c1", Name = "Northside Heating", Trade = "Gas engineer", Phone = "0113 496 0102", Email = "hello@northsideheating.example" } },
                    },
                },
                Vehicles = new()
                {
                    new()
                    {
                        Id = "demo-car", Registration = "AM21 XYZ", Make = "Volkswagen", Model = "Golf 1.5 TSI", Year = 2021, FuelType = "Petrol", Colour = "Grey",
                        Mileage = 38420, MotExpiry = t.AddMonths(4).ToString("dd/MM/yyyy"), MotTestDate = t.AddMonths(-8).ToString("dd/MM/yyyy"), MotResult = "Pass",
                        MotAdvisories = new() { "Nearside rear tyre worn close to legal limit", "Front brake discs slightly worn" },
                        MileageHistory = new()
                        {
                            new() { Date = t.AddMonths(-20).ToString("dd/MM/yyyy"), Miles = 21300, Notes = "MOT" },
                            new() { Date = t.AddMonths(-8).ToString("dd/MM/yyyy"), Miles = 31850, Notes = "MOT" },
                            new() { Date = t.AddDays(-20).ToString("dd/MM/yyyy"), Miles = 38420, Notes = "Service" },
                        },
                        TaxExpiry = t.AddMonths(6).ToString("dd/MM/yyyy"), EstimatedValue = 14250,
                        PreferredGarages = new() { new() { Id = "g1", Name = "Headingley Autocentre", Address = "22 Station Parade, Leeds", Postcode = "LS6 3AA", Phone = "0113 496 0155", ContactName = "Sam", ContactEmail = "bookings@headingleyauto.example" } },
                    },
                },
            };
        }

        #region Crypto

        private static List<MockCryptoHolding> BuildCrypto()
        {
            var t = DateTime.Today;
            const decimal btc = 58400m, eth = 2045m, sol = 118m;
            const string ledgerBtc = "bc1qdemo7k2f...x9a4", ledgerEth = "0x4F2a...9C1e";

            MockCryptoTransaction Buy(string sym, decimal qty, decimal price, DateTime d, string id) => new()
            { Symbol = sym, Type = "Buy", Quantity = qty, PriceAtTime = price, PriceSource = "exchange", Date = d, Hash = id };

            var cbBtcSend = new MockCryptoTransaction { Symbol = "BTC", Type = "Send", Quantity = 0.05m, PriceAtTime = 44100m, PriceSource = "exchange", Date = t.AddMonths(-12).AddHours(11), Hash = "7d3f9a1c...b2e4", ToAddress = "bc1qdemo7k2fq0w8x9a4" };
            var cbEthSend = new MockCryptoTransaction { Symbol = "ETH", Type = "Send", Quantity = 0.75m, PriceAtTime = 1720m, PriceSource = "exchange", Date = t.AddMonths(-9).AddHours(15), Hash = "0x91be44a7...0c3d", ToAddress = "0x4F2a81d6b3e09C1e" };

            decimal[] Spark(decimal now, params double[] steps) => steps.Select(s => Math.Round(now * (decimal)s, 2)).ToArray();

            return new()
            {
                new()
                {
                    Symbol = "BTC", Name = "Bitcoin", Quantity = 0.035m, PriceGBP = btc, PriceUSD = btc * 1.27m, Change24h = 1.42m,
                    WalletLabel = "Coinbase", WalletAddress = "Coinbase account", Network = "coinbase",
                    PriceHistory7d = Spark(btc, 0.962, 0.971, 0.968, 0.984, 0.979, 0.991, 1.0),
                    Transactions = new()
                    {
                        Buy("BTC", 0.05m, 41000m, t.AddMonths(-14).AddHours(10), "cb-demo-0001"),
                        cbBtcSend,
                        Buy("BTC", 0.035m, 52000m, t.AddMonths(-5).AddHours(13), "cb-demo-0002"),
                    },
                },
                new()
                {
                    Symbol = "ETH", Name = "Ethereum", Quantity = 0.45m, PriceGBP = eth, PriceUSD = eth * 1.27m, Change24h = -0.68m,
                    WalletLabel = "Coinbase", WalletAddress = "Coinbase account", Network = "coinbase",
                    PriceHistory7d = Spark(eth, 1.031, 1.024, 1.012, 1.018, 1.006, 1.009, 1.0),
                    Transactions = new()
                    {
                        Buy("ETH", 1.2m, 1650m, t.AddMonths(-10).AddHours(9), "cb-demo-0003"),
                        cbEthSend,
                    },
                },
                new()
                {
                    Symbol = "SOL", Name = "Solana", Quantity = 8m, PriceGBP = sol, PriceUSD = sol * 1.27m, Change24h = 2.95m,
                    WalletLabel = "Coinbase", WalletAddress = "Coinbase account", Network = "coinbase",
                    PriceHistory7d = Spark(sol, 0.921, 0.948, 0.937, 0.962, 0.975, 0.969, 1.0),
                    Transactions = new() { Buy("SOL", 8m, 95m, t.AddMonths(-4).AddHours(17), "cb-demo-0004") },
                },
                new()
                {
                    Symbol = "BTC", Name = "Bitcoin", Quantity = 0.0499m, PriceGBP = btc, PriceUSD = btc * 1.27m, Change24h = 1.42m,
                    WalletLabel = "Ledger", WalletAddress = ledgerBtc, Network = "btc",
                    PriceHistory7d = Spark(btc, 0.962, 0.971, 0.968, 0.984, 0.979, 0.991, 1.0),
                    Transactions = new()
                    {
                        new() { Symbol = "BTC", Type = "Receive", Quantity = 0.0499m, PriceAtTime = 44100m, PriceSource = "daily", Date = cbBtcSend.Date.AddMinutes(24), Hash = "7d3f9a1c...b2e4", FromAddress = "bc1qexchangehot3p7", ToAddress = "bc1qdemo7k2fq0w8x9a4" },
                    },
                },
                new()
                {
                    Symbol = "ETH", Name = "Ethereum", Quantity = 0.7495m, PriceGBP = eth, PriceUSD = eth * 1.27m, Change24h = -0.68m,
                    WalletLabel = "Ledger", WalletAddress = ledgerEth, Network = "eth",
                    PriceHistory7d = Spark(eth, 1.031, 1.024, 1.012, 1.018, 1.006, 1.009, 1.0),
                    Transactions = new()
                    {
                        new() { Symbol = "ETH", Type = "Receive", Quantity = 0.7495m, PriceAtTime = 1720m, PriceSource = "network", Date = cbEthSend.Date.AddMinutes(6), Hash = "0x91be44a7...0c3d", FromAddress = "0xA9d1e08C7793af67", ToAddress = "0x4F2a81d6b3e09C1e" },
                    },
                },
            };
        }

        #endregion
    }
}
