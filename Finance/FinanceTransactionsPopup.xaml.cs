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
using System;
#endif

#if ANDROIDX
#endif

namespace SmartCubeMobile
{
#if WINFORMS
    public partial class FinanceTransactionsPopup : Form
#endif
#if WPF
    public partial class FinanceTransactionsPopup : Popup
#endif
#if UWP || WINUI
    public partial class FinanceTransactionsPopup : UserControl
#endif
#if ANDROIDX
    public partial class FinanceTransactionsPopup : PopupWindow
#endif
#if SMARTMAUI
    public partial class FinanceTransactionsPopup : ContentPage
#endif
    {
        private MainViewModel ourviewmodel;
        private FinanceViewModel financeviewmodel;

        public FinanceTransactionsPopup(MainViewModel mainvm, FinanceViewModel fvm)
        {
            this.ourviewmodel = mainvm;
            this.financeviewmodel = fvm;
            return;
        }
        internal FinanceTransactionsPopup()
        {
#if WPF || UWP || WINUI || SMARTMAUI
            InitializeComponent();
            FrontEndGUI.SetFinanceDataContext(this, financeviewmodel);
#endif
#if WPF || UWP || WINUI
            this.PrintButton.Click += OnPrintButtonClick;
#endif
#if SMARTMAUI
            this.PrintButton.Clicked += OnPrintButtonClick;
#endif
            return;
        }

#if WINFORMS
        internal void PopupClosed(object sender, FormClosedEventArgs e)
#endif
#if WPF
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if UWP || WINUI
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
                FocusState fs = new FocusState();
                this.Focus(FocusState.Pointer);
                var abc = this.Focus(fs);
                //Console.Write(fs.ToString());
                this.LostFocus += Transactions_PopupMouseLeave;
            }
            return;
        }
#endif
#if WINFORMS
        internal void OnPrintButtonClick(object sender, EventArgs e)
#endif
#if WPF || WINUI
        internal void OnPrintButtonClick(object sender, RoutedEventArgs e)
#endif
#if UWP
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
#if WPF
                // https://www.c-sharpcorner.com/uploadfile/f2e803/basic-pdf-creation-using-itextsharp-part-ii/
                // With eternal thanks to Micke Blomquist

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //filePath = System.IO.Path.Combine(filePath, "SmartCube");
                filePath = System.IO.Path.Combine(filePath, "Transactions.pdf");
#endif
#if UWP || WINUI
                // https://www.c-sharpcorner.com/uploadfile/f2e803/basic-pdf-creation-using-itextsharp-part-ii/
                // With eternal thanks to Micke Blomquist

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //filePath = System.IO.Path.Combine(filePath, "SmartCube");
                filePath = System.IO.Path.Combine(filePath, "Transactions.pdf");
#endif
#if ANDROIDX
                string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Transactions.pdf");
#endif
#if SMARTMAUI
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Transactions.pdf");
#endif
                SmartRoutinesV2018.CreateFinanceList(ourviewmodel,
                                                    financeviewmodel,
                                                    "Transactions",
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
        internal void Transactions_PopupMouseLeftButtonDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
#endif
#if WPF
        internal void Transactions_PopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#endif
#if UWP || WINUI
        internal void Transactions_PopupMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
#endif
#if ANDROIDX || SMARTMAUI
        internal void Transactions_PopupMouseLeftButtonDown(object sender, EventArgs e)
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
        internal void Transactions_PopupMouseLeave(object sender, System.Windows.Forms.MouseEventArgs e)
        {
#endif
#if WPF
        internal void Transactions_PopupMouseLeave(object sender, MouseEventArgs e)
        {
#endif
#if UWP || WINUI
        internal void Transactions_PopupMouseLeave(object sender, RoutedEventArgs e)
        {
#endif
#if ANDROIDX || SMARTMAUI
        internal void Transactions_PopupMouseLeave(object sender, EventArgs e)
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
#if UWP || WINUI
                    this.Visibility = Visibility.Collapsed;
#endif
                }
            }
            return;
        }

#if SMARTMAUI
        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
#endif

#if SMARTMAUI
        private void OnPrintClicked(object sender, EventArgs e)
        {
            OnPrintButtonClick(sender, e);
        }
#endif
    }
}