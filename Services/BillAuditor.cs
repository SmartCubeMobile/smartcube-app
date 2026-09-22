using SmartCubeMobile.MockData;
using System.Text.RegularExpressions;

namespace SmartCubeMobile.Services
{
    public enum AuditSeverity { Info, Warning, Alert, Critical }

    public class AuditFinding
    {
        public string CheckType { get; set; }
        public AuditSeverity Severity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PotentialSavings { get; set; }
        public string BillReference { get; set; }
        public string Icon { get; set; }
    }

    public static class BillAuditor
    {
        public static List<AuditFinding> AuditBills(List<MockUtilityBill> bills, List<MockMeterReading> readings)
        {
            var findings = new List<AuditFinding>();

            findings.AddRange(CheckPriceCapCompliance(bills));
            findings.AddRange(CheckBackBilling(bills));
            findings.AddRange(CheckEstimatedReadingDrift(bills, readings));
            findings.AddRange(CheckVatRate(bills));
            findings.AddRange(CheckDuplicatePeriods(bills));
            findings.AddRange(CheckDirectDebitDiscount(bills));
            findings.AddRange(CheckSolarExport(bills));

            findings.Sort((a, b) =>
            {
                int sev = b.Severity.CompareTo(a.Severity);
                return sev != 0 ? sev : b.PotentialSavings.CompareTo(a.PotentialSavings);
            });

            return findings;
        }

        private static List<AuditFinding> CheckPriceCapCompliance(List<MockUtilityBill> bills)
        {
            var findings = new List<AuditFinding>();

            foreach (var bill in bills)
            {
                if (bill.UnitRatePence <= 0 && bill.StandingChargePence <= 0)
                    continue;
                if (bill.FuelType != "Electricity" && bill.FuelType != "Gas")
                    continue;

                var postcode = ExtractPostcode(bill.Address);
                var region = OfgemRegionData.GetRegion(postcode);
                if (region == null) continue;
                var cap = OfgemRegionData.GetCap(region, bill.FuelType, bill.PaymentMethod ?? "Direct Debit", bill.BillDate);

                if (cap == null) continue;

                if (bill.UnitRatePence > 0 && bill.UnitRatePence > cap.UnitRatePence)
                {
                    var overchargePerUnit = bill.UnitRatePence - cap.UnitRatePence;
                    var estSavings = bill.UnitsUsed > 0
                        ? Math.Round(bill.UnitsUsed * overchargePerUnit / 100m, 2)
                        : Math.Round(overchargePerUnit * 10m, 2);

                    findings.Add(new AuditFinding
                    {
                        CheckType = "Price Cap",
                        Severity = AuditSeverity.Critical,
                        Title = "Unit rate exceeds Ofgem cap",
                        Description = $"{bill.Supplier} charged {bill.UnitRatePence:F2}p/kWh for {bill.FuelType} " +
                            $"but the Ofgem cap ({cap.CapPeriod}) is {cap.UnitRatePence:F2}p/kWh. " +
                            $"Overcharge: {overchargePerUnit:F2}p/kWh.",
                        PotentialSavings = estSavings,
                        BillReference = $"{bill.Supplier} — {bill.Period}",
                        Icon = "🚨",
                    });
                }

                if (bill.StandingChargePence > 0 && bill.StandingChargePence > cap.StandingChargePence)
                {
                    var overPerDay = bill.StandingChargePence - cap.StandingChargePence;
                    var days = (bill.PeriodEnd.HasValue && bill.PeriodStart.HasValue)
                        ? (decimal)(bill.PeriodEnd.Value - bill.PeriodStart.Value).Days
                        : 30m;
                    var estSavings = Math.Round(overPerDay * days / 100m, 2);

                    findings.Add(new AuditFinding
                    {
                        CheckType = "Price Cap",
                        Severity = AuditSeverity.Critical,
                        Title = "Standing charge exceeds Ofgem cap",
                        Description = $"{bill.Supplier} charged {bill.StandingChargePence:F2}p/day for {bill.FuelType} " +
                            $"standing charge but the Ofgem cap ({cap.CapPeriod}) is {cap.StandingChargePence:F2}p/day. " +
                            $"Overcharge: {overPerDay:F2}p/day.",
                        PotentialSavings = estSavings,
                        BillReference = $"{bill.Supplier} — {bill.Period}",
                        Icon = "🚨",
                    });
                }
            }

            return findings;
        }

        private static List<AuditFinding> CheckBackBilling(List<MockUtilityBill> bills)
        {
            var findings = new List<AuditFinding>();

            foreach (var bill in bills)
            {
                if (!bill.PeriodStart.HasValue || !bill.PeriodEnd.HasValue) continue;

                var span = bill.PeriodEnd.Value - bill.PeriodStart.Value;
                if (span.TotalDays > 365)
                {
                    var monthsOver = Math.Round((span.TotalDays - 365) / 30.44, 0);
                    var proportionUnenforceable = (decimal)(span.TotalDays - 365) / (decimal)span.TotalDays;
                    var unenforceableAmount = Math.Round(bill.Amount * proportionUnenforceable, 2);

                    findings.Add(new AuditFinding
                    {
                        CheckType = "Back-billing",
                        Severity = AuditSeverity.Critical,
                        Title = "Back-billing breach — Ofgem 12-month rule",
                        Description = $"This bill from {bill.Supplier} covers {span.Days} days ({bill.PeriodStart:dd MMM yyyy} to {bill.PeriodEnd:dd MMM yyyy}). " +
                            $"Under Ofgem rules, suppliers cannot back-bill for energy used more than 12 months ago. " +
                            $"Approximately £{unenforceableAmount:F2} of this bill may be unenforceable.",
                        PotentialSavings = unenforceableAmount,
                        BillReference = $"{bill.Supplier} — {bill.Period}",
                        Icon = "⚖️",
                    });
                }
            }

            return findings;
        }

        private static List<AuditFinding> CheckEstimatedReadingDrift(List<MockUtilityBill> bills, List<MockMeterReading> readings)
        {
            var findings = new List<AuditFinding>();

            var estimatedBills = bills
                .Where(b => b.IsEstimated && (b.FuelType == "Electricity" || b.FuelType == "Gas"))
                .GroupBy(b => new { b.Supplier, b.FuelType, Postcode = ExtractPostcode(b.Address) });

            foreach (var group in estimatedBills)
            {
                var consecutive = group.OrderBy(b => b.BillDate).ToList();
                if (consecutive.Count >= 3)
                {
                    var totalEstimated = consecutive.Sum(b => b.Amount);
                    findings.Add(new AuditFinding
                    {
                        CheckType = "Estimated Readings",
                        Severity = AuditSeverity.Alert,
                        Title = $"{consecutive.Count} consecutive estimated bills",
                        Description = $"{group.Key.Supplier} has billed {group.Key.FuelType} on estimated readings " +
                            $"for {consecutive.Count} consecutive periods totalling £{totalEstimated:F2}. " +
                            $"Submit an actual meter reading to avoid a large catch-up bill.",
                        PotentialSavings = 0,
                        BillReference = $"{group.Key.Supplier} — {group.Key.FuelType}",
                        Icon = "📊",
                    });
                }
            }

            var latestReadings = readings
                .GroupBy(r => r.MeterType)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.ReadingDate).First());

            foreach (var kvp in latestReadings)
            {
                var daysSinceReading = (DateTime.Now - kvp.Value.ReadingDate).Days;
                if (daysSinceReading > 90 && kvp.Value.Source != "Smart Meter")
                {
                    findings.Add(new AuditFinding
                    {
                        CheckType = "Estimated Readings",
                        Severity = AuditSeverity.Warning,
                        Title = $"{kvp.Key} meter not read for {daysSinceReading} days",
                        Description = $"Your last {kvp.Key.ToLower()} reading was {daysSinceReading} days ago ({kvp.Value.ReadingDate:dd MMM yyyy}). " +
                            $"Suppliers will estimate usage which could lead to large catch-up bills. Submit a reading soon.",
                        PotentialSavings = 0,
                        BillReference = $"{kvp.Key} meter",
                        Icon = "⏰",
                    });
                }
            }

            return findings;
        }

        private static List<AuditFinding> CheckVatRate(List<MockUtilityBill> bills)
        {
            var findings = new List<AuditFinding>();

            foreach (var bill in bills)
            {
                if (bill.VatRate <= 0) continue;
                if (bill.FuelType != "Electricity" && bill.FuelType != "Gas") continue;

                if (bill.VatRate > 5)
                {
                    var overVat = bill.VatRate - 5m;
                    var netAmount = bill.Amount / (1 + bill.VatRate / 100m);
                    var correctVat = netAmount * 0.05m;
                    var chargedVat = netAmount * (bill.VatRate / 100m);
                    var overpaid = Math.Round(chargedVat - correctVat, 2);

                    findings.Add(new AuditFinding
                    {
                        CheckType = "VAT",
                        Severity = AuditSeverity.Critical,
                        Title = "Incorrect VAT rate on domestic energy",
                        Description = $"{bill.Supplier} applied {bill.VatRate}% VAT on this {bill.FuelType} bill. " +
                            $"Domestic energy supply in the UK is subject to 5% VAT, not {bill.VatRate}%. " +
                            $"You have been overcharged £{overpaid:F2} in VAT.",
                        PotentialSavings = overpaid,
                        BillReference = $"{bill.Supplier} — {bill.Period}",
                        Icon = "💰",
                    });
                }
            }

            return findings;
        }

        private static List<AuditFinding> CheckDuplicatePeriods(List<MockUtilityBill> bills)
        {
            var findings = new List<AuditFinding>();

            var groups = bills
                .Where(b => b.PeriodStart.HasValue && b.PeriodEnd.HasValue)
                .GroupBy(b => new { b.Supplier, b.FuelType, Postcode = ExtractPostcode(b.Address) });

            foreach (var group in groups)
            {
                var sorted = group.OrderBy(b => b.PeriodStart).ToList();
                for (int i = 0; i < sorted.Count - 1; i++)
                {
                    for (int j = i + 1; j < sorted.Count; j++)
                    {
                        if (sorted[i].PeriodEnd > sorted[j].PeriodStart)
                        {
                            var overlapDays = (sorted[i].PeriodEnd.Value - sorted[j].PeriodStart.Value).Days;
                            if (overlapDays > 2)
                            {
                                findings.Add(new AuditFinding
                                {
                                    CheckType = "Duplicate Billing",
                                    Severity = AuditSeverity.Alert,
                                    Title = "Overlapping billing periods",
                                    Description = $"{group.Key.Supplier} has overlapping {group.Key.FuelType} bills — " +
                                        $"period ending {sorted[i].PeriodEnd:dd MMM yyyy} overlaps with period starting {sorted[j].PeriodStart:dd MMM yyyy} " +
                                        $"by {overlapDays} days. You may have been double-charged.",
                                    PotentialSavings = 0,
                                    BillReference = $"{group.Key.Supplier} — {group.Key.FuelType}",
                                    Icon = "📋",
                                });
                            }
                        }
                    }
                }
            }

            return findings;
        }

        private static List<AuditFinding> CheckDirectDebitDiscount(List<MockUtilityBill> bills)
        {
            var findings = new List<AuditFinding>();

            var nonDdBills = bills
                .Where(b => b.PaymentMethod == "Standard Credit" &&
                            (b.FuelType == "Electricity" || b.FuelType == "Gas"))
                .GroupBy(b => b.Supplier)
                .ToList();

            foreach (var group in nonDdBills)
            {
                var totalPaid = group.Sum(b => b.Amount);
                var estimatedSaving = Math.Round(totalPaid * 0.06m, 2);

                findings.Add(new AuditFinding
                {
                    CheckType = "Payment Method",
                    Severity = AuditSeverity.Info,
                    Title = $"No Direct Debit discount — {group.Key}",
                    Description = $"You're paying {group.Key} by standard credit. Switching to Direct Debit " +
                        $"typically saves 5-7% on energy bills. Based on your bills, that's approximately £{estimatedSaving:F2}.",
                    PotentialSavings = estimatedSaving,
                    BillReference = group.Key,
                    Icon = "💳",
                });
            }

            return findings;
        }

        private static List<AuditFinding> CheckSolarExport(List<MockUtilityBill> bills)
        {
            var findings = new List<AuditFinding>();
            var solarBills = bills.Where(b => b.HasSolar && b.FuelType == "Electricity").ToList();
            if (solarBills.Count == 0) return findings;

            foreach (var bill in solarBills)
            {
                if (bill.ExportRatePence > 0 && bill.ExportRatePence < 4m)
                {
                    var annualExport = bill.ExportKwh > 0 ? bill.ExportKwh * 4 : 1500m;
                    var currentEarnings = annualExport * bill.ExportRatePence / 100m;
                    var betterEarnings = annualExport * 15m / 100m;
                    var potentialGain = Math.Round(betterEarnings - currentEarnings, 2);

                    findings.Add(new AuditFinding
                    {
                        CheckType = "Solar Export",
                        Severity = AuditSeverity.Alert,
                        Title = "Poor solar export rate",
                        Description = $"{bill.Supplier} is paying {bill.ExportRatePence:F1}p/kWh for your exported electricity. " +
                            $"The best SEG tariffs currently pay 12–15p/kWh. " +
                            $"Switching export tariff could earn you an extra £{potentialGain:F2}/year. " +
                            $"You can switch your export supplier independently of your import supplier.",
                        PotentialSavings = potentialGain,
                        BillReference = $"{bill.Supplier} — {bill.Period}",
                        Icon = "☀️",
                    });
                }

                if (bill.IsDeemedExport && bill.ExportKwh > 0)
                {
                    findings.Add(new AuditFinding
                    {
                        CheckType = "Solar Export",
                        Severity = AuditSeverity.Warning,
                        Title = "Export based on deemed estimate, not metered",
                        Description = $"Your export is deemed (estimated) rather than metered. Deemed export is typically set at 50% of generation. " +
                            $"If you have a smart meter that records export, request your supplier switches to metered export — " +
                            $"actual export data could increase your payments if you export more than 50%.",
                        PotentialSavings = 0,
                        BillReference = $"{bill.Supplier} — {bill.Period}",
                        Icon = "📊",
                    });
                }

                if (bill.ExportKwh > 0 && bill.UnitsUsed > 0)
                {
                    var totalGeneration = bill.GenerationKwh > 0 ? bill.GenerationKwh : bill.ExportKwh + bill.UnitsUsed * 0.3m;
                    var selfConsumptionRatio = totalGeneration > 0 ? (totalGeneration - bill.ExportKwh) / totalGeneration : 0;

                    if (selfConsumptionRatio < 0.25m && bill.ExportTariffType != "FiT")
                    {
                        var shiftableKwh = bill.ExportKwh * 0.3m;
                        var savedImport = shiftableKwh * bill.UnitRatePence / 100m;
                        var lostExport = shiftableKwh * bill.ExportRatePence / 100m;
                        var netBenefit = Math.Round((savedImport - lostExport) * 4, 2);

                        if (netBenefit > 10)
                        {
                            findings.Add(new AuditFinding
                            {
                                CheckType = "Solar Export",
                                Severity = AuditSeverity.Info,
                                Title = "Low self-consumption — battery could save money",
                                Description = $"You're exporting a high proportion of your solar generation at {bill.ExportRatePence:F1}p/kWh " +
                                    $"but importing at {bill.UnitRatePence:F1}p/kWh. A battery to shift consumption to your own solar " +
                                    $"could save approximately £{netBenefit:F2}/year by avoiding import costs.",
                                PotentialSavings = netBenefit,
                                BillReference = $"{bill.Supplier} — {bill.Period}",
                                Icon = "🔋",
                            });
                        }
                    }
                }

                if (bill.ExportRatePence > 0 && bill.ExportKwh > 0 && bill.ExportPayment > 0)
                {
                    var expectedPayment = Math.Round(bill.ExportKwh * bill.ExportRatePence / 100m, 2);
                    var difference = Math.Abs(expectedPayment - bill.ExportPayment);
                    if (difference > 1m && difference > expectedPayment * 0.1m)
                    {
                        findings.Add(new AuditFinding
                        {
                            CheckType = "Solar Export",
                            Severity = AuditSeverity.Alert,
                            Title = "Export payment doesn't match rate x units",
                            Description = $"Your bill shows {bill.ExportKwh:F0} kWh exported at {bill.ExportRatePence:F2}p/kWh " +
                                $"which should be £{expectedPayment:F2}, but the credited amount is £{bill.ExportPayment:F2}. " +
                                $"Difference: £{difference:F2}. Contact your supplier to query this.",
                            PotentialSavings = difference,
                            BillReference = $"{bill.Supplier} — {bill.Period}",
                            Icon = "⚠️",
                        });
                    }
                }
            }

            var totalExportEarnings = solarBills.Sum(b => b.ExportPayment);
            var totalExportKwh = solarBills.Sum(b => b.ExportKwh);
            if (totalExportKwh > 0 && totalExportEarnings > 0)
            {
                var avgRate = totalExportEarnings / totalExportKwh * 100m;
                var latestBill = solarBills.OrderByDescending(b => b.BillDate).First();

                findings.Add(new AuditFinding
                {
                    CheckType = "Solar Export",
                    Severity = AuditSeverity.Info,
                    Title = "Solar export summary",
                    Description = $"Across your bills: {totalExportKwh:F0} kWh exported, earning £{totalExportEarnings:F2} " +
                        $"(average {avgRate:F1}p/kWh). " +
                        $"Standing charges still apply — solar panels reduce your unit costs but not the daily standing charge " +
                        $"(currently ~57p/day electricity, ~30p/day gas).",
                    PotentialSavings = 0,
                    BillReference = $"{latestBill.Supplier} — Solar Export",
                    Icon = "☀️",
                });
            }

            return findings;
        }

        private static string ExtractPostcode(string address)
        {
            if (string.IsNullOrEmpty(address)) return "";
            var m = Regex.Match(address, @"[A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2}", RegexOptions.IgnoreCase);
            return m.Success ? m.Value.ToUpper() : "";
        }
    }

    public class OfgemCapRate
    {
        public decimal UnitRatePence { get; set; }
        public decimal StandingChargePence { get; set; }
        public string CapPeriod { get; set; }
    }

    public static class OfgemRegionData
    {
        private static readonly Dictionary<string, string> PostcodeToRegion = new(StringComparer.OrdinalIgnoreCase)
        {
            // London
            ["EC"] = "London", ["WC"] = "London", ["E"] = "London",
            ["N"] = "London", ["NW"] = "London", ["SE"] = "London",
            ["SW"] = "London", ["W"] = "London",

            // East Midlands
            ["DE"] = "East Midlands", ["LE"] = "East Midlands", ["NG"] = "East Midlands",
            ["NN"] = "East Midlands", ["LN"] = "East Midlands", ["PE"] = "East Midlands",
            ["MK"] = "East Midlands", ["OX"] = "East Midlands",

            // East of England
            ["CB"] = "Eastern", ["CO"] = "Eastern", ["IP"] = "Eastern",
            ["NR"] = "Eastern", ["CM"] = "Eastern", ["SS"] = "Eastern",
            ["SG"] = "Eastern", ["AL"] = "Eastern", ["EN"] = "Eastern",

            // Merseyside & North Wales
            ["L"] = "Merseyside & North Wales", ["CH"] = "Merseyside & North Wales",
            ["LL"] = "Merseyside & North Wales",

            // North East
            ["DH"] = "Northern", ["DL"] = "Northern", ["NE"] = "Northern",
            ["SR"] = "Northern", ["TS"] = "Northern",

            // North West
            ["BB"] = "North Western", ["BL"] = "North Western", ["CA"] = "North Western",
            ["FY"] = "North Western", ["LA"] = "North Western", ["M"] = "North Western",
            ["OL"] = "North Western", ["PR"] = "North Western", ["SK"] = "North Western",
            ["WA"] = "North Western", ["WN"] = "North Western", ["CW"] = "North Western",

            // Scotland
            ["AB"] = "Southern Scotland", ["DD"] = "Southern Scotland", ["DG"] = "Southern Scotland",
            ["EH"] = "Southern Scotland", ["FK"] = "Southern Scotland", ["G"] = "Southern Scotland",
            ["IV"] = "Northern Scotland", ["KA"] = "Southern Scotland", ["KW"] = "Northern Scotland",
            ["KY"] = "Southern Scotland", ["ML"] = "Southern Scotland", ["PA"] = "Southern Scotland",
            ["PH"] = "Northern Scotland", ["TD"] = "Southern Scotland", ["ZE"] = "Northern Scotland",
            ["HS"] = "Northern Scotland",

            // South East (excludes London postcodes)
            ["BN"] = "South East", ["CT"] = "South East", ["DA"] = "South East",
            ["GU"] = "South East", ["HP"] = "South East", ["KT"] = "South East",
            ["ME"] = "South East", ["RG"] = "South East", ["RH"] = "South East",
            ["SL"] = "South East", ["SM"] = "South East", ["TN"] = "South East",
            ["TW"] = "South East", ["UB"] = "South East", ["CR"] = "South East",
            ["BR"] = "South East", ["HA"] = "South East", ["IG"] = "South East",
            ["RM"] = "South East", ["WD"] = "South East",

            // South Wales
            ["CF"] = "South Wales", ["NP"] = "South Wales", ["SA"] = "South Wales",
            ["SY"] = "South Wales", ["LD"] = "South Wales",

            // South West
            ["BA"] = "South Western", ["BS"] = "South Western", ["DT"] = "South Western",
            ["EX"] = "South Western", ["GL"] = "South Western", ["PL"] = "South Western",
            ["SN"] = "South Western", ["SP"] = "South Western", ["TA"] = "South Western",
            ["TQ"] = "South Western", ["TR"] = "South Western", ["BH"] = "South Western",
            ["PO"] = "South Western", ["SO"] = "South Western",

            // West Midlands
            ["B"] = "West Midlands", ["CV"] = "West Midlands", ["DY"] = "West Midlands",
            ["HR"] = "West Midlands", ["ST"] = "West Midlands", ["TF"] = "West Midlands",
            ["WR"] = "West Midlands", ["WS"] = "West Midlands", ["WV"] = "West Midlands",

            // Yorkshire
            ["BD"] = "Yorkshire", ["DN"] = "Yorkshire", ["HD"] = "Yorkshire",
            ["HG"] = "Yorkshire", ["HU"] = "Yorkshire", ["HX"] = "Yorkshire",
            ["LS"] = "Yorkshire", ["S"] = "Yorkshire", ["WF"] = "Yorkshire",
            ["YO"] = "Yorkshire",
        };

        // Verified Ofgem price cap rates — national average, Direct Debit, inc. VAT
        // Source: ofgem.gov.uk quarterly announcements, cross-verified against published annual bill figures
        // Rates are national averages; regional variation is typically ±1-2p/kWh for electricity, ±0.3p for gas
        private static readonly List<QuarterlyCapEntry> QuarterlyCapRates = new()
        {
            // Q3 2023: Jul-Sep 2023 — £2,074/year (EPG ended, cap became binding rate)
            new() { Start = new DateTime(2023, 7, 1), End = new DateTime(2023, 9, 30), Label = "Q3 2023 (Jul-Sep)",
                ElecUnit = 30.11m, ElecStanding = 52.97m, GasUnit = 7.51m, GasStanding = 29.11m },
            // Q4 2023: Oct-Dec 2023 — £1,834/year
            new() { Start = new DateTime(2023, 10, 1), End = new DateTime(2023, 12, 31), Label = "Q4 2023 (Oct-Dec)",
                ElecUnit = 27.35m, ElecStanding = 53.37m, GasUnit = 6.89m, GasStanding = 29.62m },
            // Q1 2024: Jan-Mar 2024 — £1,928/year
            new() { Start = new DateTime(2024, 1, 1), End = new DateTime(2024, 3, 31), Label = "Q1 2024 (Jan-Mar)",
                ElecUnit = 28.62m, ElecStanding = 53.35m, GasUnit = 7.42m, GasStanding = 29.60m },
            // Q2 2024: Apr-Jun 2024 — £1,690/year
            new() { Start = new DateTime(2024, 4, 1), End = new DateTime(2024, 6, 30), Label = "Q2 2024 (Apr-Jun)",
                ElecUnit = 24.50m, ElecStanding = 60.10m, GasUnit = 6.04m, GasStanding = 31.43m },
            // Q3 2024: Jul-Sep 2024 — £1,568/year
            new() { Start = new DateTime(2024, 7, 1), End = new DateTime(2024, 9, 30), Label = "Q3 2024 (Jul-Sep)",
                ElecUnit = 22.36m, ElecStanding = 60.12m, GasUnit = 5.48m, GasStanding = 31.41m },
            // Q4 2024: Oct-Dec 2024 — £1,717/year
            new() { Start = new DateTime(2024, 10, 1), End = new DateTime(2024, 12, 31), Label = "Q4 2024 (Oct-Dec)",
                ElecUnit = 24.50m, ElecStanding = 60.99m, GasUnit = 6.24m, GasStanding = 31.66m },
            // Q1 2025: Jan-Mar 2025 — £1,738/year
            new() { Start = new DateTime(2025, 1, 1), End = new DateTime(2025, 3, 31), Label = "Q1 2025 (Jan-Mar)",
                ElecUnit = 24.86m, ElecStanding = 60.97m, GasUnit = 6.34m, GasStanding = 31.66m },
            // Q2 2025: Apr-Jun 2025 — £1,849/year
            new() { Start = new DateTime(2025, 4, 1), End = new DateTime(2025, 6, 30), Label = "Q2 2025 (Apr-Jun)",
                ElecUnit = 27.03m, ElecStanding = 53.80m, GasUnit = 6.99m, GasStanding = 32.67m },
            // Q3 2025: Jul-Sep 2025 — £1,720/year
            new() { Start = new DateTime(2025, 7, 1), End = new DateTime(2025, 9, 30), Label = "Q3 2025 (Jul-Sep)",
                ElecUnit = 25.73m, ElecStanding = 51.37m, GasUnit = 6.33m, GasStanding = 29.82m },
            // Q4 2025: Oct-Dec 2025 — £1,755/year
            new() { Start = new DateTime(2025, 10, 1), End = new DateTime(2025, 12, 31), Label = "Q4 2025 (Oct-Dec)",
                ElecUnit = 26.35m, ElecStanding = 53.68m, GasUnit = 6.29m, GasStanding = 34.03m },
        };

        public static string GetRegion(string postcode)
        {
            if (string.IsNullOrEmpty(postcode)) return null;

            var area = Regex.Match(postcode.Trim().ToUpper(), @"^([A-Z]{1,2})").Groups[1].Value;
            if (PostcodeToRegion.TryGetValue(area, out var region))
                return region;

            return null;
        }

        public static OfgemCapRate GetCap(string region, string fuelType, string paymentMethod, DateTime billDate)
        {
            var quarter = QuarterlyCapRates.FirstOrDefault(q => billDate >= q.Start && billDate <= q.End);
            if (quarter == null)
            {
                if (billDate > QuarterlyCapRates.Last().End)
                    quarter = QuarterlyCapRates.Last();
                else
                    return null;
            }

            if (fuelType == "Electricity")
                return new OfgemCapRate
                {
                    UnitRatePence = quarter.ElecUnit,
                    StandingChargePence = quarter.ElecStanding,
                    CapPeriod = quarter.Label,
                };

            if (fuelType == "Gas")
                return new OfgemCapRate
                {
                    UnitRatePence = quarter.GasUnit,
                    StandingChargePence = quarter.GasStanding,
                    CapPeriod = quarter.Label,
                };

            return null;
        }

        private class QuarterlyCapEntry
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string Label { get; set; }
            public decimal ElecUnit { get; set; }
            public decimal ElecStanding { get; set; }
            public decimal GasUnit { get; set; }
            public decimal GasStanding { get; set; }
        }
    }
}
