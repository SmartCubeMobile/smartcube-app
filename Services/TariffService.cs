using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    public class TariffOffer
    {
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string TariffName { get; set; }
        public string TariffTypes { get; set; }
        public string PaymentType { get; set; }
        public int? FixedTermMonths { get; set; }
        public string FixedTermEndDate { get; set; }
        public decimal AnnualCost { get; set; }
        public decimal? EstimatedAnnualCost { get; set; }
        public decimal ElecUnitRate { get; set; }
        public decimal ElecStandingCharge { get; set; }
        public decimal GasUnitRate { get; set; }
        public decimal GasStandingCharge { get; set; }
        public decimal? ElecExitFee { get; set; }
        public decimal? GasExitFee { get; set; }
        public decimal? MonthlyFee { get; set; }
        public bool Switchable { get; set; }
        public string SignupUrl { get; set; }

        public decimal CostForYou => EstimatedAnnualCost ?? AnnualCost;
    }

    public class TariffResult
    {
        public bool Ok { get; set; }
        public bool Available { get; set; }
        public string Error { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string RegionSource { get; set; }
        public DateTime? FetchedAt { get; set; }
        public decimal? AssumedElecKwh { get; set; }
        public decimal? AssumedGasKwh { get; set; }
        public decimal? UsedElecKwh { get; set; }
        public decimal? UsedGasKwh { get; set; }
        public List<TariffOffer> Tariffs { get; set; } = new();
    }

    public static class TariffService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(30) };
        private static readonly Regex PostcodeRx = new(@"\b([A-Z]{1,2}\d[A-Z\d]?)\s*(\d[A-Z]{2})\b", RegexOptions.IgnoreCase);

        public static string ExtractPostcode(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var m = PostcodeRx.Match(text);
            return m.Success ? $"{m.Groups[1].Value} {m.Groups[2].Value}".ToUpperInvariant() : null;
        }

        // Profile property first, then the utility property, then any bill address.
        public static (string Postcode, string Source) ResolvePostcode()
        {
            var profile = UserProfileDataService.GetProfile();
            var primary = profile.Properties.FirstOrDefault(p => p.IsPrimary) ?? profile.Properties.FirstOrDefault();
            var pc = ExtractPostcode(primary?.Postcode) ?? ExtractPostcode(primary?.Address);
            if (pc != null) return (pc, "your property");

            var prop = MockDataService.GetProperty();
            pc = ExtractPostcode(prop?.Postcode) ?? ExtractPostcode(prop?.Address);
            if (pc != null) return (pc, "your utility address");

            var fromBills = MockDataService.GetUtilityBills()
                .OrderByDescending(b => b.BillDate)
                .Select(b => ExtractPostcode(b.Address))
                .FirstOrDefault(p => p != null);
            if (fromBills != null) return (fromBills, "your energy bills");

            return (null, null);
        }

        // Annual kWh per fuel from the last year of bills; annualised if the bills cover less than a year.
        public static (decimal? Elec, decimal? Gas) EstimateAnnualKwh()
        {
            var bills = MockDataService.GetUtilityBills();
            return (Annualise(bills, "Electricity"), Annualise(bills, "Gas"));
        }

        // Annual energy spend (elec + gas) from the last year of bills, annualised the same way.
        public static decimal? EstimateAnnualSpend()
        {
            var bills = MockDataService.GetUtilityBills();
            var elec = AnnualiseAmount(bills, "Electricity");
            var gas = AnnualiseAmount(bills, "Gas");
            if (elec == null && gas == null) return null;
            return Math.Round((elec ?? 0) + (gas ?? 0), 0);
        }

        private static decimal? AnnualiseAmount(List<MockUtilityBill> bills, string fuel)
        {
            var cutoff = DateTime.Now.AddDays(-400);
            var recent = bills
                .Where(b => string.Equals(b.FuelType, fuel, StringComparison.OrdinalIgnoreCase) && b.Amount > 0 && b.BillDate >= cutoff)
                .OrderByDescending(b => b.BillDate)
                .ToList();
            if (recent.Count == 0) return null;

            decimal amount = 0, days = 0;
            foreach (var b in recent)
            {
                amount += b.Amount;
                days += b.PeriodStart.HasValue && b.PeriodEnd.HasValue && b.PeriodEnd > b.PeriodStart
                    ? (decimal)(b.PeriodEnd.Value - b.PeriodStart.Value).TotalDays
                    : 30;
                if (days >= 365) break;
            }
            return days <= 0 ? null : amount * 365m / Math.Max(days, 30);
        }

        private static decimal? Annualise(List<MockUtilityBill> bills, string fuel)
        {
            var cutoff = DateTime.Now.AddDays(-400);
            var recent = bills
                .Where(b => string.Equals(b.FuelType, fuel, StringComparison.OrdinalIgnoreCase) && b.UnitsUsed > 0 && b.BillDate >= cutoff)
                .OrderByDescending(b => b.BillDate)
                .ToList();
            if (recent.Count == 0) return null;

            decimal units = 0, days = 0;
            foreach (var b in recent)
            {
                units += b.UnitsUsed;
                if (b.PeriodStart.HasValue && b.PeriodEnd.HasValue && b.PeriodEnd > b.PeriodStart)
                    days += (decimal)(b.PeriodEnd.Value - b.PeriodStart.Value).TotalDays;
                else
                    days += 30;
                if (days >= 365) break;
            }
            if (days <= 0) return null;
            return Math.Round(units * 365m / Math.Max(days, 30), 0);
        }

        public static async Task<TariffResult> GetTariffs(string postcode, decimal? elecKwh, decimal? gasKwh)
        {
            var url = $"{SessionService.ServerBase}/api/tariffs?postcode={Uri.EscapeDataString(postcode)}";
            if (elecKwh > 0) url += $"&elecKwh={elecKwh}";
            if (gasKwh > 0) url += $"&gasKwh={gasKwh}";

            try
            {
                using var resp = await _http.GetAsync(url);
                var body = await resp.Content.ReadAsStringAsync();
                var j = JObject.Parse(body);
                var result = new TariffResult
                {
                    Ok = j["ok"]?.Value<bool>() ?? false,
                    Available = j["available"]?.Value<bool>() ?? false,
                    Error = j["error"]?.ToString(),
                    RegionId = j["regionId"]?.Value<int>() ?? 0,
                    RegionName = j["regionName"]?.ToString(),
                    RegionSource = j["regionSource"]?.ToString(),
                    FetchedAt = j["fetchedAt"]?.Type == JTokenType.Date ? j["fetchedAt"].Value<DateTime>() : null,
                    AssumedElecKwh = Dec(j["assumedElecKwh"]),
                    AssumedGasKwh = Dec(j["assumedGasKwh"]),
                    UsedElecKwh = Dec(j["usedElecKwh"]),
                    UsedGasKwh = Dec(j["usedGasKwh"]),
                };
                if (j["tariffs"] is JArray arr)
                    result.Tariffs = arr.Select(t => new TariffOffer
                    {
                        SupplierName = t["supplierName"]?.ToString(),
                        SupplierCode = t["supplierCode"]?.ToString(),
                        TariffName = t["tariffName"]?.ToString(),
                        TariffTypes = t["tariffTypes"]?.ToString(),
                        PaymentType = t["paymentType"]?.ToString(),
                        FixedTermMonths = t["fixedTermMonths"]?.Type == JTokenType.Integer ? t["fixedTermMonths"].Value<int>() : null,
                        FixedTermEndDate = t["fixedTermEndDate"]?.Type == JTokenType.Null ? null : t["fixedTermEndDate"]?.ToString(),
                        AnnualCost = Dec(t["annualCost"]) ?? 0,
                        EstimatedAnnualCost = Dec(t["estimatedAnnualCost"]),
                        ElecUnitRate = Dec(t["elecUnitRate"]) ?? 0,
                        ElecStandingCharge = Dec(t["elecStandingCharge"]) ?? 0,
                        GasUnitRate = Dec(t["gasUnitRate"]) ?? 0,
                        GasStandingCharge = Dec(t["gasStandingCharge"]) ?? 0,
                        ElecExitFee = Dec(t["elecExitFee"]),
                        GasExitFee = Dec(t["gasExitFee"]),
                        MonthlyFee = Dec(t["monthlyFee"]),
                        Switchable = t["switchable"]?.Value<bool>() ?? false,
                        SignupUrl = t["signupUrl"]?.Type == JTokenType.Null ? null : t["signupUrl"]?.ToString(),
                    }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return new TariffResult { Ok = false, Error = "Could not reach the SmartCube server: " + ex.Message };
            }
        }

        private static decimal? Dec(JToken t)
        {
            if (t == null || t.Type == JTokenType.Null) return null;
            return decimal.TryParse(t.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;
        }
    }
}
