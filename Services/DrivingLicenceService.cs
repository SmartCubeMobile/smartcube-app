using System.Text.RegularExpressions;
#if WINDOWS
using Windows.Media.Ocr;
using Windows.Graphics.Imaging;
using Windows.Storage;
#endif

namespace SmartCubeMobile.Services
{
    public class DrivingLicenceData
    {
        public string Surname { get; set; }
        public string FirstNames { get; set; }
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string LicenceNumber { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string IssuingAuthority { get; set; }
        public string Gender { get; set; }
        public string Categories { get; set; }
    }

    public static class DrivingLicenceService
    {
        public static async Task<(DrivingLicenceData Data, string RawText, string Error)> ExtractFromImage(string filePath)
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

                var data = ParseLicenceText(rawText);
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

        private static DrivingLicenceData ParseLicenceText(string text)
        {
            var data = new DrivingLicenceData();
            var allText = text.Replace("\n", " ").Replace("\r", " ");
            // Collapse multiple spaces
            allText = Regex.Replace(allText, @"\s+", " ").Trim();

            // 1. Find licence number (16 chars: SSSSS + 6 digits + 2 chars + 3 chars)
            var licMatch = Regex.Match(allText, @"[A-Z][A-Z9]{4}\d{6}[A-Z0-9]{5}");
            if (licMatch.Success)
            {
                data.LicenceNumber = licMatch.Value;
                DecodeLicenceNumber(data);
            }

            // 2. Extract dates — OCR reads as "DD MM YYYY" (space-separated) or "DD.MM.YYYY"
            var datePattern = @"\b(\d{2})\s(\d{2})\s(\d{4})\b";
            var dateMatches = Regex.Matches(allText, datePattern);
            var dates = new List<(string Formatted, DateTime Parsed)>();
            foreach (Match m in dateMatches)
            {
                var formatted = $"{m.Groups[1].Value}/{m.Groups[2].Value}/{m.Groups[3].Value}";
                if (DateTime.TryParseExact(formatted, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out var dt))
                {
                    dates.Add((formatted, dt));
                }
            }
            // Also check DD.MM.YYYY format
            foreach (Match m in Regex.Matches(allText, @"\b(\d{2})\.(\d{2})\.(\d{4})\b"))
            {
                var formatted = $"{m.Groups[1].Value}/{m.Groups[2].Value}/{m.Groups[3].Value}";
                if (DateTime.TryParseExact(formatted, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out var dt))
                {
                    dates.Add((formatted, dt));
                }
            }

            // Sort chronologically: DOB (earliest), issue date, expiry (latest)
            dates = dates.OrderBy(d => d.Parsed).ToList();
            if (dates.Count >= 1) data.DateOfBirth = dates[0].Formatted;
            if (dates.Count >= 2) data.IssueDate = dates[1].Formatted;
            if (dates.Count >= 3) data.ExpiryDate = dates[2].Formatted;

            // 3. Extract name — look for SURNAME MR/MRS/MS/MISS FIRSTNAME(S)
            // OCR format: "... CHAPMAN MR DANIEL ANTHONY 14 08 1985 ..."
            var titlePattern = @"\b([A-Z][A-Z'-]+)\s+(MR|MRS|MS|MISS|DR)\s+([A-Z][A-Z'-]+(?:\s+[A-Z][A-Z'-]+)*)\s+\d{2}\s\d{2}\s\d{4}";
            var nameMatch = Regex.Match(allText, titlePattern);
            if (nameMatch.Success)
            {
                data.Surname = TitleCase(nameMatch.Groups[1].Value);
                data.FirstNames = TitleCase(nameMatch.Groups[3].Value);
            }

            // Fallback: if no title found, try SURNAME FIRSTNAME(S) before first date
            if (string.IsNullOrEmpty(data.Surname) && dates.Count > 0)
            {
                var firstDateMatch = Regex.Match(allText, @"\d{2}\s\d{2}\s\d{4}");
                if (firstDateMatch.Success)
                {
                    var beforeDate = allText[..firstDateMatch.Index].Trim();
                    // Remove field numbers and "DRIVING LICENCE" header
                    beforeDate = Regex.Replace(beforeDate, @"DRIVING\s*LICEN[CS]E", "", RegexOptions.IgnoreCase);
                    beforeDate = Regex.Replace(beforeDate, @"\b\d[a-c]?[\.\s]", " ");
                    beforeDate = Regex.Replace(beforeDate, @"\s+", " ").Trim();

                    // Remove title if present
                    beforeDate = Regex.Replace(beforeDate, @"\b(MR|MRS|MS|MISS|DR)\b", "|");
                    var nameParts = beforeDate.Split('|', StringSplitOptions.RemoveEmptyEntries);

                    if (nameParts.Length >= 2)
                    {
                        var surnamePart = nameParts[0].Trim();
                        var firstPart = nameParts[1].Trim();
                        if (surnamePart.Length >= 2 && IsNameLike(surnamePart))
                            data.Surname = TitleCase(surnamePart);
                        if (firstPart.Length >= 2 && IsNameLike(firstPart))
                            data.FirstNames = TitleCase(firstPart);
                    }
                    else if (nameParts.Length == 1)
                    {
                        // Use licence number hint to split
                        var words = nameParts[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Where(w => w.Length >= 2 && IsNameLike(w)).ToList();
                        if (words.Count >= 2 && !string.IsNullOrEmpty(data.LicenceNumber))
                        {
                            var hint = data.LicenceNumber[..5].TrimEnd('9').ToUpper();
                            var surnameWord = words.FirstOrDefault(w => w.ToUpper().StartsWith(hint));
                            if (surnameWord != null)
                            {
                                data.Surname = TitleCase(surnameWord);
                                data.FirstNames = TitleCase(string.Join(" ",
                                    words.Where(w => w != surnameWord)));
                            }
                        }
                    }
                }
            }

            // 4. Extract address — comma-separated text ending with UK postcode
            var addrMatch = Regex.Match(allText, @"(\d+\s+[A-Z][A-Za-z\s]+(?:,\s*[A-Za-z\s]+)*,\s*[A-Z]{1,2}\d{1,2}\s?\d[A-Z]{2})");
            if (addrMatch.Success)
                data.Address = addrMatch.Value.Trim();

            // 5. Extract categories — slash-separated on UK licence, OCR reads as "B1/B/C1/C/BE/CE"
            var catMatch = Regex.Match(allText, @"(?:[A-Z]{1,3}\d?/){2,}[A-Z]{1,3}\d?");
            if (catMatch.Success)
            {
                var rawCats = catMatch.Value;
                var validCats = new[] { "AM", "A1", "A2", "A", "B1", "B", "BE",
                    "C1", "C1E", "C", "CE", "D1", "D1E", "D", "DE", "F", "G", "H", "K", "L", "N", "P", "Q" };
                var found = rawCats.Split('/')
                    .Select(c => c.Trim().ToUpper())
                    .Where(c => validCats.Contains(c))
                    .Distinct()
                    .ToList();
                if (found.Count > 0)
                    data.Categories = string.Join(", ", found);
            }

            // 6. DVLA check
            if (allText.Contains("DVLA", StringComparison.OrdinalIgnoreCase))
                data.IssuingAuthority = "DVLA";

            // 7. Build full name — first names + surname
            if (!string.IsNullOrEmpty(data.Surname) || !string.IsNullOrEmpty(data.FirstNames))
                data.FullName = $"{data.FirstNames} {data.Surname}".Trim();

            return data;
        }

        private static void DecodeLicenceNumber(DrivingLicenceData data)
        {
            var lic = data.LicenceNumber;
            if (string.IsNullOrEmpty(lic) || lic.Length < 16) return;

            // Positions 6-11: DOB encoding
            if (char.IsDigit(lic[5]) && char.IsDigit(lic[6]) && char.IsDigit(lic[7])
                && char.IsDigit(lic[8]) && char.IsDigit(lic[9]) && char.IsDigit(lic[10]))
            {
                int decade = lic[5] - '0';
                int monthRaw = int.Parse(lic[6..8]);
                int day = int.Parse(lic[8..10]);
                int yearDigit = lic[10] - '0';

                if (monthRaw > 50)
                {
                    data.Gender = "Female";
                    monthRaw -= 50;
                }
                else
                {
                    data.Gender = "Male";
                }

                int year = 1900 + (decade * 10) + yearDigit;
                if (year > DateTime.Now.Year) year -= 100;

                if (monthRaw >= 1 && monthRaw <= 12 && day >= 1 && day <= 31)
                {
                    if (string.IsNullOrEmpty(data.DateOfBirth))
                        data.DateOfBirth = $"{day:D2}/{monthRaw:D2}/{year}";
                }
            }
        }

        private static bool IsNameLike(string s)
        {
            return s.All(c => char.IsLetter(c) || c == '-' || c == ' ' || c == '\'');
        }

        private static string TitleCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return string.Join(" ", s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..].ToLower() : w));
        }
    }
}
