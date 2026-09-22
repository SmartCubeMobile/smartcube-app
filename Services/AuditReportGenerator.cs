using iText.Kernel.Pdf;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.IO.Font.Constants;
using SmartCubeMobile.MockData;

using PdfCell = iText.Layout.Element.Cell;
using PdfBorder = iText.Layout.Borders.Border;
using PdfTextAlign = iText.Layout.Properties.TextAlignment;
using PdfVertAlign = iText.Layout.Properties.VerticalAlignment;

namespace SmartCubeMobile.Services
{
    public static class AuditReportGenerator
    {
        public static string GenerateReport(List<AuditFinding> findings, List<MockUtilityBill> bills)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrEmpty(documentsPath) || !Directory.Exists(documentsPath))
                documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (string.IsNullOrEmpty(documentsPath) || !Directory.Exists(documentsPath))
                documentsPath = AppContext.BaseDirectory;

            var fileName = $"SmartCube_Bill_Audit_{DateTime.Now:yyyy-MM-dd_HHmm}.pdf";
            var filePath = System.IO.Path.Combine(documentsPath, fileName);

            var writer = new PdfWriter(filePath);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf, PageSize.A4);
            doc.SetMargins(40, 40, 40, 40);

            var bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var accent = new DeviceRgb(59, 130, 246);
            var grey = new DeviceRgb(148, 163, 184);
            var darkGrey = new DeviceRgb(100, 116, 139);
            var textLight = new DeviceRgb(241, 245, 249);
            var red = new DeviceRgb(239, 68, 68);
            var amber = new DeviceRgb(245, 158, 11);
            var green = new DeviceRgb(34, 197, 94);
            var dividerColor = new DeviceRgb(42, 63, 107);

            // ---- HEADER ----
            doc.Add(new Paragraph("SmartCube Bill Audit Report")
                .SetFont(bold).SetFontSize(22).SetFontColor(accent).SetMarginBottom(4));
            doc.Add(new Paragraph($"Generated {DateTime.Now:dddd d MMMM yyyy} at {DateTime.Now:HH:mm}")
                .SetFont(regular).SetFontSize(10).SetFontColor(grey).SetMarginBottom(4));
            doc.Add(new Paragraph("Automated compliance audit of your UK energy bills")
                .SetFont(regular).SetFontSize(10).SetFontColor(grey).SetMarginBottom(16));
            doc.Add(new Paragraph("").SetBorderBottom(new SolidBorder(dividerColor, 0.5f)).SetMarginBottom(8));

            // ---- SUMMARY ----
            doc.Add(new Paragraph("Audit Summary")
                .SetFont(bold).SetFontSize(16).SetFontColor(textLight).SetMarginTop(12).SetMarginBottom(8));

            var criticalCount = findings.Count(f => f.Severity == AuditSeverity.Critical);
            var totalSavings = findings.Sum(f => f.PotentialSavings);

            var summary = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1 }))
                .UseAllAvailableWidth().SetMarginBottom(12);

            AddStatCell(summary, bills.Count.ToString(), "Bills Checked", accent, bold, regular, grey);
            AddStatCell(summary, findings.Count.ToString(), "Issues Found",
                findings.Count > 0 ? red : green, bold, regular, grey);
            AddStatCell(summary, criticalCount.ToString(), "Critical", red, bold, regular, grey);
            AddStatCell(summary, $"£{totalSavings:F2}", "Potential Savings", amber, bold, regular, grey);
            doc.Add(summary);

            if (findings.Count == 0)
            {
                doc.Add(new Paragraph("All bills passed compliance checks. No issues were detected.")
                    .SetFont(regular).SetFontSize(11).SetFontColor(green).SetMarginBottom(12));
            }

            doc.Add(new Paragraph("").SetBorderBottom(new SolidBorder(dividerColor, 0.5f)).SetMarginBottom(8));

            // ---- FINDINGS ----
            if (findings.Count > 0)
            {
                doc.Add(new Paragraph("Detailed Findings")
                    .SetFont(bold).SetFontSize(16).SetFontColor(textLight).SetMarginTop(12).SetMarginBottom(8));

                var grouped = findings.GroupBy(f => f.CheckType).ToList();
                foreach (var group in grouped)
                {
                    var maxSev = group.Max(f => f.Severity);
                    var sevColor = maxSev == AuditSeverity.Critical ? red
                        : maxSev == AuditSeverity.Alert ? amber
                        : maxSev == AuditSeverity.Warning ? accent : darkGrey;
                    var sevLabel = maxSev.ToString().ToUpper();

                    var heading = new Paragraph()
                        .Add(new Text(group.Key + " ").SetFont(bold).SetFontSize(13).SetFontColor(textLight))
                        .Add(new Text("  " + sevLabel + "  ").SetFont(bold).SetFontSize(8).SetFontColor(sevColor))
                        .SetMarginTop(12).SetMarginBottom(6);
                    doc.Add(heading);

                    var explanation = GetExplanation(group.Key);
                    if (explanation != null)
                    {
                        doc.Add(new Paragraph(explanation)
                            .SetFont(regular).SetFontSize(9).SetFontColor(grey)
                            .SetMarginBottom(8).SetPaddingLeft(8)
                            .SetBorderLeft(new SolidBorder(sevColor, 2)));
                    }

                    foreach (var f in group)
                    {
                        var ft = new Table(UnitValue.CreatePercentArray(new float[] { 4, 1 }))
                            .UseAllAvailableWidth().SetMarginBottom(6);

                        var left = new PdfCell().SetBorder(PdfBorder.NO_BORDER).SetPadding(8);
                        left.Add(new Paragraph(f.Title).SetFont(bold).SetFontSize(11).SetFontColor(textLight));
                        left.Add(new Paragraph(f.Description).SetFont(regular).SetFontSize(9).SetFontColor(grey));
                        left.Add(new Paragraph("Bill: " + f.BillReference)
                            .SetFont(regular).SetFontSize(8).SetFontColor(darkGrey).SetMarginTop(4));

                        var right = new PdfCell().SetBorder(PdfBorder.NO_BORDER).SetPadding(8)
                            .SetTextAlignment(PdfTextAlign.RIGHT).SetVerticalAlignment(PdfVertAlign.MIDDLE);

                        if (f.PotentialSavings > 0)
                        {
                            right.Add(new Paragraph($"£{f.PotentialSavings:F2}")
                                .SetFont(bold).SetFontSize(14).SetFontColor(red));
                            right.Add(new Paragraph("potential saving")
                                .SetFont(regular).SetFontSize(7).SetFontColor(grey));
                        }
                        else
                        {
                            right.Add(new Paragraph(f.Severity.ToString())
                                .SetFont(bold).SetFontSize(10).SetFontColor(sevColor));
                        }

                        ft.AddCell(left);
                        ft.AddCell(right);
                        doc.Add(ft);
                    }
                }

                doc.Add(new Paragraph("").SetBorderBottom(new SolidBorder(dividerColor, 0.5f)).SetMarginBottom(8));
            }

            // ---- BILLS TABLE ----
            doc.Add(new Paragraph("Bills Analysed")
                .SetFont(bold).SetFontSize(16).SetFontColor(textLight).SetMarginTop(12).SetMarginBottom(8));

            var headerBg = new DeviceRgb(30, 41, 59);
            var rowBg1 = new DeviceRgb(15, 23, 42);
            var rowBg2 = new DeviceRgb(20, 30, 50);
            var cellText = new DeviceRgb(203, 213, 225);

            var bt = new Table(UnitValue.CreatePercentArray(new float[] { 2, 2, 1.5f, 1, 1, 1 }))
                .UseAllAvailableWidth().SetMarginBottom(16);

            foreach (var h in new[] { "Period", "Supplier", "Type", "Units", "Amount", "Rate" })
            {
                bt.AddHeaderCell(new PdfCell().SetBackgroundColor(headerBg)
                    .SetBorder(PdfBorder.NO_BORDER).SetPadding(6)
                    .Add(new Paragraph(h).SetFont(bold).SetFontSize(8).SetFontColor(grey)));
            }

            var ordered = bills.OrderByDescending(b => b.BillDate).Take(50).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                var b = ordered[i];
                var bg = i % 2 == 0 ? rowBg1 : rowBg2;
                AddCell(bt, b.Period ?? "", regular, bg, cellText);
                AddCell(bt, b.Supplier ?? "", regular, bg, cellText);
                AddCell(bt, b.FuelType ?? "", regular, bg, cellText);
                AddCell(bt, b.UnitsUsed > 0 ? $"{b.UnitsUsed:N0} {b.UoM}" : "-", regular, bg, cellText);
                AddCell(bt, $"£{b.Amount:F2}", regular, bg, cellText);
                AddCell(bt, b.UnitRatePence > 0 ? $"{b.UnitRatePence:F2}p" : "-", regular, bg, cellText);
            }
            doc.Add(bt);

            doc.Add(new Paragraph("").SetBorderBottom(new SolidBorder(dividerColor, 0.5f)).SetMarginBottom(8));

            // ---- YOUR RIGHTS ----
            doc.Add(new Paragraph("Your Rights as a UK Energy Consumer")
                .SetFont(bold).SetFontSize(16).SetFontColor(textLight).SetMarginTop(12).SetMarginBottom(8));

            AddRight(doc, bold, regular, accent, grey, "Ofgem Price Cap",
                "Your supplier cannot charge more than the Ofgem price cap for unit rates or standing charges. " +
                "The cap is reviewed quarterly (January, April, July, October) and varies by region. " +
                "If you believe you have been overcharged, contact your supplier first. If unresolved after " +
                "8 weeks, complain to the Energy Ombudsman (free service).");

            AddRight(doc, bold, regular, accent, grey, "Back-billing Protection",
                "Under Ofgem rules (Licence Condition 21B.10), suppliers cannot back-bill for energy consumed " +
                "more than 12 months ago, provided you haven't already been billed for it. This protection " +
                "applies to both gas and electricity customers. If you receive a catch-up bill covering more " +
                "than 12 months, challenge the charges beyond the 12-month window.");

            AddRight(doc, bold, regular, accent, grey, "Estimated vs Actual Readings",
                "You have the right to submit meter readings at any time. If your supplier has been estimating " +
                "your usage, you can request a corrected bill based on an actual reading. Smart meter customers " +
                "should have readings sent automatically. If estimates appear on your bill, contact your supplier " +
                "to check your smart meter connection.");

            AddRight(doc, bold, regular, accent, grey, "VAT on Domestic Energy",
                "The VAT rate on domestic energy supplies is 5%, not the standard 20%. This applies to gas and " +
                "electricity for household use. If you spot the wrong VAT rate on your bill, request an immediate " +
                "correction and refund.");

            AddRight(doc, bold, regular, accent, grey, "Solar Export (SEG)",
                "Under the Smart Export Guarantee, licensed suppliers with 150,000+ customers must offer you a " +
                "tariff for electricity you export to the grid. You can switch your export supplier independently " +
                "of your import supplier — shop around, as rates range from under 2p to over 15p/kWh. If your " +
                "export is 'deemed' (estimated at 50% of generation), ask your supplier to use actual smart meter " +
                "export readings instead. A battery can increase self-consumption from ~30% to 60-80%, shifting " +
                "units from low-value export to high-value avoided import.");

            AddRight(doc, bold, regular, accent, grey, "How to Complain",
                "1. Contact your supplier directly (phone, email, or online). " +
                "2. If unresolved after 8 weeks, or if you receive a deadlock letter, escalate to the Energy " +
                "Ombudsman at energyombudsman.org or 0330 440 1624. " +
                "3. For serious licence breaches, report to Ofgem at ofgem.gov.uk. " +
                "4. Citizens Advice provides free energy advice at 0808 223 1133.");

            doc.Add(new Paragraph("").SetBorderBottom(new SolidBorder(dividerColor, 0.5f)).SetMarginBottom(8));

            // ---- FOOTER ----
            doc.Add(new Paragraph("Disclaimer")
                .SetFont(regular).SetFontSize(8).SetFontColor(darkGrey).SetMarginTop(12).SetMarginBottom(4));
            doc.Add(new Paragraph(
                "This report is generated by SmartCube for informational purposes. While we check your bills " +
                "against published Ofgem price cap data and regulatory rules, this does not constitute legal or " +
                "financial advice. Ofgem cap rates are updated quarterly and regional variations apply. Always " +
                "verify current rates at ofgem.gov.uk. If you believe you have been overcharged, contact your " +
                "supplier directly or seek advice from Citizens Advice.")
                .SetFont(regular).SetFontSize(7).SetFontColor(darkGrey).SetMarginBottom(8));
            doc.Add(new Paragraph("SmartCube Bill Audit Report  |  " + DateTime.Now.Year)
                .SetFont(regular).SetFontSize(7).SetFontColor(darkGrey)
                .SetTextAlignment(PdfTextAlign.CENTER));

            doc.Close();
            pdf.Close();
            writer.Close();

            return filePath;
        }

        private static void AddStatCell(Table table, string value, string label,
            DeviceRgb color, PdfFont bold, PdfFont regular, DeviceRgb labelColor)
        {
            var cell = new PdfCell().SetBorder(PdfBorder.NO_BORDER).SetPadding(8)
                .SetTextAlignment(PdfTextAlign.CENTER);
            cell.Add(new Paragraph(value).SetFont(bold).SetFontSize(20).SetFontColor(color).SetMarginBottom(2));
            cell.Add(new Paragraph(label).SetFont(regular).SetFontSize(9).SetFontColor(labelColor));
            table.AddCell(cell);
        }

        private static void AddCell(Table table, string text, PdfFont font, DeviceRgb bg, DeviceRgb textColor)
        {
            table.AddCell(new PdfCell().SetBackgroundColor(bg).SetBorder(PdfBorder.NO_BORDER).SetPadding(5)
                .Add(new Paragraph(text).SetFont(font).SetFontSize(8).SetFontColor(textColor)));
        }

        private static void AddRight(Document doc, PdfFont bold, PdfFont regular,
            DeviceRgb titleColor, DeviceRgb bodyColor, string title, string body)
        {
            doc.Add(new Paragraph(title).SetFont(bold).SetFontSize(11).SetFontColor(titleColor)
                .SetMarginTop(8).SetMarginBottom(2));
            doc.Add(new Paragraph(body).SetFont(regular).SetFontSize(9).SetFontColor(bodyColor)
                .SetMarginBottom(6));
        }

        private static string GetExplanation(string checkType)
        {
            return checkType switch
            {
                "Price Cap" =>
                    "The Ofgem energy price cap sets the maximum amount suppliers can charge per unit of energy and " +
                    "for daily standing charges. It is reviewed quarterly and varies by region, fuel type, and payment " +
                    "method. If your supplier charges above the cap, they are in breach of their licence conditions. " +
                    "You are entitled to a refund of the overcharged amount. Contact your supplier citing the Ofgem " +
                    "price cap and request a recalculation. If they refuse, escalate to the Energy Ombudsman.",

                "Back-billing" =>
                    "Under Ofgem's back-billing rules (introduced January 2018), energy suppliers cannot send you a " +
                    "bill for energy used more than 12 months ago, provided you have not been billed for that energy " +
                    "before. This applies even if the supplier made an error. If a bill covers a period longer than " +
                    "12 months, the portion relating to energy used more than 12 months before the bill date is " +
                    "unenforceable. Write to your supplier stating that under Ofgem back-billing protections, you " +
                    "are only liable for the most recent 12 months of charges.",

                "Estimated Readings" =>
                    "When your supplier estimates your meter reading, they use industry averages and your past " +
                    "consumption. Over time, estimates can drift significantly from actual usage, either building " +
                    "up credit or accumulating a large debt that arrives as a shock catch-up bill. " +
                    "Submit a meter reading to your supplier to correct estimated billing.",

                "VAT" =>
                    "Domestic energy supplies in the UK are subject to VAT at the reduced rate of 5%, not the " +
                    "standard 20% rate. This has been the case since 1997 under the Value Added Tax Act 1994, " +
                    "Schedule 7A, Group 1. If your bill shows VAT at 20%, your supplier has applied the wrong " +
                    "rate. Contact them to request a corrected invoice and refund of the overcharged VAT.",

                "Duplicate Billing" =>
                    "Overlapping billing periods mean you may have been charged twice for the same energy usage. " +
                    "This can happen during supplier switches, system migrations, or billing errors. Under the " +
                    "Consumer Rights Act 2015, you are entitled to a refund for any duplicate charges.",

                "Payment Method" =>
                    "Energy suppliers typically offer a discount of 5-7% for customers who pay by Direct Debit " +
                    "compared to standard credit (quarterly billing). Switching to Direct Debit is free and can " +
                    "usually be set up online or by calling your supplier.",

                "Solar Export" =>
                    "Under the Smart Export Guarantee (SEG), energy suppliers with 150,000+ customers must offer " +
                    "a tariff for electricity you export to the grid from solar panels. However, rates vary enormously " +
                    "— from under 2p/kWh to over 15p/kWh. You can switch your export supplier independently of your " +
                    "import supplier. If you're on a deemed (estimated) export, request your supplier uses metered " +
                    "export data from your smart meter for more accurate payments. Note: solar panels reduce your " +
                    "import costs but standing charges still apply in full.",

                _ => null,
            };
        }
    }
}
