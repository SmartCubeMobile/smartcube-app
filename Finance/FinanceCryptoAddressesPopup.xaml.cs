#if WPF
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#endif

#if UWP
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
#endif

#if ANDROIDX
#endif

namespace SmartCubeMobile
{
#if WINFORMS
    public partial class FinanceCryptoAddressesPopup : Form
#endif
#if WPF
    public partial class FinanceCryptoAddressesPopup : Popup
#endif
#if UWP || WINUI
    public partial class FinanceCryptoAddressesPopup : UserControl
#endif
#if ANDROIDX
    public partial class FinanceCryptoAddressesPopup : PopupWindow
#endif
#if SMARTMAUI
    public partial class FinanceCryptoAddressesPopup : ContentPage
#endif
    {
        private MainViewModel ourviewmodel;
        private FinanceViewModel financeviewmodel;

        public FinanceCryptoAddressesPopup(MainViewModel mainvm, FinanceViewModel fvm)
        {
            this.ourviewmodel = mainvm;
            this.financeviewmodel = fvm;
            return;
        }
        internal FinanceCryptoAddressesPopup()
        {
#if WPF || UWP || WINUI || SMARTMAUI
            InitializeComponent();
#endif
#if WPF || UWP || WINUI || SMARTMAUI
            FrontEndGUI.SetFinanceDataContext(this, financeviewmodel);
#endif
            return;
        }

#if WINFORMS
        internal void PopupClosed(object sender, FormClosedEventArgs e)
#endif
#if WPF || UWP || WINUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if ANDROIDX || SMARTMAUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
        {
            return;
        }

#if UWP || WINUI
        internal void PopupOpened(object sender, ContentDialogOpenedEventArgs e)
        {
            if (sender != null && e != null)
            {
                //FocusState fs = new FocusState();
                //this.Focus(FocusState.Pointer);
                //var abc = this.Focus(fs);
                //Console.Write(fs.ToString());
                //this.LostFocus += Transactions_PopupMouseLeave;
            }
            return;
        }
#endif
#if WINFORMS
        internal void OnPrintButtonClick(object sender, EventArgs e)
#endif
#if WPF || UWP || WINUI
        internal void OnPrintButtonClick(object sender, RoutedEventArgs e)
#endif
#if ANDROIDX || SMARTMAUI
        internal void OnPrintButtonClick(object sender, EventArgs e)
#endif
        {
            // In memory of Joy - 8th July 1926 - 25th June 2021 - kind, warm-hearted, generous to a fault                
            // You bring me luck, Joy ...  You certainly do, dear..
            if (sender != null)
            {
                financeviewmodel.PrintButtonEnabled = false;
                string title = financeviewmodel.TransactionsTitle;
                if (!ourviewmodel.subscriber)
                {
                    title += " - " + SmartParametersV2016.KEASDONENERGYLTD;
                }
                financeviewmodel.TransactionsListTitle = title;
#if WPF || UWP || WINUI
                // https://www.c-sharpcorner.com/uploadfile/f2e803/basic-pdf-creation-using-itextsharp-part-ii/
                // With eternal thanks to Micke Blomquist

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //filePath = System.IO.Path.Combine(filePath, "SmartCube");
                filePath = System.IO.Path.Combine(filePath, "Addresses.pdf");
#endif
#if ANDROIDX
                string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Transactions.pdf");
#endif
#if SMARTMAUI
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Addresses.pdf");
#endif
                SmartRoutinesV2018.CreateFinanceList(ourviewmodel,
                                                    financeviewmodel,
                                                    "Addresses",
                                                    ourviewmodel.subscriber,
                                                    filePath,
                                                    title);
#if WPF || UWP || WINUI
                // Cannot use DependencyService in WPF ... or at least I
                // can't figure out how the fucking thing works
                // Declare an interface instance.
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
            }
            return;
        }

#if WINFORMS
        internal void Addresses_PopupMouseLeftButtonDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
#endif
#if WPF
        internal void Addresses_PopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#endif
#if UWP || WINUI
        internal void Addresses_PopupMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
#endif
#if ANDROIDX || SMARTMAUI
        internal void Addresses_PopupMouseLeftButtonDown(object sender, EventArgs e)
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
                        this.MouseLeave -= new MouseEventHandler(Addresses_PopupMouseLeave);
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
        internal void Addresses_PopupMouseLeave(object sender, System.Windows.Forms.MouseEventArgs e)
        {
#endif
#if WPF
        internal void Addresses_PopupMouseLeave(object sender, MouseEventArgs e)
        {
#endif
#if UWP || WINUI
        internal void Addresses_PopupMouseLeave(object sender, RoutedEventArgs e)
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
#if UWP || WINUI
            this.Visibility = Visibility.Collapsed;
#endif
#if WINFORMS || WPF || UWP || WINUI
            return;
        }
#endif

#if SMARTMAUI
        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
#endif
    }
}