using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
#if WINDOWS
using Windows.Media.Ocr;
using Windows.Graphics.Imaging;
using Windows.Storage;
#endif

namespace SmartCubeMobile.Services
{
    public class InsurancePolicyOcrData
    {
        public string Provider { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyType { get; set; }
        public string Category { get; set; }
        public decimal MonthlyPremium { get; set; }
        public decimal AnnualPremium { get; set; }
        public decimal CoverAmount { get; set; }
        public decimal Excess { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NamedInsured { get; set; }
        public string VehicleReg { get; set; }
        public string VehicleMakeModel { get; set; }
        public string PropertyAddress { get; set; }
        public string PetName { get; set; }
        public string PetBreed { get; set; }
        public string Warning { get; set; }
    }

    public class SmartScanLicenceException : Exception
    {
        public string Code { get; }
        public SmartScanLicenceException(string code, string message) : base(message) { Code = code; }
    }

    public static class InsurancePolicyOcrService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(120) };

        private static string ServerBase => SessionService.ServerBase;

        public enum ScanMode { SmartScan, Basic, Simulated }

        // Basic skips the server (no licence needed); Simulated reads a hand-written response.json instead of calling it.
        public static async Task<(InsurancePolicyOcrData Data, string RawText, string Error)> ExtractFromImage(string filePath, string categoryHint, ScanMode mode = ScanMode.SmartScan)
        {
#if WINDOWS
            string rawText;
            try
            {
                var ext = Path.GetExtension(filePath).ToLower();
                var engine = OcrEngine.TryCreateFromUserProfileLanguages();
                if (engine == null)
                    return (null, null, "OCR engine not available");

                if (ext == ".pdf")
                {
                    rawText = await OcrPdfPages(filePath, engine, maxPages: 18);
                    if (string.IsNullOrWhiteSpace(rawText))
                        return (null, rawText, "No text found in PDF");
                }
                else
                {
                    var file = await StorageFile.GetFileFromPathAsync(filePath);
                    using var stream = await file.OpenReadAsync();
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    var ocrResult = await engine.RecognizeAsync(bitmap);
                    rawText = ocrResult.Text;
                }

                if (string.IsNullOrWhiteSpace(rawText))
                    return (null, rawText, "No text found in document");
            }
            catch (Exception ex)
            {
                return (null, null, $"OCR failed: {ex.Message}");
            }

            InsurancePolicyOcrData data;
            if (mode == ScanMode.Basic)
            {
                data = ParseInsuranceTextFallback(rawText, categoryHint);
                data.Warning = "Basic read (Smart Scan not used). Check the figures before saving.";
                return (data, rawText, null);
            }
            if (mode == ScanMode.Simulated)
            {
                var (simData, simError) = ParseViaSimulation(rawText, categoryHint);
                return (simData, rawText, simError);
            }
            try
            {
                data = await ParseViaServer(rawText, categoryHint);
            }
            catch (SmartScanLicenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                data = ParseInsuranceTextFallback(rawText, categoryHint);
                data.Warning = $"Smart Scan unavailable: {ex.Message}";
            }

            return (data, rawText, null);
#else
            return (null, null, "Not supported on this platform");
#endif
        }

#if WINDOWS
        private static async Task<string> OcrPdfPages(string filePath, OcrEngine engine, int maxPages)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                var pdf = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(file);
                if (pdf.PageCount == 0) return null;

                var allText = new System.Text.StringBuilder();
                var pagesToScan = Math.Min((int)pdf.PageCount, maxPages);

                for (int i = 0; i < pagesToScan; i++)
                {
                    using var page = pdf.GetPage((uint)i);
                    using var memStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                    var options = new Windows.Data.Pdf.PdfPageRenderOptions
                    {
                        DestinationWidth = (uint)(page.Size.Width * 3),
                        DestinationHeight = (uint)(page.Size.Height * 3),
                    };
                    await page.RenderToStreamAsync(memStream, options);
                    memStream.Seek(0);

                    var decoder = await BitmapDecoder.CreateAsync(memStream);
                    var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    var ocrResult = await engine.RecognizeAsync(bitmap);
                    if (!string.IsNullOrWhiteSpace(ocrResult.Text))
                    {
                        allText.Append(ocrResult.Text);
                        allText.Append('\n');
                    }
                }

                return allText.ToString();
            }
            catch
            {
                return null;
            }
        }
#endif

        private static async Task<InsurancePolicyOcrData> ParseViaServer(string ocrText, string categoryHint)
        {
            var licence = UserProfileDataService.GetProfile().SmartScanLicence;
            if (string.IsNullOrWhiteSpace(licence))
                throw new SmartScanLicenceException("no_licence", "No Smart Scan licence entered");

            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("licence", licence),
                new KeyValuePair<string, string>("category", categoryHint),
                new KeyValuePair<string, string>("text", ocrText),
            });

            using var response = await _http.PostAsync($"{ServerBase}/Handlers/SmartScan.ashx", form);
            var body = await response.Content.ReadAsStringAsync();

            JObject result;
            try { result = JObject.Parse(body); }
            catch { throw new Exception($"Server returned an unexpected response ({(int)response.StatusCode})"); }

            if (result["ok"]?.Value<bool>() != true)
            {
                var code = result["code"]?.ToString() ?? "error";
                var message = result["error"]?.ToString() ?? "Smart Scan failed";
                if (code is "invalid_licence" or "licence_inactive" or "licence_expired" or "limit_reached")
                    throw new SmartScanLicenceException(code, message);
                throw new Exception(message);
            }

            var parsed = result["data"] as JObject ?? throw new Exception("Server returned no data");
            return FromJson(parsed, categoryHint);
        }

        // ---- Simulated Smart Scan (testing without server credits) ----
        // The app writes the OCR text to SimulationDir\last_ocr.txt; a person (or Claude Code) writes
        // SimulationDir\response.json in the same shape the server returns, tagged with the text hash.
        // Kept on C: (Documents, so drive encryption covers it) rather than D:, since it holds real policy text.
        public static readonly string SimulationDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmartCube Private", "SmartScan");

        private static (InsurancePolicyOcrData Data, string Error) ParseViaSimulation(string ocrText, string categoryHint)
        {
            Directory.CreateDirectory(SimulationDir);
            var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(ocrText)))[..16];
            File.WriteAllText(Path.Combine(SimulationDir, "last_ocr.txt"), ocrText, Encoding.UTF8);
            File.WriteAllText(Path.Combine(SimulationDir, "request.json"),
                new JObject { ["sourceHash"] = hash, ["category"] = categoryHint, ["savedAt"] = DateTime.Now.ToString("s") }.ToString(),
                Encoding.UTF8);

            var responsePath = Path.Combine(SimulationDir, "response.json");
            if (File.Exists(responsePath))
            {
                try
                {
                    var json = JObject.Parse(File.ReadAllText(responsePath, Encoding.UTF8));
                    if (string.Equals(json["sourceHash"]?.ToString(), hash, StringComparison.OrdinalIgnoreCase))
                    {
                        var data = FromJson((json["data"] as JObject) ?? json, categoryHint);
                        data.Warning = "Simulated Smart Scan (response.json), not the live service.";
                        return (data, null);
                    }
                }
                catch (Exception ex)
                {
                    return (null, $"response.json could not be read: {ex.Message}");
                }
            }

            return (null, $"Simulated Smart Scan: the document text has been saved to\n{SimulationDir}\\last_ocr.txt (hash {hash}).\n\n" +
                          "Ask Claude Code to write response.json for it, then drop the document again and choose Simulated.");
        }

        private static InsurancePolicyOcrData FromJson(JObject parsed, string categoryHint)
        {
            return new InsurancePolicyOcrData
            {
                Category = categoryHint,
                Provider = Str(parsed["provider"]),
                PolicyNumber = Str(parsed["policyNumber"]),
                PolicyType = Str(parsed["policyType"]),
                AnnualPremium = Num(parsed["annualPremium"]),
                MonthlyPremium = Num(parsed["monthlyPremium"]),
                Excess = Num(parsed["excess"]),
                CoverAmount = Num(parsed["coverAmount"]),
                StartDate = Str(parsed["startDate"]),
                EndDate = Str(parsed["endDate"]),
                NamedInsured = Str(parsed["namedInsured"]),
                VehicleReg = Str(parsed["vehicleReg"]),
                VehicleMakeModel = Str(parsed["vehicleMakeModel"]),
                PropertyAddress = Str(parsed["propertyAddress"]),
                PetName = Str(parsed["petName"]),
                PetBreed = Str(parsed["petBreed"]),
            };
        }

        private static string Str(JToken t) =>
            t == null || t.Type == JTokenType.Null ? null : t.ToString();

        private static decimal Num(JToken t)
        {
            if (t == null || t.Type == JTokenType.Null) return 0;
            return decimal.TryParse(t.ToString(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0;
        }

        private static InsurancePolicyOcrData ParseInsuranceTextFallback(string text, string categoryHint)
        {
            var data = new InsurancePolicyOcrData { Category = categoryHint };
            var allText = Regex.Replace(text.Replace("\n", " ").Replace("\r", " "), @"\s+", " ").Trim();

            string[] knownProviders = {
                "Admiral", "Aviva", "Direct Line", "DirectLine", "Churchill", "LV=",
                "AXA", "Zurich", "Legal & General", "Legal and General",
                "Royal London", "Prudential", "Scottish Widows", "Vitality",
                "NFU Mutual", "Saga", "More Than", "MoreThan", "Esure",
                "Hastings Direct", "Hastings", "Swinton", "RAC", "Green Flag",
                "Hiscox", "Allianz", "RSA", "Ageas", "Covea",
                "Standard Life", "Sun Life", "Canada Life", "MetLife",
                "Nationwide", "Post Office", "Tesco", "Sainsbury",
                "Halifax", "AA Insurance", "Ecclesiastical",
                "John Lewis", "M&S", "Markel", "Endsleigh"
            };
            foreach (var provider in knownProviders)
            {
                if (allText.Contains(provider, StringComparison.OrdinalIgnoreCase))
                {
                    data.Provider = provider;
                    break;
                }
            }

            var polMatch = Regex.Match(allText,
                @"(?:Policy|Reference|Certificate)\s*(?:No|Number|Ref|#)[.:\s]*([A-Z0-9][\w/-]{3,19})",
                RegexOptions.IgnoreCase);
            if (polMatch.Success)
                data.PolicyNumber = polMatch.Groups[1].Value.Trim();

            foreach (Match m in Regex.Matches(allText, @"[£f]\s?(\d{1,3}(?:,\d{3})*(?:\.\d{1,2})?)", RegexOptions.IgnoreCase))
            {
                if (decimal.TryParse(m.Groups[1].Value.Replace(",", ""), out var val) && val > 5 && val < 5000)
                {
                    var ctxStart = Math.Max(0, m.Index - 80);
                    var ctx = allText.Substring(ctxStart, Math.Min(160, allText.Length - ctxStart)).ToUpper();
                    if (data.AnnualPremium == 0 && (ctx.Contains("ANNUAL") || ctx.Contains("TOTAL") || ctx.Contains("PREMIUM") || ctx.Contains("PRICE")))
                        data.AnnualPremium = val;
                    else if (data.Excess == 0 && ctx.Contains("EXCESS"))
                        data.Excess = val;
                }
            }

            if (data.AnnualPremium > 0 && data.MonthlyPremium == 0)
                data.MonthlyPremium = Math.Round(data.AnnualPremium / 12, 2);

            return data;
        }
    }
}
