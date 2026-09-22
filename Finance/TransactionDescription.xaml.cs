#if WINFORMS
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Windows.Forms;
#endif

#if WPF
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
#endif

#if WINUI
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
#endif

#if ANDROIDX
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
#endif

namespace SmartCubeMobile // Leave it here!  It matches the XAML
{
#if WINFORMS
    public partial class TransactionDescriptionx : System.Windows.Forms.Form
#endif
#if WPF
    public partial class TransactionDescription : Popup
#endif
#if WINUI
    public partial class TransactionDescription : UserControl
#endif
#if ANDROIDX
    public partial class TransactionDescription : PopupWindow
#endif
#if SMARTMAUI
    public partial class TransactionDescription : Popup
#endif
    {
#if WINFORMS
        public void TransactionDescription(FinanceViewModel financeviewmodel)
#else
        public TransactionDescription(FinanceViewModel financeviewmodel)
#endif
        {
#if WPF  || WINUI || SMARTMAUI
            InitializeComponent();
#endif
#if WINFORMS
            //this.PrintButton.Click += new RoutedEventHandler((s, e) => OnPrintButtonClick(s, e, financeviewmodel));
#endif
#if WPF
            FrontEndGUI.SetFinanceDataContext(this, financeviewmodel);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler((s, e) => TransactionDescription_PopupMouseLeave(s, e));
            this.PrintButton.Click += new RoutedEventHandler((s, e) => OnPrintButtonClick(s, e, financeviewmodel));
#endif
#if WINUI
            FrontEndGUI.SetFinanceDataContext(this, financeviewmodel); 
            this.PointerExited += new PointerEventHandler((s, e) => TransactionDescription_PopupMouseLeave(s, e));
            this.PrintButton.Click += new RoutedEventHandler((s, e) => OnPrintButtonClick(s, e, financeviewmodel));
#endif
#if ANDROIDX
            //this.SetBindingContext(utilityviewmodel);
#endif
#if SMARTMAUI
            this.PrintButton.Clicked += new EventHandler((s, e) => OnPrintButtonClick(s, e, financeviewmodel));
#endif
        }

#if WINFORMS
        internal void PopupClosed(object sender, FormClosedEventArgs e)
#endif
#if WPF
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if WINUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if ANDROIDX
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if SMARTMAUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
        {
            return;
        }


#if WINFORMS
        internal void OnPrintButtonClick(object sender, EventArgs e,
                                            FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal void OnPrintButtonClick(object sender, RoutedEventArgs e,
                                            FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal void OnPrintButtonClick(object sender, RoutedEventArgs e,
                                            FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        internal void OnPrintButtonClick(object sender, EventArgs e,
                                            FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal void OnPrintButtonClick(object sender, EventArgs e,
                                            FinanceViewModel financeviewmodel)
#endif
        {
            // In memory of Joy - 8th July 1926 - 25th June 2021 - kind, warm-hearted, generous to a fault                
            // You bring me luck, Joy ...
            if (sender != null)
            {
                string title = financeviewmodel.TransactionDescriptionTitle + " - " +
                            SmartParametersV2016.KEASDONENERGYLTD;
                financeviewmodel.TransactionsListTitle = title;
                string filename = "TransactionDescription.pdf";
#if WINFORMS || WPF
                // https://www.c-sharpcorner.com/uploadfile/f2e803/basic-pdf-creation-using-itextsharp-part-ii/
                // With eternal thanks to Micke Blomquist

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                filePath = System.IO.Path.Combine(filePath, "SmartCube");
                filePath = System.IO.Path.Combine(filePath, filename);
#endif
#if WINUI
                // https://www.c-sharpcorner.com/uploadfile/f2e803/basic-pdf-creation-using-itextsharp-part-ii/
                // With eternal thanks to Micke Blomquist

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                filePath = System.IO.Path.Combine(filePath, "SmartCube");
                filePath = System.IO.Path.Combine(filePath, filename);
#endif
#if ANDROIDX
                string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), filename);
#endif
#if SMARTMAUI
                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                filePath = System.IO.Path.Combine(filePath, "SmartCube");
                filePath = System.IO.Path.Combine(filePath, filename);
#endif
                CreateTransactionDescriptionList(financeviewmodel, filePath, title);
                // Cannot use DependencyService in WPF ... or at least I
                // can't figure out how the fucking thing works
                // Declare an interface instance.
#if WPF
                IPrint print_object = new PrintService();
                // Call the member.                
                print_object.Print(filePath, financeviewmodel);
#endif
#if WINUI
                IPrint print_object = new PrintService();
                // Call the member.
                print_object.Print(filePath, financeviewmodel);
#endif
#if ANDROIDX
                // This is where the shit USED to happen ... !!!
                //PdfSharpCore.Pdf.PdfDocument printDocument = PDFManager.GeneratePDFFromView(mainGrid);                
                try
                {
                    Stream printStream = new MemoryStream(File.ReadAllBytes(filePath));
                    // Miss out all the interface shit ...
                    PrintService.Print(printStream, filePath);
                }
                catch (Exception exc)
                {
                    financeviewmodel.errorMessage = exc.Message;
                }
#endif
#if SMARTMAUI
                IPrint print_object = new PrintService();
                // Call the member.
                print_object.Print(filePath, financeviewmodel);
#endif
            }
            return;
        }

        internal void CreateTransactionDescriptionList(FinanceViewModel financeviewmodel, string filePath, string title)
        {
            PdfWriter pdfwriter = new PdfWriter(filePath);
            PdfDocument pdfdocument = new PdfDocument(pdfwriter);

            // Create an instance of the document class which represents the PDF document itself.              
            Document document = new Document(pdfdocument, pageSize: PageSize.A4, false); // <= no immediate flush so we can write page numbers

            document.SetBottomMargin(50);

            // Add meta information to the document  
            //document.AddAuthor("Micke Blomquist");
            //document.AddCreator("Sample application using iText7");
            //document.AddKeywords("PDF tutorial education");
            //document.AddSubject("Document subject - Describing the steps creating a PDF document");
            //document.AddTitle("The document title - PDF creation using iText7");

            //Paragraph header = new Paragraph("HEADER")
            //.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
            //.SetFontSize(20);            
            //document.Add(header);

            iText.Layout.Element.Paragraph subheader = new iText.Layout.Element.Paragraph(title)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(15);
            document.Add(subheader);

            iText.Layout.Element.Table table = new Table(2, false);
            iText.Layout.Element.Cell cell11 = new iText.Layout.Element.Cell(1, 1)
               .SetBackgroundColor(ColorConstants.GRAY)
               .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
               .SetFontSize(10)
               .Add(new iText.Layout.Element.Paragraph("Field"));
            iText.Layout.Element.Cell cell12 = new iText.Layout.Element.Cell(1, 1)
               .SetBackgroundColor(ColorConstants.GRAY)
               .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
               .SetFontSize(10)
               .Add(new Paragraph("Value"));

            table.AddCell(cell11);
            table.AddCell(cell12);

            foreach (FinanceViewModel.TransactionDescription view in financeviewmodel.Transaction)
            {
                iText.Layout.Element.Cell cell21 = new iText.Layout.Element.Cell()
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                                .SetFontSize(8)
                                .Add(new Paragraph(view.Field));
                iText.Layout.Element.Cell cell22 = new iText.Layout.Element.Cell()
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                                .SetFontSize(8)
                                .Add(new Paragraph(view.Value));
                table.AddCell(cell21);
                table.AddCell(cell22);
            }
            document.Add(table);

            // Line separator
            //LineSeparator lsend = new LineSeparator(new SolidLine());
            //document.Add(lsend);
            //document.Add(lsend);

            // Page numbers
            int n = pdfdocument.GetNumberOfPages();
            for (int i = 1; i <= n; i++)
            {
                document.ShowTextAligned(new Paragraph(String.Format("Page" + i + " of " + n)),
                    300, 30, i, iText.Layout.Properties.TextAlignment.CENTER,
                    iText.Layout.Properties.VerticalAlignment.MIDDLE, 0).SetFontSize(10);
            }

            // Close everything down
            document.Close();
            pdfdocument.Close();
            pdfwriter.Close();
            return;
        }

#if WINFORMS
        internal void TransactionDescription_PopupMouseLeftButtonDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
#endif
#if WPF
        internal void TransactionDescription_PopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#endif
#if WINUI
        internal void TransactionDescription_PopupMouseLeftButtonDown(object sender, EventArgs e)
        {
#endif
#if ANDROIDX
        internal void TransactionDescription_PopupMouseLeftButtonDown(object sender, EventArgs e)
        {
#endif
#if SMARTMAUI
        internal void TransactionDescription_PopupMouseLeftButtonDown(object sender, EventArgs e)
        {
#endif
            if (sender != null && e != null)
            {
                // This absolute fucking useless bollocks #1
                // THANK YOU Doguhan Uluca - you are a star ...
#if WINFORMS
                //int ClickCount = 1;
                //switch (ClickCount)
#endif
#if WPF
                switch (e.ClickCount)
                {
                    case 0:
                    case 1:
                        // Cancel this handler because this appears to get called as well
                        // when the window is removed ... and we only want to close it ONCE, DON'T WE???
                        this.MouseLeave -= new MouseEventHandler(TransactionDescription_PopupMouseLeave);
                        // Remove the popup grid from the screen
                        this.IsOpen = false;
                        break;
                    default:
                        break;
                }
#endif
            }
            return;
        }

#if WINFORMS
        internal void TransactionDescription_PopupMouseLeave(object sender, MouseEventArgs e)
        {
#endif
#if WPF
        internal void TransactionDescription_PopupMouseLeave(object sender, MouseEventArgs e)
        {
#endif
#if WINUI
        internal void TransactionDescription_PopupMouseLeave(object sender, PointerRoutedEventArgs e)
        {
#endif
#if ANDROIDX
        internal void TransactionDescription_PopupMouseLeave(object sender, EventArgs e)
        {
#endif
#if SMARTMAUI
        internal void TransactionDescription_PopupMouseLeave(object sender, EventArgs e)
        {
#endif
            if (sender != null)
            {
                if (this != null)
                {
#if WPF
            
                    if (this.IsOpen)
                    {
                        this.IsOpen = false;
                    }
#endif
#if WINUI
                    this.Visibility = Visibility.Collapsed;
#endif
                }
            }
            return;
        }
    }
}