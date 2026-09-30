using System.Diagnostics;
using System.Text;

namespace SmartCubeMobile.Services
{
    public class TransactionReportRow
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public string Symbol { get; set; }
        public string Source { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Value { get; set; }
        public string PnL { get; set; }
        public string Hash { get; set; }
    }

    public static class PdfReportService
    {
        private static readonly string[] Headers = { "Date", "Type", "Asset", "Source", "From", "To", "Quantity", "Price", "Value", "P&L", "Hash" };
        private static readonly float[] ColW = { 65, 48, 38, 72, 85, 85, 82, 65, 65, 95, 82 };

        private const float PW = 842, PH = 595, ML = 30, MR = 30, MT = 30, MB = 30;
        private const float RH = 15;

        public static async Task GenerateAndOpenAsync(string title, string subtitle, List<string> filters, List<TransactionReportRow> rows)
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmartCubeMobile");
            Directory.CreateDirectory(dir);
            var safe = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            var path = Path.Combine(dir, $"Tx_{safe}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var pdf = Build(title, subtitle, filters, rows);
            await File.WriteAllBytesAsync(path, pdf);

            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }

        private static byte[] Build(string title, string subtitle, List<string> filters, List<TransactionReportRow> rows)
        {
            var pages = new List<byte[]>();
            float titleH = 55, hdrH = RH + 4, footH = 20;
            int rowsFirst = (int)((PH - MT - MB - titleH - hdrH - footH) / RH);
            int rowsNext = (int)((PH - MT - MB - hdrH - footH) / RH);
            int total = rows.Count <= rowsFirst ? 1 : 1 + (int)Math.Ceiling((double)(rows.Count - rowsFirst) / rowsNext);
            if (rows.Count == 0) total = 1;

            int ri = 0;
            for (int p = 0; p < total; p++)
            {
                var sb = new StringBuilder();
                float y = PH - MT;

                if (p == 0)
                {
                    sb.Append($"BT /F2 14 Tf {ML} {y - 14} Td ({Esc(title)}) Tj ET\n");
                    y -= 18;
                    var sub = subtitle ?? "";
                    if (filters != null && filters.Count > 0) sub += $"  |  Filters: {string.Join(", ", filters)}";
                    sub += $"  |  Generated: {DateTime.Now:dd MMM yyyy HH:mm}";
                    sb.Append($"BT /F1 8 Tf {ML} {y - 8} Td ({Esc(sub)}) Tj ET\n");
                    y -= 12;
                    sb.Append($"BT /F1 8 Tf {ML} {y - 8} Td ({rows.Count} transactions) Tj ET\n");
                    y -= 14;
                    sb.Append($"0.7 G {ML} {y} m {PW - MR} {y} l S\n");
                    y -= 6;
                }

                sb.Append($"0.9 0.9 0.9 rg {ML} {y - RH} {PW - ML - MR} {RH} re f\n");
                sb.Append("0 0 0 rg\n");
                sb.Append($"BT /F2 7 Tf {ML + 2} {y - RH + 4} Td");
                for (int c = 0; c < Headers.Length; c++)
                {
                    if (c > 0) sb.Append($" {ColW[c - 1]} 0 Td");
                    sb.Append($" ({Esc(Headers[c])}) Tj");
                }
                sb.Append(" ET\n");
                y -= RH + 2;
                sb.Append($"0.7 G {ML} {y} m {PW - MR} {y} l S\n");
                y -= 2;

                int max = p == 0 ? rowsFirst : rowsNext;
                int drawn = 0;
                while (ri < rows.Count && drawn < max)
                {
                    var r = rows[ri];
                    if (drawn % 2 == 1)
                    {
                        sb.Append($"0.95 0.95 0.95 rg {ML} {y - RH} {PW - ML - MR} {RH} re f\n0 0 0 rg\n");
                    }

                    var cells = new[] { r.Date, r.Type, r.Symbol, r.Source, r.From, r.To, r.Quantity, r.Price, r.Value, r.PnL, r.Hash };
                    sb.Append($"BT /F1 7 Tf {ML + 2} {y - RH + 4} Td");
                    for (int c = 0; c < cells.Length; c++)
                    {
                        int mc = (int)(ColW[c] / 3.5);
                        var txt = Trunc(cells[c] ?? "-", mc);
                        if (c > 0) sb.Append($" {ColW[c - 1]} 0 Td");
                        sb.Append($" ({Esc(txt)}) Tj");
                    }
                    sb.Append(" ET\n");
                    y -= RH;
                    ri++;
                    drawn++;
                }

                if (rows.Count == 0)
                {
                    sb.Append($"BT /F1 9 Tf {PW / 2 - 50} {y - 30} Td (No transactions found) Tj ET\n");
                }

                sb.Append($"0.7 G {ML} {y} m {PW - MR} {y} l S\n");
                sb.Append($"BT /F1 8 Tf {PW / 2 - 25} {MB} Td (Page {p + 1} of {total}) Tj ET\n");
                sb.Append($"BT /F1 7 Tf 0.5 G {PW - MR - 85} {MB} Td (SmartCubeMobile) Tj 0 G ET\n");

                pages.Add(Encoding.ASCII.GetBytes(sb.ToString()));
            }

            return Assemble(pages);
        }

        private static byte[] Assemble(List<byte[]> pageStreams)
        {
            using var ms = new MemoryStream();
            var offsets = new List<long>();

            void W(string s) { var b = Encoding.ASCII.GetBytes(s); ms.Write(b, 0, b.Length); }

            W("%PDF-1.4\n");

            offsets.Add(ms.Position);
            W("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

            int baseObj = 5;
            var pageNums = Enumerable.Range(0, pageStreams.Count).Select(i => baseObj + i * 2).ToList();

            offsets.Add(ms.Position);
            W($"2 0 obj\n<< /Type /Pages /Kids [{string.Join(" ", pageNums.Select(n => $"{n} 0 R"))}] /Count {pageStreams.Count} >>\nendobj\n");

            offsets.Add(ms.Position);
            W("3 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n");

            offsets.Add(ms.Position);
            W("4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n");

            for (int i = 0; i < pageStreams.Count; i++)
            {
                var contentNum = pageNums[i] + 1;
                var stream = pageStreams[i];

                offsets.Add(ms.Position);
                W($"{pageNums[i]} 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PW} {PH}] /Contents {contentNum} 0 R /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> >>\nendobj\n");

                offsets.Add(ms.Position);
                W($"{contentNum} 0 obj\n<< /Length {stream.Length} >>\nstream\n");
                ms.Write(stream, 0, stream.Length);
                W("\nendstream\nendobj\n");
            }

            var xref = ms.Position;
            int cnt = offsets.Count + 1;
            W($"xref\n0 {cnt}\n0000000000 65535 f \n");
            foreach (var o in offsets) W($"{o:D10} 00000 n \n");
            W($"trailer\n<< /Size {cnt} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");

            return ms.ToArray();
        }

        private static string Esc(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                switch (c)
                {
                    case '(': sb.Append("\\("); break;
                    case ')': sb.Append("\\)"); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': case '\r': sb.Append(' '); break;
                    case '—': sb.Append('-'); break;
                    case '–': sb.Append('-'); break;
                    case '→': sb.Append("->"); break;
                    default:
                        if (c < 128) sb.Append(c);
                        else if (c <= 255) sb.Append($"\\{Convert.ToString(c, 8).PadLeft(3, '0')}");
                        break;
                }
            }
            return sb.ToString();
        }

        private static string Trunc(string s, int max)
        {
            if (string.IsNullOrEmpty(s) || s.Length <= max) return s;
            return s[..(max - 2)] + "..";
        }
    }
}
