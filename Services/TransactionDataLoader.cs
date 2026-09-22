using SmartCubeMobile.MockData;
using System.Globalization;

namespace SmartCubeMobile.Services
{
    public static class TransactionDataLoader
    {
        private static List<MockAccount> _accounts;
        private static List<MockTransaction> _transactions;
        private static bool _loaded;

        private static readonly Dictionary<string, string> SortCodeInstitutions = new()
        {
            { "070040", "Nationwide" },
            { "070116", "Nationwide" },
            { "071310", "Nationwide" },
            { "090126", "Santander" },
            { "200000", "Barclays" },
            { "204512", "Barclays" },
            { "110233", "Halifax" },
            { "309608", "Lloyds" },
            { "601422", "NatWest" },
        };

        private static readonly Dictionary<string, string> AccountNames = new()
        {
            { "070116/10722017", "FlexDirect" },
            { "070040/36737571", "Savings" },
            { "071310/86529847", "Monthly Saver" },
            { "070116/98765432", "Joint Account" },
            { "090126/56115735", "Current Account" },
            { "090126/87900016", "Savings Account" },
        };

        public static bool IsLoaded => _loaded;

        public static void LoadFromFile(string filePath)
        {
            if (_loaded) return;
            if (!File.Exists(filePath)) return;

            var accounts = new Dictionary<string, MockAccount>();
            var transactions = new List<MockTransaction>();
            var lastBalance = new Dictionary<string, decimal>();

            var lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = line.Split('\t');
                if (cols.Length < 27) continue;

                var username = cols[0].Trim();
                var sortCode = cols[4].Trim();
                var accountNo = cols[5].Trim();
                var heading = cols[15].Trim();
                var description = cols[17].Trim();
                var isCrypto = sortCode.Equals("Coinbase", StringComparison.OrdinalIgnoreCase);

                if (isCrypto) continue;

                if (!DateTime.TryParse(cols[11].Trim(), CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var txnDate))
                    continue;

                if (!decimal.TryParse(cols[20].Trim(), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var amountPence))
                    continue;

                decimal.TryParse(cols[22].Trim(), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var balancePence);

                var amount = amountPence / 100m;
                var balance = balancePence / 100m;
                var category = CategoriseHeading(heading, description);
                var txnType = DeriveType(heading, description, cols);
                var acctKey = $"{sortCode}/{accountNo}";

                var institution = ResolveInstitution(sortCode);
                var acctName = ResolveAccountName(acctKey, username);

                if (!accounts.ContainsKey(acctKey))
                {
                    accounts[acctKey] = new MockAccount
                    {
                        Institution = institution,
                        AccountName = acctName,
                        SortCode = FormatSortCode(sortCode),
                        AccountNumber = MaskAccountNo(accountNo),
                        TotalBalance = balance,
                        AvailableBalance = balance,
                        AccountType = sortCode.Contains("0572") ? "Credit Card" : "Current",
                        CurrencyCode = "GBP",
                    };
                }

                lastBalance[acctKey] = balance;

                transactions.Add(new MockTransaction
                {
                    Date = txnDate,
                    Description = CleanDescription(heading, description),
                    Category = category,
                    Amount = amount,
                    Balance = balance,
                    AccountName = acctName,
                    Type = txnType,
                });
            }

            foreach (var kvp in lastBalance)
            {
                if (accounts.TryGetValue(kvp.Key, out var acct))
                {
                    acct.TotalBalance = kvp.Value;
                    acct.AvailableBalance = kvp.Value;
                }
            }

            _accounts = accounts.Values.ToList();
            _transactions = transactions.OrderByDescending(t => t.Date).ToList();
            _loaded = true;
        }

        public static List<MockAccount> GetAccounts() => _accounts ?? new();
        public static List<MockTransaction> GetTransactions() => _transactions ?? new();

        private static string ResolveInstitution(string sortCode)
        {
            var clean = sortCode.Replace("-", "").Replace(" ", "");
            if (clean.Length >= 6 && SortCodeInstitutions.TryGetValue(clean[..6], out var inst))
                return inst;
            if (clean.Length >= 4)
            {
                foreach (var kvp in SortCodeInstitutions)
                {
                    if (kvp.Key.StartsWith(clean[..4])) return kvp.Value;
                }
            }
            return string.IsNullOrEmpty(clean) ? "Unknown" : "Bank";
        }

        private static string ResolveAccountName(string acctKey, string username)
        {
            if (AccountNames.TryGetValue(acctKey, out var name)) return name;
            return $"{username} Account";
        }

        private static string FormatSortCode(string sc)
        {
            var clean = sc.Replace("-", "").Replace(" ", "");
            if (clean.Length == 6)
                return $"{clean[..2]}-{clean[2..4]}-{clean[4..6]}";
            return sc;
        }

        private static string MaskAccountNo(string acctNo)
        {
            if (acctNo.Length >= 8)
                return $"****{acctNo[^4..]}";
            return acctNo;
        }

        private static string CleanDescription(string heading, string description)
        {
            if (!string.IsNullOrEmpty(heading) && heading.Length > 2)
                return heading;
            if (description.Length > 60)
                return description[..57] + "...";
            return description;
        }

        private static string DeriveType(string heading, string description, string[] cols)
        {
            var desc = description.ToUpperInvariant();
            if (desc.Contains("CARD PAYMENT")) return "Card";
            if (desc.Contains("DIRECT DEBIT")) return "DD";
            if (desc.Contains("STANDING ORDER")) return "SO";
            if (desc.Contains("FASTER PAYMENT")) return "FP";
            if (desc.Contains("BILL PAYMENT")) return "BP";
            if (desc.Contains("CASH WITHDRAWAL") || desc.Contains("ATM")) return "ATM";
            if (desc.Contains("INTEREST") || heading.Contains("Interest")) return "INT";
            if (desc.Contains("SALARY") || desc.Contains("WAGES")) return "BACS";
            if (desc.Contains("CHEQUE")) return "CHQ";
            if (heading.Contains("DWP") || heading.Contains("PENSION") ||
                heading.Contains("ANNUITY")) return "BACS";
            return "Other";
        }

        public static string CategoriseHeading(string heading, string description)
        {
            var h = heading.ToUpperInvariant();
            var d = description.ToUpperInvariant();

            // Groceries
            if (h.Contains("TESCO") || h.Contains("SAINSBURY") || h.Contains("CO OP GROUP") ||
                h.Contains("ASDA") || h.Contains("ALDI") || h.Contains("LIDL") ||
                h.Contains("MARKS&SPENCER") || h.Contains("M&S FOOD") || h.Contains("MORRISONS") ||
                h.Contains("ICELAND") || h.Contains("FARMFOODS") || h.Contains("OCADO"))
                return "Groceries";

            // Eating Out
            if (h.Contains("COSTA") || h.Contains("MCDONALDS") || h.Contains("STARBUCKS") ||
                h.Contains("GREGGS") || h.Contains("SUBWAY") || h.Contains("NANDOS") ||
                h.Contains("TASTE FOR LIFE") || h.Contains("KFC") || h.Contains("PIZZA") ||
                h.Contains("RESTAURANT") || h.Contains("CAFE") || h.Contains("WETHERSPOON") ||
                h.Contains("ROYLE GREEN") || d.Contains("INDIAN NIBBLES"))
                return "Eating Out";

            // Investments & Pensions
            if (h.Contains("STANDARD LIFE") || h.Contains("AJ BELL") || h.Contains("AVIVA") ||
                h.Contains("PRUDENTIAL") || h.Contains("CANADA LIFE") || h.Contains("L&G") ||
                h.Contains("LEGAL & GENERAL") || h.Contains("SCOTTISH WIDOWS") ||
                h.Contains("HARGREAVES"))
                return "Investments";

            // State Pension / Benefits
            if (h.Contains("DWP") || d.Contains("DWP") || d.Contains("STATE PENSION"))
                return "State Pension";

            // Cash Withdrawals
            if (h.Contains("ATM") || h.Contains("CASH WITHDRAWAL") || d.Contains("CASH WITHDRAWAL"))
                return "Cash";

            // Shopping & Retail
            if (h.Contains("IKEA") || h.Contains("B&M") || h.Contains("WILKO") || h.Contains("B&Q") ||
                h.Contains("TK MAXX") || h.Contains("ARGOS") || h.Contains("AMAZON") ||
                h.Contains("WYNORS") || h.Contains("HOME BARGAINS") || h.Contains("PRIMARK") ||
                h.Contains("MATALAN") || h.Contains("NEXT ") || h.Contains("JOHN LEWIS") ||
                h.Contains("EBAY") || h.Contains("PP*") || h.Contains("PAYPAL"))
                return "Shopping";

            // Family Transfers
            if (h.Contains("CHAPMAN") || h.Contains("MARSH") || h.Contains("ANNA") ||
                h.Contains("AMELIA") || h.Contains("MARIA") || h.Contains("R CHAPMAN") ||
                h.Contains("MRS&MR") || d.Contains("CHAPMAN") || d.Contains("MARSH"))
                return "Family";

            // Bills & Utilities
            if (h.Contains("BRITISH GAS") || h.Contains("EDF") || h.Contains("SSE") ||
                h.Contains("COUNCIL TAX") || h.Contains("WATER") || h.Contains("SEVERN TRENT") ||
                h.Contains("THAMES WATER") || h.Contains("TV LICENCE") ||
                h.Contains("VIRGIN MEDIA") || h.Contains("SKY") || h.Contains("BT ") ||
                d.Contains("COUNCIL TAX") || d.Contains("DIRECT DEBIT"))
                return "Bills";

            // Leisure & Fitness
            if (h.Contains("LEISURE") || h.Contains("GYM") || h.Contains("SWIMMING") ||
                h.Contains("EVERYBODY") || h.Contains("WILMSLOW"))
                return "Leisure";

            // Health & Personal Care
            if (h.Contains("BOOTS") || h.Contains("SUPERDRUG") || h.Contains("HAIR") ||
                h.Contains("V12 HAIR") || h.Contains("PHARMACY"))
                return "Health";

            // Charity
            if (h.Contains("PALESTINIAN") || h.Contains("CHARITY") || h.Contains("OXFAM") ||
                h.Contains("RED CROSS") || h.Contains("DONATE"))
                return "Charity";

            // Education
            if (h.Contains("SCHOOL") || h.Contains("GREENBANK") || h.Contains("COLLEGE") ||
                h.Contains("UNIVERSITY"))
                return "Education";

            // Transport
            if (h.Contains("FUEL") || h.Contains("PETROL") || h.Contains("SHELL") ||
                h.Contains("BP ") || h.Contains("ESSO") || h.Contains("TRAIN") ||
                h.Contains("RAIL") || h.Contains("PARKING") || h.Contains("HALFORDS") ||
                d.Contains("PETROL") || d.Contains("FUEL"))
                return "Transport";

            // Insurance
            if (h.Contains("INSURANCE") || d.Contains("INSURANCE") ||
                h.Contains("LIFE") && !h.Contains("STANDARD LIFE"))
                return "Insurance";

            // Interest
            if (h.Contains("INTEREST") || d.Contains("INTEREST ADDED") || d.Contains("INTEREST PAID"))
                return "Interest";

            // Non-Sterling / International
            if (h.Contains("NON-STERLING") || d.Contains("NON-STERLING"))
                return "International";

            // Internal bank transfers
            if (d.Contains("STANDING ORDER") || d.Contains("INTERNAL TRANSFER"))
                return "Transfer";

            // Bill payments to known accounts
            if (h.Contains("BILL PAYMENT") || d.Contains("BILL PAYMENT"))
                return "Transfer";

            // Faster payments (generic)
            if (h.Contains("FASTER PAYMENT") || d.Contains("FASTER PAYMENT"))
                return "Transfer";

            // Income
            if (d.Contains("SALARY") || d.Contains("WAGES") || d.Contains("PAYROLL"))
                return "Income";

            // Subscriptions
            if (h.Contains("NETFLIX") || h.Contains("SPOTIFY") || h.Contains("AMAZON PRIME") ||
                h.Contains("DISNEY") || h.Contains("APPLE"))
                return "Subscriptions";

            return "Other";
        }
    }
}
