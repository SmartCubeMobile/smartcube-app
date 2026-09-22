#if ANDROIDX
using Android.Content;
using Android.OS;
using Android.Print;
using Java.IO;
#endif
#if WINFORMS || WPF || UWP
using Microsoft.DotNet.PlatformAbstractions;
#endif

//#if POSSIBLYNEEDED
//[assembly: Xamarin.Forms.Dependency(typeof(SmartCubeMobile.Droid.Print))]
//#endif
namespace SmartCubeMobile
{
//#if POSSIBLYNEEDED
//    public class Print : IPrintService
//   {
//        void IPrintService.Print(Stream inputStream, string fileName)
//        {
//#endif
#if ANDROIDX
    public static class PrintService
    {
        public static void Print(Stream inputStream, string fileName)
        {
#endif
            if (inputStream.CanSeek)
            {
                //Reset the position of PDF document stream to be printed
                inputStream.Position = 0;
            }
            string createdFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            using (FileStream dest = System.IO.File.OpenWrite(createdFilePath))
                inputStream.CopyTo(dest);
            string filePath = createdFilePath;
//#if POSSIBLYNEEDED
//            Android.App.Activity activity = Xamarin.Essentials.Platform.CurrentActivity;
//#endif
#if ANDROIDX
            // This (probably) won't work ....see above? Can try that??
            Activity activity = new Activity();
#endif
            PrintManager printManager = (PrintManager)activity.GetSystemService(Context.PrintService);
            PrintDocumentAdapter pda = new CustomPrintDocumentAdapter(filePath);
            //Print with null PrintAttributes
            printManager.Print(fileName, pda, null);
        }

        internal class CustomPrintDocumentAdapter : PrintDocumentAdapter
        {
            internal string FileToPrint { get; set; }

            internal CustomPrintDocumentAdapter(string fileDesc)
            {
                FileToPrint = fileDesc;
            }
            public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
            {
                if (cancellationSignal.IsCanceled)
                {
                    callback.OnLayoutCancelled();
                    return;
                }
                PrintDocumentInfo pdi = new PrintDocumentInfo.Builder(FileToPrint).SetContentType(Android.Print.PrintContentType.Document).Build();
                callback.OnLayoutFinished(pdi, true);
            }

            public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
            {
                FileInputStream input = null;
                FileOutputStream output = null;

                try
                {
                    //Create FileInputStream object from the given file
                    input = new FileInputStream(FileToPrint);
                    //Create FileOutputStream object from the destination FileDescriptor instance
                    output = new FileOutputStream(destination.FileDescriptor);

                    byte[] buf = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = input.Read(buf)) > 0)
                    {
                        //Write the contents of the given file to the print destination
                        output.Write(buf, 0, bytesRead);
                    }

                    callback.OnWriteFinished(new PageRange[] { PageRange.AllPages });

                }
                catch (Java.IO.FileNotFoundException e)
                {
                    //Catch exception
                    e.PrintStackTrace();
                }
                catch (Java.IO.IOException e)
                {
                    e.PrintStackTrace();
                }
                catch (Exception e)
                {
                    //Catch exception
                    string error = e.Message;
                }
                //finally
                //{
                //    try
                //    {
                //        if (input != null)
                //        {
                //            input.Close();
                //        }
                //        if (output != null)
                //        {
                //            output.Close();
                //        }
                //    }
                //    catch (IOException e)
                //    {
                //        e.PrintStackTrace();
                //    }
                //}
            }
        }
    }
}