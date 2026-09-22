using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using SmartCubeMobile.MockData;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SmartCubeMobile.Services
{
    public class ParsedBill
    {
        public string Supplier { get; set; }
        public string FuelType { get; set; }
        public decimal Amount { get; set; }
        public decimal UnitsUsed { get; set; }
        public string UoM { get; set; } = "kWh";
        public string Period { get; set; }
        public DateTime BillDate { get; set; }
        public string Address { get; set; }
        public string AccountNumber { get; set; }
        public string RawText { get; set; }
        public decimal UnitRatePence { get; set; }
        public decimal StandingChargePence { get; set; }
        public decimal VatRate { get; set; }
        public bool IsEstimated { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string PaymentMethod { get; set; }
        public bool HasSolar { get; set; }
        public decimal ExportKwh { get; set; }
        public decimal ExportRatePence { get; set; }
        public decimal ExportPayment { get; set; }
        public decimal GenerationKwh { get; set; }
        public string ExportTariffType { get; set; }
        public bool IsDeemedExport { get; set; }
        public decimal MeterReadingStart { get; set; }
        public decimal MeterReadingEnd { get; set; }
        public List<WaterChargePeriod> WaterPeriods { get; set; }
        public List<WaterPayment> Payments { get; set; }
    }

    public class WaterPayment
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class WaterChargePeriod
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal Volume { get; set; }
        public decimal FreshRate { get; set; }
        public decimal FreshCharge { get; set; }
        public decimal FreshStanding { get; set; }
        public decimal WasteRate { get; set; }
        public decimal WasteCharge { get; set; }
        public decimal WasteStanding { get; set; }
    }

    public static class BillPdfParser
    {
        private static readonly Dictionary<string, string[]> SupplierKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            ["EDF"] = new[] { "edf", "edf energy" },
            ["British Gas"] = new[] { "british gas", "centrica" },
            ["Octopus Energy"] = new[] { "octopus energy", "octopus" },
            ["OVO Energy"] = new[] { "ovo energy", "ovo" },
            ["E.ON"] = new[] { "e.on", "eon", "e.on next" },
            ["Scottish Power"] = new[] { "scottish power", "scottishpower" },
            ["Shell Energy"] = new[] { "shell energy" },
            ["Bulb"] = new[] { "bulb energy", "bulb" },
            ["SSE"] = new[] { "sse energy", "sse" },
            ["Utility Warehouse"] = new[] { "utility warehouse" },
            ["So Energy"] = new[] { "so energy" },
            ["Good Energy"] = new[] { "good energy" },
            ["United Utilities"] = new[] { "united utilities" },
            ["Severn Trent"] = new[] { "severn trent" },
            ["Thames Water"] = new[] { "thames water" },
            ["Anglian Water"] = new[] { "anglian water" },
            ["Yorkshire Water"] = new[] { "yorkshire water" },
            ["South West Water"] = new[] { "south west water" },
            ["Southern Water"] = new[] { "southern water" },
            ["Wessex Water"] = new[] { "wessex water" },
            ["Northumbrian Water"] = new[] { "northumbrian water" },
            ["Welsh Water"] = new[] { "dŵr cymru", "welsh water", "dwr cymru" },
            ["South East Water"] = new[] { "south east water" },
            ["Bristol Water"] = new[] { "bristol water" },
            ["Affinity Water"] = new[] { "affinity water" },
        };

        private static readonly HashSet<string> WaterSupplierNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "United Utilities", "Severn Trent", "Thames Water", "Anglian Water",
            "Yorkshire Water", "South West Water", "Southern Water", "Wessex Water",
            "Northumbrian Water", "Welsh Water", "South East Water", "Bristol Water",
            "Affinity Water",
        };

        public static ParsedBill ParseFromFile(string filePath)
        {
            var text = ExtractText(filePath);
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return ParseText(text);
        }

        private static string ExtractText(string filePath)
        {
            using var reader = new PdfReader(filePath);
            using var doc = new PdfDocument(reader);
            var sb = new System.Text.StringBuilder();
            for (int i = 1; i <= doc.GetNumberOfPages(); i++)
            {
                var page = doc.GetPage(i);
                var strategy = new SimpleTextExtractionStrategy();
                var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                sb.AppendLine(pageText);
            }
            return sb.ToString();
        }

        private static ParsedBill ParseText(string text)
        {
            var bill = new ParsedBill { RawText = text };
            var lower = text.ToLowerInvariant();

            bill.Supplier = DetectSupplier(lower);
            bill.FuelType = DetectFuelType(lower, bill.Supplier);

            if (bill.FuelType == "Water")
            {
                ParseWaterBill(text, lower, bill);
                return bill;
            }

            bill.Amount = ExtractAmount(text);
            bill.UnitsUsed = ExtractUsage(text, out string uom);
            bill.UoM = uom;
            bill.BillDate = ExtractBillDate(text);
            bill.Period = ExtractPeriod(text, bill.BillDate);
            bill.Address = ExtractAddress(text);
            bill.AccountNumber = ExtractAccountNumber(text);
            bill.UnitRatePence = ExtractUnitRate(text);
            bill.StandingChargePence = ExtractStandingCharge(text);
            bill.VatRate = ExtractVatRate(text, lower);
            bill.IsEstimated = DetectEstimated(lower);
            ExtractPeriodDates(text, bill);
            bill.PaymentMethod = DetectPaymentMethod(lower);
            bill.HasSolar = DetectSolar(lower);
            if (bill.HasSolar)
                ExtractSolarData(text, lower, bill);
            ExtractMeterReadings(text, bill);

            return bill;
        }

        private static string DetectSupplier(string lower)
        {
            foreach (var kv in SupplierKeywords)
            {
                foreach (var keyword in kv.Value)
                {
                    if (lower.Contains(keyword))
                        return kv.Key;
                }
            }
            return "Unknown Supplier";
        }

        private static string DetectFuelType(string lower, string supplier)
        {
            if (WaterSupplierNames.Contains(supplier)
                || lower.Contains("fresh water") || lower.Contains("wastewater")
                || lower.Contains("waste water") || lower.Contains("sewerage")
                || (lower.Contains("water charges") && !lower.Contains("hot water")))
                return "Water";

            bool hasElec = lower.Contains("electricity") || lower.Contains("electric");
            bool hasGas = lower.Contains("gas") && !lower.Contains("british gas");
            bool hasDual = lower.Contains("dual fuel") || (hasElec && hasGas);

            if (hasDual) return "Dual Fuel";
            if (hasElec) return "Electricity";
            if (hasGas) return "Gas";
            return "Energy";
        }

        private static decimal ExtractAmount(string text)
        {
            var patterns = new[]
            {
                @"total\s+(?:electricity\s+)?charges\s+for\s+this\s+period[:\s]*£([\d,]+\.?\d*)",
                @"total\s+(?:gas\s+)?charges\s+for\s+this\s+period[:\s]*£([\d,]+\.?\d*)",
                @"total\s+(?:amount\s+)?(?:due|to pay|payable|owed)[:\s]*£([\d,]+\.?\d*)",
                @"amount\s+(?:due|to pay|payable)[:\s]*£([\d,]+\.?\d*)",
                @"you\s+(?:owe|need to pay)[:\s]*£([\d,]+\.?\d*)",
                @"your\s+bill\s+is[:\s]*£([\d,]+\.?\d*)",
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success && decimal.TryParse(match.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                {
                    if (amount > 5 && amount < 5000)
                        return amount;
                }
            }

            var chargeMatches = Regex.Matches(text, @"(?:electricity|gas)\s+[\d].*?£([\d,]+\.\d{2})", RegexOptions.IgnoreCase);
            decimal chargeTotal = 0;
            foreach (Match m in chargeMatches)
            {
                if (decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    chargeTotal += val;
            }
            if (chargeTotal > 5) return chargeTotal;

            var allAmounts = Regex.Matches(text, @"£([\d,]+\.\d{2})", RegexOptions.IgnoreCase);
            var candidates = new List<decimal>();
            foreach (Match m in allAmounts)
            {
                var idx = m.Index;
                var contextStart = Math.Max(0, idx - 40);
                var context = text.Substring(contextStart, Math.Min(40, idx - contextStart)).ToLowerInvariant();
                if (context.Contains("balance") || context.Contains("credit") || context.Contains("annual") || context.Contains("year"))
                    continue;

                if (decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 10 && val < 2000)
                        candidates.Add(val);
                }
            }
            return candidates.Count > 0 ? candidates.Max() : 0;
        }

        private static decimal ExtractUsage(string text, out string uom)
        {
            uom = "kWh";

            var usedPatterns = new[]
            {
                @"(?:electricity|energy)\s+used\s*([\d,]+\.?\d*)\s*kWh",
                @"usage[:\s]*([\d,]+\.?\d*)\s*kWh",
                @"consumption[:\s]*([\d,]+\.?\d*)\s*(?:units|kWh)",
                @"units?\s+used[:\s]*([\d,]+\.?\d*)",
                @"you\s+used\s+([\d,]+\.?\d*)\s*kWh",
            };

            foreach (var pattern in usedPatterns)
            {
                var m = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 0 && val < 50000)
                        return val;
                }
            }

            var kwhPattern = @"([\d,]+\.?\d*)\s*kWh";
            decimal maxKwh = 0;
            foreach (Match m in Regex.Matches(text, kwhPattern, RegexOptions.IgnoreCase))
            {
                var idx = m.Index;
                var contextStart = Math.Max(0, idx - 60);
                var context = text.Substring(contextStart, Math.Min(60, idx - contextStart)).ToLowerInvariant();
                if (context.Contains("estimated annual") || context.Contains("annual usage") ||
                    context.Contains("a year") || context.Contains("comparison") ||
                    context.Contains("same period") || context.Contains("previous year"))
                    continue;

                if (decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > maxKwh && val < 50000)
                        maxKwh = val;
                }
            }

            if (maxKwh > 0) return maxKwh;

            var m3Match = Regex.Match(text, @"([\d,]+\.?\d*)\s*m[³3]", RegexOptions.IgnoreCase);
            if (m3Match.Success && decimal.TryParse(m3Match.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var m3Val))
            {
                if (m3Val > 0 && m3Val < 50000)
                {
                    uom = "m³";
                    return m3Val;
                }
            }

            return 0;
        }

        private static DateTime ExtractBillDate(string text)
        {
            var patterns = new[]
            {
                @"(?:bill|invoice|statement)\s*date[:\s]*([\d]{1,2}[\s/\-\.]+\w+[\s/\-\.]+[\d]{2,4})",
                @"(?:date|issued)[:\s]*([\d]{1,2}[\s/\-\.]+\w+[\s/\-\.]+[\d]{2,4})",
                @"([\d]{1,2}\s+(?:January|February|March|April|May|June|July|August|September|October|November|December)\s+\d{4})",
                @"([\d]{1,2}/[\d]{1,2}/[\d]{2,4})",
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var dateStr = match.Groups[1].Value.Trim();
                    var formats = new[]
                    {
                        "dd MMMM yyyy", "d MMMM yyyy",
                        "dd MMM yyyy", "d MMM yyyy",
                        "dd/MM/yyyy", "d/M/yyyy",
                        "dd-MM-yyyy", "d-M-yyyy",
                        "dd.MM.yyyy", "d.M.yyyy",
                        "dd/MM/yy", "d/M/yy",
                    };
                    if (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                        return dt;
                    if (DateTime.TryParse(dateStr, CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var dt2))
                        return dt2;
                }
            }

            return DateTime.Now;
        }

        private static string ExtractPeriod(string text, DateTime billDate)
        {
            var periodMatch = Regex.Match(text, @"(?:period|from)[:\s]*([\d]{1,2}[\s/\-\.]+\w+[\s/\-\.]+[\d]{2,4})\s*(?:to|[-–])\s*([\d]{1,2}[\s/\-\.]+\w+[\s/\-\.]+[\d]{2,4})", RegexOptions.IgnoreCase);
            if (periodMatch.Success)
                return $"{periodMatch.Groups[1].Value.Trim()} - {periodMatch.Groups[2].Value.Trim()}";

            return billDate.ToString("MMM yyyy");
        }

        private static string ExtractAddress(string text)
        {
            var postcodePattern = @"[A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2}";
            var allPostcodes = Regex.Matches(text, postcodePattern, RegexOptions.IgnoreCase);
            if (allPostcodes.Count == 0) return "";

            var supplyLabels = new[]
            {
                @"supply\s*address", @"your\s*address", @"property\s*address",
                @"premises\s*address", @"supply\s*point", @"your\s*property",
                @"electricity\s*supply", @"gas\s*supply", @"meter\s*address",
                @"your\s*home", @"address\s*where\s*we\s*supply",
            };

            foreach (var label in supplyLabels)
            {
                var labelMatch = Regex.Match(text, label, RegexOptions.IgnoreCase);
                if (!labelMatch.Success) continue;

                var afterLabel = text.Substring(labelMatch.Index);
                var nearbyPostcode = Regex.Match(afterLabel, postcodePattern, RegexOptions.IgnoreCase);
                if (nearbyPostcode.Success && nearbyPostcode.Index < 300)
                {
                    return PostcodeWithContext(afterLabel, nearbyPostcode);
                }
            }

            var supplierHints = new[]
            {
                @"registered\s*(office|address)", @"head\s*office", @"correspondence",
                @"write\s*to\s*us", @"our\s*address", @"return\s*address",
                @"PO\s*Box", @"Freepost",
            };
            var supplierPostcodeIndices = new HashSet<int>();
            foreach (var hint in supplierHints)
            {
                var hintMatch = Regex.Match(text, hint, RegexOptions.IgnoreCase);
                if (!hintMatch.Success) continue;
                foreach (Match pc in allPostcodes)
                {
                    if (Math.Abs(pc.Index - hintMatch.Index) < 300)
                        supplierPostcodeIndices.Add(pc.Index);
                }
            }

            if (allPostcodes.Count > 1)
            {
                foreach (Match pc in allPostcodes)
                {
                    if (!supplierPostcodeIndices.Contains(pc.Index))
                    {
                        var start = Math.Max(0, pc.Index - 80);
                        var chunk = text.Substring(start, pc.Index - start + pc.Length);
                        return PostcodeWithContext(chunk, Regex.Match(chunk, postcodePattern, RegexOptions.IgnoreCase));
                    }
                }
            }

            var lastPostcode = allPostcodes[allPostcodes.Count - 1];
            if (!supplierPostcodeIndices.Contains(lastPostcode.Index))
                return PostcodeWithContext(text, lastPostcode);

            var firstPostcode = allPostcodes[0];
            return PostcodeWithContext(text, firstPostcode);
        }

        private static string PostcodeWithContext(string text, Match postcodeMatch)
        {
            var idx = postcodeMatch.Index;
            var start = Math.Max(0, idx - 80);
            var chunk = text.Substring(start, idx - start + postcodeMatch.Length);
            var lines = chunk.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length >= 2)
                return string.Join(", ", lines.TakeLast(2).Select(l => l.Trim()));
            return postcodeMatch.Value.ToUpper().Trim();
        }

        private static string ExtractAccountNumber(string text)
        {
            var accMatch = Regex.Match(text, @"account\s*(?:number|no|ref)?[:\s]*([\dA-Z\-]{6,20})", RegexOptions.IgnoreCase);
            if (accMatch.Success)
                return accMatch.Groups[1].Value.Trim();
            return "";
        }

        private static decimal ExtractUnitRate(string text)
        {
            var patterns = new[]
            {
                @"unit\s*rate[:\s]*([\d]+\.?\d*)\s*p(?:ence)?(?:\s*/\s*kWh)?",
                @"([\d]+\.?\d*)\s*p(?:ence)?\s*/\s*kWh",
                @"rate[:\s]*([\d]+\.?\d*)\s*p\b",
            };
            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                {
                    if (rate > 1 && rate < 100)
                        return rate;
                }
            }
            return 0;
        }

        private static decimal ExtractStandingCharge(string text)
        {
            var patterns = new[]
            {
                @"standing\s*charge[:\s]*([\d]+\.?\d*)\s*p(?:ence)?(?:\s*/\s*day)?",
                @"daily\s*(?:standing\s*)?charge[:\s]*([\d]+\.?\d*)\s*p",
                @"([\d]+\.?\d*)\s*p(?:ence)?\s*(?:per|/)\s*day",
            };
            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var charge))
                {
                    if (charge > 5 && charge < 200)
                        return charge;
                }
            }
            return 0;
        }

        private static decimal ExtractVatRate(string text, string lower)
        {
            var vatMatch = Regex.Match(text, @"VAT\s*(?:@|at)?\s*([\d]+(?:\.\d+)?)\s*%", RegexOptions.IgnoreCase);
            if (vatMatch.Success && decimal.TryParse(vatMatch.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                return rate;

            if (lower.Contains("vat") && lower.Contains("20%")) return 20;
            if (lower.Contains("vat") && lower.Contains("5%")) return 5;
            return 0;
        }

        private static bool DetectEstimated(string lower)
        {
            if (lower.Contains("smart meter reading") || lower.Contains("actual reading") || lower.Contains("actual meter"))
                return false;

            return lower.Contains("estimated reading") || lower.Contains("estimated meter")
                || lower.Contains("based on an estimate")
                || (lower.Contains("reading") && lower.Contains("(e)"));
        }

        private static void ExtractPeriodDates(string text, ParsedBill bill)
        {
            var m = Regex.Match(text,
                @"(?:period|from)[:\s]*([\d]{1,2}[\s/\-\.]+\w+[\s/\-\.]+[\d]{2,4})\s*(?:to|[-–])\s*([\d]{1,2}[\s/\-\.]+\w+[\s/\-\.]+[\d]{2,4})",
                RegexOptions.IgnoreCase);
            if (!m.Success) return;

            var formats = new[]
            {
                "dd MMMM yyyy", "d MMMM yyyy", "dd MMM yyyy", "d MMM yyyy",
                "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy",
                "dd.MM.yyyy", "d.M.yyyy", "dd/MM/yy", "d/M/yy",
            };

            if (DateTime.TryParseExact(m.Groups[1].Value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
                bill.PeriodStart = start;
            else if (DateTime.TryParse(m.Groups[1].Value.Trim(), CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var s2))
                bill.PeriodStart = s2;

            if (DateTime.TryParseExact(m.Groups[2].Value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                bill.PeriodEnd = end;
            else if (DateTime.TryParse(m.Groups[2].Value.Trim(), CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var e2))
                bill.PeriodEnd = e2;
        }

        private static string DetectPaymentMethod(string lower)
        {
            if (lower.Contains("direct debit") || lower.Contains("dd")) return "Direct Debit";
            if (lower.Contains("prepayment") || lower.Contains("pay as you go") || lower.Contains("payg")) return "Prepayment";
            if (lower.Contains("standard credit") || lower.Contains("quarterly")) return "Standard Credit";
            return "Direct Debit";
        }

        private static bool DetectSolar(string lower)
        {
            return lower.Contains("export") || lower.Contains("solar") || lower.Contains("photovoltaic")
                || lower.Contains("pv generation") || lower.Contains("feed-in tariff") || lower.Contains("feed in tariff")
                || lower.Contains("fit generation") || lower.Contains("smart export guarantee") || lower.Contains("seg ")
                || lower.Contains("seg rate") || lower.Contains("generation meter")
                || lower.Contains("deemed export") || lower.Contains("electricity exported");
        }

        private static void ExtractSolarData(string text, string lower, ParsedBill bill)
        {
            bill.ExportKwh = ExtractExportKwh(text);
            bill.ExportRatePence = ExtractExportRate(text);
            bill.GenerationKwh = ExtractGenerationKwh(text);
            bill.ExportPayment = ExtractExportPayment(text);
            bill.IsDeemedExport = lower.Contains("deemed export") || lower.Contains("deemed");

            if (lower.Contains("feed-in tariff") || lower.Contains("feed in tariff") || lower.Contains("fit "))
                bill.ExportTariffType = "FiT";
            else if (lower.Contains("smart export guarantee") || lower.Contains("seg"))
                bill.ExportTariffType = "SEG";
            else if (lower.Contains("agile") && lower.Contains("outgoing"))
                bill.ExportTariffType = "Agile Export";
            else if (bill.ExportRatePence > 0 || bill.ExportKwh > 0)
                bill.ExportTariffType = "SEG";

            if (bill.ExportPayment == 0 && bill.ExportKwh > 0 && bill.ExportRatePence > 0)
                bill.ExportPayment = Math.Round(bill.ExportKwh * bill.ExportRatePence / 100m, 2);
        }

        private static decimal ExtractExportKwh(string text)
        {
            var patterns = new[]
            {
                @"export(?:ed)?\s*(?:units?|electricity)?[:\s]*([\d,]+\.?\d*)\s*kWh",
                @"([\d,]+\.?\d*)\s*kWh\s*export(?:ed)?",
                @"electricity\s+exported[:\s]*([\d,]+\.?\d*)\s*kWh",
                @"export\s+reading.*?([\d,]+\.?\d*)\s*kWh",
                @"units?\s+exported[:\s]*([\d,]+\.?\d*)",
                @"deemed\s+export[:\s]*([\d,]+\.?\d*)\s*kWh",
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 0 && val < 50000)
                        return val;
                }
            }
            return 0;
        }

        private static decimal ExtractExportRate(string text)
        {
            var patterns = new[]
            {
                @"export\s*(?:unit\s*)?rate[:\s]*([\d]+\.?\d*)\s*p(?:ence)?(?:\s*/\s*kWh)?",
                @"SEG\s*(?:rate|tariff)?[:\s]*([\d]+\.?\d*)\s*p(?:ence)?",
                @"feed.in\s*(?:tariff)?\s*(?:export)?\s*rate[:\s]*([\d]+\.?\d*)\s*p",
                @"export[:\s]*([\d]+\.?\d*)\s*p\s*/\s*kWh",
                @"([\d]+\.?\d*)\s*p(?:ence)?\s*/\s*kWh\s*(?:export|SEG)",
                @"smart\s*export\s*guarantee[:\s]*([\d]+\.?\d*)\s*p",
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                {
                    if (rate > 0 && rate < 50)
                        return rate;
                }
            }
            return 0;
        }

        private static decimal ExtractGenerationKwh(string text)
        {
            var patterns = new[]
            {
                @"generation[:\s]*([\d,]+\.?\d*)\s*kWh",
                @"generated[:\s]*([\d,]+\.?\d*)\s*kWh",
                @"([\d,]+\.?\d*)\s*kWh\s*(?:generated|generation)",
                @"total\s+(?:solar\s+)?generation[:\s]*([\d,]+\.?\d*)",
                @"PV\s+generation[:\s]*([\d,]+\.?\d*)\s*kWh",
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 0 && val < 50000)
                        return val;
                }
            }
            return 0;
        }

        private static void ExtractMeterReadings(string text, ParsedBill bill)
        {
            var smartReadings = Regex.Matches(text,
                @"(\d{3,7}\.\d{1,4})\s*(?:Smart\s*meter\s*reading|actual|customer)", RegexOptions.IgnoreCase);
            if (smartReadings.Count >= 2)
            {
                var vals = new List<decimal>();
                foreach (Match sr in smartReadings)
                {
                    if (decimal.TryParse(sr.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                        vals.Add(v);
                }
                if (vals.Count >= 2)
                {
                    vals.Sort();
                    bill.MeterReadingStart = vals[0];
                    bill.MeterReadingEnd = vals[vals.Count - 1];
                    return;
                }
            }
            else if (smartReadings.Count == 1)
            {
                if (decimal.TryParse(smartReadings[0].Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                {
                    bill.MeterReadingEnd = v;
                    return;
                }
            }

            var dateReadings = Regex.Matches(text,
                @"\d{1,2}\s+\w{3,9}\s+\d{4}\s+([\d,]+\.?\d*)\s*(?:Smart|meter|actual|reading|customer|estimated)", RegexOptions.IgnoreCase);
            if (dateReadings.Count >= 2)
            {
                var vals = new List<decimal>();
                foreach (Match dr in dateReadings)
                {
                    if (decimal.TryParse(dr.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var v) && v >= 100)
                        vals.Add(v);
                }
                if (vals.Count >= 2)
                {
                    vals.Sort();
                    bill.MeterReadingStart = vals[0];
                    bill.MeterReadingEnd = vals[vals.Count - 1];
                    return;
                }
            }

            var startPatterns = new[]
            {
                @"(?:previous|opening|start|old|from|first)\s*(?:meter\s*)?read(?:ing)?[:\s]*([\d,]+\.?\d*)",
                @"previous[:\s]*([\d,]+\.?\d*)",
                @"start\s*(?:of\s*period)?[:\s]*([\d,]+\.?\d*)",
            };
            var endPatterns = new[]
            {
                @"(?:present|closing|end|new|current|latest|final|last|to)\s*(?:meter\s*)?read(?:ing)?[:\s]*([\d,]+\.?\d*)",
                @"current[:\s]*([\d,]+\.?\d*)",
                @"latest[:\s]*([\d,]+\.?\d*)",
            };

            foreach (var p in startPatterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val >= 100 && val < 9999999) { bill.MeterReadingStart = val; break; }
                }
            }
            foreach (var p in endPatterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val >= 100 && val < 9999999 && val != bill.MeterReadingStart) { bill.MeterReadingEnd = val; break; }
                }
            }

            if (bill.MeterReadingStart == 0 && bill.MeterReadingEnd == 0)
            {
                var meterSection = Regex.Match(text, @"(?:charges\s+for\s+meter|meter\s+reading|your\s*usage)\s*\n?([\s\S]{0,400})", RegexOptions.IgnoreCase);
                if (meterSection.Success)
                {
                    var section = meterSection.Value;
                    var numbers = Regex.Matches(section, @"\b(\d{3,7}\.?\d{0,4})\b");
                    var candidates = new List<decimal>();
                    foreach (Match n in numbers)
                    {
                        if (decimal.TryParse(n.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var val) && val >= 100 && val < 9999999)
                            candidates.Add(val);
                    }
                    if (candidates.Count >= 2)
                    {
                        candidates.Sort();
                        bill.MeterReadingStart = candidates[0];
                        bill.MeterReadingEnd = candidates[candidates.Count - 1];
                    }
                    else if (candidates.Count == 1)
                        bill.MeterReadingEnd = candidates[0];
                }
            }

            if (bill.MeterReadingStart == 0 && bill.MeterReadingEnd == 0)
            {
                var readingPairs = Regex.Matches(text,
                    @"(\d{3,7}\.?\d{0,4})\s*(?:to|[-–—]|→)\s*(\d{3,7}\.?\d{0,4})");
                foreach (Match m in readingPairs)
                {
                    if (decimal.TryParse(m.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var s) &&
                        decimal.TryParse(m.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var e) &&
                        e > s && s >= 100 && e < 9999999)
                    {
                        bill.MeterReadingStart = s;
                        bill.MeterReadingEnd = e;
                        break;
                    }
                }
            }

            if (bill.MeterReadingStart > 0 && bill.MeterReadingEnd > 0 && bill.MeterReadingEnd < bill.MeterReadingStart)
            {
                (bill.MeterReadingStart, bill.MeterReadingEnd) = (bill.MeterReadingEnd, bill.MeterReadingStart);
            }
        }

        #region Water Bill Parsing

        private static void ParseWaterBill(string text, string lower, ParsedBill bill)
        {
            bill.UoM = "m³";
            bill.VatRate = 0;
            bill.BillDate = ExtractBillDate(text);
            bill.Period = ExtractPeriod(text, bill.BillDate);
            bill.Address = ExtractWaterAddress(text);
            bill.AccountNumber = ExtractAccountNumber(text);
            bill.IsEstimated = DetectEstimated(lower);
            ExtractPeriodDates(text, bill);
            bill.PaymentMethod = DetectPaymentMethod(lower);
            ExtractWaterMeterReadings(text, bill);

            bill.UnitsUsed = ExtractWaterUsage(text);
            if (bill.UnitsUsed == 0 && bill.MeterReadingEnd > 0 && bill.MeterReadingStart > 0)
                bill.UnitsUsed = bill.MeterReadingEnd - bill.MeterReadingStart;

            bill.WaterPeriods = ExtractWaterPeriods(text);
            bill.Payments = ExtractWaterPayments(text);

            if (bill.WaterPeriods != null && bill.WaterPeriods.Count > 0)
            {
                bill.Amount = bill.WaterPeriods.Sum(p =>
                    p.FreshCharge + p.FreshStanding + p.WasteCharge + p.WasteStanding);
            }

            if (bill.Amount <= 0)
                bill.Amount = ExtractWaterTotalAmount(text);

            if (bill.UnitsUsed > 0 && bill.Amount > 0 && (bill.WaterPeriods == null || bill.WaterPeriods.Count == 0))
                bill.UnitRatePence = Math.Round(bill.Amount / bill.UnitsUsed * 100m, 2);
        }

        private static string ExtractWaterAddress(string text)
        {
            var postcodeRx = @"[A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2}";
            var allPostcodes = Regex.Matches(text, postcodeRx, RegexOptions.IgnoreCase);
            if (allPostcodes.Count == 0) return ExtractAddress(text);

            var excludeHints = new[] { @"registered", @"head\s*office", @"PO\s*Box", @"Freepost", @"Lingley", @"meter\s*number" };
            var excludeIndices = new HashSet<int>();
            foreach (var hint in excludeHints)
            {
                var hm = Regex.Match(text, hint, RegexOptions.IgnoreCase);
                if (!hm.Success) continue;
                foreach (Match pc in allPostcodes)
                {
                    if (Math.Abs(pc.Index - hm.Index) < 400)
                        excludeIndices.Add(pc.Index);
                }
            }

            foreach (Match pc in allPostcodes)
            {
                if (excludeIndices.Contains(pc.Index)) continue;
                var start = Math.Max(0, pc.Index - 150);
                var chunk = text.Substring(start, pc.Index - start + pc.Length);
                var lines = chunk.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                var addressLines = lines.Where(l =>
                {
                    var t = l.Trim();
                    if (t.Length < 2) return false;
                    if (Regex.IsMatch(t, @"^\d{5,}$")) return false;
                    if (t.StartsWith("Page ")) return false;
                    return true;
                }).TakeLast(5).Select(l => l.Trim()).ToList();

                if (addressLines.Count >= 2)
                    return string.Join(", ", addressLines);
                return pc.Value.ToUpper();
            }

            return ExtractAddress(text);
        }

        private static void ExtractWaterMeterReadings(string text, ParsedBill bill)
        {
            var prevRx = @"(?:previous|old|last|opening)\s+(?:our\s+)?read(?:ing)?[\s\S]{0,20}?(\d{3,7})";
            var currRx = @"(?:current|new|present|latest|closing)\s+(?:our\s+)?read(?:ing)?[\s\S]{0,20}?(\d{3,7})";

            var pm = Regex.Match(text, prevRx, RegexOptions.IgnoreCase);
            if (pm.Success && decimal.TryParse(pm.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var prev))
                bill.MeterReadingStart = prev;

            var cm = Regex.Match(text, currRx, RegexOptions.IgnoreCase);
            if (cm.Success && decimal.TryParse(cm.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var curr))
                bill.MeterReadingEnd = curr;

            if (bill.MeterReadingStart == 0 && bill.MeterReadingEnd == 0)
                ExtractMeterReadings(text, bill);
        }

        private static decimal ExtractWaterUsage(string text)
        {
            var patterns = new[]
            {
                @"(?:you've\s+used|water\s+you've\s+used|units?\s+used|water\s+used|consumption|you\s+(?:have\s+)?used)[\s:]*([\d,]+\.?\d*)\s*(?:m[³3]|cubic\s*metres?)",
                @"([\d,]+\.?\d*)\s*(?:m[³3]|cubic\s*metres?)\s*(?:used|consumed)",
                @"=\s*([\d,]+\.?\d*)\s*(?:m[³3]|cubic\s*metres?)",
                @"\b([\d,]+\.?\d*)m[³3]\b",
                @"\(([\d,]+)\s*litres\)",
                @"([\d,]+)\s+litres",
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    if (p.Contains("litres"))
                    {
                        if (decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var litres) && litres > 100)
                            return Math.Round(litres / 1000m, 3);
                    }
                    else if (decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    {
                        if (val > 0 && val < 10000)
                            return val;
                    }
                }
            }
            return 0;
        }

        private static decimal ExtractWaterTotalAmount(string text)
        {
            var patterns = new[]
            {
                @"(?:total|replacement)\s+charges?\s*[:\s]*£([\d,]+\.?\d*)",
                @"total\s+(?:water\s+)?(?:amount|charges?)\s*[:\s]*£([\d,]+\.?\d*)",
                @"amount\s+(?:due|to pay|payable)[:\s]*£([\d,]+\.?\d*)",
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 5 && val < 5000)
                        return val;
                }
            }

            decimal sectionTotal = 0;
            foreach (Match fm in Regex.Matches(text, @"(?:Fresh\s+water|Wastewater|Clean\s+water|Sewerage)\s*\n\s*£([\d,]+\.?\d*)", RegexOptions.IgnoreCase))
            {
                if (decimal.TryParse(fm.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var sv))
                    sectionTotal += sv;
            }
            if (sectionTotal > 5) return sectionTotal;

            return ExtractAmount(text);
        }

        private static List<WaterChargePeriod> ExtractWaterPeriods(string text)
        {
            var periods = ExtractWaterPeriodsByChargesFor(text);
            if (periods != null && periods.Count > 0)
                return periods;

            return ExtractWaterPeriodsBySections(text);
        }

        private static List<WaterPayment> ExtractWaterPayments(string text)
        {
            var payments = new List<WaterPayment>();
            var section = Regex.Match(text, @"Payments you(?:'ve| have) made([\s\S]*?)Total since", RegexOptions.IgnoreCase);
            if (!section.Success) return payments;

            var rx = new Regex(@"✔\s*(\d{2})/(\d{2})/(\d{2,4})\s*£([\d,]+\.\d{2})");
            foreach (Match m in rx.Matches(section.Groups[1].Value))
            {
                int day = int.Parse(m.Groups[1].Value);
                int month = int.Parse(m.Groups[2].Value);
                int year = int.Parse(m.Groups[3].Value);
                if (year < 100) year += 2000;

                if (decimal.TryParse(m.Groups[4].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                {
                    payments.Add(new WaterPayment
                    {
                        Date = new DateTime(year, month, day),
                        Amount = amount
                    });
                }
            }
            return payments;
        }

        private static readonly string[] DateFormatsWater = {
            "dd MMMM yyyy", "d MMMM yyyy", "dd MMM yyyy", "d MMM yyyy",
            "dd/MM/yyyy", "d/M/yyyy",
        };

        private static List<WaterChargePeriod> ExtractWaterPeriodsByChargesFor(string text)
        {
            var periods = new List<WaterChargePeriod>();

            var chargesForRx = @"Charges\s+for[:\s]*(\d{1,2}\s+\w+\s+\d{4})\s*[-–]\s*(\d{1,2}\s+\w+\s+\d{4})";
            var dateMatches = Regex.Matches(text, chargesForRx, RegexOptions.IgnoreCase);
            if (dateMatches.Count == 0) return null;

            for (int i = 0; i < dateMatches.Count; i++)
            {
                var dm = dateMatches[i];
                if (!TryParseDate(dm.Groups[1].Value.Trim(), out var start)) continue;
                if (!TryParseDate(dm.Groups[2].Value.Trim(), out var end)) continue;

                var blockStart = dm.Index + dm.Length;
                var blockEnd = (i + 1 < dateMatches.Count)
                    ? dateMatches[i + 1].Index
                    : Math.Min(blockStart + 800, text.Length);
                var block = text.Substring(blockStart, blockEnd - blockStart);

                var usageRx = @"Usage\s+([\d,]+\.?\d*)\s*m[³3]\s*@\s*£([\d.]+)\s*(?:per\s*m[³3])?\s*=?\s*£([\d,]+\.\d{2})";
                var usageMatches = Regex.Matches(block, usageRx, RegexOptions.IgnoreCase);

                var standingRx = @"(\d+)\s*days?\s*@\s*£[\d.]+\s*a\s*year\s*=\s*£([\d,]+\.?\d*)";
                var standingMatches = Regex.Matches(block, standingRx, RegexOptions.IgnoreCase);

                var wp = new WaterChargePeriod { PeriodStart = start, PeriodEnd = end };

                if (usageMatches.Count >= 1)
                {
                    decimal.TryParse(usageMatches[0].Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var vol);
                    decimal.TryParse(usageMatches[0].Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate);
                    decimal.TryParse(usageMatches[0].Groups[3].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var charge);
                    wp.Volume = vol;
                    wp.FreshRate = rate;
                    wp.FreshCharge = charge;
                }
                if (standingMatches.Count >= 1)
                {
                    decimal.TryParse(standingMatches[0].Groups[2].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var sc);
                    wp.FreshStanding = sc;
                }
                if (usageMatches.Count >= 2)
                {
                    decimal.TryParse(usageMatches[1].Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate);
                    decimal.TryParse(usageMatches[1].Groups[3].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var charge);
                    wp.WasteRate = rate;
                    wp.WasteCharge = charge;
                }
                if (standingMatches.Count >= 2)
                {
                    decimal.TryParse(standingMatches[1].Groups[2].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var sc);
                    wp.WasteStanding = sc;
                }

                periods.Add(wp);
            }

            return periods.Count > 0 ? periods : null;
        }

        private static List<WaterChargePeriod> ExtractWaterPeriodsBySections(string text)
        {
            var periods = new Dictionary<string, WaterChargePeriod>();
            var dateRangeRx = @"(\d{1,2}\s+\w+\s+\d{4})\s*(?:to|-|–)\s*(\d{1,2}\s+\w+\s+\d{4})";

            foreach (var isFresh in new[] { true, false })
            {
                var headers = isFresh
                    ? new[] { "fresh water", "clean water", "water supply" }
                    : new[] { "wastewater", "waste water", "sewerage", "used water" };

                int sectionStart = -1;
                foreach (var h in headers)
                {
                    var idx = text.IndexOf(h, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0) { sectionStart = idx; break; }
                }
                if (sectionStart < 0) continue;

                var section = text.Substring(sectionStart, Math.Min(2000, text.Length - sectionStart));

                var otherHeaders = isFresh
                    ? new[] { "wastewater", "waste water", "sewerage" }
                    : new[] { "fresh water", "clean water", "water supply" };
                foreach (var oh in otherHeaders)
                {
                    var oIdx = section.IndexOf(oh, 10, StringComparison.OrdinalIgnoreCase);
                    if (oIdx > 0) { section = section.Substring(0, oIdx); break; }
                }

                foreach (Match dm in Regex.Matches(section, dateRangeRx, RegexOptions.IgnoreCase))
                {
                    if (!TryParseDate(dm.Groups[1].Value.Trim(), out var start)) continue;
                    if (!TryParseDate(dm.Groups[2].Value.Trim(), out var end)) continue;

                    var key = $"{start:yyyyMMdd}_{end:yyyyMMdd}";
                    if (!periods.ContainsKey(key))
                        periods[key] = new WaterChargePeriod { PeriodStart = start, PeriodEnd = end };

                    var afterIdx = dm.Index + dm.Length;
                    var nextDate = Regex.Match(section.Substring(afterIdx), dateRangeRx, RegexOptions.IgnoreCase);
                    var blockLen = nextDate.Success ? nextDate.Index : Math.Min(500, section.Length - afterIdx);
                    var block = section.Substring(afterIdx, blockLen);

                    var vrcMatch = Regex.Match(block,
                        @"([\d,]+\.?\d*)\s*m[³3]\s*(?:at|@)\s*£?([\d.]+).*?=?\s*£([\d,]+\.\d{2})",
                        RegexOptions.IgnoreCase);
                    decimal volume = 0, rate = 0, charge = 0, standing = 0;
                    if (vrcMatch.Success)
                    {
                        decimal.TryParse(vrcMatch.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out volume);
                        decimal.TryParse(vrcMatch.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out rate);
                        decimal.TryParse(vrcMatch.Groups[3].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out charge);
                    }

                    var standMatch = Regex.Match(block, @"(?:standing|fixed)\s*charge[\s\S]{0,30}?£([\d,]+\.?\d*)", RegexOptions.IgnoreCase);
                    if (standMatch.Success)
                        decimal.TryParse(standMatch.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out standing);

                    if (isFresh)
                    {
                        periods[key].Volume = volume;
                        periods[key].FreshRate = rate;
                        periods[key].FreshCharge = charge;
                        periods[key].FreshStanding = standing;
                    }
                    else
                    {
                        if (periods[key].Volume == 0) periods[key].Volume = volume;
                        periods[key].WasteRate = rate;
                        periods[key].WasteCharge = charge;
                        periods[key].WasteStanding = standing;
                    }
                }
            }

            var result = periods.Values.OrderBy(p => p.PeriodStart).ToList();
            return result.Count > 0 ? result : null;
        }

        private static bool TryParseDate(string s, out DateTime dt)
        {
            if (DateTime.TryParseExact(s, DateFormatsWater, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;
            return DateTime.TryParse(s, CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out dt);
        }

        public static List<MockUtilityBill> SplitWaterPeriods(ParsedBill parsed)
        {
            if (parsed.Payments != null && parsed.Payments.Count > 0)
                return SplitByPayments(parsed);

            if (parsed.WaterPeriods == null || parsed.WaterPeriods.Count == 0)
                return new List<MockUtilityBill> { ToBill(parsed) };

            var bills = new List<MockUtilityBill>();
            foreach (var wp in parsed.WaterPeriods)
            {
                var totalCharge = wp.FreshCharge + wp.FreshStanding + wp.WasteCharge + wp.WasteStanding;
                var combinedRate = wp.FreshRate + wp.WasteRate;
                var days = (wp.PeriodEnd - wp.PeriodStart).Days;
                var dailyStanding = days > 0
                    ? Math.Round((wp.FreshStanding + wp.WasteStanding) / days * 100m, 2)
                    : 0m;

                bills.Add(new MockUtilityBill
                {
                    BillDate = wp.PeriodEnd,
                    Supplier = parsed.Supplier,
                    FuelType = "Water",
                    Amount = totalCharge,
                    UnitsUsed = wp.Volume,
                    UoM = "m³",
                    Period = $"{wp.PeriodStart:dd MMM yyyy} - {wp.PeriodEnd:dd MMM yyyy}",
                    Address = parsed.Address,
                    UnitRatePence = Math.Round(combinedRate * 100m, 2),
                    StandingChargePence = dailyStanding,
                    VatRate = 0,
                    IsEstimated = parsed.IsEstimated,
                    PeriodStart = wp.PeriodStart,
                    PeriodEnd = wp.PeriodEnd,
                    PaymentMethod = parsed.PaymentMethod ?? "Direct Debit",
                    IsFromPdf = true,
                    MeterReadingStart = parsed.MeterReadingStart,
                    MeterReadingEnd = parsed.MeterReadingEnd,
                });
            }
            return bills;
        }

        private static List<MockUtilityBill> SplitByPayments(ParsedBill parsed)
        {
            var bills = new List<MockUtilityBill>();
            var totalPaid = parsed.Payments.Sum(p => p.Amount);
            var totalUsage = parsed.UnitsUsed;

            foreach (var payment in parsed.Payments)
            {
                var proportion = totalPaid > 0 ? payment.Amount / totalPaid : 0m;
                var usage = Math.Round(totalUsage * proportion, 3);
                var periodStart = new DateTime(payment.Date.Year, payment.Date.Month, 1);
                var periodEnd = periodStart.AddMonths(1).AddDays(-1);

                bills.Add(new MockUtilityBill
                {
                    BillDate = payment.Date,
                    Supplier = parsed.Supplier,
                    FuelType = "Water",
                    Amount = payment.Amount,
                    UnitsUsed = usage,
                    UoM = "m³",
                    Period = payment.Date.ToString("MMMM yyyy"),
                    Address = parsed.Address,
                    UnitRatePence = usage > 0 ? Math.Round(payment.Amount / usage * 100m, 2) : 0m,
                    StandingChargePence = 0,
                    VatRate = 0,
                    IsEstimated = parsed.IsEstimated,
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd,
                    PaymentMethod = parsed.PaymentMethod ?? "Direct Debit",
                    IsFromPdf = true,
                    MeterReadingStart = parsed.MeterReadingStart,
                    MeterReadingEnd = parsed.MeterReadingEnd,
                });
            }
            return bills;
        }

        #endregion

        private static decimal ExtractExportPayment(string text)
        {
            var patterns = new[]
            {
                @"export\s*(?:payment|credit|earnings?)[:\s]*£([\d,]+\.?\d*)",
                @"SEG\s*(?:payment|credit|earnings?)[:\s]*£([\d,]+\.?\d*)",
                @"feed.in\s*(?:tariff)?\s*(?:payment|credit)[:\s]*£([\d,]+\.?\d*)",
                @"generation\s*(?:payment|credit)[:\s]*£([\d,]+\.?\d*)",
                @"export\s*credit[:\s]*-?\s*£([\d,]+\.?\d*)",
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 0 && val < 10000)
                        return val;
                }
            }
            return 0;
        }

        public static MockUtilityBill ToBill(ParsedBill parsed)
        {
            return new MockUtilityBill
            {
                BillDate = parsed.BillDate,
                Supplier = parsed.Supplier,
                FuelType = parsed.FuelType,
                Amount = parsed.Amount,
                UnitsUsed = parsed.UnitsUsed,
                UoM = parsed.UoM,
                Period = parsed.Period,
                Address = parsed.Address,
                UnitRatePence = parsed.UnitRatePence,
                StandingChargePence = parsed.StandingChargePence,
                VatRate = parsed.VatRate > 0 ? parsed.VatRate : 5m,
                IsEstimated = parsed.IsEstimated,
                PeriodStart = parsed.PeriodStart,
                PeriodEnd = parsed.PeriodEnd,
                PaymentMethod = parsed.PaymentMethod ?? "Direct Debit",
                IsFromPdf = true,
                HasSolar = parsed.HasSolar,
                ExportKwh = parsed.ExportKwh,
                ExportRatePence = parsed.ExportRatePence,
                ExportPayment = parsed.ExportPayment,
                GenerationKwh = parsed.GenerationKwh,
                ExportTariffType = parsed.ExportTariffType,
                IsDeemedExport = parsed.IsDeemedExport,
                MeterReadingStart = parsed.MeterReadingStart,
                MeterReadingEnd = parsed.MeterReadingEnd,
            };
        }

        public static List<MockUtilityBill> SplitDualFuel(ParsedBill parsed)
        {
            if (parsed.FuelType == "Water")
                return SplitWaterPeriods(parsed);

            if (parsed.FuelType != "Dual Fuel")
                return new List<MockUtilityBill> { ToBill(parsed) };

            var text = parsed.RawText ?? "";

            var (elecSection, gasSection) = SplitFuelSections(text);

            var elecAmount = ExtractFuelTotal(text, "electricity");
            var gasAmount = ExtractFuelTotal(text, "gas");
            if (elecAmount == 0 && gasAmount == 0)
            {
                elecAmount = Math.Round(parsed.Amount / 2, 2);
                gasAmount = parsed.Amount - elecAmount;
            }
            else if (elecAmount == 0) elecAmount = parsed.Amount - gasAmount;
            else if (gasAmount == 0) gasAmount = parsed.Amount - elecAmount;

            decimal elecUsage = 0, gasUsage = 0;
            decimal elecRate = 0, gasRate = 0;
            decimal elecStanding = 0, gasStanding = 0;
            decimal elecMeterStart = 0, elecMeterEnd = 0;
            decimal gasMeterStart = 0, gasMeterEnd = 0;

            if (!string.IsNullOrEmpty(elecSection))
            {
                elecUsage = ExtractSectionUsage(elecSection);
                elecRate = ExtractUnitRate(elecSection);
                elecStanding = ExtractStandingCharge(elecSection);
                var elecParsed = new ParsedBill();
                ExtractMeterReadings(elecSection, elecParsed);
                elecMeterStart = elecParsed.MeterReadingStart;
                elecMeterEnd = elecParsed.MeterReadingEnd;
            }
            if (!string.IsNullOrEmpty(gasSection))
            {
                gasUsage = ExtractSectionUsage(gasSection);
                gasRate = ExtractUnitRate(gasSection);
                gasStanding = ExtractStandingCharge(gasSection);
                var gasParsed = new ParsedBill();
                ExtractMeterReadings(gasSection, gasParsed);
                gasMeterStart = gasParsed.MeterReadingStart;
                gasMeterEnd = gasParsed.MeterReadingEnd;
            }

            return new List<MockUtilityBill>
            {
                new MockUtilityBill
                {
                    BillDate = parsed.BillDate,
                    Supplier = parsed.Supplier,
                    FuelType = "Electricity",
                    Amount = elecAmount,
                    UnitsUsed = elecUsage,
                    UoM = "kWh",
                    Period = parsed.Period,
                    Address = parsed.Address,
                    UnitRatePence = elecRate,
                    StandingChargePence = elecStanding,
                    VatRate = parsed.VatRate > 0 ? parsed.VatRate : 5m,
                    IsEstimated = parsed.IsEstimated,
                    PeriodStart = parsed.PeriodStart,
                    PeriodEnd = parsed.PeriodEnd,
                    PaymentMethod = parsed.PaymentMethod ?? "Direct Debit",
                    IsFromPdf = true,
                    HasSolar = parsed.HasSolar,
                    ExportKwh = parsed.ExportKwh,
                    ExportRatePence = parsed.ExportRatePence,
                    ExportPayment = parsed.ExportPayment,
                    GenerationKwh = parsed.GenerationKwh,
                    ExportTariffType = parsed.ExportTariffType,
                    IsDeemedExport = parsed.IsDeemedExport,
                    MeterReadingStart = elecMeterStart,
                    MeterReadingEnd = elecMeterEnd,
                },
                new MockUtilityBill
                {
                    BillDate = parsed.BillDate,
                    Supplier = parsed.Supplier,
                    FuelType = "Gas",
                    Amount = gasAmount,
                    UnitsUsed = gasUsage,
                    UoM = "kWh",
                    Period = parsed.Period,
                    Address = parsed.Address,
                    UnitRatePence = gasRate,
                    StandingChargePence = gasStanding,
                    VatRate = parsed.VatRate > 0 ? parsed.VatRate : 5m,
                    IsEstimated = parsed.IsEstimated,
                    PeriodStart = parsed.PeriodStart,
                    PeriodEnd = parsed.PeriodEnd,
                    PaymentMethod = parsed.PaymentMethod ?? "Direct Debit",
                    IsFromPdf = true,
                    MeterReadingStart = gasMeterStart,
                    MeterReadingEnd = gasMeterEnd,
                },
            };
        }

        private static (string elec, string gas) SplitFuelSections(string text)
        {
            var gasBoundaries = new[]
            {
                @"(?:^|\n)\s*(?:⚡\s*)?Gas\s+(?:Meter|charges|Supply)",
                @"Gas\s+Meter\s+point\s+reference",
                @"Gas\s+charges\s+for\s+meter",
                @"Total\s+electricity\s+charges\s+for\s+this\s+period",
                @"Your\s+gas\s+charges",
            };

            int gasSplit = -1;
            foreach (var pattern in gasBoundaries)
            {
                var m = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    gasSplit = m.Index;
                    break;
                }
            }

            if (gasSplit > 0)
                return (text.Substring(0, gasSplit), text.Substring(gasSplit));

            return (text, text);
        }

        private static decimal ExtractFuelTotal(string text, string fuel)
        {
            var patterns = new[]
            {
                $@"total\s+{fuel}\s+charges\s+(?:for\s+this\s+period\s*)?£([\d,]+\.?\d*)",
                $@"{fuel}\s+[\d]{{1,2}}\s+\w+\s+\d{{4}}\s*[-–]\s*[\d]{{1,2}}\s+\w+\.?\s+\d{{4}}\s+£([\d,]+\.?\d*)",
                $@"{fuel}\s+charges?\s*£([\d,]+\.?\d*)",
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 1 && val < 5000)
                        return val;
                }
            }
            return 0;
        }

        private static decimal ExtractSectionUsage(string section)
        {
            var patterns = new[]
            {
                @"(?:electricity|energy)\s+used\*?\s*([\d,]+\.?\d*)\s*kWh",
                @"used\s*([\d,]+\.?\d*)\s*kWh",
                @"usage[:\s]*([\d,]+\.?\d*)\s*kWh",
                @"consumption[:\s]*([\d,]+\.?\d*)\s*(?:units|kWh)",
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(section, p, RegexOptions.IgnoreCase);
                if (m.Success && decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 0 && val < 50000)
                        return val;
                }
            }

            foreach (Match m in Regex.Matches(section, @"([\d,]+\.?\d*)\s*kWh", RegexOptions.IgnoreCase))
            {
                var idx = m.Index;
                var ctxStart = Math.Max(0, idx - 60);
                var ctx = section.Substring(ctxStart, Math.Min(60, idx - ctxStart)).ToLowerInvariant();
                if (ctx.Contains("estimated annual") || ctx.Contains("annual usage") ||
                    ctx.Contains("a year") || ctx.Contains("comparison") || ctx.Contains("previous year"))
                    continue;

                if (decimal.TryParse(m.Groups[1].Value.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                {
                    if (val > 0 && val < 50000)
                        return val;
                }
            }

            return 0;
        }
    }
}
