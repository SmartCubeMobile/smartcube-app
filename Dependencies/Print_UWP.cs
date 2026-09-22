// NATIVE UWP || WINUI IMPLEMENTATION
#if UWP
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Pdf;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
#endif

#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Printing;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Pdf;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
#endif

namespace SmartCubeMobile
{
    // Pinched from Telerik's example!!
    public class PrintService : IPrint
    {
        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;
        //private FinanceViewModel financeviewmodel;
        async void IPrint.Print(string filePath)
        {
            //financeviewmodel = financevm;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(filePath));

            PdfDocument doc = await PdfDocument.LoadFromFileAsync(file);
            await Load(doc);

            // Wouldn't have got it working without this
            // https://stackoverflow.com/questions/39074684/how-to-print-in-UWP || WINUI-app  Jay Zuo
            //string printerName = "Canon TS5100 series";


            if (!PrintManager.IsSupported())
            {
                // Tell the user how to print
                Console.WriteLine("Printing not supported");
                //MainPage.Current.NotifyUser("Print contract registered with customization, use the Print button to print.", NotifyType.StatusMessage);
            }
            else
            {
                // Remove the print button
                //InvokePrintingButton.Visibility = Visibility.Collapsed;

                // Inform user that Printing is not supported
                Console.WriteLine("Not Supported");

                //MainPage.Current.NotifyUser("Printing is not supported.", NotifyType.ErrorMessage);

                // Printing-related event handlers will never be called if printing
                // is not supported, but it's okay to register for them anyway.

                // Register for PrintTaskRequested event
                printMan = PrintManager.GetForCurrentView();
                printMan.PrintTaskRequested += PrintTaskRequested;

                // Build a PrintDocument and register for callbacks
                printDoc = new PrintDocument();
                printDocSource = printDoc.DocumentSource;
                printDoc.Paginate += Paginate;
                printDoc.GetPreviewPage += GetPreviewPage;
                printDoc.AddPages += AddPages;
            }
            return;
        }

        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Create the PrintTask.
            // Defines the title and delegate for PrintTaskSourceRequested
            PrintTask printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            // Handle PrintTask.Completed to catch failed print jobs
            printTask.Completed += PrintTaskCompleted;
            return;
        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            // Set the document source.
            args.SetSource(printDocSource);
            return;
        }

        private void Paginate(object sender, PaginateEventArgs e)
        {
            // As I only want to print one Rectangle, so I set the count to 1
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
            return;
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // Provide a UIElement as the print preview.
            printDoc.SetPreviewPage(e.PageNumber, PdfPages[0]);
            return;
        }

        private void AddPages(object sender, AddPagesEventArgs e)
        {
            foreach (UIElement element in PdfPages)
            {
                printDoc.AddPage(element);
            }
            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
            return;
        }

        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, failed to print.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });
            }
            MainMeter.financeviewmodel.PrintButtonEnabled = true;
        }

        public async Task Load(PdfDocument pdfDoc)
        {
            PdfPages.Clear();

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                BitmapImage image = new BitmapImage();
                PdfPage page = pdfDoc.GetPage(i);
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream);
                    await image.SetSourceAsync(stream);
                }

                //Canvas canvass = new Canvas();
                Image ib = new Image()
                {
                    Source = image
                    //    //ImageSource = image
                };
                //canvass.Background = ib;
                PdfPages.Add(ib);
            }
            return;
        }

        public ObservableCollection<Image> PdfPages
        {
            get;
            set;
        } = new ObservableCollection<Image>();
    }
}