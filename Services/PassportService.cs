using System.Text.RegularExpressions;
#if WINDOWS
using Windows.Media.Ocr;
using Windows.Graphics.Imaging;
using Windows.Storage;
#endif

namespace SmartCubeMobile.Services
{
    public class PassportData
    {
        public string Surname { get; set; }
        public string FirstNames { get; set; }
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string PlaceOfBirth { get; set; }
    }

    public static class PassportService
    {
        public static async Task<(PassportData Data, string RawText, string Error)> ExtractFromImage(string filePath)
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

                var data = ParsePassportText(rawText);
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

        private static PassportData ParsePassportText(string text)
        {
            var data = new PassportData();
            var allText = text.Replace("\n", " ").Replace("\r", " ");
            allText = Regex.Replace(allText, @"\s+", " ").Trim();

            // UK passport uses bilingual labels with field numbers:
            // Surname/Nom (1) CHAPMAN
            // Given names/Prénoms (2) DANIEL ANTHONY
            // Nationality/Nationalité (3) BRITISH CITIZEN
            // Date of birth/... (4) 14 AUG/AOUT 85
            // Sex/Sexe (5) M
            // Date of issue/... (6) ...
            // Date of expiry/... (7) ...
            // Place of birth/... (8) ...
            // Passport No/Passeport No. XXXXXXXXX

            // 1. Surname — text between (1) and next label/field number
            var surnameMatch = Regex.Match(allText, @"\(1\)\s*([A-Z][A-Z' -]+?)(?=\s+Given|\s+\(2\)|\s*$)", RegexOptions.IgnoreCase);
            if (surnameMatch.Success)
                data.Surname = TitleCase(surnameMatch.Groups[1].Value.Trim());

            // 2. First names — text between (2) and next label/field number
            var givenMatch = Regex.Match(allText, @"\(2\)\s*([A-Z][A-Z' -]+?)(?=\s+Nation|\s+\(3\)|\s+Date|\s*$)", RegexOptions.IgnoreCase);
            if (givenMatch.Success)
                data.FirstNames = TitleCase(givenMatch.Groups[1].Value.Trim());

            // 3. Nationality — text between (3) and next label
            var natMatch = Regex.Match(allText, @"\(3\)\s*([A-Z][A-Z ]+?)(?=\s+Date|\s+\(4\)|\s+TIZEN|\s+C\s|\s*$)", RegexOptions.IgnoreCase);
            if (natMatch.Success)
            {
                var nat = natMatch.Groups[1].Value.Trim();
                if (nat.StartsWith("BRITISH", StringComparison.OrdinalIgnoreCase))
                    data.Nationality = "British Citizen";
                else
                    data.Nationality = TitleCase(nat);
            }

            // If no field numbers found, try MRZ
            if (string.IsNullOrEmpty(data.Surname))
            {
                var mrzMatch = Regex.Match(allText, @"P<([A-Z]{3})([A-Z]+)<<([A-Z<]+)");
                if (mrzMatch.Success)
                {
                    data.Surname = TitleCase(mrzMatch.Groups[2].Value.Replace("<", " ").Trim());
                    data.FirstNames = TitleCase(mrzMatch.Groups[3].Value.Replace("<", " ").Trim());
                    data.Nationality = mrzMatch.Groups[1].Value == "GBR" ? "British Citizen" : mrzMatch.Groups[1].Value;
                }
            }

            // 4. Passport number — digits after "Passport No" or "Passeport No"
            var passMatch = Regex.Match(allText, @"(?:Passport|Passeport)\s*(?:No|N[o0])[\]./:\s]*(\d[\d\s]{6,10})", RegexOptions.IgnoreCase);
            if (passMatch.Success)
                data.PassportNumber = passMatch.Groups[1].Value.Replace(" ", "").Trim();

            // Fallback: standalone 9-digit number
            if (string.IsNullOrEmpty(data.PassportNumber))
            {
                var digitMatch = Regex.Match(allText, @"\b(\d{9})\b");
                if (digitMatch.Success)
                    data.PassportNumber = digitMatch.Value;
            }

            // MRZ line 2 for passport number + DOB + gender + expiry
            var mrz2 = Regex.Match(allText, @"(\d{9})\d([A-Z]{3})(\d{6})\d([MF<])(\d{6})");
            if (mrz2.Success)
            {
                if (string.IsNullOrEmpty(data.PassportNumber))
                    data.PassportNumber = mrz2.Groups[1].Value;
                data.DateOfBirth = MrzDateToFormatted(mrz2.Groups[3].Value);
                data.Gender = mrz2.Groups[4].Value == "M" ? "Male" : mrz2.Groups[4].Value == "F" ? "Female" : null;
                data.ExpiryDate = MrzDateToFormatted(mrz2.Groups[5].Value);
            }

            // 5. Gender — after Sex/Sexe or (5)
            if (string.IsNullOrEmpty(data.Gender))
            {
                var sexMatch = Regex.Match(allText, @"(?:Sex|Sexe|\(5\))\s*\(?(\d?\)?\s*)?([MF])\b", RegexOptions.IgnoreCase);
                if (sexMatch.Success)
                    data.Gender = sexMatch.Groups[2].Value.ToUpper() == "M" ? "Male" : "Female";
                else if (Regex.IsMatch(allText, @"\bMALE\b", RegexOptions.IgnoreCase))
                    data.Gender = "Male";
                else if (Regex.IsMatch(allText, @"\bFEMALE\b", RegexOptions.IgnoreCase))
                    data.Gender = "Female";
            }

            // 6. Dates — DD MMM YY or DD/MM/YYYY or DD MM YYYY patterns
            // UK passport dates use "14 AUG/AOUT 85" format
            var months = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"JAN",1},{"FEB",2},{"MAR",3},{"APR",4},{"MAY",5},{"JUN",6},
                {"JUL",7},{"AUG",8},{"SEP",9},{"OCT",10},{"NOV",11},{"DEC",12},
                {"JAN.",1},{"FEB.",2},{"MAR.",3},{"APR.",4},{"JUN.",6},
                {"JUL.",7},{"AUG.",8},{"SEP.",9},{"OCT.",10},{"NOV.",11},{"DEC.",12}
            };

            // Pattern: DD MON/MON YY (e.g. "14 AUG /AOUT 85" or "14 AUG 85")
            var dateTextMatches = Regex.Matches(allText, @"(\d{1,2})\s+([A-Z]{3})(?:\s*/\s*[A-Z]+)?\s+(\d{2})\b");
            var parsedDates = new List<(string Formatted, DateTime Parsed)>();
            foreach (Match dm in dateTextMatches)
            {
                var day = int.Parse(dm.Groups[1].Value);
                var monStr = dm.Groups[2].Value.ToUpper();
                var yearShort = int.Parse(dm.Groups[3].Value);
                if (months.TryGetValue(monStr, out var month))
                {
                    var year = yearShort + (yearShort > 50 ? 1900 : 2000);
                    if (day >= 1 && day <= 31)
                    {
                        try
                        {
                            var dt = new DateTime(year, month, day);
                            parsedDates.Add(($"{day:D2}/{month:D2}/{year}", dt));
                        }
                        catch { }
                    }
                }
            }

            // Also try DD/MM/YYYY or DD.MM.YYYY
            foreach (Match m in Regex.Matches(allText, @"\b(\d{2})[/.\-](\d{2})[/.\-](\d{4})\b"))
            {
                var formatted = $"{m.Groups[1].Value}/{m.Groups[2].Value}/{m.Groups[3].Value}";
                if (DateTime.TryParseExact(formatted, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out var dt))
                    parsedDates.Add((formatted, dt));
            }

            parsedDates = parsedDates.OrderBy(d => d.Parsed).ToList();
            if (string.IsNullOrEmpty(data.DateOfBirth) && parsedDates.Count >= 1)
                data.DateOfBirth = parsedDates[0].Formatted;
            if (string.IsNullOrEmpty(data.IssueDate) && parsedDates.Count >= 2)
                data.IssueDate = parsedDates[1].Formatted;
            if (string.IsNullOrEmpty(data.ExpiryDate) && parsedDates.Count >= 3)
                data.ExpiryDate = parsedDates[2].Formatted;

            // 7. Place of birth — after (8) or "Place of birth"
            var pobMatch = Regex.Match(allText, @"(?:\(8\)|Place\s*of\s*birth)[/\w\s]*?([A-Z]{2,}(?:\s+[A-Z]{2,})*?)(?=\s+\(|\s+Date|\s+Authority|\s+P<|\s*$)", RegexOptions.IgnoreCase);
            if (pobMatch.Success)
            {
                var pob = pobMatch.Groups[1].Value.Trim();
                if (pob.Length >= 2)
                    data.PlaceOfBirth = TitleCase(pob);
            }

            // 8. Nationality fallback
            if (string.IsNullOrEmpty(data.Nationality))
            {
                if (allText.Contains("BRITISH", StringComparison.OrdinalIgnoreCase))
                    data.Nationality = "British Citizen";
                else if (allText.Contains("GBR"))
                    data.Nationality = "British Citizen";
            }

            // Build full name
            if (!string.IsNullOrEmpty(data.Surname) || !string.IsNullOrEmpty(data.FirstNames))
                data.FullName = $"{data.FirstNames} {data.Surname}".Trim();

            return data;
        }

        private static string MrzDateToFormatted(string yymmdd)
        {
            if (yymmdd.Length != 6) return null;
            if (!int.TryParse(yymmdd[..2], out var yy)) return null;
            if (!int.TryParse(yymmdd[2..4], out var mm)) return null;
            if (!int.TryParse(yymmdd[4..6], out var dd)) return null;
            var year = yy + (yy > 50 ? 1900 : 2000);
            if (mm < 1 || mm > 12 || dd < 1 || dd > 31) return null;
            return $"{dd:D2}/{mm:D2}/{year}";
        }

        private static string TitleCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return string.Join(" ", s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..].ToLower() : w));
        }
    }
}
