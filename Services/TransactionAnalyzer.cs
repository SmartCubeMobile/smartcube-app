using System.Text.Json;
using System.Text.RegularExpressions;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    // Categorises bank transactions from their description and spots repeating payments.
    //   - Same amount on a regular schedule (monthly, weekly, yearly...) -> subscription
    //   - Regular schedule but the amount moves a little each time -> utility bill
    //   - Known merchants (energy suppliers, councils, supermarkets...) get their own category first
    //   - Money moving between the user's own accounts -> Transfer
    // Categories the user picks by hand are remembered per merchant and always win.
    public static class TransactionAnalyzer
    {
        public static readonly string[] Categories =
        {
            "Subscriptions", "Energy Bills", "Water", "Council Tax", "Broadband & Phone", "Insurance",
            "Mortgage & Rent", "Credit Card Payment", "Loans & Credit", "Investments", "Crypto", "Bills & Utilities", "Groceries", "Eating Out", "Fuel",
            "Transport", "Shopping", "Health", "Entertainment", "Cash", "Transfer", "Income", "State Pension", "General",
        };

        // Recurring kinds shown in the analysis panel.
        public const string KindSubscription = "Subscription";
        public const string KindBill = "Bill";
        public const string KindVariableBill = "Utility bill";
        public const string KindStandingOrder = "Standing order";
        public const string KindIncome = "Income";
        public const string KindRegularSpend = "Regular spend";

        private static readonly (string Category, Regex Pattern)[] Keywords = BuildKeywords();

        private static (string, Regex)[] BuildKeywords()
        {
            (string, string)[] raw =
            {
                ("Council Tax", @"COUNCIL|C\.?\s?TAX|BOROUGH|\bMBC\b|\bCBC\b"),
                ("Water", @"\bWATER\b|SEVERN TRENT|UNITED UTILITIES|ANGLIAN|AFFINITY W|WESSEX W|NORTHUMBRIAN|DWR CYMRU|WELSH WATER|SES W|SOUTH EAST W|PORTSMOUTH W|BRISTOL W|SOUTH STAFFS"),
                ("Energy Bills", @"OCTOPUS|BRITISH GAS|\bEDF\b|\bE\.?ON\b|EON NEXT|\bOVO\b|SCOTTISH ?POWER|\bSSE\b|UTILITA|SO ENERGY|SHELL ENERGY|\bBULB\b|UTILITY WAREHOUSE|OUTFOX|CO-?OP(ERATIVE)? ENERGY|ECOTRICITY|GOOD ENERGY|\bNPOWER\b|BOOST POWER|SMARTEST ENERGY|FLOW ENERGY|\bENERGY\b"),
                ("Broadband & Phone", @"\bBT\b|BT GROUP|BRITISH TELECOM|\bSKY\b|VIRGIN MEDIA|VODAFONE|\bEE\b|EE LIMITED|\bO2\b|TELEFONICA|\bTHREE\b|HUTCHISON|TALKTALK|PLUSNET|GIFFGAFF|TESCO MOBILE|\bID MOBILE|\bVOXI\b|LEBARA|LYCAMOBILE|SMARTY|HYPEROPTIC|COMMUNITY FIBRE|NOW BROADBAND|SHELL BROADBAND"),
                ("Insurance", @"INSURANCE|INSURE|ASSURANCE|\bAVIVA\b|ADMIRAL|DIRECT LINE|\bLV\b|LIVERPOOL VICTORIA|\bAXA\b|CHURCHILL|HASTINGS|\bESURE\b|\bAGEAS\b|LEGAL (AND|&) GENERAL|ROYAL LONDON|VITALITY|PETPLAN|\bSAGA\b|ZURICH|ALLIANZ|HISCOX|\bBUPA\b|SHEILAS WHEELS|\bRAC\b|\bAA\b|GREEN FLAG|TV LICEN"),
                ("Mortgage & Rent", @"MORTGAGE|\bRENT\b|LETTINGS|HOUSING ASSOC|ESTATE AGENT"),
                ("Credit Card Payment", @"BARCLAYCARD|AMERICAN EXPRESS|\bAMEX\b|CAPITAL ONE|\bMBNA\b|VANQUIS|\bAQUA\b|NEWDAY|MARBLES|FLUID CARD|\bZABLE\b|TESCO BANK|SAINSBURYS BANK|JOHN LEWIS (FINANCE|CARD)|M&S BANK|CREDIT ?CARD|\bCC PAYMENT|CARD REPAYMENT|HSBC CARD|HALIFAX CARD|LLOYDS CARD|NATWEST CARD|SANTANDER CARD|NATIONWIDE CARD|VIRGIN MONEY CARD"),
                ("Crypto", @"COINBASE|BINANCE|KRAKEN|PAYWARD|CRYPTO\.?COM|FORIS|BITSTAMP|GEMINI|UPHOLD|BITPANDA|\bLEDGER\b|BITCOIN|\bCRYPTO\b|COINJAR|SWISSBORG|\bLUNO\b|COINCORNER|OKX|BYBIT|KUCOIN|WIREX|NEXO"),
                ("Investments", @"VANGUARD|HARGREAVES|AJ ?BELL|FIDELITY|TRADING ?212|FREETRADE|NUTMEG|MONEYBOX|INTERACTIVE INVESTOR|II\.CO\.UK|WEALTHIFY|\bPLUM\b|\bCHIP\b|EVELYN|\bETORO\b|MONEYFARM|INVESTENGINE|STOCKS ?(AND|&) ?SHARES|\bINVEST(MENT)?S?\b|PREMIUM BONDS|NS ?& ?I|NATIONAL SAVINGS|SCOTTISH WIDOWS|STOCKS ?& ?SHARES ISA"),
                ("Loans & Credit", @"\bLOAN\b|KLARNA|CLEARPAY|PAYPAL CREDIT|ZOPA|\bFINANCE\b|MOTOR FINANCE|BLACK HORSE|LENDING|\bPAYPAL PAY IN 3"),
                ("Subscriptions", @"NETFLIX|SPOTIFY|DISNEY|AMAZON PRIME|PRIME VIDEO|AMZN DIGITAL|APPLE\.COM|ITUNES|GOOGLE (PLAY|STORAGE|ONE)|YOUTUBE|MICROSOFT|\bXBOX\b|PLAYSTATION|NINTENDO|NOW ?TV|PATREON|AUDIBLE|\bADOBE\b|DROPBOX|ICLOUD|PURE ?GYM|THE GYM|\bGYM\b|DAVID LLOYD|OPENAI|CHATGPT|ANTHROPIC|CLAUDE\.AI|PARAMOUNT|BRITBOX|DISCOVERY\+|DAZN|DUOLINGO|\bCANVA\b|LINKEDIN|NORTON|MCAFEE"),
                ("Groceries", @"TESCO|SAINSBURY|\bASDA\b|MORRISON|\bALDI\b|\bLIDL\b|CO-?OP\b|CO OP FOOD|WAITROSE|ICELAND|M ?& ?S FOOD|M&S SIMPLY|MARKS (AND|&) SPENCER|\bOCADO\b|FARMFOODS|\bSPAR\b|BUDGENS|COSTCO|ONE STOP|NISA|HERON FOODS|BOOKER"),
                ("Fuel", @"\bSHELL\b|\bBP\b|\bESSO\b|TEXACO|\bGULF\b|\bJET\b|\bMFG\b|MOTOR FUEL|PETROL|FILLING STATION|SERVICE STATION|APPLEGREEN|RONTEC|EUROGARAGES"),
                ("Transport", @"\bTFL\b|TRANSPORT FOR|TRAINLINE|NATIONAL RAIL|\bLNER\b|AVANTI|NORTHERN TRAINS|\bGWR\b|CROSSCOUNTRY|\bUBER\b(?! ?EATS)|\bBOLT\b|STAGECOACH|ARRIVA|FIRST BUS|PARKING|RINGGO|PAYBYPHONE|\bNCP\b|\bDVLA\b|DART CHARGE|TAXI|\bMOT\b|KWIK FIT|HALFORDS"),
                ("Eating Out", @"MCDONALD|\bKFC\b|BURGER KING|NANDO|GREGGS|COSTA|STARBUCKS|\bPRET\b|SUBWAY|DOMINO|PIZZA|JUST EAT|JUST-EAT|DELIVEROO|UBER ?EATS|WETHERSPOON|RESTAURANT|\bCAFE\b|COFFEE|TAKEAWAY|\bPUB\b|\bBAR\b|\bINN\b|TOBY CARVERY|HARVESTER|FRANKIE|WAGAMAMA|FIVE GUYS|TACO BELL|PAPA JOHN|CHIPPY|FISH (AND|&) CHIPS"),
                ("Health", @"\bBOOTS\b|SUPERDRUG|PHARMACY|CHEMIST|DENTAL|DENTIST|OPTICIAN|SPECSAVERS|VISION EXPRESS|\bNHS\b|LLOYDS PHARMACY|HOLLAND (AND|&) BARRETT"),
                ("Entertainment", @"CINEMA|ODEON|CINEWORLD|\bVUE\b|TICKETMASTER|SEE TICKETS|\bSTEAM\b|EVENTBRITE|BOWLING|THEATRE|NATIONAL LOTTERY|CAMELOT|BET365|SKY BET|PADDY ?POWER|WILLIAM HILL|LADBROKES|CORAL"),
                ("Shopping", @"AMAZON|\bAMZN\b|\bEBAY\b|\bARGOS\b|\bB ?& ?Q\b|SCREWFIX|TOOLSTATION|WICKES|\bIKEA\b|CURRYS|JOHN LEWIS|\bNEXT\b|PRIMARK|TK ?MAXX|\bH ?& ?M\b|SHEIN|\bTEMU\b|\bB ?& ?M\b|HOME BARGAINS|POUNDLAND|\bWILKO|DUNELM|\bETSY\b|\bASOS\b|\bJD SPORTS|SPORTS DIRECT|DECATHLON|\bHOBBYCRAFT|WHSMITH|\bWORKS\b|PAYPAL"),
                ("Cash", @"\bATM\b|CASH ?(WITHDRAWAL|MACHINE)?\b|\bLINK\b|NOTEMACHINE|CARDTRONICS"),
                ("Transfer", @"TRANSFER|\bTFR\b|\bTRF\b|TO SAVINGS|FROM SAVINGS|SAVINGS|\bISA\b|MONZO POT|\bPOT\b|OWN ACCOUNT|TO A/C|FROM A/C|INTERNAL"),
            };
            return raw.Select(r => (r.Item1, new Regex(r.Item2, RegexOptions.IgnoreCase | RegexOptions.Compiled))).ToArray();
        }

        private static readonly Regex IncomePattern = new(@"SALARY|WAGES|PAYROLL|\bPAY\b|HMRC|TAX CREDIT|UNIVERSAL CREDIT|CHILD BENEFIT|DIVIDEND|INTEREST", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex PensionPattern = new(@"\bDWP\b.*(PENSION|\bSP\b|STATE)|STATE PENSION|\bDWP\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex OtherPensionPattern = new(@"PENSION|ANNUITY", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #region Merchant names

        private static readonly Regex PrefixPattern = new(
            @"^(CARD PAYMENT( TO)?|CARD PURCHASE|CONTACTLESS( PAYMENT)?|DEBIT CARD|DIRECT DEBIT( PAYMENT)?( TO)?|STANDING ORDER( TO)?|BILL PAYMENT( TO)?|FASTER PAYMENTS?( TO| FROM)?|PAYMENT (TO|FROM)|BANK GIRO CREDIT|CREDIT FROM|TRANSFER (TO|FROM)|DD|SO|BGC|BP|FPO|FPI|VIS|VISA|POS|CHQ|DEB|CPT|TFR|TRF|\)\)\))\s+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ProcessorPattern = new(@"\b(PAYPAL|PP|SQ|SUMUP|ZETTLE|IZ|SP|CRV|WWW)\s*\*\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex NoiseWords = new(@"^(LTD|LIMITED|PLC|UK|GB|GBR|THE|CO|COM|INC|LLC|ONLINE|PAYMENT|PAYMENTS|REF|ON|AT|TO|FROM|DD|SO|CC)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // A stable name for "who was paid": drops bank prefixes, card-processor tags, reference numbers,
        // dates and store numbers, then keeps the first two meaningful words.
        public static string MerchantKey(string description)
        {
            var s = (description ?? "").Trim().ToUpperInvariant();
            for (int i = 0; i < 3; i++) s = PrefixPattern.Replace(s, "");
            s = ProcessorPattern.Replace(s, "");
            var star = s.IndexOf('*');
            if (star > 2) s = s[..star];
            s = Regex.Replace(s, @"\bON \d{1,2} [A-Z]{3}\b.*$", "");
            var words = Regex.Split(s, @"[^A-Z0-9&+']+")
                .Select(w =>
                {
                    // "SPOTIFYP3F2" -> "SPOTIFYP" (a reference glued to the name); pure numbers/refs go.
                    if (!w.Any(char.IsDigit)) return w;
                    var lead = Regex.Match(w, @"^[A-Z]{4,}").Value;
                    return lead;
                })
                .Where(w => w.Length > 0 && !NoiseWords.IsMatch(w))
                .Take(2).ToArray();
            var key = string.Join(" ", words);
            return key.Length > 0 ? key : s.Trim().Length > 0 ? s.Trim() : (description ?? "").Trim().ToUpperInvariant();
        }

        public static string DisplayName(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            // Short words are usually initials (EDF, BT, KFC), so they stay in capitals.
            return string.Join(" ", key.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select((w, i) =>
                w.Length <= 3 && i == 0 ? w : char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant()));
        }

        #endregion

        #region User rules

        private static Dictionary<string, string> _rules;
        private static string RulesPath => Path.Combine(MockDataService.DataDir, "category_rules.json");

        private static Dictionary<string, string> Rules
        {
            get
            {
                if (_rules != null) return _rules;
                try
                {
                    if (File.Exists(RulesPath))
                        _rules = JsonSerializer.Deserialize<Dictionary<string, string>>(SecureFile.ReadAllText(RulesPath));
                }
                catch { }
                _rules = new Dictionary<string, string>(_rules ?? new(), StringComparer.OrdinalIgnoreCase);
                foreach (var k in _rules.Where(r => r.Value == "Energy").Select(r => r.Key).ToList())
                    _rules[k] = "Energy Bills";   // renamed category
                return _rules;
            }
        }

        public static string GetRule(string merchantKey) => Rules.TryGetValue(merchantKey ?? "", out var c) ? c : null;

        public static void SetRule(string merchantKey, string category)
        {
            if (string.IsNullOrEmpty(merchantKey)) return;
            if (string.IsNullOrEmpty(category)) Rules.Remove(merchantKey);
            else Rules[merchantKey] = category;
            try { SecureFile.WriteAllText(RulesPath, JsonSerializer.Serialize(Rules, new JsonSerializerOptions { WriteIndented = true })); }
            catch { }
        }

        #endregion

        public static string KeywordCategory(string description)
        {
            var d = description ?? "";
            foreach (var (cat, re) in Keywords)
                if (re.IsMatch(d)) return cat;
            return null;
        }

        public static TransactionAnalysis Analyse(IEnumerable<MockTransaction> transactions, IEnumerable<MockAccount> accounts = null)
        {
            var list = (transactions ?? Enumerable.Empty<MockTransaction>()).Where(t => t != null).ToList();
            var result = new TransactionAnalysis();

            var ownNumbers = (accounts ?? Enumerable.Empty<MockAccount>())
                .Select(a => Regex.Replace(a.AccountNumber ?? "", @"\D", ""))
                .Where(n => n.Length >= 4).Select(n => n[^4..]).ToHashSet();

            // Pairs of equal-and-opposite amounts in two different accounts within two days are transfers
            // between the user's own accounts.
            // When one side of the pair is a credit card, it's a card repayment rather than a plain transfer.
            var cardAccounts = (accounts ?? Enumerable.Empty<MockAccount>())
                .Where(a => (a.AccountType ?? "").Contains("Credit", StringComparison.OrdinalIgnoreCase)
                         || (a.AccountName ?? "").Contains("Credit Card", StringComparison.OrdinalIgnoreCase))
                .Select(a => a.AccountName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            bool IsCardAccount(string name) => name != null && (cardAccounts.Contains(name) || name.Contains("Credit Card", StringComparison.OrdinalIgnoreCase));

            var transferPairs = new HashSet<MockTransaction>(ReferenceEqualityComparer.Instance);
            var cardPayments = new HashSet<MockTransaction>(ReferenceEqualityComparer.Instance);
            var incomingByAmount = list.Where(t => t.Amount > 0).ToLookup(t => t.Amount);
            foreach (var outTxn in list.Where(t => t.Amount < 0))
            {
                var match = incomingByAmount[-outTxn.Amount].FirstOrDefault(t => t.AccountName != outTxn.AccountName
                    && Math.Abs((t.Date - outTxn.Date).TotalDays) <= 2 && !transferPairs.Contains(t));
                if (match == null) continue;
                transferPairs.Add(outTxn); transferPairs.Add(match);
                if (IsCardAccount(match.AccountName)) { cardPayments.Add(outTxn); cardPayments.Add(match); }
            }
            // Repayments seen only on the card side ("PAYMENT RECEIVED - THANK YOU").
            foreach (var t in list.Where(t => t.Amount > 0 && IsCardAccount(t.AccountName)
                && Regex.IsMatch(t.Description ?? "", @"PAYMENT|THANK YOU|DIRECT DEBIT|\bDD\b", RegexOptions.IgnoreCase)))
                cardPayments.Add(t);

            foreach (var t in list)
            {
                var key = MerchantKey(t.Description);
                result.Items.Add(new AnalysedTransaction { Transaction = t, MerchantKey = key, MerchantName = DisplayName(key) });
            }

            // Repeating payments: same merchant, same direction.
            foreach (var group in result.Items.GroupBy(a => (a.MerchantKey, In: a.Transaction.Amount > 0)))
            {
                var recurring = DetectRecurring(group.OrderBy(a => a.Transaction.Date).ToList());
                if (recurring == null) continue;
                result.Recurring.Add(recurring);
                foreach (var a in group) a.Recurring = recurring;
            }

            foreach (var a in result.Items)
            {
                var t = a.Transaction;
                var desc = t.Description ?? "";
                var rule = GetRule(a.MerchantKey);
                var keyword = KeywordCategory(desc);
                string category;

                if (rule != null) { category = rule; a.Reason = "Your choice for this payee"; }
                else if (cardPayments.Contains(t) || (keyword == "Credit Card Payment" && t.Amount < 0))
                { category = "Credit Card Payment"; a.Reason = t.Amount < 0 ? "Paying off a credit card" : "Repayment received on your credit card"; }
                else if (keyword is "Investments" or "Crypto")
                {
                    category = keyword;
                    a.Reason = t.Amount < 0
                        ? (keyword == "Crypto" ? "Money sent to a crypto exchange or wallet" : "Money paid into an investment or savings platform")
                        : (keyword == "Crypto" ? "Money back from a crypto exchange" : "Money back from an investment platform");
                }
                else if (transferPairs.Contains(t) || keyword == "Transfer"
                         || (keyword == null && t.Type != "Card" && ownNumbers.Any(n => Regex.IsMatch(desc, $@"\b\d*{n}\b"))))
                { category = "Transfer"; a.Reason = "Money moved between your own accounts"; }
                else if (t.Amount > 0)
                {
                    if (PensionPattern.IsMatch(desc)) { category = "State Pension"; a.Reason = "State Pension"; }
                    else if (OtherPensionPattern.IsMatch(desc)) { category = "Income"; a.Reason = "Pension payment"; }
                    else if (a.Recurring == null && keyword != null && !IncomePattern.IsMatch(desc)) { category = keyword; a.Reason = "Refund"; }
                    else { category = "Income"; a.Reason = a.Recurring?.Reason ?? "Money in"; }
                }
                else if (a.Recurring != null)
                {
                    category = RecurringCategory(a.Recurring, keyword, t);
                    a.Reason = a.Recurring.Reason;
                }
                else if (keyword != null) { category = keyword; a.Reason = "Recognised payee"; }
                else if (t.Category == "Cash" || t.Type == "ATM") { category = "Cash"; a.Reason = "Cash withdrawal"; }
                else if (t.Category == "Transfer") { category = "Transfer"; a.Reason = "Bank transfer"; }
                else if (t.Type == "DD" || t.Type == "SO") { category = "Bills & Utilities"; a.Reason = t.Type == "DD" ? "Direct Debit" : "Standing order"; }
                else { category = string.IsNullOrEmpty(t.Category) || t.Category is "Shopping" or "Bills" or "Debit" ? "General" : t.Category; a.Reason = "From your bank"; }

                a.Category = category;
            }

            // The recurring summary takes the category its payments ended up with (a user rule wins).
            foreach (var r in result.Recurring)
            {
                var cats = result.Items.Where(a => ReferenceEquals(a.Recurring, r)).GroupBy(a => a.Category).OrderByDescending(g => g.Count()).First();
                r.Category = cats.Key;
                // The card side of a repayment is the same money as the bank side, so it's not income.
                if (r.Category == "Transfer" || (r.IsIncome && r.Category is "Credit Card Payment" or "Investments" or "Crypto")) r.Kind = "Transfer";
                else if (r.IsIncome) r.Kind = KindIncome;
                else if (r.Category == "Subscriptions") r.Kind = KindSubscription;
                else if (r.Category is "Credit Card Payment" or "Loans & Credit" or "Mortgage & Rent") r.Kind = KindBill;
                else if (IsBillCategory(r.Category)) r.Kind = r.FixedAmount || r.Kind != KindVariableBill ? KindBill : KindVariableBill;
                else if (r.Kind is KindSubscription or KindVariableBill) r.Kind = KindRegularSpend;

                var every = Period(r.Frequency);
                var oldReason = r.Reason;
                r.Reason = r.Kind switch
                {
                    KindBill when r.FixedAmount => $"Same amount every {every} to a bill payee (fixed instalments)",
                    KindBill => $"Bill payee paid every {every}; the amount changes with usage",
                    KindVariableBill => $"Paid every {every} but the amount changes: most likely a utility bill",
                    KindSubscription => $"Same amount every {every}: most likely a subscription",
                    KindStandingOrder => $"Same amount every {every} by standing order",
                    KindRegularSpend => $"Regular spending, about every {every}",
                    _ => r.Reason,
                };
                foreach (var a in result.Items.Where(a => ReferenceEquals(a.Recurring, r) && a.Reason == oldReason))
                    a.Reason = r.Reason;
            }
            result.Index();
            result.Recurring = result.Recurring
                .OrderBy(r => KindOrder(r.Kind)).ThenByDescending(r => r.MonthlyCost).ToList();
            return result;
        }

        public static bool IsBillCategory(string c) => c is "Energy Bills" or "Water" or "Council Tax" or "Broadband & Phone" or "Insurance"
            or "Mortgage & Rent" or "Credit Card Payment" or "Loans & Credit" or "Bills & Utilities";

        private static int KindOrder(string kind) => kind switch
        {
            KindIncome => 0, KindBill => 1, KindVariableBill => 2, KindSubscription => 3, KindStandingOrder => 4, KindRegularSpend => 5, _ => 6,
        };

        private static string RecurringCategory(RecurringPayment r, string keyword, MockTransaction t)
        {
            // Named bill payees keep their own category even when the Direct Debit is a fixed amount
            // (energy and council tax are usually paid in fixed monthly instalments).
            if (keyword != null && (IsBillCategory(keyword) || keyword == "Subscriptions")) return keyword;
            if (keyword is "Groceries" or "Eating Out" or "Fuel" or "Transport" or "Health" or "Cash" or "Investments" or "Crypto") return keyword;

            return r.Kind switch
            {
                KindSubscription => "Subscriptions",
                KindVariableBill => "Bills & Utilities",
                KindStandingOrder => "Bills & Utilities",
                _ => keyword ?? (t.Type == "DD" ? "Bills & Utilities" : "General"),
            };
        }

        private static RecurringPayment DetectRecurring(List<AnalysedTransaction> items)
        {
            // One payment per day counts once (split payments, retries).
            var byDay = items.GroupBy(a => a.Transaction.Date.Date).Select(g => g.First()).ToList();
            if (byDay.Count < 2) return null;

            var gaps = new List<double>();
            for (int i = 1; i < byDay.Count; i++)
                gaps.Add((byDay[i].Transaction.Date.Date - byDay[i - 1].Transaction.Date.Date).TotalDays);
            var median = Median(gaps);

            (string Name, double Min, double Max, double Days)? freq = median switch
            {
                >= 6 and <= 8 => ("Weekly", 5, 9, 7),
                >= 13 and <= 16 => ("Fortnightly", 11, 17, 14),
                >= 26 and <= 35 => ("Monthly", 24, 38, 30.44),
                >= 83 and <= 98 => ("Quarterly", 75, 105, 91.3),
                >= 350 and <= 380 => ("Yearly", 340, 390, 365.25),
                _ => null,
            };
            if (freq == null) return null;
            var f = freq.Value;

            // Most gaps must fit the schedule; a skipped period (a gap of about twice the schedule) is allowed.
            int onSchedule = gaps.Count(g => (g >= f.Min && g <= f.Max) || (g >= f.Min * 2 && g <= f.Max * 2));
            if (onSchedule < Math.Ceiling(gaps.Count * 0.75)) return null;

            var amounts = byDay.Select(a => Math.Abs(a.Transaction.Amount)).ToList();
            var typical = (decimal)Median(amounts.Select(x => (double)x).ToList());
            var min = amounts.Min(); var max = amounts.Max();
            bool fixedAmount = max - min <= 0.01m;
            bool closeAmounts = typical > 0 && (max - min) / typical <= 0.35m;

            var first = byDay[0].Transaction;
            var type = items.GroupBy(a => a.Transaction.Type).OrderByDescending(g => g.Count()).First().Key;
            bool isDebitOrder = type is "DD" or "SO";
            bool isIncome = first.Amount > 0;

            // Two payments are only enough when they are an exact monthly repeat or a Direct Debit.
            if (byDay.Count == 2 && !(fixedAmount || isDebitOrder)) return null;
            // Everyday spending at the same shop is only "regular" if the amounts are similar.
            if (!fixedAmount && !closeAmounts && !isDebitOrder && !isIncome) return null;
            if (isIncome && typical < 20) return null;

            string kind, reason;
            if (isIncome) { kind = KindIncome; reason = $"Paid to you {f.Name.ToLowerInvariant()}"; }
            else if (fixedAmount && type == "SO") { kind = KindStandingOrder; reason = $"Same amount {f.Name.ToLowerInvariant()} by standing order"; }
            else if (fixedAmount) { kind = KindSubscription; reason = $"Same amount every {Period(f.Name)}: most likely a subscription"; }
            else if ((closeAmounts || isDebitOrder) && f.Name is "Monthly" or "Quarterly") { kind = KindVariableBill; reason = $"Paid every {Period(f.Name)} but the amount changes: most likely a utility bill"; }
            else { kind = KindRegularSpend; reason = $"Regular {f.Name.ToLowerInvariant()} spending"; }

            var last = byDay[^1].Transaction;
            var account = items.GroupBy(a => a.Transaction.AccountName).OrderByDescending(g => g.Count()).First().Key;
            return new RecurringPayment
            {
                Key = items[0].MerchantKey,
                Name = items[0].MerchantName,
                Kind = kind,
                Reason = reason,
                Frequency = f.Name,
                IsIncome = isIncome,
                FixedAmount = fixedAmount,
                TypicalAmount = typical,
                MinAmount = min,
                MaxAmount = max,
                MonthlyCost = Math.Round(typical * (decimal)(30.44 / f.Days), 2),
                Count = byDay.Count,
                LastDate = last.Date,
                NextDue = f.Name == "Monthly" ? last.Date.AddMonths(1) : last.Date.AddDays(Math.Round(median)),
                AccountName = account,
                PaymentType = type,
            };
        }

        private static string Period(string freq) => freq switch
        {
            "Weekly" => "week", "Fortnightly" => "fortnight", "Monthly" => "month", "Quarterly" => "quarter", _ => "year",
        };

        private static double Median(List<double> v)
        {
            if (v.Count == 0) return 0;
            var s = v.OrderBy(x => x).ToList();
            return s.Count % 2 == 1 ? s[s.Count / 2] : (s[s.Count / 2 - 1] + s[s.Count / 2]) / 2;
        }

        #region Bill payments

        private static readonly Regex SupplierNoise = new(@"\b(ENERGY|POWER|ELECTRIC(ITY)?|LTD|LIMITED|PLC|UK|SUPPLY|SERVICES|THE|WATER|BROADBAND)\b|[^A-Z0-9]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string CompactKey(string s) => SupplierNoise.Replace((s ?? "").ToUpperInvariant(), "");

        public static bool DescriptionMatchesSupplier(string description, string supplier)
        {
            if (string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(supplier)) return false;
            var d = description.ToUpperInvariant();
            if (d.Contains(supplier.Trim().ToUpperInvariant())) return true;
            var ks = CompactKey(supplier);
            if (ks.Length < 3) return Regex.IsMatch(d, $@"\b{Regex.Escape(supplier.Trim().ToUpperInvariant())}\b");
            return CompactKey(description).Contains(ks);
        }

        // Finds the bank account a bill was paid from: the payment to that supplier closest to the bill
        // (in amount and date), or failing that the account that usually pays the supplier.
        public static (string Account, bool Exact) FindPaymentAccount(MockUtilityBill bill, IEnumerable<MockTransaction> transactions)
        {
            if (bill == null || transactions == null) return (null, false);
            var payments = transactions.Where(t => t.Amount < 0 && DescriptionMatchesSupplier(t.Description, bill.Supplier)).ToList();
            if (payments.Count == 0) return (null, false);

            var refDate = bill.BillDate != default ? bill.BillDate : bill.PeriodEnd ?? DateTime.Today;
            var near = payments
                .Select(t => new { t, days = (t.Date - refDate).TotalDays })
                .Where(x => x.days >= -25 && x.days <= 45)
                .OrderBy(x => (bill.Amount > 0 ? (double)(Math.Abs(Math.Abs(x.t.Amount) - bill.Amount) / bill.Amount) : 0) + Math.Abs(x.days) / 60.0)
                .FirstOrDefault();
            if (near != null) return (near.t.AccountName, true);

            var usual = payments.GroupBy(t => t.AccountName).OrderByDescending(g => g.Count()).First().Key;
            return (usual, false);
        }

        #endregion
    }

    public class AnalysedTransaction
    {
        public MockTransaction Transaction { get; set; }
        public string MerchantKey { get; set; }
        public string MerchantName { get; set; }
        public string Category { get; set; }
        public string Reason { get; set; }
        public RecurringPayment Recurring { get; set; }
    }

    public class RecurringPayment
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public string Category { get; set; }
        public string Reason { get; set; }
        public string Frequency { get; set; }
        public bool IsIncome { get; set; }
        public bool FixedAmount { get; set; }
        public decimal TypicalAmount { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal MonthlyCost { get; set; }
        public int Count { get; set; }
        public DateTime LastDate { get; set; }
        public DateTime NextDue { get; set; }
        public string AccountName { get; set; }
        public string PaymentType { get; set; }
    }

    public class TransactionAnalysis
    {
        public List<AnalysedTransaction> Items { get; } = new();
        public List<RecurringPayment> Recurring { get; set; } = new();

        private Dictionary<MockTransaction, AnalysedTransaction> _byTxn;

        internal void Index()
        {
            _byTxn = new Dictionary<MockTransaction, AnalysedTransaction>(ReferenceEqualityComparer.Instance);
            foreach (var a in Items) _byTxn[a.Transaction] = a;
        }

        public AnalysedTransaction For(MockTransaction t) =>
            t != null && _byTxn != null && _byTxn.TryGetValue(t, out var a) ? a : null;
    }
}
