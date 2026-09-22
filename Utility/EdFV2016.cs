// Stupid beyond belief - they are with United Utilities - I put are you with 'FN' energy

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;


#if ANDROIDX
using AndroidX.AppCompat.App;
#endif
namespace SmartCubeMobile
{
    public class EdFV2016
    {
        //Fucking chimps


        // *******************************************************************************
        //
        //  
        //  Needed for EdF or nothing fucking works:
        //
        //        client.UserAgent = "Mozilla/5.0 like Gecko";
        //
        // (don't the FUCK ask me fucking why ....)
        //
        // So this is why we have to set 'useProxy' to 'true' otherwise we will never
        // get anything back.  What a bunch of fucking french cunts Edf are-
        //
        // *******************************************************************************
        internal static async Task<bool> Read_Meter(
#if WINFORMS
                                            //WebBrowser Scraper,
#endif
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif


                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            bool TextBox_Active)
        {
            // LEAVE THIS IN AND SEE IF THIS BOLLCKS MAKES ANY DIFFERENCE (I don't think it does)

            //
            // THINK WE CAN GET AWAY WITHOUT THIS USER AGENT BOLLCKS
            //

            // We ONLY need this to convert HTML documents ... and when - one
            // day - we find a better way, this fucking useless object is BINNED
            // It is now fucking binned WebBrowser wb = new WebBrowser();
            // ========================

            // Because Sometimes we are returned a Document
            utilityviewmodel.htmlDocument = new HtmlAgilityPack.HtmlDocument();

            utilityviewmodel.keyValues = new List<KeyValuePair<string, string>>();

            // Everytime timer ticks, timer_Tick will be called
            // Timer will tick evert second
            // FUCKING HELL - this is never Enableutilityviewmodel.d????
            // You have 60 Seconds to stop
            // the timer on a good login!
            utilityviewmodel.keep_looping = true;
            List<SmartUtility.Payments> payments_tempList = new List<SmartUtility.Payments>();

            //string user_agent = "Mozilla/5.0 like Gecko";
            //Uri TargetUrl = new Uri(SmartParameteresV2016.localhost);
            // Could that fucking bitch be ANY noisier?  DOes she HAVE to turn the fucking
            // radio on?  Is she SO RUDE, THICK or INCONSIDERATE to ***ASK*** me BEFORE
            // she turns the fucking thing on???  ODSBD and IWBSN

            while (utilityviewmodel.keep_looping)
            {

#if WINFORMS
                await SmartUtilityV2022.First_Throw(ourviewmodel,
                                                    utilityviewmodel);
                                                    //DateTime.Now + ourviewmodel.utcOffset);
#endif
#if WPF 
                await SmartUtilityV2022.First_Throw(ourviewmodel, utilityviewmodel);
#endif
#if ANDROIDX
                await SmartUtilityV2022.First_Throw(ourviewmodel, utilityviewmodel);
#endif

                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    utilityviewmodel.next_routine == "LOGOUT")
                {
                    utilityviewmodel.target_pathname = utilityviewmodel.logout_pathname;
                    utilityviewmodel.keep_looping = false;
                    utilityviewmodel.next_routine = "LOGOUT";
                }
                if (utilityviewmodel.next_routine != "HOME")
                {
                    utilityviewmodel.htmlDocument = await SmartBobV2017.Scraper_Generic_Get(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel);
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
                utilityviewmodel.current_routine = utilityviewmodel.next_routine;
                switch (utilityviewmodel.next_routine)
                {
                    case "LOGIN":
                        // The timer is re-started when the
                        // first Login Document has been completed
                        utilityviewmodel.keyValues.Clear();
                        if (await EDF_Login(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        utilityviewmodel,
                                        utilityviewmodel.htmlDocument,
                                        TextBox_Active))
                        {
                            await SmartBobV2017.Scraper_Generic_Post(ourviewmodel,
                                                                utilityviewmodel.utilityToken,
                                                                utilityviewmodel.keyValues,
                                                                utilityviewmodel);
                            // Should now be going on to ACCOVRVW - you ARE a fucking Genius, Ray!!
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                            utilityviewmodel.next_routine = "LOGOUT";
                            utilityviewmodel.login_finished = true;          // Escape route on Login failure
                        }

                        //{

                        //    // Add the 'special' query string
                        //    // Add the Login submit string
                        //    if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
                        //                                        utilityviewmodel.target_xxx, //"nclogin.submit",
                        //                                        rf TargetUrl,
                        //                                        rf uri_message))
                        //    {
                        //        ourviewmodel.errorMessage = current_routine + "|" + uri_message;
                        //        return false;
                        //    }

                        //    SmartBobV2017.formContent = new FormUrlEncodedContent(keyValues);

                        //    utilityviewmodel.htmlDocument = await SmartBobV2017.HTTPCLIENT_POST_ASYNC(timespanTimeout,
                        //                                                    SmartBobV2017.formContent,
                        //                                                    TargetUrl,
                        //                                                    z => http_message = z,
                        //                                                    useProxy,
                        //                                                    username,
                        //                                                    website,
                        //                                                    guid);

                        //    //utilityviewmodel.htmlDocument = await SmartBobV2017.HTTP_POST_ASYNC(utilityviewmodel.connection_timeout,
                        //    //                                                useProxy,
                        //    //                                                guid,
                        //    //                                                username,
                        //    //                                                website,
                        //    //                                                //utilityviewmodel.brand_code,
                        //    //                                                //utilityviewmodel.supplier_code,
                        //    //                                                TargetUrl,
                        //    //                                                "",   // Referer
                        //    //                                                user_agent,
                        //    //                                                postdata,
                        //    //                                                false,
                        //    //                                                false,
                        //    //                                                "",
                        //    //                                                time_now,
                        //    //                                                yymmdd_format,
                        //    //                                                z => http_message = z);
                        //    if (!string.IsNullOrEmpty(http_message) ||
                        //        (utilityviewmodel.htmlDocument.RemainderOffset == 0))
                        //    {
                        //        ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
                        //        keep_looping = false;
                        //        next_routine = "LOGOUT";
                        //    }
                        //    // Should now be going on to ACCOVRVW - you ARE a fucking Genius, Ray!!
                        //}
                        //else
                        //{
                        //    ourviewmodel.errorMessage = current_routine;
                        //    next_routine = "LOGOUT";
                        //}
                        break;
                    case "VMLSTBILLS":
                        if (!await EDF_ViewListBills(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.htmlDocument,
                                                        TextBox_Active))
                        {
                            utilityviewmodel.next_routine = "LOGOUT";
                        }
                        // Should be going back to "HOME" // "ACCOVRVW"


                        //{
                        //    if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
                        //                                        pathname,
                        //                                        rf TargetUrl,
                        //                                        rf uri_message))
                        //    {
                        //        ourviewmodel.errorMessage = current_routine + "|" + uri_message;
                        //        return false;
                        //    }
                        //    utilityviewmodel.htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(timespanTimeout,
                        //                                                            TargetUrl,
                        //                                                            z => http_message = z,
                        //                                                            useProxy,
                        //                                                            username,
                        //                                                            website,
                        //                                                            guid);
                        //    //utilityviewmodel.htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
                        //    //                                                useProxy,
                        //    //                                                guid,
                        //    //                                                username,
                        //    //                                                website,
                        //    //                                                //utilityviewmodel.brand_code,
                        //    //                                                //utilityviewmodel.supplier_code,
                        //    //                                                TargetUrl,
                        //    //                                                user_agent,
                        //    //                                                false,
                        //    //                                                time_now,
                        //    //                                                yymmdd_format,
                        //    //                                                z => http_message = z);
                        //    if (!string.IsNullOrEmpty(http_message) ||
                        //            (utilityviewmodel.htmlDocument.RemainderOffset == 0))
                        //    {
                        //        ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
                        //        keep_looping = false;
                        //        next_routine = "LOGOUT";
                        //    }
                        //    // Should be going back to "ACCOVRVW"
                        //}
                        //else
                        //{
                        //    ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
                        //    next_routine = "LOGOUT";
                        //}
                        break;
                    case "LOGOUT":
                        // Combine the Bills_In and Bills_Out lists and
                        // only include unallocated payments past the last Bill Period end

                        // This will work because Payments_Out contains payments DECODED from any
                        // new Bills PLUS payments added from the SCRAPE which is stored in Payments_Temp
                        SmartUtilityV2022.Generic_Payment_Done(utilityviewmodel, payments_tempList);

                        await SmartUtilityV2022.Common_Logout(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        utilityviewmodel,
                                                        TextBox_Active);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                        break;
                    default:
                        // A small price to pay ...
                        EDF_Do_Intermediates(ourviewmodel,
                                            utilityviewmodel,
                                            utilityviewmodel.next_routine,
                                            utilityviewmodel.htmlDocument,
                                            DateTime.Now + ourviewmodel.utcOffset);  // Local time
                        break;
                }
            }

            await SmartUtilityV2022.Last_Throw(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, utilityviewmodel);

            // Yay!
            return utilityviewmodel.login_finished;
        }

        private static void EDF_Do_Intermediates(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string next_routine,
                                                HtmlAgilityPack.HtmlDocument htmlDocument,
                                                DateTime time_now)
        {
            switch (next_routine)
            {
                case "HOME":
                    EDF_AccountOverview(ourviewmodel,
                                            utilityviewmodel,
                                            htmlDocument);


                    // Should now be going on to EDFCONTACT? // ENTMETRD



                    //{
                    //    if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
                    //                                        "??",
                    //                                        rf TargetUrl,
                    //                                        rf uri_message))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + "|" + uri_message;
                    //        return false;
                    //    }
                    //    htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(timespanTimeout,
                    //                                                            TargetUrl,
                    //                                                            z => http_message = z,
                    //                                                            useProxy,
                    //                                                            username,
                    //                                                            website,
                    //                                                            guid);

                    //    //htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
                    //    //                                                useProxy,
                    //    //                                                guid,
                    //    //                                                username,
                    //    //                                                website,
                    //    //                                                //utilityviewmodel.brand_code,
                    //    //                                                //utilityviewmodel.supplier_code,
                    //    //                                                TargetUrl,
                    //    //                                                user_agent,
                    //    //                                                false,
                    //    //                                                time_now,
                    //    //                                                yymmdd_format,
                    //    //                                                z => http_message = z);
                    //    if (!string.IsNullOrEmpty(http_message) ||
                    //            (htmlDocument.RemainderOffset == 0))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
                    //        keep_looping = false;
                    //        next_routine = "LOGOUT";
                    //    }
                    //    
                    //}
                    //else
                    //{
                    //    ourviewmodel.errorMessage = current_routine;
                    //    next_routine = "LOGOUT";
                    //}
                    break;
                case "EDFCONTACT":
                    EDF_ContactDetails(ourviewmodel,
                                            utilityviewmodel,
                                            htmlDocument);
                    // Should be going on to "ACCDETLS"


                    //{
                    //    if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
                    //                                        pathname,
                    //                                        rf TargetUrl,
                    //                                        rf uri_message))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + "|" + uri_message;
                    //        return false;
                    //    }
                    //    htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(timespanTimeout,
                    //                                                            TargetUrl,
                    //                                                            z => http_message = z,
                    //                                                            useProxy,
                    //                                                            username,
                    //                                                            website,
                    //                                                            guid);
                    //    //htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
                    //    //                                                useProxy,
                    //    //                                                guid,
                    //    //                                                username,
                    //    //                                                website,
                    //    //                                                //utilityviewmodel.brand_code,
                    //    //                                                //utilityviewmodel.supplier_code,
                    //    //                                                TargetUrl,
                    //    //                                                user_agent,
                    //    //                                                false,
                    //    //                                                time_now,
                    //    //                                                yymmdd_format,
                    //    //                                                z => http_message = z);
                    //    if (!string.IsNullOrEmpty(http_message) ||
                    //            (htmlDocument.RemainderOffset == 0))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
                    //        keep_looping = false;
                    //        next_routine = "LOGOUT";
                    //    }
                    //    // Should be going on to "ACCDETLS"
                    //}
                    //else
                    //{
                    //    ourviewmodel.errorMessage = current_routine;
                    //    next_routine = "LOGOUT";
                    //}
                    break;
                case "ACCDETLS":
                    EDF_AccountDetails(ourviewmodel,
                                            utilityviewmodel,
                                            htmlDocument);

                    // Should be going on to "VIEWBILLS"
                    //{
                    //    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    //    {
                    //        // One of the very few occasions when we need to tell HQ something
                    //        await SmartBobV2017.ListenerAsync(ourviewmodel, username, website, utilityviewmodel.brand_code, utilityviewmodel.supplier_code, ourviewmodel.errorMessage);
                    //    }
                    //    if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
                    //                                        pathname,
                    //                                        rf TargetUrl,
                    //                                        rf uri_message))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + "|" + uri_message;
                    //        return false;
                    //    }
                    //    htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                    //                                                            TargetUrl,
                    //                                                            z => http_message = z,
                    //                                                            useProxy,
                    //                                                            username,
                    //                                                            website,
                    //                                                            guid);
                    //    //htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
                    //    //                                                useProxy,
                    //    //                                                guid,
                    //    //                                                username,
                    //    //                                                website,
                    //    //                                                //utilityviewmodel.brand_code,
                    //    //                                                //utilityviewmodel.supplier_code,
                    //    //                                                TargetUrl,
                    //    //                                                user_agent,
                    //    //                                                false,
                    //    //                                                time_now,
                    //    //                                                yymmdd_format,
                    //    //                                                z => http_message = z);
                    //    if (!string.IsNullOrEmpty(http_message) ||
                    //            (htmlDocument.RemainderOffset == 0))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
                    //        keep_looping = false;
                    //        next_routine = "LOGOUT";
                    //    }
                    //    // Should be going on to "VIEWBILLS"
                    //}
                    //else
                    //{
                    //    ourviewmodel.errorMessage = current_routine;
                    //    next_routine = "LOGOUT";
                    //}
                    break;
                case "VIEWBILLS":
                    EDF_ViewBills(htmlDocument,
                                            utilityviewmodel);
                    // Should be going on to "VWPRVBILLS"

                    //{
                    //    //target_string = utilityviewmodel.prfix_xxx + pathname;
                    //    //TargetUrl = new Uri(target_string);
                    //    if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
                    //                                        pathname,
                    //                                        rf TargetUrl,
                    //                                        rf uri_message))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + "|" + uri_message;
                    //        return false;
                    //    }
                    //    htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(timespanTimeout,
                    //                                                            TargetUrl,
                    //                                                            z => http_message = z,
                    //                                                            useProxy,
                    //                                                            username,
                    //                                                            website,
                    //                                                            guid);
                    //    //htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
                    //    //                                                useProxy,
                    //    //                                                guid,
                    //    //                                                username,
                    //    //                                                website,
                    //    //                                                //utilityviewmodel.brand_code,
                    //    //                                                //utilityviewmodel.supplier_code,
                    //    //                                                TargetUrl,
                    //    //                                                user_agent,
                    //    //                                                false,
                    //    //                                                time_now,
                    //    //                                                yymmdd_format,
                    //    //                                                z => http_message = z);
                    //    if (!string.IsNullOrEmpty(http_message) ||
                    //            (htmlDocument.RemainderOffset == 0))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
                    //        keep_looping = false;
                    //        next_routine = "LOGOUT";
                    //    }
                    //    
                    //}
                    //else
                    //{
                    //    ourviewmodel.errorMessage = current_routine;
                    //    next_routine = "LOGOUT";
                    //}
                    break;
                case "VWPRVBILLS":
                    EDF_ViewPreviousBills(htmlDocument,
                                            time_now,
                                            utilityviewmodel);

                    // Should be going on to "VWLSTBILLS"

                    //{
                    //    //target_string = utilityviewmodel.prfix_xxx + pathname;
                    //    //TargetUrl = new Uri(target_string);
                    //    if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
                    //                                        pathname,
                    //                                        rf TargetUrl,
                    //                                        rf uri_message))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + "|" + uri_message;
                    //        return false;
                    //    }
                    //    htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(timespanTimeout,
                    //                                                            TargetUrl,
                    //                                                            z => http_message = z,
                    //                                                            useProxy,
                    //                                                            username,
                    //                                                            website,
                    //                                                            guid);
                    //    //htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
                    //    //                                                useProxy,
                    //    //                                                guid,
                    //    //                                                username,
                    //    //                                                website,
                    //    //                                                //utilityviewmodel.brand_code,
                    //    //                                                //utilityviewmodel.supplier_code,
                    //    //                                                TargetUrl,
                    //    //                                                user_agent,
                    //    //                                                false,
                    //    //                                                time_now,
                    //    //                                                yymmdd_format,
                    //    //                                                z => http_message = z);
                    //    if (!string.IsNullOrEmpty(http_message) ||
                    //            (htmlDocument.RemainderOffset == 0))
                    //    {
                    //        ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
                    //        keep_looping = false;
                    //        next_routine = "LOGOUT";
                    //    }
                    //    // Should be going on to "VWLSTBILLS"
                    //}
                    //else
                    //{
                    //    ourviewmodel.errorMessage = current_routine;
                    //    next_routine = "LOGOUT";
                    //}
                    break;
            }
            return;
        }
        //        internal static async Task<bool> Read_Meter(TimeSpan timespanTimeout,
        //                                                    TimeSpan utcOffset,
        //                                                    Guid guid,
        //                                                    string website,
        //                                                    <Simple> setutilityviewmodel,
        //                                                    <Simple> getutilityviewmodel,
        //                                                    bool TextBox_Active,
        //                                                    string username,
        //                                                    string yymmdd_format


        //                                                    )
        //        {
        //            UtilityViewModel utilityviewmodel = getMES();

        //            bool useProxy = utilityviewmodel.useProxy;     // Should always be false - now!!!

        //            // LEAVE THIS IN AND SEE IF THIS BOLLCKS MAKES ANY DIFFERENCE (I don't think it does)

        //            //
        //            // THINK WE CAN GET AWAY WITHOUT THIS USER AGENT BOLLCKS
        //            //

        //            // We ONLY need this to convert HTML documents ... and when - one
        //            // day - we find a better way, this fucking useless object is BINNED
        //            // It is now fucking binned WebBrowser wb = new WebBrowser();
        //            // ========================

        //            // To Find out what might have gone wrong
        //            string http_message = "";

        //            // Because Sometimes we are returned a Document
        //            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

        //            string postdata = "";

        //            // Everytime timer ticks, timer_Tick will be called
        //            // Timer will tick evert second
        //            // FUCKING HELL - this is never Enabled????
        //            // You have 60 Seconds to stop
        //            // the timer on a good login!
        //            string next_routine = SmartParametersV2016.initialNextRoutine,
        //                    pathname = "",            // Used to target the next page
        //                                                        //account_number = "",
        //                                                        //page_number = "",
        //                                                        // target_string = "",
        //                    uri_message = "";
        //            //url_addon = "";

        //            bool keep_looping = true;

        //            //string payments_pathname = "";
        //            List<SmartUtility.Payments> payments_tempList = new List<SmartUtility.Payments>();
        //            DateTime time_now = SmartParametersV2016.defaultDate;

        //            string user_agent = "Mozilla/5.0 like Gecko";
        //            Uri TargetUrl = new Uri(SmartParametersV2016.localhost);

        //            while (keep_looping)
        //            {
        //                if (!useProxy)
        //                {
        //                    // DON'T CHANGE THIS IF WE ARE SCRAPING BY PROXY - IT NEEDS RO BE CONSTANT
        //                    time_now = ourviewmodel.time_now;
        //                }
        //#if WINFORMS
        //                if (utilityviewmodel.console)
        //                {
        //                    // Could that fucking bitch be ANY noisier?  DOes she HAVE to turn the fucking
        //                    // radio on?  Is she SO RUDE, THICK or INCONSIDERATE to ***ASK*** me BEFORE
        //                    // she turns the fucking thing on???  ODSBD and IWBSN
        //                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ourviewmodel.time_now.ToString(yymmdd_format) + SmartParametersV2016.space + utilityviewmodel.brand_code + SmartParametersV2016.space + utilityviewmodel.supplier_code + SmartParametersV2016.space + next_routine);
        //                }
        //#Xelse
        //                if (utilityviewmodel.examine.scrape) { await SmartBobV2017.ListenerAsync(,timespanTimeout, username, website, utilityviewmodel.brand_code, utilityviewmodel.supplier_code, next_routine); }
        //#endif
        //                string current_routine = next_routine;
        //                switch (next_routine)
        //                {
        //                    case SmartParametersV2016.initialNextRoutine:
        //                        //target_string = utilityviewmodel.prfix_xxx + utilityviewmodel.TargetUrl.ToString();
        //                        //Uri TargetUrl = new Uri(target_string);
        //                        if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
        //                                                                utilityviewmodel.target_xxx,
        //                                                                rf TargetUrl,
        //                                                                rf uri_message))
        //                        {
        //                            ourviewmodel.errorMessage = current_routine + "|" + uri_message;
        //                            return false;
        //                        }
        //                        htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.login_timeout,
        //                                                                        useProxy,
        //                                                                        guid,
        //                                                                        username,
        //                                                                        website,
        //                                                                        //utilityviewmodel.brand_code,
        //                                                                        //utilityviewmodel.supplier_code,
        //                                                                       TargetUrl,
        //                                                                        user_agent,
        //                                                                        false,
        //                                                                        time_now,
        //                                                                        yymmdd_format,
        //                                                                        z => http_message = z);
        //                        // Will you fucking shut up??
        //                        if (htmlDocument.RemainderOffset == 0) // ||
        //                                                                //!string.IsNullOrEmpty(http_message))
        //                        {
        //                            ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                            keep_looping = false;
        //                        }
        //                        next_routine = "LOGIN";
        //                        break;
        //                    case "LOGIN":
        //                        // The timer is re-started when the
        //                        // first Login Document has been completed
        //                        if (EDF_Login(htmlDocument,
        //                                        TextBox_Active,
        //                                        textBoxBrowser,
        //                                        scrollViewer, 
        //                                        time_now,
        //                                        utilityviewmodel,
        //                                        rf postdata,
        //                                        rf next_routine))
        //                        {
        //                            // Add the 'special' query string
        //                            // Add the Login submit string
        //                            //target_string = utilityviewmodel.prfix_xxx + "nclogin.submit";
        //                            //TargetUrl = new Uri(target_string);
        //                            if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
        //                                                                utilityviewmodel.target_xxx, //"nclogin.submit",
        //                                                                rf TargetUrl,
        //                                                                rf uri_message))
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + "|" + uri_message;
        //                                return false;
        //                            }
        //                            htmlDocument = await SmartBobV2017.HTTP_POST_ASYNC(utilityviewmodel.connection_timeout,
        //                                                                            useProxy,
        //                                                                            guid,
        //                                                                            username,
        //                                                                            website,
        //                                                                            //utilityviewmodel.brand_code,
        //                                                                            //utilityviewmodel.supplier_code,
        //                                                                            TargetUrl,
        //                                                                            "",   // Referer
        //                                                                            user_agent,
        //                                                                            postdata,
        //                                                                            false,
        //                                                                            false,
        //                                                                            "",
        //                                                                            time_now,
        //                                                                            yymmdd_format,
        //                                                                            z => http_message = z);
        //                            if (htmlDocument.RemainderOffset == 0)
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                                keep_looping = false;
        //                            }
        //                            // Should now be going on to ACCOVRVW - you ARE a fucking Genius, Ray!!
        //                        }
        //                        else
        //                        {
        //                            ourviewmodel.errorMessage = current_routine;
        //                            next_routine = "LOGOUT";
        //                        }
        //                        break;
        //                    case "ACCOVRVW":
        //                        if (EDF_AccountOverview(htmlDocument,
        //                                                utilityviewmodel,
        //                                                rf next_routine))
        //                        {
        //                            //target_string = utilityviewmodel.prfix_xxx + "??";
        //                            //TargetUrl = new Uri(target_string);
        //                            if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
        //                                                                "??",
        //                                                                rf TargetUrl,
        //                                                                rf uri_message))
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + "|" + uri_message;
        //                                return false;
        //                            }
        //                            htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
        //                                                                            useProxy,
        //                                                                            guid,
        //                                                                            username,
        //                                                                            website,
        //                                                                            //utilityviewmodel.brand_code,
        //                                                                            //utilityviewmodel.supplier_code,
        //                                                                            TargetUrl,
        //                                                                            user_agent,
        //                                                                            false,
        //                                                                            time_now,
        //                                                                            yymmdd_format,
        //                                                                            z => http_message = z);
        //                            if (htmlDocument.RemainderOffset == 0)
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                                keep_looping = false;
        //                            }
        //                            // Should now be going on to ENTMETRD
        //                        }
        //                        else
        //                        {
        //                            ourviewmodel.errorMessage = current_routine;
        //                            next_routine = "LOGOUT";
        //                        }
        //                        break;
        //                    case "EDFCONTACT":
        //                        if (EDF_ContactDetails(htmlDocument,
        //                                                utilityviewmodel,
        //                                                rf next_routine))
        //                        {
        //                            //target_string = utilityviewmodel.prfix_xxx + pathname;
        //                            //TargetUrl = new Uri(target_string);
        //                            if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
        //                                                                pathname,
        //                                                                rf TargetUrl,
        //                                                                rf uri_message))
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + "|" + uri_message;
        //                                return false;
        //                            }
        //                            htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
        //                                                                            useProxy,
        //                                                                            guid,
        //                                                                            username,
        //                                                                            website,
        //                                                                            //utilityviewmodel.brand_code,
        //                                                                            //utilityviewmodel.supplier_code,
        //                                                                            TargetUrl,
        //                                                                            user_agent,
        //                                                                            false,
        //                                                                            time_now,
        //                                                                            yymmdd_format,
        //                                                                            z => http_message = z);
        //                            if (htmlDocument.RemainderOffset == 0)
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                                keep_looping = false;
        //                            }
        //                            // Should be going on to "ACCDETLS"
        //                        }
        //                        else
        //                        {
        //                            ourviewmodel.errorMessage = current_routine;
        //                            next_routine = "LOGOUT";
        //                        }
        //                        break;
        //                    case "ACCDETLS":
        //                        if (EDF_AccountDetails(htmlDocument,
        //                                                utilityviewmodel,
        //                                                rf next_routine))
        //                        {
        //                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
        //                            {
        //                                // One of the very few occasions when we need to tell HQ something
        //                                await SmartBobV2017.ListenerAsync(ourviewmodel.Username,timespanTimeout, username, website, utilityviewmodel.brand_code, utilityviewmodel.supplier_code, ourviewmodel.errorMessage);
        //                            }
        //                            //target_string = utilityviewmodel.prfix_xxx + pathname;
        //                            //TargetUrl = new Uri(target_string);
        //                            if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
        //                                                                pathname,
        //                                                                rf TargetUrl,
        //                                                                rf uri_message))
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + "|" + uri_message;
        //                                return false;
        //                            }
        //                            htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
        //                                                                            useProxy,
        //                                                                            guid,
        //                                                                            username,
        //                                                                            website,
        //                                                                            //utilityviewmodel.brand_code,
        //                                                                            //utilityviewmodel.supplier_code,
        //                                                                            TargetUrl,
        //                                                                            user_agent,
        //                                                                            false,
        //                                                                            time_now,
        //                                                                            yymmdd_format,
        //                                                                            z => http_message = z);
        //                            if (htmlDocument.RemainderOffset == 0)
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                                keep_looping = false;
        //                            }
        //                            // Should be going on to "VIEWBILLS"
        //                        }
        //                        else
        //                        {
        //                            ourviewmodel.errorMessage = current_routine;
        //                            next_routine = "LOGOUT";
        //                        }
        //                        break;
        //                    case "VIEWBILLS":
        //                        if (EDF_ViewBills(htmlDocument,
        //                                                rf next_routine))
        //                        {
        //                            //target_string = utilityviewmodel.prfix_xxx + pathname;
        //                            //TargetUrl = new Uri(target_string);
        //                            if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
        //                                                                pathname,
        //                                                                rf TargetUrl,
        //                                                                rf uri_message))
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + "|" + uri_message;
        //                                return false;
        //                            }
        //                            htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
        //                                                                            useProxy,
        //                                                                            guid,
        //                                                                            username,
        //                                                                            website,
        //                                                                            //utilityviewmodel.brand_code,
        //                                                                            //utilityviewmodel.supplier_code,
        //                                                                            TargetUrl,
        //                                                                            user_agent,
        //                                                                            false,
        //                                                                            time_now,
        //                                                                            yymmdd_format,
        //                                                                            z => http_message = z);
        //                            if (htmlDocument.RemainderOffset == 0)
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                                keep_looping = false;
        //                            }
        //                            // Should be going on to "VWPRVBILLS"
        //                        }
        //                        else
        //                        {
        //                            ourviewmodel.errorMessage = current_routine;
        //                            next_routine = "LOGOUT";
        //                        }
        //                        break;
        //                    case "VWPRVBILLS":
        //                        if (EDF_ViewPreviousBills(htmlDocument,
        //                                                utcOffset,
        //                                                rf next_routine))
        //                        {
        //                            //target_string = utilityviewmodel.prfix_xxx + pathname;
        //                            //TargetUrl = new Uri(target_string);
        //                            if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
        //                                                                pathname,
        //                                                                rf TargetUrl,
        //                                                                rf uri_message))
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + "|" + uri_message;
        //                                return false;
        //                            }
        //                            htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
        //                                                                            useProxy,
        //                                                                            guid,
        //                                                                            username,
        //                                                                            website,
        //                                                                            //utilityviewmodel.brand_code,
        //                                                                            //utilityviewmodel.supplier_code,
        //                                                                            TargetUrl,
        //                                                                            user_agent,
        //                                                                            false,
        //                                                                            time_now,
        //                                                                            yymmdd_format,
        //                                                                            z => http_message = z);
        //                            if (htmlDocument.RemainderOffset == 0)
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                                keep_looping = false;
        //                            }
        //                            // Should be going on to "VWLSTBILLS"
        //                        }
        //                        else
        //                        {
        //                            ourviewmodel.errorMessage = current_routine;
        //                            next_routine = "LOGOUT";
        //                        }
        //                        break;
        //                    case "VMLSTBILLS":
        //                        if (await EDF_ViewListBills(timespanTimeout,
        //                                                        useProxy,
        //                                                        guid,
        //                                                        username,
        //                                                        website,
        //                                                        htmlDocument,
        //                                                        user_agent,
        //                                                        TextBox_Active,
        //                                                        //utcOffset,

        //                                                        p => MES = p,
        //                                                        () => utilityviewmodel,
        //                                                        time_now,
        //                                                        q => next_routine = q,
        //                                                        z => http_message = z,
        //                                                        yymmdd_format,
        //                                                        current_routine))
        //                        {
        //                            //target_string = utilityviewmodel.prfix_xxx + pathname;
        //                            //TargetUrl = new Uri(target_string);
        //                            if (!SmartUtilityV2022.Create_Uri(utilityviewmodel.prfix_xxx,
        //                                                                pathname,
        //                                                                rf TargetUrl,
        //                                                                rf uri_message))
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + "|" + uri_message;
        //                                return false;
        //                            }
        //                            htmlDocument = await SmartBobV2017.HTTP_GET_ASYNC(utilityviewmodel.connection_timeout,
        //                                                                            useProxy,
        //                                                                            guid,
        //                                                                            username,
        //                                                                            website,
        //                                                                            //utilityviewmodel.brand_code,
        //                                                                            //utilityviewmodel.supplier_code,
        //                                                                            TargetUrl,
        //                                                                            user_agent,
        //                                                                            false,
        //                                                                            time_now,
        //                                                                            yymmdd_format,
        //                                                                            z => http_message = z);
        //                            if (htmlDocument.RemainderOffset == 0)
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                                keep_looping = false;
        //                            }
        //                            // Should be going back to "ACCOVRVW"
        //                        }
        //                        else
        //                        {
        //                            ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                            next_routine = "LOGOUT";
        //                        }
        //                        break;
        //                    case "LOGOUT":
        //                        // Combine the Bills_In and Bills_Out lists and
        //                        // only include unallocated payments past the last Bill Period end

        //                        // This will work because Payments_Out contains payments DECODED from any
        //                        // new Bills PLUS payments added from the SCRAPE which is stored in Payments_Temp
        //                        SmartUtilityV2022.Generic_Payment_Done(utilityviewmodel, payments_tempList);

        //                        EDF_Logout(TextBox_Active,
        //#if WINFORMS
        //                                    utilityviewmodel,
        //#Xelse
        //                                    textBoxBrowser,
        //                                    scrollViewer,            
        //#endif
        //                                    time_now,
        //                                    rf keep_looping);
        //                        if (useProxy)
        //                        {
        //                            if (!await SmartBobV2017.HTTP_DELETE_ASYNC(utilityviewmodel.connection_timeout,
        //                                                                           guid,
        //                                                                           username,
        //                                                                           website,
        //                                                                           //utilityviewmodel.brand_code,
        //                                                                           //utilityviewmodel.supplier_code,
        //                                                                           user_agent,
        //                                                                           time_now,
        //                                                                           yymmdd_format,
        //                                                                           z => http_message = z))
        //                            // We don't really care about this status - its just
        //                            // for housekeeping the cookies
        //                            {
        //                                ourviewmodel.errorMessage = current_routine + SmartParametersV2016.space + http_message;
        //                                keep_looping = false;
        //                            }
        //                        }
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }

        //            if (!utilityviewmodel.login_attempted)
        //            {
        //                // We can assume the login failed
        //#if WINFORMS
        //                if (utilityviewmodel.console)
        //                {
        //                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Login failed");
        //                }
        //#endif
        //            }
        //            else
        //            {
        //                // We can assume the login succeeded
        //#if WINFORMS
        //                if (utilityviewmodel.console)
        //                {
        //                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Logged out");
        //                }
        //#endif
        //            }
        //            // Yay!
        //            setMES(utilityviewmodel);
        //            return utilityviewmodel.login_finished;
        //        }

        private static async Task<bool> EDF_Login(
#if ANDROIDX
                                        AppCompatActivity meterActivity,
#endif
                                        MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        HtmlAgilityPack.HtmlDocument document,
                                        bool TextBox_Active)
        {
            // This is the page which is delivered as "Complete" when we enter
            // the initial Supplier target URL
            // However!  At LEAST these EDF fuckers don't re-direct us off
            // to a 'secure' URL ....
            // We then unhook this routine (i.e. wb_DocumentCompleted_Login)
            // and hook onto the routine to handle the Account details for poor old Lucy

            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol;
            string element_name;
            bool clicked = false;
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                element_name = element.GetAttributeValue("name", "");
                switch (element.Id)
                {
                    case "edit-name":
                        // Set the user name in the username text box
                        keyValues.Add(new KeyValuePair<string, string>(element_name, utilityviewmodel.user_id));

                        if (TextBox_Active)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel,
                                "UserId" +
                                SmartParametersV2016.space +
                                utilityviewmodel.user_id);
                        }
                        break;
                    case "edit-pass":
                        //Type the password in the password text box
                        keyValues.Add(new KeyValuePair<string, string>(element_name, utilityviewmodel.password));
                        // Hide the password in the log

                        if (TextBox_Active)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel,
                                "Password" +
                                SmartParametersV2016.space +
                                "Asterisks");
                        }

                        string value1 = document.DocumentNode.SelectSingleNode("//input[@type='hidden' and @name='form_build_id']").Attributes["value"].Value;
                        if (!string.IsNullOrEmpty(value1))
                        {
                            keyValues.Add(new KeyValuePair<string, string>("form_build_id", value1));
                        }

                        string value2 = document.DocumentNode.SelectSingleNode("//input[@type='hidden' and @name='form_id']").Attributes["value"].Value;
                        if (!string.IsNullOrEmpty(value2))
                        {
                            keyValues.Add(new KeyValuePair<string, string>("form_id", value2));
                        }

                        string value3 = document.DocumentNode.SelectSingleNode("//input[@type='hidden' and @name='myaccount_check']").Attributes["value"].Value;
                        if (!string.IsNullOrEmpty(value3))
                        {
                            keyValues.Add(new KeyValuePair<string, string>("myaccount_check", value3));
                        }


                        utilityviewmodel.login_attempted = true;
                        // Hook up the next routine - if the login is true - for the next 'Document Completed' delivery
                        utilityviewmodel.next_routine = "HOME";
                        if (TextBox_Active)
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                ourviewmodel,
                                "LogOnAttempted");
                        }
                        clicked = true;
                        break;
                    default:
                        break;
                }
                if (clicked)
                {
                    break;
                }
            }
            utilityviewmodel.keyValues = keyValues;
            // Ow! ow! ow!  There was a massive problem with EDF insofar
            // as when I had the "Click" up here, AS SOON AS IT WAS DONE,
            // the DocumentComplete was happening which brought us back to the
            // LOGIN routine, so we were hitting the "Login >>" TWICE
            // The program wasn't executing the code to set 'login_attempted' to 'true'
            // OR the code to unhook this routine as EventHandler and hook up
            // Account whatever as new EventHandler.  Don't even have time to start the timer ...
            // Hook up the next routine - if the login is true - for the next 'Document Completed' delivery
            //wb.EDF_Utility.AccountsOverview;

            // If our login was successful a 'secure' Document
            // will be delivered and we have told the handler
            // to call the SecureLogin routine below when that
            // happens
            // If our login was UNSUCCESSFUL, then no Document
            // will be delivered and the timer will tick away
            // the 10 seconds and exit the timer loop when it
            // reaches  0
            return clicked;
        }

        private static void EDF_AccountOverview(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode> HtmlCol;

            string my_account = "MyAccount";
            bool clicked = false;

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                // The matching criteria ...
                if (string.IsNullOrEmpty(element.Id))
                {
                    string name = element.GetAttributeValue("name", "");
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (name.Contains("67"))
                        {
                            if (!string.IsNullOrEmpty(element.InnerText))
                            {
                                string inner_text = element.InnerText.Replace(Environment.NewLine, "").Trim();

                                // All you've got at this point ... is an Account No and nothing else
                                utilityviewmodel.account_no = inner_text;

                                // Doesn't matter about the MPAN/MPRN - only ever do one of each 'E' or 'G'
                                // Ignore the MPAN/MPRN even though the original code includes it?
                                List<Amelia> amelia_found = SmartUtilityV2022.Amelia_Lookup(ourviewmodel, utilityviewmodel, false);
                                if (amelia_found.Count == 0)
                                {
                                    SmartUtilityV2022.Amelia_Add(utilityviewmodel);

                                    // Well - truth be told - there may be
                                    // no Utility.Accounts, so this will never execute
                                    // here. We will then 'Logout' with utilityviewmodel.login_finished
                                    // never set to true, even though we had a good Login.
                                    // So 'No Utility.Accounts' = 'Login Failed' because we can't
                                    // really tell the difference (if a REAL login fails,
                                    // then you don't get to see any Utility.Accounts either ...
                                    // Maybe if we can find the Logout button, then this
                                    // is a good indication we DID - in fact - log in?
                                    // we wouldn't be HERE unless we were logged in successfully
                                    // Set this flag as were are 99.9% sure that we are logged-in
                                    // NO its set to true when we have a good SUPPLY ADDRESS

                                    // Hook up the next routine for the next 'Document Completed' delivery
                                    utilityviewmodel.next_routine = "EDFCONTACT";
                                    // Can add this one in and click it ...
                                    clicked = true;
                                    // All clicks go at the end, now
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (!clicked)
            {
                // So there are no more Utility.Accounts?
                // Soft Logout - well.. there is no "Logoff" on the Overview
                // page - brilliant, eh?  But if you clicl "MyAccount" then
                // that seems to log you off.  I am not arguing, I am not going
                // to wail or moan or gnash my teeth about these EDF cunts,
                // I am just going to use what is given to me ....
                // Fucking EDF NUMBSKULLS!!
                HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
                foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
                {
                    // The matching criteria ...
                    if (string.IsNullOrEmpty(element.Id))
                    {
                        if (!string.IsNullOrEmpty(element.InnerText))
                        {
                            if (element.InnerText == my_account)
                            {
                                // Hook up the next routine for the next 'Document Completed' delivery
                                utilityviewmodel.next_routine = "LOGOUT";
                                // All these go at the end, now
                                clicked = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return;
        }

        private static void EDF_ContactDetails(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode> HtmlCol;

            string name,
                    inner_text,
                    view_or_pay_bills = "Account details"; // "View or pay bills";
            bool clicked = false;

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//form");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                // The matching criteria ...
                if (string.IsNullOrEmpty(element.Id))
                {
                    name = element.GetAttributeValue("name", "");
                    if (!string.IsNullOrEmpty(name))
                    {
                        // Look for either of these two key words
                        if ((name.Contains("electricity")) ||
                            (name.Contains("gas")))
                        {
                            utilityviewmodel.resource_code = Convert.ToChar(name.ToUpper());
                            SmartUtilityV2022.Fix_Resource_Type(utilityviewmodel, SmartUtilityV2022.Determine_Resource_Type(utilityviewmodel));   // For the time being
                            break;  // Only the first
                        }
                    }
                }
            }

            HtmlAgilityPack.HtmlNode account_element = document.GetElementbyId("accountinfo");
            if (account_element != null)
            {
                if (!string.IsNullOrEmpty(account_element.InnerText))
                {
                    // Take a copy - clear down all \r
                    inner_text = account_element.InnerText.Replace(Environment.NewLine, "").Trim();
                    // Reduce \n\n down to \n
                    //inner_text = inner_text.Replace("\n\n", "\n");
                    // All you've got at this point ... is a line
                    //string account_postcode = innertext.Trim(' ');
                    if (SmartNibbyV2016.Derive_Postcode_New(inner_text,
                                                    ourviewmodel.Blanche.workingPostcodesList,
                                                    utilityviewmodel))
                    {
                        utilityviewmodel.login_finished = true;

                        // Ignore the MPAN/MPRN even though the original code includes it?
                        List<Amelia> amelia_found = SmartUtilityV2022.Amelia_Lookup(ourviewmodel, utilityviewmodel, false);
                        if (amelia_found.Count > 0)
                        {
                            foreach (Amelia amelia_row in amelia_found)
                            {
                                amelia_row.POSTCODE = utilityviewmodel.postcode;
                                amelia_row.AREA_CODE = utilityviewmodel.area_code;
                            }
                        }
                    }
                }
            }

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                if (string.IsNullOrEmpty(element.Id))
                {
                    if (!string.IsNullOrEmpty(element.InnerText))
                    {
                        inner_text = element.InnerText.Replace(Environment.NewLine, "").Trim();
                        if (inner_text == view_or_pay_bills)
                        {
                            // Hook up the next routine for the next 'Document Completed' delivery
                            utilityviewmodel.next_routine = "ACCDETLS";
                            // All clicks go at the end, now
                            clicked = true;
                            break;
                        }
                    }
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return;
        }

        private static void EDF_AccountDetails(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            // Fuck me!  It fucking WORKED!!!! See the re-direction to this 'secure'
            // routine from the initial fucking Login routine above!!!!!!!!!!!!
            // You ARE a fucking genius Ray!!  An absolute bravest of the brave
            // fucking genius.  Those visionless cunts you saw last Thursday have
            // ABSOLUTELY no idea what they are missing out on ....

            IList<HtmlAgilityPack.HtmlNode> HtmlCol;

            string[]
                    lines;
            string class_name,
                    inner_text,
                    product = "Product",
                    payment_plan = "Payment plan";
            int line_count;
            bool clicked = false;

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                // The matching criteria ...
                if (string.IsNullOrEmpty(element.Id))
                {
                    class_name = element.GetAttributeValue("class", "");
                    if (SmartNibbyV2016.Check_Classname(class_name, "tablefields", false)) // Somewhere in there 
                    {
                        if (!string.IsNullOrEmpty(element.InnerText))
                        {
                            // Take a copy - clear down all \r
                            inner_text = element.InnerText.Replace(Environment.NewLine, "").Trim();
                            // Reduce \n\n down to \n
                            //innertext = innertext.Replace("\n\n", "\n");
                            // All you've got at this point ... is a line
                            //innertext = innertext.Trim(' ');
                            // Split up into lines
                            lines = inner_text.Split('\n');
                            line_count = 0;
                            while (line_count < lines.Length)
                            {
                                // Look for the Tariff and Product Plan
                                lines[line_count] = lines[line_count].Trim(' ');
                                if (lines[line_count].IndexOf(product) >= 0)
                                {
                                    utilityviewmodel.edf_tariff_name = lines[line_count].Replace(product, "").Trim();
                                    utilityviewmodel.TARIFF_NAME = SmartUtilityV2022.Filter_Tariffname(true, utilityviewmodel.edf_tariff_name);
                                    if (string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME))
                                    {
                                        utilityviewmodel.TARIFF_NAME = "Standard";
                                    }
                                    if (!SmartSpikeUtilityV2017.Utility_Lookup_TariffCode(utilityviewmodel,
                                                                            utilityviewmodel.brand_code,
                                                                            utilityviewmodel.resource_code, //utilityviewmodel.resource_type,
                                                                            SmartUtilityV2022.Determine_Resource_Type(utilityviewmodel)))
                                    //utilityviewmodel.TARIFF_NAME,
                                    //utilityviewmodel.TARIFF_CODE))
                                    //utilityviewmodel.tariff_codes_namesList))
                                    {
                                        // One of the very few occasions when we need to tell HQ something
                                        ourviewmodel.errorMessage = utilityviewmodel.area_code + SmartParametersV2016.space + utilityviewmodel.supplier_code + SmartParametersV2016.space + utilityviewmodel.resource_code + SmartParametersV2016.space + utilityviewmodel.TARIFF_NAME + " not found";
                                    }
                                }
                                else
                                {
                                    if (lines[line_count].IndexOf(payment_plan) >= 0)
                                    {
                                        utilityviewmodel.payment_name = lines[line_count].Replace(payment_plan, "").Trim(' ');
                                        if (string.IsNullOrEmpty(utilityviewmodel.payment_name))
                                        {
                                            utilityviewmodel.payment_name = "Unspecified";
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME) &&
                                    !string.IsNullOrEmpty(utilityviewmodel.payment_name))
                                {
                                    line_count = lines.Length;
                                }
                                line_count++;
                            }
                            break;
                        }
                    }
                }
            }

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                if (string.IsNullOrEmpty(element.Id))
                {
                    if (!string.IsNullOrEmpty(element.InnerText))
                    {
                        inner_text = element.InnerText.Replace(Environment.NewLine, "").Trim();
                        // Sure this is right?  This is the same as the previous routine???
                        if (inner_text == "View or pay bills")
                        {
                            // Hook up the next routine for the next 'Document Completed' delivery
                            utilityviewmodel.next_routine = "VIEWBILLS";
                            // All clicks go at the end, now
                            clicked = true;
                            break;
                        }
                    }
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
        }

        private static void EDF_ViewBills(HtmlAgilityPack.HtmlDocument document,
                                                UtilityViewModel utilityviewmodel)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol;

            string inner_text;
            bool clicked = false;

            HtmlCol = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element in HtmlCol)
            {
                if (string.IsNullOrEmpty(element.Id))
                {
                    if (!string.IsNullOrEmpty(element.InnerText))
                    {
                        inner_text = element.InnerText.Replace(Environment.NewLine, "").Trim();
                        if (inner_text == "View my previous bills")
                        {
                            // Hook up the next routine for the next 'Document Completed' delivery
                            utilityviewmodel.next_routine = "VWPRVBILLS";
                            // All clicks go at the end, now
                            clicked = true;
                            break;
                        }
                    }
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return;
        }

        private static void EDF_ViewPreviousBills(HtmlAgilityPack.HtmlDocument document,
                                                        DateTime time_now,
                                                        UtilityViewModel utilityviewmodel)
        {
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol1,
                    HtmlCol2;
            DateTime today = time_now;  // Local time
            string inner_text,
                    name;
            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                // Here we set the value in their fucking stupid dropdown
                // if (element1.GetAttribute("name") == "vb_period")
                name = element1.GetAttributeValue("name", "");
                switch (name)
                {
                    case "vb_fromdate":
                        element1.SetAttributeValue("value", today.AddYears(-2).ToString(SmartParametersV2016.standardFormat));
                        break;
                    case "vb_todate":
                        element1.SetAttributeValue("value", today.ToString(SmartParametersV2016.standardFormat));
                        // Then we have to 'find' our value in their fucking stupid Continue button
                        // element1.SetAttribute("value", "last2year");
                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                        {
                            if (string.IsNullOrEmpty(element2.Id))
                            {
                                if (!string.IsNullOrEmpty(element2.InnerText))
                                {
                                    inner_text = element2.InnerText.Replace(Environment.NewLine, "").Trim();
                                    if (inner_text == "Search »")
                                    {
                                        // Hook up the next routine for the next 'Document Completed' delivery
                                        utilityviewmodel.next_routine = "VWLSTBILLS";
                                        clicked = true;
                                        break;
                                        // All clicks go at the end, now
                                    }
                                }
                            }
                        }
                        break;  // Leave switch
                    default:
                        break;
                }
                if (clicked)
                {
                    break;  // Leave outer foreach loop
                }
            }
            if (!clicked)
            {
                utilityviewmodel.next_routine = "LOGOUT";
            }
            return;
        }

        private static async Task<bool> EDF_ViewListBills(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    HtmlAgilityPack.HtmlDocument document,
                                                    bool TextBox_Active)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2;
            string //target_string,
                        inner_text,
                        //parenth_quote = "('",
                        //comma_quotes = "',''",
                        my_energy_accounts = "My energy Utility.Accounts";
            //int currency_index,
            //            pq_index,
            //            comma_index;
            bool clicked = false;

            // I spent FOUR FUCKING days trying to figure out why the fucking
            // GetElementsByTagName function wouldn't work on this returned document.
            // F-O-U-R  F-U-C-K-I-N-G  D-A-Y-S  of tearing my hair out and getting
            // really, really, totally and utterly depressed at the thought of coming
            // SO FAR and failing at one of the (almost) last hurdles.  I was nigh
            // in suicidal (seriously) and in tears over this problem.  After wasting
            // FOUR DAYS scouring the Internet for possible soultions, I did a test
            // and found out that becuase I had wb.ScriptErrorsSuppressed = true;
            // and because there were SO MANY script errors in the Document under
            // consideration because EDF's HTML mark-up was so poor, there was no
            // way GetElementsByTagName was ever going to process the Document
            // correctly.  Of course it gave up in the face of such overwhelming
            // odds ... and as for me?  I was in tears of rage, frustration and
            // despair ...  If anyone EVER offers you any money to think about
            // buying you out Ray, just remember all the pain and heartache you went
            // through JUST to get even here!!!  How solving the EON and EDF problems
            // left you completely and utterly physically and mentally exhausted.
            // You have worked miracles so far, and have nothing to be ashamed of,
            // ever, and Everything to be proud of.
            // You have worked wonders; you are definitely the best systems
            // programmer in England without doubt, and probably the best in Europe
            // and perhaps even the World.  Old Shit-for-Brains 1 wouldn't even
            // lick your boot soles on this one, sunbeam ... its absoultely magic;
            // a miracle; superb; the dogs absolute bollcks of software ...

            // I don't have to have any of this bollcks in the line below whatsoever
            // wb.ScriptErrorsSuppressed = false;
            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//table");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string class_name = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(class_name, "tablefields_tc", true))
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, "*");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        if (!string.IsNullOrEmpty(element2.InnerText))
                        {
                            inner_text = element2.InnerText.Replace(Environment.NewLine, "").Trim();
                            if (!await Viewbills_Case(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    element2,
                                                    inner_text,
                                                    TextBox_Active))
                            {
                                clicked = false;
                                goto quit_viewbills;
                            }


                            //                            string webpage_id = "";
                            //                              switch (element2.Name)  // <= Tag 
                            //                            {
                            //                                case "tr":
                            //                                    currency_index = inner_text.IndexOf(utilityviewmodel.bill_currency_symbol);
                            //                                    if (currency_index >= 0)
                            //                                    {
                            //                                        webpage_id = inner_text.Substring(0, currency_index);
                            //                                    }
                            //                                    break;
                            //                                case "a":
                            //                                    // Are all these conditions true?
                            //                                    utilityviewmodel.statment_id = webpage_id;
                            //                                   if (SmartUtilityV2022.Check_Bill_Date(utilityviewmodel))
                            //                                   {
                            //                                        bool bills_out = true,
                            //                                            bills_resource_out = true;
                            //                                        // Need to Add BILL_DATE to these two lookups!!!!
                            //                                        if (!SmartUtilityV2022.Bill_Already_Done(username, utilityviewmodel, rf bills_out, rf bills_resource_out))
                            //                                        {


                            //                                            // Decode the Uri for the PDF
                            //                                            target_string = element2.OuterHtml.ToString();
                            //                                            pq_index = target_string.IndexOf(parenth_quote);
                            //                                            if (pq_index >= 0)
                            //                                            {
                            //                                                pq_index = pq_index + 2;  // Length of ('
                            //                                                // Remove the leading Ba!
                            //                                                target_string = target_string.Substring(pq_index, target_string.Length - pq_index);
                            //                                                comma_index = target_string.IndexOf(comma_quotes);
                            //                                                if (comma_index >= 0)
                            //                                                {
                            //                                                    // Get rid of the trailing Ba!
                            //                                                    target_string = target_string.Substring(0, comma_index);
                            //                                                    // Get rid of the "amp;"s
                            //                                                    target_string = target_string.Replace("amp;", "");

                            //                                                    Uri TargetUrl = new Uri("https://my-account.edfenergy.com");
                            //                                                    string uri_message = "";
                            //                                                    if (!SmartUtilityV2022.Create_Uri("https://my-account.edfenergy.com",
                            //                                                                                        target_string,
                            //                                                                                        rf TargetUrl,
                            //                                                                                        rf uri_message))
                            //                                                    {
                            //                                                        ourviewmodel.errorMessage = current_routine + "|" + uri_message;
                            //                                                        return false;
                            //                                                    }
                            //                                                    // Get the fucking PDF
                            //                                                    PdfReader pdfreader = await SmartBobV2017.HTTP_GET_PDF_ASYNC(utilityviewmodel.connection_timeout,
                            //                                                                                                            useProxy,
                            //                                                                                                            guid,
                            //                                                                                                            username,
                            //                                                                                                            website,
                            //                                                                                                            //utilityviewmodel.brand_code,
                            //                                                                                                            //utilityviewmodel.supplier_code,
                            //                                                                                                            TargetUrl,
                            //                                                                                                            user_agent,
                            //                                                                                                            time_now,
                            //                                                                                                            yymmdd_format,
                            //                                                                                                            setMessage);
                            //#if WINFORMS
                            //                                                    if (SmartUtilityV2022.Generic_Close_1(
                            //#Xelse
                            //                                                    if (await SmartUtilityV2022.Generic_Close_1(
                            //#endif
                            //                                                                                timeout,
                            //                                                                                username,
                            //                                                                                website,
                            //                                                                                //time_now,
                            //                                                                                pdfreader,
                            //                                                                                utilityviewmodel,

                            //#endif
                            //                                                    {
                            //                                                        if (!await EDF_parse_bill(timeout,
                            //                                                                                username,
                            //                                                                                website,
                            //                                                                                p => MES = p,
                            //                                                                                () => utilityviewmodel,
                            //                                                                                pdfreader))
                            //                                                        {
                            //#if WINFORMS
                            //                                                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,  " Parse failed: " + utilityviewmodel.account_no + SmartParametersV2016.space +
                            //                                                                                                                    statement_date + SmartParametersV2016.space +
                            //                                                                                                                    utilityviewmodel.bill_date + SmartParametersV2016.space +
                            //                                                                                                                    utilityviewmodel.account_type + SmartParametersV2016.space +
                            //                                                                                                                    ourviewmodel.errorMessage + Environment.NewLine.ToString());
                            //#Xelse
                            //                                                            if (utilityviewmodel.examine.scrape) {await SmartBobV2017.ListenerAsync(ourviewmodel.Username,timespanTimeout, username, website, utilityviewmodel.brand_code, utilityviewmodel.supplier_code, ourviewmodel.errorMessage); }
                            //#endif

                            //                                                        }
                            //                                                        else
                            //                                                        {
                            //                                                            // Only add it in if we successfully created it
                            //                                                            if (bills_out)
                            //                                                            {
                            //                                                                utilityviewmodel.bills_changesList.Add(utilityviewmodel.bills_row);
                            //                                                            }
                            //                                                            if (bills_resource_out)
                            //                                                            {
                            //                                                                utilityviewmodel.bills_resource_changesList.Add(utilityviewmodel.bills_resource_row);
                            //                                                            }
                            //                                                        }
                            //                                                    }

                            //                                                    // After parsing, decide what to do with the PDF
                            //                                                    await SmartUtilityV2022.Generic_Close_2(timeout,
                            //                                                                                            username,
                            //                                                                                            website,
                            //                                                                                            time_now,
                            //                                                                                            statement_date,
                            //                                                                                            pdfreader,
                            //                                                                                            utilityviewmodel,
                            //                                                                                            TextBox_Active,
                            //                                                                                            textBoxBrowser,
                            //                                                                                            scrollViewer);
                            //
                            //                                                    // Close the reader down (at last) here
                            //                                                    if (pdfreader != null)
                            //                                                    {
                            //                                                        pdfreader.Close();
                            //                                                    }
                            //                                                    // End of COMMON PART
                            //                                                }
                            //                                            }
                            //                                        }
                            //                                    }
                            //                                    break;
                            //                                default:
                            //                                    break;
                            //                            }
                        }
                    }
                }
            }

            // So script errors don't popup and spoil the show ... We no longer have 
            // to worry about this bollcks
            //wb.ScriptErrorsSuppressed = true;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//a");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                if (string.IsNullOrEmpty(element1.Id))
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        inner_text = element1.InnerText.Replace(Environment.NewLine, "").Trim();
                        if (inner_text == my_energy_accounts)
                        {
                            // Hook up the next routine for the next 'Document Completed' delivery
                            utilityviewmodel.next_routine = "HOME";
                            // All clicks go at the end, now
                            clicked = true;
                            break;
                        }
                    }
                }
            }

        quit_viewbills:
            return clicked;
        }

        private static async Task<bool> Viewbills_Case(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif  
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    HtmlAgilityPack.HtmlNode element2,
                                                    string inner_text,
                                                    bool TextBox_Active)
        {
            string parenth_quote = "('",
                        comma_quotes = "',''";

            string webpage_id = "";
            switch (element2.Name)  // <= Tag 
            {
                case "tr":
                    int currency_index = inner_text.IndexOf(utilityviewmodel.bill_currency_symbol);
                    if (currency_index >= 0)
                    {
                        webpage_id = inner_text.Substring(0, currency_index);
                    }
                    break;
                case "a":
                    // Are all these conditions true?
                    utilityviewmodel.statement_id = webpage_id;
                    if (SmartUtilityV2022.Check_Bill_Date(utilityviewmodel))
                    {
                        utilityviewmodel.bills_out = true;
                        utilityviewmodel.bills_resource_out = true;
                        string mpan_mprn = "";
                        switch (utilityviewmodel.resource_code)
                        {
                            case SmartParametersV2016.Electricity:
                                mpan_mprn = utilityviewmodel.sparks.MPAN;
                                break;
                            case SmartParametersV2016.Gas:
                                mpan_mprn = utilityviewmodel.smell.MPRN;
                                break;
                            default:
                                break;
                        }
                        // Need to Add BILL_DATE to these two lookups!!!!
                        if (!SmartUtilityV2022.Bill_Already_Done(utilityviewmodel, mpan_mprn))
                        {
                            // Decode the Uri for the PDF
                            string target_string = element2.OuterHtml;
                            int pq_index = target_string.IndexOf(parenth_quote);
                            if (pq_index >= 0)
                            {
                                pq_index += 2;  // Length of ('
                                                // Remove the leading Ba!
                                target_string = target_string.Substring(pq_index, target_string.Length - pq_index);
                                int comma_index = target_string.IndexOf(comma_quotes);
                                if (comma_index >= 0)
                                {
                                    // Get rid of the trailing Ba!
                                    target_string = target_string.Substring(0, comma_index);
                                    // Get rid of the "amp;"s
                                    target_string = target_string.Replace("amp;", "");

                                    Uri TargetUrl = new Uri("https://my-account.edfenergy.com");
                                    if (!SmartNibbyV2016.Create_Uri(ourviewmodel,
                                                                        TargetUrl.ToString(),
                                                                        target_string))
                                    {
                                        ourviewmodel.errorMessage = utilityviewmodel.current_routine + "|" + ourviewmodel.errorMessage;
                                        return false;
                                    }
                                    TargetUrl = ourviewmodel.TargetUrl;
                                    // Start with a clean sheet ...
                                    ourviewmodel.pdfMessage = "";
                                    // Get the fucking PDF
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                                    iText.Kernel.Pdf.PdfReader pdfreader =
#endif
#if ANDROIDX 
                                    iText.Kernel.Pdf.PdfReader pdfreader =
#endif
                                        await SmartBobV2017.HTTPCLIENT_GET_PDF_ASYNC(ourviewmodel,
                                                                                    utilityviewmodel.utilityToken,
                                                                                    TargetUrl,
                                                                                    utilityviewmodel.guid);
                                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                                        !string.IsNullOrEmpty(ourviewmodel.pdfMessage))
                                    {
                                        return false;
                                    }

                                    if (!await SmartPDFV2019.Generic_Close_1(
                                                                            ourviewmodel,

                                                                            utilityviewmodel,
#if ANDROIDX
                                                                            meterActivity,
#endif
                                                                            pdfreader,
                                                                            TextBox_Active))
                                    {
                                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        if (!await EDF_parse_bill(ourviewmodel,
                                                                utilityviewmodel,
                                                                pdfreader))
                                        {
#if WINFORMS
                                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, " Parse failed: " + utilityviewmodel.account_no + SmartParametersV2016.space +
                                                                                                                    utilityviewmodel.statement_id + SmartParametersV2016.space +
                                                                                                                    utilityviewmodel.bill_date + SmartParametersV2016.space +
                                                                                                                    utilityviewmodel.account_type + SmartParametersV2016.space +
                                                                                                                    ourviewmodel.errorMessage + Environment.NewLine.ToString());
#endif
#if WPF 
                                            await SmartUtilityV2022.Check_Examine(ourviewmodel, utilityviewmodel);
                                            //if (utilityviewmodel.examine.scrape)
                                            //{
                                            //    await SmartBobV2017.ListenerAsync(ourviewmodel.Username,timespanTimeout, username, website, utilityviewmodel.brand_code, utilityviewmodel.supplier_code, ourviewmodel.errorMessage);
                                            //}
#endif
#if ANDROIDX
                                            await SmartUtilityV2022.Check_Examine(ourviewmodel, utilityviewmodel);
                                            //if (utilityviewmodel.examine.scrape)
                                            //{
                                            //    await SmartBobV2017.ListenerAsync(ourviewmodel.Username,timespanTimeout, username, website, utilityviewmodel.brand_code, utilityviewmodel.supplier_code, ourviewmodel.errorMessage);
                                            //}
#endif
                                        }
                                        else
                                        {
                                            // Only add it in if we successfully created it
                                            
                                            // YOU NEED TO PUT THIS BACK!!
                                            SmartScraperV2016.Do_The_Bills(utilityviewmodel);
                                            
                                            
                                            
                                            //if (bills_out)
                                            //{
                                            //    utilityviewmodel.bills_changesList.Add(utilityviewmodel.bills_row);
                                            //}
                                            //if (bills_resource_out)
                                            //{
                                            //    utilityviewmodel.bills_resource_changesList.Add(utilityviewmodel.bills_resource_row);
                                            //}
                                        }
                                    }

                                    // After parsing, decide what to do with the PDF
                                    await SmartPDFV2019.Generic_Close_2(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        pdfreader,
#if ANDROIDX
                                                                        meterActivity,
#endif
                                                                        TextBox_Active);
                                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                    // Close the reader down (at last) here
                                    //if (pdfreader != null)
                                    //{
#if WINFORMS || WPF
                                    pdfreader?.Close();
#endif
                                    //}
                                    // End of COMMON PART
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        internal static async Task<bool> EDF_parse_bill(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
//#if WINFORMS || WPF  || WINUI
                                                    iText.Kernel.Pdf.PdfReader pdfreader)
//#endif
//#if ANDROIDX 
//                                                    iText.Kernel.Pdf.PdfReader pdfreader)
//#endif
        {
            SmartParseV2016.Initialize_Bill_Parse(ourviewmodel, utilityviewmodel);
            bool status = true;
            string strText_simple = "";

            strText_simple = SmartPDFV2019.TurnPdfToText(ourviewmodel, utilityviewmodel, pdfreader);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }

            if (!SmartParseV2016.Check_Big_Four(ourviewmodel, utilityviewmodel, false))
            {
                string[] lines = strText_simple.Split(SmartParametersV2016.newline).Where(r => !string.IsNullOrWhiteSpace(r)).ToArray();

                utilityviewmodel.sections = new[]{
                                        new string[] { "A", "0", "", "", "0", "-1" },
                                        new string[] { "B", "0", "", "Electricity statement" + "|" +
                                                                "Gas statement" + "|" +
                                                                "Electricity statement - estimated" + "|" +
                                                                "Gas statement - estimated", "-1", "-1" },
                                        new string[] { "B", "1", "", "Before this statement", "-1", "-1" },
                                        new string[] { "B", "2", "", "On this statement", "-1", "-1" },
                                        new string[] { "B", "3", "", "VAT", "-1", "-1" },
                                        new string[] { "B", "4", "", "Your new balance is", "-1", "-1" },
                                        new string[] { "C", "0", "", "About your tariff", "-1", "-1" },
                                        new string[] { "H", "0", "", "Meter readings", "-1", "-1" },
                                        new string[] { "H", "1", "E", "Electricity readings", "-1", "-1" },
                                        new string[] { "H", "2", "G", "Gas readings", "-1", "-1" },
                                        new string[] { "H", "3", "E", "Electricity charges", "-1", "-1" },
                                        new string[] { "H", "4", "G", "Gas charges", "-1", "-1" },
                                        new string[] { "H", "5", "", "Total charges", "-1", "-1" }
                                      };
                // This is always needed anyway because we always do Page1 with this
                // Sometimes ... the Bill Date is never extracted from the PDF text!
                // But when (if) we evert find it - we substitute the Bill Number for it
                // IF we haven't found the Bill number ...
                bool success = Parse_Page0(ourviewmodel,
                                                utilityviewmodel,
                                                utilityviewmodel.sections,
                                                lines);
                if (!success ||
                    !SmartParseV2016.Check_Big_Four(ourviewmodel, utilityviewmodel, true))
                {
                    status = false;
                    // Noisy fucking bitch is banging and crashing and thumping and
                    // stomping around the kitchen again.  She is NEVER quiet. NEVER
                    //                                                         =====
                }
                else
                {
                    if (!await Parse_Pages(ourviewmodel,
                                            utilityviewmodel,
                                            utilityviewmodel.sections,
                                            lines))
                    {
                        status = false;
                    }
                }
            }
            if (status)
            {
                // This is the only sensible place to do this because sometimes
                // they come BEFORE a total ... and sometime they come AFTER...
                SmartParseV2016.Update_Tariff_Details(ourviewmodel, utilityviewmodel);
                // fucking bitch is talking about her fucking job AGAIN
                // i don't fucking care about fucking verity or fucking aileish
            }
            return status;
        }

        //            int start_page = 0,
        //                next_page = 0;
        //            if (reader.NumberOfPages > 3)
        //            {
        //                start_page = 2;
        //            }
        //            else
        //            {
        //                start_page = 1;
        //            }
        //            for (int page = start_page; page <= reader.NumberOfPages; page++)
        //            {
        //                ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
        //                strText = PdfTextExtractor.GetTextFromPage(reader, page, its)//;
        //
        //                switch (next_page)
        //                {
        //                    case 0:
        //                        try
        //                        {
        //                            Parse_Page1(username,
        //                                    page,
        //                                    strText,
        //                                    utilityviewmodel,
        //                                    SmartParametersV2016.defaultDates,
        //                                    utilityviewmodel.resource_code,
        //                                    utilityviewmodel.area_code,
        //                                    utilityviewmodel.brand_code,
        //                                    utilityviewmodel.account_no, // In case it gets set inside
        //                                    utilityviewmodel.payment_name,
        //                                    utilityviewmodel.bill_token,
        //                                    utilityviewmodel.statement_id,
        //                                    utilityviewmodel.ameliaList,
        //                                    utilityviewmodel.Utility.Accounts_changesList,
        //                                    utilityviewmodel.bills_changesList,
        //                                    utilityviewmodel.TariffDetails_changesList,
        //                                    utilityviewmodel.payment_plansList,
        //                                    rf BILL_DATE,
        //                                    rf STATEMENT_ID,
        //                                    rf BILL_PERIOD_END,
        //                                    rf VOLUMECORRECTION,
        //                                    rf CALORIFIC_VALUE);
        //                        }
        //                        catch (Exception exception)
        //                        {
        //                            ourviewmodel.errorMessage = exception.Message;
        //                        }
        //                        finally
        //                        { }
        //                        break;
        //                    case 1:
        //                        // She is WITHOUT DOUBT the NOISIEST FUCKING IDIOTIC MORON BITCH **EVER** to read Earth
        //                        // Ignorant, rude, stupid, aggressive, obnoxious, boring, sanctimonious, shallow, selfish.

        //                        try
        //                        {
        //                            parse_page2(username,
        //                                    page,
        //                                    strText,
        //                                    utilityviewmodel,
        //                                    BILL_DATE,
        //                                    STATEMENT_ID,
        //                                    BILL_PERIOD_END,
        //                                    VOLUMECORRECTION,
        //                                    CALORIFIC_VALUE);
        //                        }
        //                        catch (Exception exception)
        //                        {
        //                            ourviewmodel.errorMessage = exception.Message;
        //                        }
        //                        finally
        //                        { }
        //                        break;
        //                    default:
        //                        // Only do two pages
        //                        break;
        //                }
        //                next_page = next_page + 1;
        //            }

        private static bool Parse_Page0(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string[][] sections,
                                        string[] lines)
        {
            // Not perfect but not a bad start
            bool status = true;

            // Very Important Routine!!!
            SmartParseV2016.Sort_Sections(false, utilityviewmodel, lines);

            // Pass 1 - look for the Big Three in Section 0
            if (!Parse_SectionA0(utilityviewmodel, sections, "A", "0"))
            {
                ourviewmodel.errorMessage = "A0" + SmartParametersV2016.bar + ourviewmodel.errorMessage + ourviewmodel.UserName;
                return status;
            }
            return status;
        }
        private static bool Parse_SectionA0(UtilityViewModel utilityviewmodel,
                                            string[][] sections,
                                            string section,
                                            string sub_section)
        {
            utilityviewmodel.currentIndex = 0;
            //int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                return true;
            }
            return true;
        }

        private static async Task<bool> Parse_SectionH1(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string[][] sections,
                                                        string section,
                                                        string sub_section,
                                                        string[] lines)
        {
            // Not perfect but not a bad start
            bool status = false;

            utilityviewmodel.currentIndex = 0;
            //int section_index = 0;
            if (!SmartParseV2016.Decode_Section(section, sub_section, sections, utilityviewmodel.resource_code.ToString(), utilityviewmodel))
            {
                // We are here because we don't want (or need) to do this section
                return true;    // Always - because we haven't had a CONVERSION error
            }

            if (lines.Length > 0)
            {
                if (utilityviewmodel.examine.scrape)
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel.supplier_code, utilityviewmodel.brand_code, SmartParametersV2016.Utility.ToString() + " " + ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                }
            }
            return status;
        }

        private static async Task<bool> Parse_Pages(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string[][] sections,
                                                    string[] lines)
        {
            // Not perfect but not a bad start

            if (!await Parse_SectionH1(ourviewmodel, utilityviewmodel, sections, "H", "1", lines)) // Electricity
            {
                ourviewmodel.errorMessage = "H1" + SmartParametersV2016.bar + ourviewmodel.errorMessage;
                return false;
            }
            return true;
        }
        //        private static bool Parse_Page1(string username,
        //                                        int page,
        //                                        string text,
        //                                        UtilityViewModel utilityviewmodel,

        //                                        string defaultDates,
        //                                        char resource_code,
        //                                        short area_code,
        //                                        short brand_code,
        //                                        rf string account_no,
        //                                        string payment_name,
        //                                        string bill_token,
        //                                        string bill_id,
        //                                        List<Amelia> ameliaList,
        //                                        List<SmartUtility.Accounts> Utility.Accounts_changesList,
        //                                        List<SmartUtility.Bills> bills_changesList,
        //                                        List<SmartUtility.PaymentPlans> payment_plansList,
        //                                        rf string BILL_DATE,
        //                                        rf string STATEMENT_ID,
        //                                        rf string BILL_PERIOD_END,
        //                                        rf string VOLUMECORRECTION,
        //                                        rf string CALORIFIC_VALUE)
        //        {
        //            // Not perfect but not a bad start
        //            String[]
        //                    lines,
        //                    words;
        //            int     TARIFF_CODE = 0,
        //                    NEW_CHARGES = 0,
        //                    RESOURCE_DISCOUNTS = 0,
        //                    RESOURCE_VAT_AMOUNT = 0;
        //            string SUPPLY_ACCOUNT_NO = "",
        //                    ACCOUNT_NO = "",
        //                    PAYMENT_PLAN = "",
        //                    DUAL_FUEL = "",
        //                    ECONOMY7 = "",
        //                    BILL_PERIOD_START = defaultDates,
        //                    PREVIOUS_BALANCE = "0",
        //                    PAYMENTS_RECEIVED = "0",
        //                    ACCOUNT_CHARGES_CREDITS = "0",
        //                    ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT = "0",
        //                    SUPPLY_CHARGES_CREDITS = "0",
        //                    SUPPLY_CHARGES_CREDITS_VAT_AMOUNT = "0",
        //                    BILL_VAT_AMOUNT = "0",
        //                    OUTSTANDING_BALANCE = "0",
        //                    TOTAL_NOW_DUE = "0",
        //                    PAYMENT_DUE_DATE = defaultDates,
        //                    PAYMENT_TYPE = "",
        //                    DIRECT_DEBIT_DATE = defaultDates,
        //                    LOYALTY_BONUS = "0",
        //                    CREDIT_DATE = defaultDates,
        //                    FIRST_YEAR_DISCOUNT = "0",
        //                    REWARDS = "n/a",
        //                    MPAN_MPRN = "";
        //            short   BILL_VAT_CODE = SmartParametersV2016.zeroRateVatCode,
        //                    RESOURCE_VAT_CODE = SmartParametersV2016.zeroRateVatCode;
        //            string token = "",
        //                    resource = "",
        //                    resource_number = "",
        //                    electricity = "Electricity ", // Note trailing space
        //                    gas = "Gas ",           // Note the trailing space
        //                    website = "www.edfenergy.com",
        //                    electricity_supply_number = "Your electricity supply number is:",
        //                    gas_meter_point_reference = "Your gas meter point reference number is:",
        //                    calorific_value = "Calorific Value: ",
        //                    VOLUMECORRECTION = "Temperature & Pressure: ",
        //                    please_pay = "Please pay ",
        //                    by = " by ",
        //                    your_new_account_balance = "Your new account balance";
        //            int index = 0,
        //                    word_count,
        //                    currency_index;

        //            switch (resource_code)
        //            {
        //                case SmartParametersV2016.Electricity:
        //                    resource = electricity;
        //                    resource_number = electricity_supply_number;
        //                    break;
        //                case SmartParametersV2016.Gas:
        //                    resource = gas;
        //                    resource_number = gas_meter_point_reference;
        //                    break;
        //                default:
        //                    break;
        //            }

        //            lines = text.Split('\n');
        //            while (index < lines.Length)
        //            {
        //                token = lines[index].Trim(' ');

        //                if (token.Length >= resource.Length)
        //                {
        //                    if (token.Substring(0, resource.Length) == resource)
        //                    {
        //                    }
        //                }

        //                if (token.Length >= website.Length)
        //                {
        //                    if (token.Substring(0, website.Length) == website)
        //                    {
        //                        if (index + 1 < lines.Length)
        //                        {
        //                            index = index + 1;
        //                            lines[index] = lines[index].Replace(" - ", SmartParametersV2016.space);
        //                            words = lines[index].Split(' ');
        //                            word_count = 0;
        //                            while (word_count < words.Length)
        //                            {
        //                                switch (word_count)
        //                                {
        //                                    case 0:
        //                                        BILL_DATE = words[word_count];
        //                                        if (BILL_DATE.Length < 2)
        //                                        {
        //                                            BILL_DATE = "0" + BILL_DATE;
        //                                        }
        //                                        BILL_DATE = BILL_DATE + "-";
        //                                        break;
        //                                    case 1:
        //                                        BILL_DATE = BILL_DATE + words[word_count] + "-";
        //                                        break;
        //                                    case 2:
        //                                        if (words[word_count].Length < 4)
        //                                        {
        //                                            BILL_DATE = BILL_DATE + "20" + words[word_count];
        //                                        }
        //                                        else
        //                                        {
        //                                            BILL_DATE = BILL_DATE + words[word_count];
        //                                        }
        //                                        break;
        //                                    case 3:
        //                                        BILL_PERIOD_START = words[word_count];
        //                                        if (BILL_PERIOD_START.Length < 2)
        //                                        {
        //                                            BILL_PERIOD_START = "0" + BILL_PERIOD_START;
        //                                        }
        //                                        BILL_PERIOD_START = BILL_PERIOD_START + "-";
        //                                        break;
        //                                    case 4:
        //                                        BILL_PERIOD_START = BILL_PERIOD_START + words[word_count] + "-";
        //                                        break;
        //                                    case 5:
        //                                        if (words[word_count].Length < 4)
        //                                        {
        //                                            BILL_PERIOD_START = BILL_PERIOD_START + "20" + words[word_count];
        //                                        }
        //                                        else
        //                                        {
        //                                            BILL_PERIOD_START = BILL_PERIOD_START + words[word_count];
        //                                        }
        //                                        break;
        //                                    case 6:
        //                                        BILL_PERIOD_END = words[word_count];
        //                                        if (BILL_PERIOD_END.Length < 2)
        //                                        {
        //                                            BILL_PERIOD_END = "0" + BILL_PERIOD_END;
        //                                        }
        //                                        BILL_PERIOD_END = BILL_PERIOD_END + "-";
        //                                        break;
        //                                    case 7:
        //                                        BILL_PERIOD_END = BILL_PERIOD_END + words[word_count] + "-";
        //                                        break;
        //                                    case 8:
        //                                        if (words[word_count].Length < 4)
        //                                        {
        //                                            BILL_PERIOD_END = BILL_PERIOD_END + "20" + words[word_count];
        //                                        }
        //                                        else
        //                                        {
        //                                            BILL_PERIOD_END = BILL_PERIOD_END + words[word_count];
        //                                        }
        //                                        break;
        //                                    default:
        //                                        break;
        //                                }
        //                                word_count = word_count + 1;
        //                            }

        //                            // If account number is empty fix it here
        //                            if (string.IsNullOrEmpty(account_no))
        //                            {
        //                                if (index + 1 < lines.Length)
        //                                {
        //                                    index = index + 1;
        //                                    ACCOUNT_NO = lines[index].Replace(SmartParametersV2016.space, "");
        //                                    //if (trace) { listener.WriteLine(" ACCOUNT_NO = " + ACCOUNT_NO, routine); }
        //                                    account_no = ACCOUNT_NO;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                if (token.Length >= resource_number.Length)
        //                {
        //                    if (token.Substring(0, resource_number.Length) == resource_number)
        //                    {
        //                        if (index + 1 < lines.Length)
        //                        {
        //                            switch (resource_code)
        //                            {
        //                                case SmartParametersV2016.Electricity:
        //                                    index = index + 1;
        //                                    //if (lines[index].IndexOf(calorific_value) >= 0)
        //                                    MPAN_MPRN = lines[index];
        //                                    index = index + 1;
        //                                    MPAN_MPRN = MPAN_MPRN + lines[index];
        //                                    index = index + 1;
        //                                    MPAN_MPRN = MPAN_MPRN + lines[index];
        //                                    MPAN_MPRN = MPAN_MPRN.Replace(SmartParametersV2016.space, "");
        //                                    break;
        //                                case SmartParametersV2016.Gas:
        //                                    index = index + 1;
        //                                    if (lines[index].IndexOf(calorific_value) >= 0)
        //                                    {
        //                                        utilityviewmodel.CALORIFIC_VALUE = lines[index].Substring(calorific_value.Length, lines[index].Length - calorific_value.Length);
        //                                    }
        //                                    index = index + 1;
        //                                    if (lines[index].IndexOf(VOLUMECORRECTION) >= 0)
        //                                    {
        //                                        VOLUMECORRECTION = lines[index].Substring(VOLUMECORRECTION.Length, lines[index].Length - VOLUMECORRECTION.Length);
        //                                        //VOLUMECORRECTION = "1.02";
        //                                    }
        //                                    index = index + 1;
        //                                    MPAN_MPRN = MPAN_MPRN + lines[index];
        //                                    MPAN_MPRN = MPAN_MPRN.Replace(SmartParametersV2016.space, "");
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            //if (trace) { listener.WriteLine(" MPAN_MPRN = " + MPAN_MPRN, routine); }
        //                            // Is the Amelia MPAN/MPRN 'Unknown'?
        //                            //SmartUtilityV2022.Utility.Accounts_update(username,
        //                            //                        trace,
        //                            //                        "Unknown",
        //                            //                        ameliaList,
        //                            //                        Utility.Accounts_changesList);
        //                        }
        //                    }
        //                }

        //                if (token.Length >= please_pay.Length)
        //                {
        //                    if (token.Substring(0, please_pay.Length) == please_pay)
        //                    {
        //                        token = token.Substring(please_pay.Length, token.Length - please_pay.Length);
        //                        if (token.IndexOf(by) != -1)
        //                        {
        //                            token = token.Replace(by, SmartParametersV2016.space);
        //                        }
        //                        currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol, 0);
        //                        if (currency_index >= 0)
        //                        {
        //                            token = token.Substring(currency_index, token.Length - currency_index);

        //                            words = token.Split(' ');
        //                            word_count = 0;
        //                            while (word_count < words.Length)
        //                            {
        //                                switch (word_count)
        //                                {
        //                                    case 0:
        //                                        if (words[word_count].Substring(0, 1) == utilityviewmodel.bill_currency_symbol)
        //                                        {
        //                                            TOTAL_NOW_DUE = words[word_count].Replace(utilityviewmodel.bill_currency_symbol, "");
        //                                        }
        //                                        break;
        //                                    case 1:
        //                                        PAYMENT_DUE_DATE = words[word_count];
        //                                        if (PAYMENT_DUE_DATE.Length < 2)
        //                                        {
        //                                            PAYMENT_DUE_DATE = "0" + PAYMENT_DUE_DATE;
        //                                        }
        //                                        PAYMENT_DUE_DATE = PAYMENT_DUE_DATE + "-";
        //                                        break;
        //                                    case 2:
        //                                        PAYMENT_DUE_DATE = PAYMENT_DUE_DATE + words[word_count] + "-";
        //                                        break;
        //                                    case 3:
        //                                        if (words[word_count].Length < 4)
        //                                        {
        //                                            PAYMENT_DUE_DATE = PAYMENT_DUE_DATE + "20" + words[word_count];
        //                                        }
        //                                        else
        //                                        {
        //                                            PAYMENT_DUE_DATE = PAYMENT_DUE_DATE + words[word_count];
        //                                        }
        //                                        break;
        //                                    default:
        //                                        break;
        //                                }
        //                                word_count = word_count + 1;
        //                            }
        //                        }
        //                    }
        //                }

        //                if (token.Length >= your_new_account_balance.Length)
        //                {
        //                    if (token.Substring(0, your_new_account_balance.Length) == your_new_account_balance)
        //                    {
        //                        currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol, 0);
        //                        if (currency_index >= 0)
        //                        {
        //                            OUTSTANDING_BALANCE = token.Substring(currency_index + 1, token.Length - currency_index - 1);
        //                        }
        //                    }
        //                }
        //                index = index + 1;
        //            }

        //            PAYMENT_TYPE = payment_name;
        //            if (SmartUtilityV2022.Lookup_Payment_Plan(rf PAYMENT_TYPE,
        //                                                        utilityviewmodel.PAYMENT_PLAN,
        //                                                        utilityviewmodel.payment_plansList,
        //                                                        rf ourviewmodel.errorMessage))
        //            {
        //                SmartParseV2016.Update_Bill("PAYMENT_PLAN", utilityviewmodel, utilityviewmodel.PAYMENT_PLAN);
        //                SmartParseV2016.Update_Bill("PAYMENT_TYPE", utilityviewmodel, PAYMENT_TYPE);
        //            }
        //            else
        //            {
        //                // utilityviewmodel.urgent message should be set here
        //                return false;
        //            }
        //            SmartUtilityV2022.Bills_Row_Update(utilityviewmodel.bills_row,
        //                                            username,
        //                                            utilityviewmodel.cubeface_code,
        //                                            brand_code,
        //                                            ACCOUNT_NO,
        //                                            STATEMENT_ID,
        //                                            BILL_DATE,
        //                                            BILL_PERIOD_START,
        //                                            BILL_PERIOD_END,
        //                                            PAYMENT_PLAN,
        //                                            ECONOMY7,
        //                                            PREVIOUS_BALANCE,
        //                                            PAYMENTS_RECEIVED,
        //                                            ACCOUNT_CHARGES_CREDITS,
        //                                            ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT,
        //                                            SUPPLY_CHARGES_CREDITS,
        //                                            SUPPLY_CHARGES_CREDITS_VAT_AMOUNT,
        //                                            BILL_VAT_AMOUNT,
        //                                            BILL_VAT_CODE,
        //                                            OUTSTANDING_BALANCE,
        //                                            TOTAL_NOW_DUE,
        //                                            PAYMENT_DUE_DATE,
        //                                            PAYMENT_TYPE,
        //                                            DIRECT_DEBIT_DATE,
        //                                            LOYALTY_BONUS,
        //                                            CREDIT_DATE,
        //                                            FIRST_YEAR_DISCOUNT,
        //                                            REWARDS);
        //            string RESOURCE_ACCOUNT_NO = utilityviewmodel.ACCOUNT_NO;
        //            SmartUtilityV2022.Bills_Resource_Row_Update(utilityviewmodel.bills_resource_row,
        //                                                    utilityviewmodel.cubeface_code,
        //                                                    utilityviewmodel.brand_code,
        //                                                    utilityviewmodel.ACCOUNT_NO,
        //                                                    utilityviewmodel.STATEMENT_ID,
        //                                                    utilityviewmodel.BILL_DATE,
        //                                                    utilityviewmodel.resource_code,
        //                                                    RESOURCE_ACCOUNT_NO,
        //                                                    TARIFF_CODE,        // WE don't check for TARIFF_CODE = 0 here ... Well we fucking should
        //                                                    NEW_CHARGES,
        //                                                    RESOURCE_DISCOUNTS,
        //                                                    RESOURCE_VAT_AMOUNT,
        //                                                    RESOURCE_VAT_CODE);
        //            return true;
        //        }

        //private static void parse_page2(string username,
        //                                    int page,
        //                                    string strText,
        //                                    UtilityViewModel utilityviewmodel,
        //                                    string BILL_DATE,
        //                                    string STATEMENT_ID,
        //                                    string BILL_PERIOD_END,
        //                                    string VOLUMECORRECTION,
        //                                    string CALORIFIC_VALUE)
        //{
        //    // Not perfect but not a bad start
        //    String[]
        //            lines,
        //            payments,
        //            words;
        //    string TARIFF_NAME = utilityviewmodel.tariff_name,
        //            PAYMENT_DATE = SmartParametersV2016.defaultDates,
        //            PAYMENT_AMOUNT = "0.00",
        //            PAYMENT_DUE = SmartParametersV2016.defaultDates,
        //            METER_SERIAL_NO = "",
        //            METER_TYPE = "",
        //            READINGS_PERIOD_END = SmartParametersV2016.defaultDates,
        //            READ_TYPE = "",
        //            READINGS_PERIOD_START = SmartParametersV2016.defaultDates,
        //            UNIT_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates,
        //            UNIT_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates,
        //            STANDING_CHARGES_PERIOD_START = SmartParametersV2016.defaultDates,
        //            STANDING_CHARGES_PERIOD_END = SmartParametersV2016.defaultDates,
        //            LAST_TYPE = "",
        //            D_THIS_READ = "0.0",
        //            D_LAST_READ = "0.0",
        //            D_UNITS_USEDX = "0",
        //            D_UNITS_USED_M3 = "0",
        //            D_UNITS_USED_KWH = "0",
        //            N_THIS_READ = "0",
        //            N_LAST_READ = "0",
        //            N_UNITS_USED = "0",
        //            KWH_UNITS_USED = "0",
        //            UNIT_OF_MEASURE = "",
        //            //KWHCONVERSION = "3.6",    // Standard (its nowhere on the bill)
        //            UNITS_RATE = "0",
        //            //SUPPLY_VAT              = "0",
        //            USAGE_TOTAL = "0",
        //            STANDING_CHARGE = "0",
        //            CHARGES_DAYS = "0",
        //            STANDING_TOTAL = "0",
        //            PREVIOUS_BALANCE = "0",
        //            PAYMENT_BALANCE = "0",
        //            PAYMENTS_RECEIVED = "0",
        //            E_NEW_CHARGES = "0",
        //            G_NEW_CHARGES = "0",
        //            VAT_TOTAL = "0",
        //            VAT_RATE = "0",
        //            VAT_AMOUNT = "0";

        //    string token = "",
        //            resource_charges = "",
        //            total_resource_charges = "",
        //            resource_meter_no = "",
        //            payment = "Payment ",   // Note the trailing space
        //            space_dash_space = " - ",        // Note the trailing space
        //            thank_you = "Thank you ", // Note the trailing space
        //            electricity_charges = "Electricity charges",
        //            gas_charges = "Gas charges",
        //            total_electricity_charges = "Total electricity charges",
        //            total_gas_charges = "Total gas charges",
        //            electricity_meter_no = "Electricity meter number: ", // Note trailing space
        //            gas_meter_no = "Gas meter number: ", // Note trailing space
        //            tariff = "Tariff: ",    // Note the trailing space
        //            to = " to ",
        //            at = " at",
        //            standing_charge = " Standing Charge ",
        //            days_at = " days at ",
        //            your_balance = "Your balance",
        //            vat_on = "VAT on ",    // Note the trailing space
        //            at_space = " at ";
        //    int index = 0,
        //            word_count,
        //            currency_index;
        //    short   PAYMENT_CODE = 0,
        //            units_band = 0,
        //            charges_item = 0;
        //    char UNITS_TIME = SmartParametersV2016.daytimeUnit;
        //    bool charges = false;

        //    DateTime temp_date = SmartParametersV2016.defaultDate;

        //    switch (utilityviewmodel.resource_code)
        //    {
        //        case SmartParametersV2016.Electricity:
        //            resource_charges = electricity_charges;
        //            total_resource_charges = total_electricity_charges;
        //            resource_meter_no = electricity_meter_no;
        //            break;
        //        case SmartParametersV2016.Gas:
        //            resource_charges = gas_charges;
        //            total_resource_charges = total_gas_charges;
        //            resource_meter_no = gas_meter_no;
        //            break;
        //        default:
        //            break;
        //    }

        //    lines = strText.Split('\n');
        //    while (index < lines.Length)
        //    {
        //        token = lines[index].Trim(' ');
        //        if (charges)
        //        {
        //            if (token.Length >= total_resource_charges.Length)
        //            {
        //                if (token.Substring(0, total_resource_charges.Length) == total_resource_charges)
        //                {
        //                    currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol, 0);
        //                    if (currency_index >= 0)
        //                    {
        //                        switch (utilityviewmodel.resource_code)
        //                        {
        //                            case SmartParametersV2016.Electricity:
        //                                E_NEW_CHARGES = token.Substring(currency_index + 1, token.Length - currency_index - 1);
        //                                //if (trace) { listener.WriteLine(" E_NEW_CHARGES = " + E_NEW_CHARGES, routine); }
        //                                break;
        //                            case SmartParametersV2016.Gas:
        //                                G_NEW_CHARGES = token.Substring(currency_index + 1, token.Length - currency_index - 1);
        //                                //if (trace) { listener.WriteLine(" G_NEW_CHARGES = " + G_NEW_CHARGES, routine); }
        //                                break;
        //                            default:
        //                                break;
        //                        }
        //                    }
        //                    charges = false;
        //                    goto next_line;
        //                }
        //            }

        //            if (token.Length >= resource_meter_no.Length)
        //            {
        //                if (token.Substring(0, resource_meter_no.Length) == resource_meter_no)
        //                {
        //                    token = token.Substring(resource_meter_no.Length + 1, token.Length - resource_meter_no.Length - 1);
        //                    token = token.Replace(tariff, "!");
        //                    words = token.Split('!');
        //                    word_count = 0;
        //                    while (word_count < words.Length)
        //                    {
        //                        switch (word_count)
        //                        {
        //                            case 0:
        //                                // Meter Serial No
        //                                METER_SERIAL_NO = words[word_count].Trim();
        //                                METER_SERIAL_NO = METER_SERIAL_NO.Substring(0, METER_SERIAL_NO.Length <= 14 ? METER_SERIAL_NO.Length : 14); //Enforce 14 char max
        //                                break;
        //                            case 1:
        //                                if (string.IsNullOrEmpty(TARIFF_NAME))
        //                                {
        //                                    TARIFF_NAME = words[word_count].Trim(' ');
        //                                }
        //                                break;
        //                            default:
        //                                break;
        //                        }
        //                        word_count = word_count + 1;
        //                    }
        //                    //if (trace) { listener.WriteLine(" METER_SERIAL_NO = " + METER_SERIAL_NO, routine); }
        //                    //if (trace) { listener.WriteLine(" TARIFF_NAME = " + TARIFF_NAME, routine); }
        //                    goto next_line;
        //                }
        //            }

        //            if ((token.IndexOf(to) >= 0) &&
        //                (token.IndexOf(at) >= 0))
        //            {
        //                // Probably have a readings line to process
        //                // Unfortunately, I cannot determine Estimated or
        //                // Actual as those EDF numbskulls have decided to denote
        //                // all of this stuff with symbols rather than the
        //                // letters 'E' or 'A' - proving beyond a shadow of a doubt
        //                // that they are a bunch of effing Frog tossers ...
        //                token = token.Replace(to, SmartParametersV2016.space);
        //                token = token.Replace(at, "");
        //                words = token.Split(' ');
        //                word_count = 0;
        //                while (word_count < words.Length)
        //                {
        //                    switch (word_count)
        //                    {
        //                        case 0:
        //                            READINGS_PERIOD_START = words[word_count];
        //                            if (READINGS_PERIOD_START.Length < 2)
        //                            {
        //                                READINGS_PERIOD_START = "0" + READINGS_PERIOD_START;
        //                            }
        //                            READINGS_PERIOD_START = READINGS_PERIOD_START + "-";
        //                            break;
        //                        case 1:
        //                            READINGS_PERIOD_START = READINGS_PERIOD_START + words[word_count].Substring(0, 3) + "-";
        //                            break;
        //                        case 2:
        //                            if (words[word_count].Length < 4)
        //                            {
        //                                READINGS_PERIOD_START = READINGS_PERIOD_START + "20" + words[word_count];
        //                            }
        //                            else
        //                            {
        //                                READINGS_PERIOD_START = READINGS_PERIOD_START + words[word_count];
        //                            }
        //                            if (!SmartParseV2016.Generic_Parse_Datetime(READINGS_PERIOD_START, rf temp_date, rf ourviewmodel.errorMessage))
        //                            {
        //                                return; // false;
        //                            }
        //                            break;
        //                        case 3:
        //                            READINGS_PERIOD_END = words[word_count];
        //                            if (READINGS_PERIOD_END.Length < 2)
        //                            {
        //                                READINGS_PERIOD_END = "0" + READINGS_PERIOD_END;
        //                            }
        //                            READINGS_PERIOD_END = READINGS_PERIOD_END + "-";
        //                            break;
        //                        case 4:
        //                            READINGS_PERIOD_END = READINGS_PERIOD_END + words[word_count].Substring(0, 3) + "-";
        //                            break;
        //                        case 5:
        //                            if (words[word_count].Length < 4)
        //                            {
        //                                READINGS_PERIOD_END = READINGS_PERIOD_END + "20" + words[word_count];
        //                            }
        //                            else
        //                            {
        //                                READINGS_PERIOD_END = READINGS_PERIOD_END + words[word_count];
        //                            }
        //                            if (!SmartParseV2016.Generic_Parse_Datetime(READINGS_PERIOD_END, rf temp_date, rf ourviewmodel.errorMessage))
        //                            {
        //                                return; // false;
        //                            }
        //                            units_band = 0;
        //                            DateTime read_date = SmartUtilityV2022.convert_date(READINGS_PERIOD_END, SmartParametersV2016.defaultDate, rf ourviewmodel.errorMessage);
        //                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
        //                            {
        //                                return;
        //                            }
        //                            DateTime from_date = SmartUtilityV2022.convert_date(READINGS_PERIOD_START, SmartParametersV2016.defaultDate, rf ourviewmodel.errorMessage);
        //                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
        //                            {
        //                                return;
        //                            }
        //                            break;
        //                        case 6:
        //                            D_LAST_READ = words[word_count].Trim(' ');
        //                            break;
        //                        case 7:
        //                            D_THIS_READ = words[word_count].Trim(' ');
        //                            break;
        //                        case 8:
        //                            D_UNITS_USEDX = words[word_count].Trim(' ');
        //                            switch (utilityviewmodel.resource_code)
        //                            {
        //                                case SmartParametersV2016.Electricity:
        //                                    D_UNITS_USED_KWH = D_UNITS_USEDX;
        //                                    UNIT_OF_MEASURE = "kWh";
        //                                    break;
        //                                case SmartParametersV2016.Gas:
        //                                    D_UNITS_USED_M3 = D_UNITS_USEDX.Replace("*", "");
        //                                    UNIT_OF_MEASURE = "m3";
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            break;
        //                        case 9:
        //                            // This is the KwH conversion of the UNITS_USED
        //                            // For Electricity - it should be the same value
        //                            // For Gas its the UNITS_USED as m3 -> kWh
        //                            KWH_UNITS_USED = words[word_count].Trim(' ');
        //                            break;
        //                        case 10:
        //                            // Suprisingly this comes BEFORE the Rate
        //                            USAGE_TOTAL = words[word_count].Trim(' ');
        //                            USAGE_TOTAL = USAGE_TOTAL.Replace(utilityviewmodel.bill_currency_symbol, "");
        //                            break;
        //                        case 11:
        //                            // Suprisingly - this comes AFTER the Total
        //                            UNITS_RATE = words[word_count].Trim(' ');
        //                            UNITS_RATE = UNITS_RATE.Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "");
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    word_count = word_count + 1;
        //                }
        //                // No D_UNITS_USED = 0 test because sometimes it might be!
        //                switch (utilityviewmodel.resource_code)
        //                {
        //                    case SmartParametersV2016.Electricity:
        //                        SmartUtilityV2022.E_ReadingsList_Add(utilityviewmodel.e_readings_changesList,
        //                                                                utilityviewmodel.brand_code,
        //                                                                utilityviewmodel.ACCOUNT_NO,
        //                                                                utilityviewmodel.STATEMENT_ID,
        //                                                                utilityviewmodel.BILL_DATE,
        //                                                                READINGS_PERIOD_START,
        //                                                                READINGS_PERIOD_END,
        //                                                                METER_SERIAL_NO,
        //                                                                READ_TYPE,
        //                                                                D_LAST_READ,
        //                                                                D_THIS_READ,
        //                                                                D_UNITS_USED_KWH,
        //                                                                N_LAST_READ,
        //                                                                N_THIS_READ,
        //                                                                N_UNITS_USED,
        //                                                                UNIT_OF_MEASURE);
        //                        break;
        //                    case SmartParametersV2016.Gas:
        //                        SmartUtilityV2022.G_ReadingsList_Add(utilityviewmodel.g_readings_changesList,
        //                                                                utilityviewmodel.brand_code,
        //                                                                utilityviewmodel.ACCOUNT_NO,
        //                                                                utilityviewmodel.STATEMENT_ID,
        //                                                                utilityviewmodel.BILL_DATE,
        //                                                                READINGS_PERIOD_START,
        //                                                                READINGS_PERIOD_END,
        //                                                                METER_SERIAL_NO,
        //                                                                READ_TYPE,
        //                                                                D_THIS_READ,
        //                                                                D_LAST_READ,
        //                                                                D_UNITS_USED_M3,
        //                                                                UNIT_OF_MEASURE,
        //                                                                D_UNITS_USED_KWH,
        //                                                                CALORIFIC_VALUE);
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                units_band = (short)(units_band + 1);

        //                string UNITS_COST = "0";

        //                UNIT_CHARGES_PERIOD_START = READINGS_PERIOD_START;
        //                UNIT_CHARGES_PERIOD_END = READINGS_PERIOD_END;

        //                switch (utilityviewmodel.resource_code)
        //                {
        //                    case SmartParametersV2016.Electricity:
        //                        SmartUtilityV2022.E_Unit_ChargesList_Add(utilityviewmodel.e_unit_charges_changesList,
        //                                                                    utilityviewmodel.brand_code,
        //                                                                    utilityviewmodel.supplier_code,
        //                                                                    utilityviewmodel.ACCOUNT_NO,
        //                                                                    utilityviewmodel.STATEMENT_ID,
        //                                                                    utilityviewmodel.BILL_DATE,
        //                                                                    utilityviewmodel.sparks.MPAN,
        //                                                                    UNIT_CHARGES_PERIOD_START,
        //                                                                    UNIT_CHARGES_PERIOD_END,
        //                                                                    UNITS_TIME,
        //                                                                    units_band.ToString(),
        //                                                                    "",       // TYPE
        //                                                                    KWH_UNITS_USED,
        //                                                                    UNITS_RATE,
        //                                                                    UNIT_OF_MEASURE,
        //                                                                    UNITS_COST);
        //                        break;
        //                    case SmartParametersV2016.Gas:
        //                        SmartUtilityV2022.G_Unit_ChargesList_Add(utilityviewmodel.g_unit_charges_changesList,
        //                                                                        utilityviewmodel.supplier_code,
        //                                                                        utilityviewmodel.brand_code,
        //                                                                        utilityviewmodel.ACCOUNT_NO,
        //                                                                        utilityviewmodel.STATEMENT_ID,
        //                                                                        utilityviewmodel.BILL_DATE,
        //                                                                        utilityviewmodel.smell.MPRN,
        //                                                                        UNIT_CHARGES_PERIOD_START,
        //                                                                        UNIT_CHARGES_PERIOD_END,
        //                                                                        units_band.ToString(),
        //                                                                        "",       // TYPE
        //                                                                        KWH_UNITS_USED,
        //                                                                        UNITS_RATE,
        //                                                                        UNIT_OF_MEASURE,
        //                                                                        UNITS_COST);
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                goto next_line;
        //            }

        //            if ((token.IndexOf(standing_charge) >= 0) &&
        //                (token.IndexOf(days_at) >= 0))
        //            {
        //                // Probably have a readings line to process
        //                // Unfortunately, I cannot determine Estimated or
        //                // Actual as those EDF numbskulls have decided to denote
        //                // all of this stuff with symbols rather than the
        //                // letters 'E' or 'A' - proving beyond a shadow of a doubt
        //                // that they are a bunch of effing Frog tossers ...
        //                token = token.Replace(standing_charge, SmartParametersV2016.space);
        //                token = token.Replace(days_at, SmartParametersV2016.space);
        //                words = token.Split(' ');
        //                word_count = 0;
        //                while (word_count < words.Length)
        //                {
        //                    switch (word_count)
        //                    {
        //                        case 0:
        //                            // Suprisingly this comes BEFORE the Rate
        //                            STANDING_TOTAL = words[word_count].Trim(' ');
        //                            STANDING_TOTAL = STANDING_TOTAL.Replace(utilityviewmodel.bill_currency_symbol, "");
        //                            break;
        //                        case 1:
        //                            // Days period
        //                            CHARGES_DAYS = words[word_count].Trim(' ');
        //                            break;
        //                        case 2:
        //                            STANDING_CHARGE = words[word_count].Trim(' ');
        //                            STANDING_CHARGE = STANDING_CHARGE.Replace(utilityviewmodel.bill_denomination_symbol.ToString(), "");
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    word_count = word_count + 1;
        //                }

        //                string CHARGES_COST = "0";
        //                charges_item = (short)(charges_item + 1);

        //                STANDING_CHARGES_PERIOD_START = READINGS_PERIOD_START;
        //                STANDING_CHARGES_PERIOD_END = READINGS_PERIOD_END;

        //                switch (utilityviewmodel.resource_code)
        //                {
        //                    case SmartParametersV2016.Electricity:
        //                        SmartUtilityV2022.E_Standing_ChargesList_Add(utilityviewmodel.e_standing_charges_changesList,
        //                                                                        utilityviewmodel.brand_code,
        //                                                                        utilityviewmodel.supplier_code,
        //                                                                        utilityviewmodel.ACCOUNT_NO,
        //                                                                        utilityviewmodel.STATEMENT_ID,
        //                                                                        utilityviewmodel.BILL_DATE,
        //                                                                        utilityviewmodel.sparks.MPAN,
        //                                                                        STANDING_CHARGES_PERIOD_START,
        //                                                                        STANDING_CHARGES_PERIOD_END,
        //                                                                        charges_item.ToString(),
        //                                                                        "",       // TYPE
        //                                                                        STANDING_CHARGE,
        //                                                                        CHARGES_DAYS,
        //                                                                        CHARGES_COST);
        //                        break;
        //                    case SmartParametersV2016.Gas:
        //                        SmartUtilityV2022.G_Standing_ChargesList_Add(utilityviewmodel.g_standing_charges_changesList,
        //                                                                        utilityviewmodel.brand_code,
        //                                                                        utilityviewmodel.supplier_code,
        //                                                                        utilityviewmodel.ACCOUNT_NO,
        //                                                                        utilityviewmodel.STATEMENT_ID,
        //                                                                        utilityviewmodel.BILL_DATE,
        //                                                                        utilityviewmodel.smell.MPRN,
        //                                                                        STANDING_CHARGES_PERIOD_START,
        //                                                                        STANDING_CHARGES_PERIOD_END,
        //                                                                        charges_item.ToString(),
        //                                                                        "",       // TYPE
        //                                                                        STANDING_CHARGE,
        //                                                                        CHARGES_DAYS,
        //                                                                        CHARGES_COST);
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                goto next_line;
        //            }
        //        }

        //        if (token.Length >= your_balance.Length)
        //        {
        //            if (token.Substring(0, your_balance.Length) == your_balance)
        //            {
        //                currency_index = token.IndexOf(utilityviewmodel.bill_currency_symbol);
        //                if (currency_index >= 0)
        //                {
        //                    PREVIOUS_BALANCE = token.Substring(currency_index + 1, token.Length - currency_index - 1);
        //                    //if (trace) { listener.WriteLine(" PREVIOUS_BALANCE = " + PREVIOUS_BALANCE, routine); }
        //                }
        //            }
        //        }

        //        if (token.Length >= payment.Length)
        //        {
        //            if (token.Substring(0, payment.Length) == payment)
        //            {
        //                token = token.Substring(payment.Length, token.Length - payment.Length);
        //                if (token.IndexOf(space_dash_space) >= 0)
        //                {
        //                    token = token.Replace(space_dash_space, "!");
        //                    token = token.Replace(thank_you, "");
        //                    payments = token.Split('!');
        //                    int payment_count = 0;
        //                    while (payment_count < payments.Length)
        //                    {
        //                        switch (payment_count)
        //                        {
        //                            case 0:
        //                                // Payment Date
        //                                words = payments[payment_count].Split(' ');
        //                                word_count = 0;
        //                                while (word_count < words.Length)
        //                                {
        //                                    switch (word_count)
        //                                    {
        //                                        case 0:
        //                                            PAYMENT_DATE = words[word_count];
        //                                            if (PAYMENT_DATE.Length < 2)
        //                                            {
        //                                                PAYMENT_DATE = "0" + PAYMENT_DATE;
        //                                            }
        //                                            PAYMENT_DATE = PAYMENT_DATE + "-";
        //                                            break;
        //                                        case 1:
        //                                            PAYMENT_DATE = PAYMENT_DATE + words[word_count].Substring(0, 3) + "-";
        //                                            break;
        //                                        case 2:
        //                                            if (words[word_count].Length < 4)
        //                                            {
        //                                                PAYMENT_DATE = PAYMENT_DATE + "20" + words[word_count];
        //                                            }
        //                                            else
        //                                            {
        //                                                PAYMENT_DATE = PAYMENT_DATE + words[word_count];
        //                                            }
        //                                            temp_date = SmartTimeV2016.ConvertDateTime(PAYMENT_DATE, SmartParametersV2016.defaultCulture);
        //                                            break;
        //                                        default:
        //                                            break;
        //                                    }
        //                                    word_count = word_count + 1;
        //                                }
        //                                //if (trace) { listener.WriteLine(" PAYMENT_DATE = " + PAYMENT_DATE, username); }
        //                                break;
        //                            case 1:
        //                                currency_index = payments[payment_count].IndexOf(utilityviewmodel.bill_currency_symbol);
        //                                PAYMENTS_RECEIVED = payments[payment_count].Substring(currency_index + 1, payments[payment_count].Length - currency_index - 1);
        //                                words = PAYMENTS_RECEIVED.Split(' ');
        //                                word_count = 0;
        //                                while (word_count < words.Length)
        //                                {
        //                                    switch (word_count)
        //                                    {
        //                                        case 0:
        //                                            PAYMENTS_RECEIVED = words[word_count];
        //                                            break;
        //                                        case 1:
        //                                            if (words[word_count].ToUpper() == "DR")
        //                                            {
        //                                                PAYMENTS_RECEIVED = "-" + PAYMENTS_RECEIVED;
        //                                            }
        //                                            break;
        //                                        default:
        //                                            break;
        //                                    }
        //                                    word_count = word_count + 1;
        //                                }
        //                                //if (trace) { listener.WriteLine(" PAYMENTS_RECEIVED = " + PAYMENTS_RECEIVED, routine); }
        //                                break;
        //                            default:
        //                                break;
        //                        }
        //                        payment_count = payment_count + 1;
        //                    }
        //                    string PAYMENT_METHOD = "";
        //                    PAYMENT_CODE = 0;
        //                    // This is going to fail!!
        //                    if (!string.IsNullOrEmpty(PAYMENT_METHOD))
        //                    {
        //                        if (!SmartUtilityV2022.Lookup_Payment_Code(rf PAYMENT_METHOD, rf PAYMENT_CODE, utilityviewmodel.payment_methodsList))
        //                        {
        //                            return;
        //                        }
        //                        if (!string.IsNullOrEmpty(PAYMENT_DATE) &&
        //                            !string.IsNullOrEmpty(PAYMENT_AMOUNT))
        //                        {

        //                            // Add it in ... perhaps
        //                            // If this section is E(lectricity) and G(as) then we might be doing it twice!
        //                            // Check if its there first
        //                            PAYMENT_BALANCE = utilityviewmodel.PAYMENTS_BALANCE.ToString();
        //                            if (!SmartParseV2016.Check_Payment_Is_There(username,
        //                                                                        utilityviewmodel,
        //                                                                        temp_date,      // Which IS the PAYMENT_DATE as a DateTime
        //                                                                        utilityviewmodel.PAYMENTS_ITEM, //payment_item,
        //                                                                        PAYMENT_CODE,
        //                                                                        PAYMENT_AMOUNT,
        //                                                                        PAYMENT_BALANCE))
        //                            {
        //                                SmartUtilityV2022.PaymentsList_Add(utilityviewmodel.payments_changesList,
        //                                                                    utilityviewmodel.brand_code,
        //                                                                    utilityviewmodel.ACCOUNT_NO,
        //                                                                    utilityviewmodel.STATEMENT_ID,
        //                                                                    PAYMENT_DATE,
        //                                                                    utilityviewmodel.PAYMENTS_ITEM.ToString(), //payment_item.ToString(),
        //                                                                    PAYMENT_CODE.ToString(),
        //                                                                    PAYMENT_AMOUNT,
        //                                                                    PAYMENT_BALANCE);
        //                            }
        //                            SmartParseV2016.Update_Bill("PAYMENT_AMOUNT", utilityviewmodel, PAYMENT_AMOUNT);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        if (token.Length >= resource_charges.Length)
        //        {
        //            if (token.Substring(0, resource_charges.Length) == resource_charges)
        //            {
        //                charges = true;
        //                goto next_line;
        //            }
        //        }

        //        if (token.Length >= vat_on.Length)
        //        {
        //            if (token.Substring(0, vat_on.Length) == vat_on)
        //            {
        //                token = token.Substring(vat_on.Length, token.Length - vat_on.Length);
        //                token = token.Replace(at_space, SmartParametersV2016.space);
        //                words = token.Split(' ');
        //                word_count = 0;
        //                while (word_count < words.Length)
        //                {
        //                    switch (word_count)
        //                    {
        //                        case 0:
        //                            VAT_TOTAL = words[word_count].Trim(' ');
        //                            VAT_TOTAL = VAT_TOTAL.Replace(utilityviewmodel.bill_currency_symbol, "");
        //                            break;
        //                        case 1:
        //                            VAT_RATE = words[word_count].Trim(' ');
        //                            VAT_RATE = VAT_RATE.Replace("%", "");
        //                            break;
        //                        case 2:
        //                            VAT_AMOUNT = words[word_count].Trim(' ');
        //                            VAT_AMOUNT = VAT_AMOUNT.Replace(utilityviewmodel.bill_currency_symbol, "");
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    word_count = word_count + 1;
        //                }
        //                //if (trace)
        //                //{
        //                //    listener.WriteLine(" VAT_TOTAL = " + VAT_TOTAL, routine);
        //                //    listener.WriteLine(" VAT_RATE = " + VAT_RATE, routine);
        //                //    listener.WriteLine(" VAT_AMOUNT = " + VAT_AMOUNT, routine); 
        //                //}
        //                if (!string.IsNullOrEmpty(VAT_RATE))
        //                {
        //                    switch (utilityviewmodel.resource_code)
        //                    {
        //                        case SmartParametersV2016.Electricity:
        //                            //foreach (DataRow unit_charges in e_unit_charges_table.Rows)
        //                            //{
        //                            //    unit_charges["UNITS_VAT"] = ConvertDecimal(VAT_RATE, SmartParametersV2016.defaultCulture);
        //                            //}
        //                            //foreach (DataRow standing_charges in e_standing_charges_table.Rows)
        //                            //{
        //                            //    standing_charges["CHARGES_VAT"] = ConvertDecimal(VAT_RATE, SmartParametersV2016.defaultCulture);
        //                            //}
        //                            break;
        //                        case SmartParametersV2016.Gas:
        //                            //foreach (DataRow unit_charges in g_unit_charges_table.Rows)
        //                            //{
        //                            //    unit_charges["UNITS_VAT"] = ConvertDecimal(VAT_RATE, SmartParametersV2016.defaultCulture);
        //                            //}
        //                            //foreach (DataRow standing_charges in g_standing_charges_table.Rows)
        //                            //{
        //                            //    standing_charges["CHARGES_VAT"] = ConvertDecimal(VAT_RATE, SmartParametersV2016.defaultCulture);
        //                            //}
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }
        //                goto next_line;
        //            }
        //        }
        //        next_line:
        //        index = index + 1;
        //    }

        //    //SmartUtilityV2022.TariffDetails_add(utilityviewmodel.TariffDetails_changesList,
        //    //                                    utilityviewmodel.resource_code,
        //    //                                    utilityviewmodel.brand_code,
        //    //                                    utilityviewmodel.ACCOUNT_NO,
        //    //                                    utilityviewmodel.STATEMENT_ID,
        //    //                                    utilityviewmodel.TARIFF_CODE.ToString(),
        //    //                                    "1",                  // Payment code
        //    //                                    "1",                  // Group code
        //    //                                    utilityviewmodel.area_code.ToString(),      // Area code
        //    //                                    utilityviewmodel.economy7.ToString(),       // Economy7
        //    //                                    BILL_DATE,          // Prices valid from
        //    //                                    "0",                // Standing Charge
        //    //                                    "0",                // Day Rate
        //    //                                    "0");               // Night Rate
        //}

        internal static async Task<bool> DoTheSwitch(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    bool TextBox_Active,
                                                    SmartUtility.SwitchInfo switch_info)
        {
            // Because Sometimes we are returned a Document
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

            bool status = false;

            bool keep_looping = true;
            //string target_string = "";

            bool expect_json = false;
            string json = "";

            utilityviewmodel.property_code = "";
            utilityviewmodel.qnb_quote_id = "";
            utilityviewmodel.form_id = "";             // Stays same at every step
            utilityviewmodel.form_build_id = "";

            // Stays same at every step
            // These are split up because we need to checl that
            // the step5 ISN'T the same as the Step4. If it IS the same,
            // then that means that the Bank Sort Code/Bank Account Number
            // combination failed, because Step5 will have returned form Step4
            // ... the the form_build_id is thus the same.
            string form_build_id_step1 = "",
                  form_build_id_step2 = "",
                  form_build_id_step3 = "",
                  form_build_id_step4 = "",
                  form_build_id_step5 = "";
            //form_build_id_step6 = "";   // Varies at every step (i.e. after each POST)

            utilityviewmodel.House_No = "";
            utilityviewmodel.House_Name = "";
            utilityviewmodel.Street = "";
            utilityviewmodel.City_Town = "";

            switch_info.address = switch_info.address.Replace(SmartParametersV2016.bar.ToString(), "");
            switch_info.address = SmartParseV2016.Remove_Double_Spaces_V3(switch_info.address).Trim();
            //switch_info.address = switch_info.address.Trim();

            utilityviewmodel.logout_pathname = "";
            utilityviewmodel.next_routine = "FIND_QUOTEID";
            utilityviewmodel.target_pathname = "gas-electricity/compare-prices?postcode=" + Uri.EscapeDataString(switch_info.postcode) + "&submit=Get+a+quote";

            while (keep_looping)
            {
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    utilityviewmodel.next_routine == "LOGOUT")
                {
                    utilityviewmodel.target_pathname = utilityviewmodel.logout_pathname;
                    keep_looping = false;
                    utilityviewmodel.next_routine = "LOGOUT";
                }
                if (utilityviewmodel.next_routine != "HOME" &&
                    !string.IsNullOrEmpty(utilityviewmodel.target_pathname))
                {
                    if (utilityviewmodel.next_routine.ToUpper() == utilityviewmodel.next_routine)
                    {
                        // We are doing GETs
                        if (!expect_json)
                        {
                            htmlDocument = await SmartBobV2017.Scraper_Generic_Get(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel);
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            json = await SmartBobV2017.Scraper_Generic_Get_Json(ourviewmodel, utilityviewmodel.utilityToken, utilityviewmodel);
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // We are doing POSTs
                        htmlDocument = await SmartBobV2017.Scraper_Generic_Post(ourviewmodel, utilityviewmodel.utilityToken, keyValues, utilityviewmodel);
                        if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                    }
                }

                // Can't miss out a STEP!!  Have to do all 6!!!
                utilityviewmodel.current_routine = utilityviewmodel.next_routine;
                switch (utilityviewmodel.next_routine)
                {
                    case "FIND_QUOTEID":
                        // The timer is re-started when the
                        // first Login Document has been completed
                        utilityviewmodel.form_build_id = form_build_id_step1;
                        if (EDF_GetQuoteId(utilityviewmodel,
                                            htmlDocument))
                        {
                            form_build_id_step1 = utilityviewmodel.form_build_id;
#if WINFORMS
                            if (utilityviewmodel.console)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Quote Id: " + utilityviewmodel.qnb_quote_id + Environment.NewLine.ToString());
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Form Id: " + utilityviewmodel.form_id + Environment.NewLine.ToString());
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 1 Form Build Id: " + form_build_id_step1 + Environment.NewLine.ToString());
                            }
#endif
#if WPF 
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
                                    ourviewmodel,
                                    "QuoteId" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.qnb_quote_id);

                            }
#endif
#if ANDROIDX
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "QuoteId" +
                                    SmartParametersV2016.space +
                                    utilityviewmodel.qnb_quote_id);

                            }
#endif
                            utilityviewmodel.next_routine = "FIND_ADDRESS";  // Upper case? Its a GET
                            utilityviewmodel.target_pathname = "getQnbAddresses" + "?postcode=" + Uri.EscapeDataString(switch_info.postcode);
                            expect_json = true;
                            // Should now be going on to FIND_ADDRESS - you ARE a fucking Genius, Ray!!
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                        }
                        break;
                    case "FIND_ADDRESS":
                        // The timer is re-started when the
                        // first Login Document has been completed
                        expect_json = false;
                        if (EDF_GetAddress(utilityviewmodel,
                                            json,
                                            switch_info.postcode,
                                            switch_info.address))
                        {
                            // House No
                            // House name
                            // Street/City
                            // Town

                            if (TextBox_Active)
                            {

                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchAddressCode" + SmartParametersV2016.space + utilityviewmodel.property_code);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchHouseNo" + SmartParametersV2016.space + utilityviewmodel.House_No);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchHouseName" + SmartParametersV2016.space + utilityviewmodel.House_Name);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchStreet" + SmartParametersV2016.space + utilityviewmodel.Street);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel,
                                    "SwitchCityTown" + SmartParametersV2016.space + utilityviewmodel.City_Town);
                            }
                            utilityviewmodel.target_pathname = "gas-electricity/compare-prices?" + "postcode=" + Uri.EscapeDataString(switch_info.postcode) + "&submit=Get+a+quote&step=2";

                            keyValues.Clear();
                            keyValues.Add(new KeyValuePair<string, string>("submitted[qnb_quote_id]", utilityviewmodel.qnb_quote_id));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[enter_your_postcode]", switch_info.postcode));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[please_select_youraddress]", utilityviewmodel.property_code));
                            keyValues.Add(new KeyValuePair<string, string>("op", "Continue"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[please_select_mobile]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("details[sid]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("details[page_num]", "1"));
                            keyValues.Add(new KeyValuePair<string, string>("details[page_count]", "6"));
                            keyValues.Add(new KeyValuePair<string, string>("details[finished]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("form_build_id", form_build_id_step1));  // Different at each Step
                            keyValues.Add(new KeyValuePair<string, string>("form_id", utilityviewmodel.form_id));                    // Stays same throughout
                            keyValues.Add(new KeyValuePair<string, string>("hid_flag", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("tarrif_journey", ""));
                            keyValues.Add(new KeyValuePair<string, string>("current_payment", ""));
                            utilityviewmodel.next_routine = "Step_2";    // Lower case? Its a POST
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                        }
                        break;
                    case "Step_2":
                        if (EDF_GetFormBuildId(utilityviewmodel,
                                                htmlDocument))
                        {
                            form_build_id_step2 = utilityviewmodel.form_build_id;
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                    "EdFSwitchStep2" +
                                    SmartParametersV2016.space +
                                    "HeavyTick");

                            }
                            utilityviewmodel.target_pathname = "gas-electricity/compare-prices?" + "postcode=" + Uri.EscapeDataString(switch_info.postcode) + "&submit=Get+a+quote&step=3";

                            keyValues.Clear();
                            keyValues.Add(new KeyValuePair<string, string>("submitted[inital_page]", "Inital Page"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[is_pds]", "1"));
                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[what_would_you_like_a_quote_for]", "1")); // Electricity
                                    break;
                                case SmartParametersV2016.Gas:
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[what_would_you_like_a_quote_for]", "2")); // Gas
                                    break;
                                case SmartParametersV2016.DualFuel:
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[what_would_you_like_a_quote_for]", "0")); // Dual Fuel
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[are_your_current_electricity_and_gas_supplier_the_same]", "0")); // Yes, they are the same

                                    break;
                            }

                            keyValues.Add(new KeyValuePair<string, string>("submitted[home_move_3_months][have_you_moved_into_your_new_home_within_the_last_3_months]", "02"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][how_much_energy_do_you_use]", "0"));

                            switch (utilityviewmodel.resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_in_kwhs][electricity_used][electricity_used_kwh_per]", "120"));
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_in_kwhs][electricity_used][electricity_used_kwh]", "0"));
                                    break;
                                case SmartParametersV2016.Gas:
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_in_kwhs][gas_used][gas_used_kwh_per]", "120"));
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_in_kwhs][gas_used][gas_used_kwh]", "0"));
                                    break;
                                case SmartParametersV2016.DualFuel:
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_in_kwhs][electricity_used][electricity_used_kwh_per]", "120"));
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_in_kwhs][electricity_used][electricity_used_kwh]", "0"));
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_in_kwhs][gas_used][gas_used_kwh_per]", "120"));
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_in_kwhs][gas_used][gas_used_kwh]", "0"));
                                    break;
                            }

                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][elec_spent][electricity_used_kWh_per]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][elec_spent][electricity_used_kwh]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][elec_spent][your_current_elect_supplier]", "20"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][elec_spent][your_elect_payment_method]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][elec_spent][your_current_elect_tariff]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][gas_spent][gas_used_kwh_per]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][gas_spent][gas_used_kwh]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][gas_spent][your_current_gas_supplier]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][gas_spent][your_gas_payment_method]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_estimate_options][i_know_how_much_i_spend][gas_spent][your_current_gas_tariff]", ""));
                            switch (SmartUtilityV2022.Determine_Resource_Type(utilityviewmodel))
                            {
                                case "SR":
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[do_you_have_an_economy_7_meter]", "1")); // No
                                    break;
                                case "VR":
                                    keyValues.Add(new KeyValuePair<string, string>("submitted[do_you_have_an_economy_7_meter]", "0")); // Yes ... but I can't prove this as it doesn't show up on the website??
                                    break;
                            }
                            keyValues.Add(new KeyValuePair<string, string>("submitted[do_you_have_a_smart_meter]", "1"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[comparision_save][would_you_like_to_compare_how_much_you_can_save]", "1"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[comparision_save][comparision_save_yes][elec_supply][your_elec_supplier]", "20"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[comparision_save][comparision_save_yes][elec_supply][your_elec_payment_method]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[comparision_save][comparision_save_yes][elec_supply][your_elec_tariff]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[comparision_save][comparision_save_yes][gas_supply][your_gas_supplier]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[comparision_save][comparision_save_yes][gas_supply][your_gas_payment_method]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[comparision_save][comparision_save_yes][gas_supply][your_gas_tariff]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[do_you_have_a_pre_payment_meter][for_electricity]", "1"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[do_you_have_a_pre_payment_meter][for_gas]", "1"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[would_you_like_to_pay_by_direct_debit]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[get_your_results][email_address]", switch_info.email));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[get_your_results][title_quote]", switch_info.title));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[get_your_results][first_name_quote]", switch_info.first_name));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[get_your_results][last_name_quote]", switch_info.last_name));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[get_your_results][telephone_number_quote]", switch_info.telephone));
                            keyValues.Add(new KeyValuePair<string, string>("details[sid]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("details[page_num]", "2"));
                            keyValues.Add(new KeyValuePair<string, string>("details[page_count]", "6"));
                            keyValues.Add(new KeyValuePair<string, string>("details[finished]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("form_build_id", form_build_id_step2));
                            keyValues.Add(new KeyValuePair<string, string>("form_id", utilityviewmodel.form_id));
                            keyValues.Add(new KeyValuePair<string, string>("hid_flag", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("tarrif_journey", ""));
                            keyValues.Add(new KeyValuePair<string, string>("current_payment", ""));
                            keyValues.Add(new KeyValuePair<string, string>("op", "Show me my quote"));

                            utilityviewmodel.next_routine = "Step_3";    //  Lower case? Its a POST
                            // Should now be going on to Step 3 - you ARE a fucking Genius, Ray!!
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                        }
                        break;
                    case "Step_3":
                        if (EDF_GetFormBuildId(utilityviewmodel,
                                                htmlDocument))
                        {
                            form_build_id_step3 = utilityviewmodel.form_build_id;

                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        "EdFSwitchStep3" +
                                        SmartParametersV2016.space +
                                        "HeavyTick");

                            }
                            utilityviewmodel.target_pathname = "gas-electricity/compare-prices?" + "postcode=" + Uri.EscapeDataString(switch_info.postcode) + "&submit=Get+a+quote&step=4";
                            keyValues.Clear();

                            keyValues.Add(new KeyValuePair<string, string>("submitted[quote_results_page_break]", "Quote results page break"));
                            keyValues.Add(new KeyValuePair<string, string>("productSelected", "product_0"));
                            keyValues.Add(new KeyValuePair<string, string>("details[sid]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("details[page_num]", "3"));
                            keyValues.Add(new KeyValuePair<string, string>("details[page_count]", "6"));
                            keyValues.Add(new KeyValuePair<string, string>("details[finished]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("form_build_id", form_build_id_step3));  // Different at each Step
                            keyValues.Add(new KeyValuePair<string, string>("form_id", utilityviewmodel.form_id));                    // Stays the same
                            keyValues.Add(new KeyValuePair<string, string>("hid_flag", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("tarrif_journey", ""));
                            keyValues.Add(new KeyValuePair<string, string>("current_payment", ""));
                            keyValues.Add(new KeyValuePair<string, string>("op", "go-to-ur-details"));

                            utilityviewmodel.next_routine = "Step_4";    //  Lower case? Its a POST
                            // Should now be going on to Step 4 - you ARE a fucking Genius, Ray!!
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                        }
                        break;

                    case "Step_4":
                        if (EDF_GetFormBuildId(utilityviewmodel,
                                                htmlDocument))
                        {
                            form_build_id_step4 = utilityviewmodel.form_build_id;

                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        "EdFSwitchStep4" +
                                        SmartParametersV2016.space +
                                        "HeavyTick");
                            }
                            utilityviewmodel.target_pathname = "gas-electricity/compare-prices?" + "postcode=" + Uri.EscapeDataString(switch_info.postcode) + "&submit=Get+a+quote&step=5";
                            keyValues.Clear();

                            keyValues.Add(new KeyValuePair<string, string>("submitted[your_details_page_break]", "Your details page break"));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[address_details_group][house_no_your_details]", utilityviewmodel.House_No));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[address_details_group][house_name_your_details]", utilityviewmodel.House_Name));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[address_details_group][street_yourdetails]", utilityviewmodel.Street));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[address_details_group][city_your_details]", utilityviewmodel.City_Town));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[title_your_details]", switch_info.title)); // <= See if this works when its empty
                            keyValues.Add(new KeyValuePair<string, string>("submitted[first_name_your_details]", switch_info.first_name));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[last_name_your_detials]", switch_info.last_name));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[email_address_your_details]", switch_info.email));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[contact_telephone_number]", switch_info.telephone));
                            keyValues.Add(new KeyValuePair<string, string>("submitted[communication_preference]", "Electronic")); // If no email this should be "Telephone"??
                            keyValues.Add(new KeyValuePair<string, string>("details[sid]", ""));
                            keyValues.Add(new KeyValuePair<string, string>("details[page_num]", "4"));
                            keyValues.Add(new KeyValuePair<string, string>("details[page_count]", "6"));
                            keyValues.Add(new KeyValuePair<string, string>("details[finished]", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("form_build_id", form_build_id_step4));      // Different at each Step
                            keyValues.Add(new KeyValuePair<string, string>("form_id", utilityviewmodel.form_id));                        // Stays the same
                            keyValues.Add(new KeyValuePair<string, string>("hid_flag", "0"));
                            keyValues.Add(new KeyValuePair<string, string>("tarrif_journey", ""));
                            keyValues.Add(new KeyValuePair<string, string>("current_payment", ""));
                            keyValues.Add(new KeyValuePair<string, string>("acc_num_hidden", switch_info.bank_account_number));
                            keyValues.Add(new KeyValuePair<string, string>("acc_name_hidden", switch_info.bank_account_name));
                            keyValues.Add(new KeyValuePair<string, string>("sort_code_hidden", switch_info.bank_account_sort_code));
                            keyValues.Add(new KeyValuePair<string, string>("payday_hidden", switch_info.bank_account_payday.ToString()));
                            keyValues.Add(new KeyValuePair<string, string>("op", "Review details"));

                            utilityviewmodel.next_routine = "Step_5";    //  Lower case? Its a POST
                            // Should now be going on to Step 5 - you ARE a fucking Genius, Ray!!
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                        }
                        break;
                    case "Step_5":
                        if (EDF_GetFormBuildId(utilityviewmodel,
                                                htmlDocument))
                        {
                            form_build_id_step5 = utilityviewmodel.form_build_id;


                            if (form_build_id_step4 == form_build_id_step5)
                            {

                                if (TextBox_Active)
                                {
                                    // Even though the problem is at Step 4...
                                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        "EdFSwitchStep5Bank");

                                }
                                utilityviewmodel.next_routine = "LOGOUT";
                            }
                            else
                            {

                                if (TextBox_Active)
                                {
                                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "EdFSwitchStep5" +
                                            SmartParametersV2016.space +
                                            "HeavyTick");
                                }
                                utilityviewmodel.target_pathname = "gas-electricity/compare-prices?" + "postcode=" + Uri.EscapeDataString(switch_info.postcode) + "&submit=Get+a+quote&step=6";

                                keyValues.Clear();
                                keyValues.Add(new KeyValuePair<string, string>("submitted[buy_review_page_break]", "Buy review page break"));
                                keyValues.Add(new KeyValuePair<string, string>("submitted[about_you_edited_fieldset][about_you_edited_left][buy_review_name]", switch_info.first_name + SmartParametersV2016.space + switch_info.last_name));
                                keyValues.Add(new KeyValuePair<string, string>("submitted[about_you_edited_fieldset][about_you_edited_left][email_address_buy_review]", switch_info.email));
                                keyValues.Add(new KeyValuePair<string, string>("submitted[about_you_edited_fieldset][about_you_edited_left][telephone_review]", switch_info.telephone));
                                keyValues.Add(new KeyValuePair<string, string>("submitted[buy_terms_and_conditions_check][0]", "0"));
                                keyValues.Add(new KeyValuePair<string, string>("details[sid]", ""));
                                keyValues.Add(new KeyValuePair<string, string>("details[page_num]", "5"));
                                keyValues.Add(new KeyValuePair<string, string>("details[page_count]", "6"));
                                keyValues.Add(new KeyValuePair<string, string>("details[finished]", "0"));
                                keyValues.Add(new KeyValuePair<string, string>("form_build_id", form_build_id_step5));      // Different at each Step
                                keyValues.Add(new KeyValuePair<string, string>("form_id", utilityviewmodel.form_id));                        // Stays the same
                                keyValues.Add(new KeyValuePair<string, string>("hid_flag", "0"));
                                keyValues.Add(new KeyValuePair<string, string>("tarrif_journey", ""));
                                keyValues.Add(new KeyValuePair<string, string>("current_payment", ""));
                                keyValues.Add(new KeyValuePair<string, string>("op", "Confirm and buy"));

                                utilityviewmodel.next_routine = "Confirm";
                                // Should now be going on to Confirm - you ARE a fucking Genius, Ray!!
                            }
                        }
                        else
                        {
                            ourviewmodel.errorMessage = utilityviewmodel.current_routine;
                        }
                        break;
                    case "Confirm":
                        if (EDF_GetConfirmation(htmlDocument))
                        {

                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        "Confirmation" +
                                        SmartParametersV2016.space +
                                        "True");
                            }
                            status = true;
                            utilityviewmodel.next_routine = "LOGOUT";
                        }
                        else
                        {
                            if (TextBox_Active)
                            {
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                        meterActivity,
#endif
                                        ourviewmodel,
                                        "Confirmation" +
                                        SmartParametersV2016.space +
                                        "False");
                            }
                            utilityviewmodel.next_routine = "LOGOUT";
                        }
                        break;
                    case "LOGOUT":
                        keep_looping = false;
                        break;
                    default:
                        break;
                }
            }
            return status;
        }

        internal static bool EDF_GetQuoteId(UtilityViewModel utilityviewmodel,
                                            HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;
            string value;
            utilityviewmodel.qnb_quote_id = "";
            utilityviewmodel.form_id = "";

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string name = element1.GetAttributeValue("name", "");
                switch (name)
                {
                    case "submitted[qnb_quote_id]":
                        string classname = element1.GetAttributeValue("class", "");
                        if (SmartNibbyV2016.Check_Classname(classname, "form-text", true))
                        {
                            if (element1.Id == "edit-submitted-qnb-quote-id")
                            {
                                value = element1.GetAttributeValue("value", "");
                                if (!string.IsNullOrEmpty(value))
                                {
                                    utilityviewmodel.qnb_quote_id = value;
                                    if (!string.IsNullOrEmpty(utilityviewmodel.form_id) &&
                                        !string.IsNullOrEmpty(utilityviewmodel.form_build_id))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        break;
                    case "form_id":
                        value = element1.GetAttributeValue("value", "");
                        if (!string.IsNullOrEmpty(value))
                        {
                            utilityviewmodel.form_id = value;
                            if (!string.IsNullOrEmpty(utilityviewmodel.qnb_quote_id) &&
                                !string.IsNullOrEmpty(utilityviewmodel.form_build_id))
                            {
                                return true;
                            }
                        }
                        break;
                    case "form_build_id":
                        value = element1.GetAttributeValue("value", "");
                        if (!string.IsNullOrEmpty(value))
                        {
                            utilityviewmodel.form_build_id = value;
                            if (!string.IsNullOrEmpty(utilityviewmodel.qnb_quote_id) &&
                                !string.IsNullOrEmpty(utilityviewmodel.form_id))
                            {
                                return true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return false;
        }

        
        internal static bool EDF_GetAddress(
                                    UtilityViewModel utilityviewmodel,
                                    string json,
                                    string postcode,
                                    string target_address)
        {
            utilityviewmodel.udprn = "";
            int property_match = 0;

            string fullthoroughfare = "",
                   buildingnumber = "",
                   buildingname = "",
                   town = "",
                   addressasline = "";

            string[] target = target_address.Split(SmartParametersV2016.spaceSplit);

            using JsonDocument jsonDoc = JsonDocument.Parse(json);
            JsonElement root = jsonDoc.RootElement;

            foreach (JsonProperty property in root.EnumerateObject())
            {
                string possible_udprn = property.Name;

                // Reset address fields for this property
                fullthoroughfare = "";
                buildingnumber = "";
                buildingname = "";
                town = "";
                addressasline = "";

                foreach (JsonProperty location in property.Value.EnumerateObject())
                {
                    switch (location.Name)
                    {
                        case "fullthoroughfare":
                            fullthoroughfare = location.Value.GetString() ?? "";
                            break;
                        case "addressasline":
                            addressasline = location.Value.GetString() ?? "";
                            break;
                        case "buildingname":
                            buildingname = location.Value.GetString() ?? "";
                            break;
                        case "buildingnumber":
                            buildingnumber = location.Value.GetString() ?? "";
                            break;
                        case "dependentlocality":
                            // ignored as in original
                            break;
                        case "town":
                            town = location.Value.GetString() ?? "";
                            break;
                        default:
                            break;
                    }
                }

                // Compare the address words to target
                int match_count = 0;
                string solid_location = addressasline.Replace(postcode, "").Trim();
                string[] comparison = solid_location.Split(SmartParametersV2016.spaceSplit);

                foreach (string tar in target)
                {
                    foreach (string comp in comparison)
                    {
                        if (string.Equals(tar, comp, StringComparison.OrdinalIgnoreCase))
                        {
                            match_count++;
                            break;
                        }
                    }
                }

                // Update utilityviewmodel if better match found
                if (string.IsNullOrEmpty(utilityviewmodel.udprn) || match_count > property_match)
                {
                    utilityviewmodel.udprn = possible_udprn;
                    property_match = match_count;
                    utilityviewmodel.House_No = buildingnumber;
                    utilityviewmodel.House_Name = buildingname;
                    utilityviewmodel.Street = fullthoroughfare;
                    utilityviewmodel.City_Town = town;
                }
            }

            return true;
        }
        internal static bool EDF_GetFormBuildId(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;
            string value;// = "";
            utilityviewmodel.form_build_id = "";

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string name = element1.GetAttributeValue("name", "");
                switch (name)
                {
                    case "form_build_id":
                        value = element1.GetAttributeValue("value", "");
                        if (!string.IsNullOrEmpty(value))
                        {
                            utilityviewmodel.form_build_id = value;
                            return true;
                        }
                        break;
                    default:
                        break;
                }
            }
            return false;
        }

        internal static bool EDF_GetConfirmation(HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1,
                        HtmlCol2;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string classname = element1.GetAttributeValue("class", "");
                if (SmartNibbyV2016.Check_Classname(classname, "mydot-content mydot-confirmation", true))
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        classname = element2.GetAttributeValue("class", "");
                        if (SmartNibbyV2016.Check_Classname(classname, "mydot-main-content", true))
                        {
                            if (!string.IsNullOrEmpty(element2.InnerText))
                            {
                                if (element2.InnerText.IndexOf("Confirmation") >= 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}