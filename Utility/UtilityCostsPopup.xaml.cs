#if WPF
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
#endif

#if UWP
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace SmartCubeMobile
{
#if WINFORMS
    public partial class UtilityCostsPopup : Form
#endif
#if WPF
    public partial class UtilityCostsPopup : Popup
#endif
#if UWP
    public partial class UtilityCostsPopup : ContentDialog
#endif
#if ANDROIDX
    public partial class UtilityCostsPopup : PopupWindow
#endif
#if SMARTMAUI
    public partial class UtilityCostsPopup : ContentPage
#endif
    {
        private MainViewModel ourviewmodel;
        private UtilityViewModel utilityviewmodel;
        public UtilityCostsPopup(MainViewModel mainvm, UtilityViewModel utilityvm)
        {
            this.ourviewmodel = mainvm;
            this.utilityviewmodel = utilityvm;
            return;
        }
        public object DependencyService { get; private set; }

        public UtilityCostsPopup()
        {
#if WINFORMS || WPF || UWP || SMARTMAUI
            InitializeComponent();
#endif
#if WPF || UWP
            DataContext = utilityviewmodel;
#endif
#if ANDROIDX
            //this.SetBindingContext(utilityviewmodel);
#endif
        }

#if WINFORMS
        internal void PopupClosed(object sender, FormClosedEventArgs e)
#endif
#if WPF || UWP || SMARTMAUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if ANDROIDX 
        internal void PopupClosed(object sender, EventArgs e)
#endif
        {
            return;
        }


#if WINFORMS
        internal void OnPrintButtonClick(object sender, EventArgs e)
#endif
#if WPF || UWP
        internal void OnPrintButtonClick(object sender, RoutedEventArgs e)
#endif
#if ANDROIDX || SMARTMAUI
        internal void OnPrintButtonClick(object sender, EventArgs e)
#endif
        {
            // In memory of Joy - 8th July 1926 - 25th June 2021 - kind, warm-hearted, generous to a fault                
            // You bring me luck, Joy ...
            if (sender != null)
            {
                string title = utilityviewmodel.UtilityCostsTitle + " - " +
                            SmartParametersV2016.KEASDONENERGYLTD;
                            utilityviewmodel.UtilityCostsTitle = title;
#if WPF || UWP
                // https://www.c-sharpcorner.com/uploadfile/f2e803/basic-pdf-creation-using-itextsharp-part-ii/
                // With eternal thanks to Micke Blomquist

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                filePath = System.IO.Path.Combine(filePath, "SmartCube");
                filePath = System.IO.Path.Combine(filePath, "Transactions.pdf");
#endif
#if ANDROIDX
                string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "UtilityCosts.pdf");
#endif
#if SMARTMAUI
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UtilityCosts.pdf");
#endif
                SmartRoutinesV2018.CreateUtilityList(ourviewmodel,
                                        utilityviewmodel, 
                                        "Costs",
                                        ourviewmodel.subscriber,
                                        filePath,
                                        title); // Don't need totals

#if WPF || UWP
                // Cannot use DependencyService in WPF ... or at least I
                // can't figure out how the fucking thing works
                // Declare an interface instance.
#if WPF
                IPrint print_object = new PrintService();
                // Call the member.
                print_object.Print(filePath);
#endif
#if UWP
                //IPrint print_object = new PrintService();
                // Call the member.
                //print_object.Print(filePath);
#endif


#endif
#if ANDROIDX
                // This is where the shit USED to happen ... !!!
                //PdfSharpCore.Pdf.PdfDocument printDocument = PDFManager.GeneratePDFFromView(mainGrid);                
                try
                {
                    Stream printStream = new MemoryStream(File.ReadAllBytes(filePath));
//#if POSSIBLYNEEDED
//                    DependencyService.Get<IPrintService>().Print(printStream, filePath);
//#endif
                    // Miss out all the interface shit ...
                    PrintService.Print(printStream, filePath);
                }
                catch (Exception exc)
                {
                    Console.Write(exc.Message);
                }
#endif
            }
            return;
        }

#if WINFORMS
        internal void UtilityCosts_PopupMouseLeftButtonDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
#endif
#if WPF
        internal void UtilityCosts_PopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#endif
#if ANDROIDX || SMARTMAUI
        internal void UtilityCosts_PopupMouseLeftButtonDown(object sender, EventArgs e)
        {
#endif
#if WINFORMS || WPF 
            if (sender != null && e != null)
            {
                // This absolute fucking useless bollocks #1
                // THANK YOU Doguhan Uluca - you are a star ...
#endif
#if ANDROIDX || SMARTMAUI
                if (sender != null && e != null)
                {
                    // This absolute fucking useless bollocks #1
                    // THANK YOU Doguhan Uluca - you are a star ...
#endif
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
                        this.MouseLeave -= new MouseEventHandler(Transactions_PopupMouseLeave);
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
        internal void UtilityCosts_PopupMouseLeave(object sender, System.Windows.Forms.MouseEventArgs e)
        {
#endif
#if WPF || UWP
        internal void UtilityCosts_PopupMouseLeave(object sender, MouseEventArgs e)
        {
#endif
#if WPF
            if (sender != null && 
                e != null)
            {
                if (this != null)
                {
                    if (this.IsOpen)
                    {
                        this.IsOpen = false;
                    }
                }
            }
#endif
#if WINFORMS || WPF || UWP
            return;
        }
#endif
    }
}