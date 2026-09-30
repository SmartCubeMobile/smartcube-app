using System;
using System.Threading.Tasks;
using iText.Kernel.Pdf;

#if ANDROIDX
using AndroidX.AppCompat.App;
#endif

//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is

namespace SmartCubeMobile
{
    public class SmartPDFV2019 // : IDisposable
    {

#if ANDROIDX 

        //internal static async Task<string> TurnPdfToText(MainViewModel ourviewmodel, 
        //                                    UtilityViewModel utilityviewmodel,
        //                                    PdfDocument pdfDoc)// pdfFile)
        //{
        //    string strText_simple = "";

        //PdfDocument pdfDoc = await PdfDocument.LoadFromFileAsync(pdfFile);
        //for (int page_no = 1; page_no <= pdfDoc.PageCount; page_no++)
        //{
        //    using (PdfPage page = pdfDoc.GetPage((uint)page_no))
        //    {
        //        try
        //        {
        //            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
        //            //Default is actual size. Render pdf page to stream 
        //            await page.RenderToStreamAsync(stream);


        //            // New OcrEngine with default language 
        //            OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
        //            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
        //            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        //            // Get recognition result 
        //            OcrResult result = await ocrEngine.RecognizeAsync(softwareBitmap);
        //            // Add to result list 
        //            if (!string.IsNullOrEmpty(strText_simple))
        //            {
        //                strText_simple = strText_simple + '\n';
        //            }
        //            strText_simple = strText_simple + result.Text;

        //        }
        //        catch (NullReferenceException exception)
        //        {
        //            ourviewmodel.errorMessage = "Parse" + SmartParametersV2016.bar + utilityviewmodel.statement_id + " Page " + page_no + SmartParametersV2016.space + exception.Message;
        //        }
        //        catch (Exception exception)
        //        {
        //            ourviewmodel.errorMessage = "Parse" + SmartParametersV2016.bar + utilityviewmodel.statement_id + " Page " + page_no + SmartParametersV2016.space + exception.Message;
        //        }
        //        //if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
        //        //{
        //        //    status = false;
        //        //    goto good_old_days;
        //        //}
        //    }
        //}
        //pdfDoc.Close();

        //  return strText_simple;
        //}
#endif
        internal static string TurnPdfToText(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            iText.Kernel.Pdf.PdfReader pdfreader)
        {
            string strText_simple = "";
            //#if WINFORMS || WPF
            //int page_count = Convert.ToInt32(reader.NumberOfPages);
            //for (int page_no = 1; page_no <= page_count; page_no++)
            //{
            //    // DO NOT CHANGE THIS FROM 'SIMPLE EXTRACTION STRATEGY'  IF YOU DO, THEN THE TEXT DOES
            //    // NOT GET EXTRACTED IN A WAY THAT THE LINES CAN BE PROCESSED BY THI PROGRAM.
            //    // ** DO NOT FUCK WITH THIS **  <= THIS MEANS YOU
            //    //                                 --------------                
            //    //
            //    // Reading PDF problems?  Don't forget to try fixing them by downloading
            //    // the latest version of itextsharp.dll from NuGet packages
            //    //
            //    // Encoding is losing me some of the trailing text!!!
            //    //

            //    try      // The try has been moved up here because sometimes the GetTextFromPage fails
            //    {
            //        ITextExtractionStrategy its_simple = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();

            //        if (!string.IsNullOrEmpty(strText_simple))
            //        {
            //            strText_simple = strText_simple + '\n';
            //        }
            //        strText_simple = strText_simple + PdfTextExtractor.GetTextFromPage(reader, page_no, its_simple);
            //    }
            //    catch (NullReferenceException exception)
            //    {
            //        ourviewmodel.errorMessage = "Parse" + SmartParametersV2016.bar + utilityviewmodel.statement_id + " Page " + page_no + SmartParametersV2016.space + exception.Message;
            //    }
            //    catch (Exception exception)
            //    {
            //        ourviewmodel.errorMessage = "Parse" + SmartParametersV2016.bar + utilityviewmodel.statement_id + " Page " + page_no + SmartParametersV2016.space + exception.Message;
            //    }
            //    //if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            //    //{
            //    //    status = false;
            //    //    goto good_old_days;
            //    //}

            //}
            //#endif

            PdfDocument pdfDoc = new PdfDocument(pdfreader);
            int page_count = pdfDoc.GetNumberOfPages();
            for (int page_no = 1; page_no <= page_count; page_no++)
            {
                try
                {
                    iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy();
                    if (!string.IsNullOrEmpty(strText_simple))
                    {
                        strText_simple += '\n';
                    }
                    strText_simple += iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page_no), strategy);
                }
                catch (NullReferenceException exception)
                {
                    ourviewmodel.errorMessage = "Parse" + SmartParametersV2016.bar + utilityviewmodel.statement_id + " Page " + page_no + SmartParametersV2016.space + exception.Message;
                }
                catch (Exception exception)
                {
                    ourviewmodel.errorMessage = "Parse" + SmartParametersV2016.bar + utilityviewmodel.statement_id + " Page " + page_no + SmartParametersV2016.space + exception.Message;
                }
                //if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                //{
                //    status = false;
                //    goto good_old_days;
                //}
            }
            pdfDoc.Close();
            return strText_simple;
        }

        internal static async Task<bool> Generic_Close_1(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                                                        iText.Kernel.Pdf.PdfReader pdfreader,
#endif
#if ANDROIDX 
                                                        AppCompatActivity meterActivity,
                                                        PdfReader pdfreader,
#endif
                                                        bool TextBox_Active)
        {
            if (pdfreader != null &&
                string.IsNullOrEmpty(utilityviewmodel.pdf_message))
            {
                return true;
            }
            ourviewmodel.errorMessage = ("Failed to load: " + utilityviewmodel.resource_code + SmartParametersV2016.space + utilityviewmodel.statement_id + SmartParametersV2016.space + utilityviewmodel.pdf_message).Trim();
            if (TextBox_Active)
            {
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Urgent message: " + ourviewmodel.errorMessage);
                return false;
            }
            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
            {
                return false;
            }
            return false;   // PDFs can take a long time, but the remote site could have shut up shop
            // Constant unwanted running commentary on her irrelevant life
        }


        internal static async Task<bool> Generic_Close_2(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                                                        iText.Kernel.Pdf.PdfReader pdfreader,
#endif
#if ANDROIDX 
                                                        PdfReader pdfreader,
                                                        AppCompatActivity meterActivity,
#endif
                                                        bool TextBox_Active)
        {
            // If either of this is bad ...
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                // One of the very few occasions when we need to tell HQ something
#if WINFORMS
                if (utilityviewmodel.console)
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                        "Urgent message: " + ourviewmodel.errorMessage + Environment.NewLine.ToString());
                }
                // No wonder I can't work with that fucking idiot yelling down the phone to her fucking stupid sister
#endif
#if WPF  || WINUI || SMARTMAUI
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
                {
                    return false;
                }
#endif
#if ANDROIDX
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage);
#endif
                return false;
            }
            else
            {
                // ... or this is bad ...
                if (!string.IsNullOrEmpty(utilityviewmodel.pdf_message))
                {
                    // One of the very few occasions when we need to tell HQ something
#if WINFORMS
                    if (utilityviewmodel.console)
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                            "PDF message: " + utilityviewmodel.pdf_message + Environment.NewLine.ToString());
                    }
                    // No wonder I can't work with that fucking idiot yelling down the phone to her fucking stupid sister
#endif
                    if (TextBox_Active)
                    {
                        ourviewmodel.errorMessage = "FailedToLoad" + " " + utilityviewmodel.resource_code + " " + utilityviewmodel.statement_id;
                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel, ourviewmodel.errorMessage);
                        return false;
                    }
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + utilityviewmodel.pdf_message))
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
#if WINFORMS
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                        (DateTime.Now + ourviewmodel.utcOffset).ToString() + // Local time
                        SmartParametersV2016.space + "Processed: " + utilityviewmodel.resource_code + SmartParametersV2016.space + utilityviewmodel.statement_id + Environment.NewLine.ToString());
#endif
                    if (TextBox_Active)
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif    
                                ourviewmodel,
                                "Processed" +
                                SmartParametersV2016.space +
                                utilityviewmodel.resource_code +
                                SmartParametersV2016.space +
                                utilityviewmodel.statement_id);
                    }
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + "Processed" +
                            SmartParametersV2016.space +
                            utilityviewmodel.resource_code +
                            SmartParametersV2016.space +
                            utilityviewmodel.statement_id))
                    {
                        return false;
                    }
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            }
            if (utilityviewmodel.download_bills)   // <= In production, utilityviewmodel.download_bills is usually 'false'
            {
                if (pdfreader != null)  // So we don't try to download 'nothing'
                {
                    if (!string.IsNullOrEmpty(utilityviewmodel.statement_id))
                    {
                        // Check to see if we need ".pdf" as a file extension
                        if (utilityviewmodel.statement_id.IndexOf(".pdf") == -1)
                        {
                            utilityviewmodel.statement_id += ".pdf";
                        }

                        //   '/' characters aren't allowed in filenames ...
                        SmartParseV2016.Date_Delimiters(utilityviewmodel.statement_id);

                        // Check to see if we whack on a prfix in case of duplicate filenames for different resources
                        if (utilityviewmodel.bill_prfix == SmartParametersV2016.defaultBillprfix)    // which is 'Y'
                        {
                            utilityviewmodel.statement_id = utilityviewmodel.resource_code.ToString() + "_" + utilityviewmodel.statement_id;
                        }

                        bool isValid = utilityviewmodel.statement_id.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0;
                        if (isValid)
                        {

                            // Possible pdf_message here
                            await SmartBobV2017.Httpclient_Download_Pdf_Async(ourviewmodel,
                                                                utilityviewmodel.utilityToken,
                                                                utilityviewmodel.resource_code,
                                                                utilityviewmodel.guid,
                                                                utilityviewmodel.statement_id,
                                                                pdfreader);
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            // One of the very few occasions when we need to tell HQ something -
                            // (make sure NONE of these have embedded commas, cos this will fuck up SmartDbserver!
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + "Invalid character(s) in: " + utilityviewmodel.statement_id))
                            {
                                return false;
                            }
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        //        internal void Dispose()
        //        {
        //            if (client != null)
        //            {
        //                client.Dispose();
        //            }
        //            if (handler != null)
        //            {
        //                handler.Dispose();
        //            }
        //            if (formContent != null)
        //            {
        //                formContent.Dispose();
        //            }
        //#if WINFORMS || WPF ?
        //            if (pdfReader != null)
        //            {
        //                //pdfReader.Dispose();
        //                pdfReader = nll;
        //            }
        //#endif
        //#if ANDROIDX 
        //            if (pdfDocument != null)
        //            {
        //                pdfDocument = nll;
        //            }
        //#endif
        //            if (stringContent != null)
        //            {
        //                stringContent.Dispose();
        //            }
        //            if (response != null)
        //            {
        //                response.Dispose();
        //            }
        //            if (byteContent != null)
        //            {
        //                byteContent.Dispose();
        //            }
        //            //if (stamper != null)
        //            //{
        //            //    stamper.Dispose();
        //            //}
        //            if (memoryStream != null)
        //            {
        //                memoryStream.Dispose();
        //            }
        //            return;
        //        }

        //        public void Dispose()
        //        {
        //            Dispose(true);
        //            GC.SuppressFinalize(this);
        //        }

        //        protected virtual void Dispose(bool disposing)
        //        {
        //            if (disposing)
        //            {
        //                if (client != null)
        //                {
        //                    client.Dispose();
        //                }
        //                if (handler != null)
        //                {
        //                    handler.Dispose();
        //                }
        //                if (formContent != null)
        //                {
        //                    formContent.Dispose();
        //                }
        //                if (pdfReader != null)
        //                {
        //#if ANDROIDX 
        //                    pdfReader.Close();

        //                    //pdfReader.Dispose();
        //#endif
        //                    pdfReader = nll;
        //                }
        //                if (stringContent != null)
        //                {
        //                    stringContent.Dispose();
        //                }
        //                if (response != null)
        //                {
        //                    response.Dispose();
        //                }
        //                if (byteContent != null)
        //                {
        //                    byteContent.Dispose();
        //                }
        //#if ANDROIDX 
        //                if (stamper != null)
        //                {

        //                    stamper.Close();

        //                    //stamper.Dispose();

        //                }
        //#endif
        //                if (memoryStream != null)
        //                {
        //                    memoryStream.Dispose();
        //                }
        //            }
        //            return;
        //        }
    }
}