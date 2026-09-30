using System.Text.RegularExpressions;
#if WINDOWS
using Windows.Media.Ocr;
using Windows.Graphics.Imaging;
using Windows.Storage;
#endif

namespace SmartCubeMobile.Services
{
    public class PayslipData
    {
        public string NiNumber { get; set; }
        public string TaxCode { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
        public decimal TaxDeducted { get; set; }
        public decimal NiDeducted { get; set; }
        public decimal AnnualGross { get; set; }
        public string Employer { get; set; }
        public string PayDate { get; set; }
        public string EmployeeName { get; set; }
    }

    public static class PayslipService
    {
        public static async Task<(PayslipData Data, string RawText, string Error)> ExtractFromImage(string filePath)
        {
#if WINDOWS
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                using var stream = await file.OpenReadAsync();
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                var engine = OcrEngine.TryCreateFromUserProfileLanguages();
                if (engine == null)
                    return (null, null, "OCR engine not available");

                var ocrResult = await engine.RecognizeAsync(bitmap);
                var rawText = ocrResult.Text;

                if (string.IsNullOrWhiteSpace(rawText))
                    return (null, rawText, "No text found in image");

                var data = ParsePayslipText(rawText);
                return (data, rawText, null);
            }
            catch (Exception ex)
            {
                return (null, null, $"OCR failed: {ex.Message}");
            }
#else
            return (null, null, "Not supported on this platform");
#endif
        }

        private static PayslipData ParsePayslipText(string text)
        {
            var data = new PayslipData();
            var allText = text.Replace("\n", " ").Replace("\r", " ");
            allText = Regex.Replace(allText, @"\s+", " ").Trim();

            // 1. NI Number — AB 12 34 56 C (with or without spaces)
            var niMatch = Regex.Match(allText, @"\b([A-CEGHJ-PR-TW-Z]{2})\s?(\d{2})\s?(\d{2})\s?(\d{2})\s?([A-D])\b");
            if (niMatch.Success)
                data.NiNumber = $"{niMatch.Groups[1].Value} {niMatch.Groups[2].Value} {niMatch.Groups[3].Value} {niMatch.Groups[4].Value} {niMatch.Groups[5].Value}";

            // 2. Tax Code — standalone pattern, UK format: optional S/C prefix + digits + letter(s)
            var taxCodes = Regex.Matches(allText, @"\b([SC]?\d{2,4}[LTMX]1?|BR|D[01]|NT|0T|K\d{1,4}[LTMX]?)\b");
            foreach (Match tc in taxCodes)
            {
                var code = tc.Value.ToUpper();
                // Skip if it's clearly part of a date or number sequence
                var idx = tc.Index;
                var before = idx > 0 ? allText[idx - 1] : ' ';
                var after = idx + tc.Length < allText.Length ? allText[idx + tc.Length] : ' ';
                if (before == '.' || after == '.' || before == '/' || after == '/') continue;
                data.TaxCode = code;
                break;
            }

            // 3. Employer — look for "Ltd", "Limited", "PLC", "LLP", "Group", "Inc"
            var companyMatch = Regex.Match(allText, @"([\w\s&]+(?:Ltd|Limited|PLC|LLP|Group|Inc|Co)\.?)\b", RegexOptions.IgnoreCase);
            if (companyMatch.Success)
            {
                var employer = companyMatch.Value.Trim();
                // Clean up: take reasonable length, trim leading junk
                if (employer.Length > 60)
                    employer = Regex.Match(employer, @"[A-Z][\w\s&]{2,}(?:Ltd|Limited|PLC|LLP|Group|Inc)\.?", RegexOptions.IgnoreCase).Value;
                if (employer.Length >= 3)
                    data.Employer = employer.Trim();
            }

            // 4. Pay date — DD/MM/YYYY with various separators
            var dateMatches = Regex.Matches(allText, @"\b(\d{1,2})\s?[/.\-]\s?(\d{1,2})\s?[/.\-]\s?(\d{4})\b");
            foreach (Match dm in dateMatches)
            {
                var formatted = $"{dm.Groups[1].Value}/{dm.Groups[2].Value}/{dm.Groups[3].Value}";
                data.PayDate = formatted;
                break;
            }

            // 5. Money amounts — collect all decimal numbers that look like pay amounts
            var amounts = new List<(decimal Value, int Position, string Context)>();
            foreach (Match m in Regex.Matches(allText, @"£?\s?(\d{1,6}(?:,\d{3})*\.\d{2})\b"))
            {
                var numStr = m.Groups[1].Value.Replace(",", "");
                if (decimal.TryParse(numStr, out var val) && val >= 1)
                {
                    var ctxStart = Math.Max(0, m.Index - 40);
                    var ctxLen = Math.Min(40, m.Index - ctxStart);
                    var context = allText.Substring(ctxStart, ctxLen).ToUpper();
                    amounts.Add((val, m.Index, context));
                }
            }

            // Also check for whole numbers that could be annual figures (like 35986)
            foreach (Match m in Regex.Matches(allText, @"\b(\d{4,6})\b"))
            {
                if (m.Value.Contains('.')) continue;
                if (decimal.TryParse(m.Value, out var val) && val >= 5000 && val <= 200000)
                {
                    var ctxStart = Math.Max(0, m.Index - 40);
                    var ctxLen = Math.Min(40, m.Index - ctxStart);
                    var context = allText.Substring(ctxStart, ctxLen).ToUpper();
                    if (context.Contains("TOTAL") || context.Contains("YTD") || context.Contains("TO DATE")
                        || context.Contains("CUMULATIVE") || context.Contains("ANNUAL"))
                    {
                        data.AnnualGross = val;
                    }
                }
            }

            // Try to identify amounts by nearby text
            foreach (var (val, pos, ctx) in amounts)
            {
                if (val >= 500 && val <= 20000 && data.GrossPay == 0
                    && (ctx.Contains("GROSS") || ctx.Contains("TOTAL")))
                {
                    data.GrossPay = val;
                }
                else if (val >= 500 && val <= 20000 && data.NetPay == 0
                    && (ctx.Contains("NET") || ctx.Contains("TAKE HOME")))
                {
                    data.NetPay = val;
                }
                else if (val >= 1 && val < 2000 && data.TaxDeducted == 0
                    && (ctx.Contains("PAYE") || ctx.Contains("TAX")))
                {
                    data.TaxDeducted = val;
                }
                else if (val >= 1 && val < 2000 && data.NiDeducted == 0
                    && (ctx.Contains("NI") || ctx.Contains("NATIONAL")))
                {
                    data.NiDeducted = val;
                }
            }

            // If no context-matched gross pay, take the largest amount as gross
            if (data.GrossPay == 0 && amounts.Count > 0)
            {
                var sorted = amounts.Where(a => a.Value >= 100 && a.Value <= 50000)
                    .OrderByDescending(a => a.Value).ToList();
                if (sorted.Count >= 1) data.GrossPay = sorted[0].Value;
                if (sorted.Count >= 2 && data.NetPay == 0)
                {
                    // Second largest in similar range is likely net pay or gross for tax
                    var second = sorted[1].Value;
                    if (second < data.GrossPay && second > data.GrossPay * 0.5m)
                        data.NetPay = second;
                }
            }

            // Estimate annual from gross if no YTD/annual found
            if (data.AnnualGross == 0 && data.GrossPay > 0)
                data.AnnualGross = Math.Round(data.GrossPay * 12, 0);

            // 6. Employee name — look for Mr/Mrs/Ms/Miss followed by name
            var nameMatch = Regex.Match(allText, @"\b(Mr\.?|Mrs\.?|Ms\.?|Miss)\s+([A-Z][A-Za-z]+(?:\s+[A-Z][A-Za-z]+){0,2})\b");
            if (nameMatch.Success)
                data.EmployeeName = nameMatch.Value;

            return data;
        }
    }
}
