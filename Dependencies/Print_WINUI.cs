
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Printing;
using Windows.Data.Pdf;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace SmartCubeMobile
{
    public class PrintService : IPrint
    {
        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;

        private FinanceViewModel financeviewmodel;
        private UtilityViewModel utilityviewmodel;
        private Window _window;

        public List<Image> PdfPages { get; private set; } = new();

        public async Task Print(string filePath, object viewmodel)
        {
            switch (viewmodel)
            {
                case FinanceViewModel:
                    financeviewmodel = viewmodel as FinanceViewModel;
                    break;
                case UtilityViewModel:
                    utilityviewmodel = viewmodel as UtilityViewModel;
                    break;
                default:
                    break;
            }
            //_window = mainWindow;
            _window = ((App)Application.Current).MainWindow;

            // ✅ Load file correctly (supports real file paths)
            StorageFile file;
            if (filePath.StartsWith("ms-appx") || filePath.StartsWith("ms-appdata"))
            {
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(filePath));
            }
            else
            {
                file = await StorageFile.GetFileFromPathAsync(filePath);
            }

            PdfDocument doc = await PdfDocument.LoadFromFileAsync(file);
            await Load(doc);

            if (!PrintManager.IsSupported())
                return;

            var hwnd = WindowNative.GetWindowHandle(_window);
            printMan = PrintManagerInterop.GetForWindow(hwnd);

            // ✅ Create PrintDocument BEFORE showing UI
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;

            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;

            // ✅ Register event BEFORE showing UI
            printMan.PrintTaskRequested += PrintTaskRequested;

            try
            {
                await PrintManagerInterop.ShowPrintUIForWindowAsync(hwnd);
            }
            catch (Exception)
            {
                // Optional: handle dialog failure
            }
        }

        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var printTask = args.Request.CreatePrintTask("Print", request =>
            {
                request.SetSource(printDocSource);
            });

            printTask.Completed += PrintTaskCompleted;
        }

        private void Paginate(object sender, PaginateEventArgs e)
        {
            printDoc.SetPreviewPageCount(PdfPages.Count, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDoc.SetPreviewPage(e.PageNumber, PdfPages[e.PageNumber - 1]);
        }

        private void AddPages(object sender, AddPagesEventArgs e)
        {
            foreach (var page in PdfPages)
            {
                printDoc.AddPage(page);
            }

            printDoc.AddPagesComplete();
        }

        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            // ✅ Cleanup event to prevent leaks
            if (printMan != null)
            {
                printMan.PrintTaskRequested -= PrintTaskRequested;
            }

            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await _window.DispatcherQueue.EnqueueAsync(async () =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Printing error",
                        Content = "Sorry, failed to print.",
                        CloseButtonText = "OK",
                        XamlRoot = _window.Content.XamlRoot
                    };

                    await dialog.ShowAsync();
                });
            }

            // ✅ Ensure UI thread update
            await _window.DispatcherQueue.EnqueueAsync(() =>
            {
                financeviewmodel.PrintButtonEnabled = true;
            });
        }

        public async Task Load(PdfDocument pdfDoc)
        {
            PdfPages.Clear();

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                var page = pdfDoc.GetPage(i);
                var image = new BitmapImage();

                using (var stream = new InMemoryRandomAccessStream())
                {
                    // Optional: improve performance with render options
                    await page.RenderToStreamAsync(stream);
                    stream.Seek(0);
                    await image.SetSourceAsync(stream);
                }

                PdfPages.Add(new Image
                {
                    Source = image
                });
            }
        }
    }
}