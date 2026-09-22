#if WINFORMS
using System.Windows.Forms;
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCubeMobile
{
    public class SmartLoginV2016
    {
        internal static async Task<bool> Login(
#if WINFORMS
                                    RichTextBox textBoxConsole,
                                    Action<string> set_errorMessage,   // Unoffical error message
#endif
                                    SignInViewModel signinviewmodel,    // errorMessage is ALWAYS empty when we come in here
                                    CancellationTokenSource signinCts,
                                    string website,
                                    string UserName,
                                    string PassWord
#if SMARTMAUI
                                    ,Action<string> setProgress = null
#endif
#if ANDROIDX
                                    ,TextView LoginLed1
                                    ,TextView LoginLed2
                                    ,TextView LoginLed3
                                    ,TextView LoginLed4
                                    ,TextView LoginLed5
                                    ,TextView LoginLed6
                                    ,TextView ErrorMessageText
#endif

                                    )

        {
            bool we_are_in = false;

            // When the page Loads - if we are not logged in leave the LoginStatusMessage there
            // Otherwise tells the user when their membership expires
            // and load up the UserName parameter with the logged in UserName
            string trace = "false",
                    subscriber = "false",
                    multimeter = "false",
                    administrator = "false",
                    userId = "",
                    passwordHash = "";
            string expiration1 = SmartParametersV2016.defaultDates,
                   expiration2 = SmartParametersV2016.defaultDates,
                   expiration3 = SmartParametersV2016.defaultDates,
                   previousLogon = SmartParametersV2016.defaultDates;
            DateTime last_login_time,
                    expiration_date1,
                    expiration_date2,
                    expiration_date3,
                    previousLogon_date,
                    defaultDate = SmartTimeV2016.ConvertDateTime(SmartParametersV2016.defaultDates);
            signinviewmodel.TargetUrl = new Uri(SmartParametersV2016.localWebsite);

            if (signinCts.Token.IsCancellationRequested)
            {
                signinviewmodel.errorMessage = "Operation cancelled";
                return false;
            }
            try
            {
                if (!SmartRoutinesV2018.CreateUriLogin(signinviewmodel,
                                                    website,
                                                    SmartParametersV2016.websiteLogin))
                {
                    // Bad news ...
                    return false;
                }
                else
                {
                    HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

                    string referer = Guid.NewGuid().ToString(SmartParametersV2016.guidFormat);
#if WINFORMS
                    //textBoxConsole.AppendText("Login: GUID sent " + referer + Environment.NewLine.ToString());
                    //textBoxConsole.ScrollToCaret();
#endif
#if SMARTMAUI
                    setProgress?.Invoke("Requesting login page...");
#endif
                    htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_SYNC_CHALLENGE(signinviewmodel,
                                                                                        signinCts.Token,
                                                                                        signinviewmodel.TargetUrl,
                                                                                        referer);
                    string guid_challenge_key = SmartEncryptionV2016.MangleGuidKey(referer);

                    if (!string.IsNullOrEmpty(signinviewmodel.errorMessage))
                    {
#if WINFORMS
                        set_errorMessage(signinviewmodel.errorMessage);
#endif
                        // If you are using IIS Express for this then fucking
                        //
                        //                   ==> DON'T <==
                        //
                        // Its a complete pile of useless steaming excrement.
                        // You will not get *ANYTHING* done using IIS Express,
                        // always, always, always use IIS. You have been warned!
                        //
                        // **ANDROID** (1)
                        // If you get "Failed to connect to localhost/127.0.0.1:80"
                        // then you probably have the WRONG website configured i.e.
                        // you have it set as "http://localhost" but this Android shite
                        // has no concept of 'localhost' so you have to give it
                        // the connection string of "http://chapmans" which is the
                        // system name of this PC. Also check to make sure KeasdonEnergy
                        // latest version is built correctly. That helps enormously...
                        //
                        // **ANDROID** (2)
                        // If you get "Network subsystem is down" its because you (probably)
                        // have the website pointed to 'localhost' which doesn't work for
                        // the emulator (or any Android tablet) so you have to change the
                        // website to point to 192.168.1.16 or something like that
                        // Same as if you get "Notfound" you are using the wrong website
                        //
                        // Always remember to 'kill' off the IIS Worker Process task
                        // after every error because a lot of IIS stuff is cached and even
                        // though a change you have made SHOULD work, the IIS Worker Process
                        // will look in its cache first and repeat a previous error before
                        // using the new changes you have made.  This IIS bollocks is A BITCH.
                        //
                        // If you get "Service unavailable" (HTTP: 503) make sure the
                        // DefaultAppPool in IIS is 'Started'
                        // If you get 'Service Unavailable' here then it could mean that
                        // the DefaultAppPool is 'stopped' and it needs to be re-started in
                        // IIS Manager
                        //
                        // If you get 'Not found' here, then you need to check the
                        // access rights to KeasdonEnergyV2023 and make sure you enable:
                        // 'DefaultAppPool'
                        //
                        // If you get 'Not found' here, then it means that (possibly)
                        // one or all of the following hasn't been installed:
                        // Microsoft .Net Runtime - 6.0.3 (x64)
                        // Microsoft.Net Runtime -6.0.3(x86)
                        // Microsoft.Net 6.0.3 - Windows Server Hosting
                        // Microsoft.Net Core 6.0.3 - Shared Framework(x64)
                        // Microsoft.Net Core 6.0.3 - Shared Framework(x86)
                        // the Project URL
                        // 
                        // You *DO NOT* need to create a 'Virtual Directory'!
                        // so don't be lured or bamboozled into creating one!
                        //
                        // YOU ALSO NEED A BINDING THAT SAYS:
                        // Type Hostname    Port    IP Address  Binding Information
                        // http localhost   80      *
                        // http CHAPMANS    80      *
                        // (where CHAPMANS is the host name of the website if you are testing Android)
                        // The SmartSwitch program looks for 'http://localhost/Identity/Account/Login'
                        // not 'http://localhost/KeasdonEnergyV2023/Identity/Account/Login' so unless
                        // the Project URL points to the 'right place', its never going to
                        // find it.  God, this stuff is absolute bollocks ...
                        //
                        // If you get 'internalServerError' then perhaps you still have
                        // KeasdonEnergyV2023 website built with Debug instead of Release?
                        // OR!!!!
                        // You have deleted all the bin and obj directories for KeasdonEnergyV2023 <= It was this one!!
                        // when you backed it up ... and haven't rebuilt it yet??
                        //
                        // If you get "No route to host" when you are running the XamarShit Android
                        // version then check out:
                        // https://www.codeproject.com/Questions/279206/SocketExcetption-No-Route-to-host
                        // which the Chinks advise you to turn off the WiFi and then turn
                        // it on again.  Worked for me!  (But other than that, you're going to struggle, sorry)
                        //
                        // If you get "Connection refused" for ANDROID then you have the TargetUrl
                        // set as "http://localhost/Account/Login" which isn't good ...
                        // You need to set the localwebsite = "http://10.0.2.2" which is
                        // a 'special' IP address which lets the Android Emulator use it
                        // as a 'localhost'.  The Android Emulator doesn't understand
                        // 'localhost' but it understands 10.0.2.2   What a fucking mess
                        //
                        //  *************You need to take this OUT for PRODUCTION!!!*******
                        //
                        //  OH! And you have to change the Bindings in IIS Manager to allow
                        //  for FUCKING ANYTHING to connect in and use Port 80. So my
                        // definitions for 'CHAPMANS' and localhost were fucking useless
                        // And you will see one for :
                        //  Type   Host Name     Port    Address
                        //  http     <blank>     80      *
                        //  which lets any fucker in.
                        //
                        // If you get "Connection failed" for ANDROID then make sure you have
                        // a valid TargetUrl e.g. http://192.168.0.137/Identity/Account
                        // and    android:usesCleartextTraffic="true"  in the mainfest file
                        //
                        //
                        //  If you get "Cleartext HTTP traffic to chapmans not permitted" 
                        // when you are developing ANDROID then you need to modify the
                        // AndroidManifest.xml file to include:
                        //
                        // android:usesCleartextTraffic="true"
                        //
                        // so the Android Chimps will let me pass my challenge string unhindered
                        // (You couldn't make this bollocks up, you really couldn't ...)
#if WINFORMS
                        textBoxConsole.AppendText("Login: setting Led1 to red " + signinviewmodel.errorMessage + Environment.NewLine.ToString());
                        textBoxConsole.ScrollToCaret();
#endif
                        signinviewmodel.Led1 = signinviewmodel.redColour;
                        signinviewmodel.errorMessage = signinviewmodel.errorMessage;
#if ANDROIDX
                        LoginLed1.SetTextColor(signinviewmodel.Led1);
                        ErrorMessageText.Text = signinviewmodel.errorMessage;
#endif
                        return false;
                    }

                    if (htmlDocument.RemainderOffset == 0)
                    {
                        // Is the IIS Default Application Pool started??
#if WINFORMS
                        textBoxConsole.AppendText("Login: setting Led1 to red " + "as HTML document empty" + Environment.NewLine.ToString());
                        textBoxConsole.ScrollToCaret();
#endif
                        signinviewmodel.Led1 = signinviewmodel.redColour;
                        signinviewmodel.errorMessage = "HTML document empty";
#if ANDROIDX
                        LoginLed1.SetTextColor(signinviewmodel.Led1);
                        ErrorMessageText.Text = "HTML document empty";
#endif
                        return false;
                    }
                    else
                    {
#if WINFORMS
                        //textBoxConsole.AppendText("Login: setting Led1 to green " + Environment.NewLine.ToString());
                        //textBoxConsole.ScrollToCaret();
#endif
                        signinviewmodel.Led1 = signinviewmodel.greenColour;
#if ANDROIDX
                        LoginLed1.SetTextColor(signinviewmodel.Led1);
#endif
                        // We are back successfully ... Now try and find the Email ID and get its value
                        IList<HtmlAgilityPack.HtmlNode> HtmlCol1;
                        string challenge = "";
                        string request_verification = "";
                        HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(htmlDocument.DocumentNode, ".//input");
                        foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
                        {
                            if (element1.Id == "Rays")
                            {
                                // Decode it because it was encoded on the website side
                                // Oh. HttpUtility is missing (no surprise there)
                                // So its been changed to this WebUtility bollocks which
                                // - not unsurprisingly for the Chimps, encodes and decodes
                                // differently! I've only wasted TWO FUCKING DAYS OF MY LIFE
                                // on this shit.  As you change it here, though, it has
                                // to match the same in KeasdonEnergy which encodes it.
                                //challenge = HttpUtility.UrlDecode(element1.GetAttributeValue("value", ""));
                                challenge = System.Net.WebUtility.UrlDecode(element1.GetAttributeValue("value", ""));
                            }
                            else
                            {
                                string name = element1.GetAttributeValue("name", "");
                                if (name == "__RequestVerificationToken")
                                {
                                    request_verification = element1.GetAttributeValue("value", "");
                                }
                            }
                            if (!string.IsNullOrEmpty(challenge) && !string.IsNullOrEmpty(request_verification))
                            {
#if WINFORMS
                                //textBoxConsole.AppendText("Login: breaking for non-empty challenge and non-empty Request Verification token" + Environment.NewLine.ToString());
                                //textBoxConsole.ScrollToCaret();
#endif
                                break;
                            }
                        }
                        // The challenge is "RaysText" sent over from the website Login page
                        if (string.IsNullOrEmpty(challenge) || string.IsNullOrEmpty(request_verification))
                        {
#if WINFORMS
                            //textBoxConsole.AppendText("Login: setting Led2 red" + Environment.NewLine.ToString());
                            //textBoxConsole.ScrollToCaret();
#endif
                            signinviewmodel.Led2 = signinviewmodel.redColour;
                            signinviewmodel.errorMessage = SmartParametersV2016.challengeFailed;
#if ANDROIDX
                            LoginLed2.SetTextColor(signinviewmodel.Led2);
                            ErrorMessageText.Text = signinviewmodel.errorMessage;
#endif
                            return false;
                        }
#if WINFORMS
                        //textBoxConsole.AppendText("Login: GCK " + guid_challenge_key + " CHALLENGE " + challenge + Environment.NewLine.ToString());
                        //textBoxConsole.ScrollToCaret();
#endif
                        string challenge_time = SmartEncryptionV2016.DoTheBiz("", guid_challenge_key, challenge, false);
#if WINFORMS
                        //textBoxConsole.AppendText("Login: challenge time " + challenge_time + Environment.NewLine.ToString());
                        //textBoxConsole.ScrollToCaret();
#endif
                        if (!string.IsNullOrEmpty(challenge_time))
                        {
#if WINFORMS
                            //textBoxConsole.AppendText("Login: setting Led2 green" + Environment.NewLine.ToString());
                            //textBoxConsole.ScrollToCaret();
#endif
                            signinviewmodel.Led2 = signinviewmodel.greenColour;
#if ANDROIDX
                            LoginLed2.SetTextColor(signinviewmodel.Led2);
#endif
                            string guid = Guid.NewGuid().ToString(SmartParametersV2016.guidFormat);
#if WINFORMS
                            //textBoxConsole.AppendText("Login: new GUID " + guid + Environment.NewLine.ToString());
                            //textBoxConsole.ScrollToCaret();
#endif
                            // At the moment we do THIS fudging of the GUID (but we could do anything)
                            if (!string.IsNullOrEmpty(guid))
                            {
                                string guid_key = SmartEncryptionV2016.MangleGuidKey(guid);
                                string encrypted_username = "",
                                        encrypted_password = "",
                                        encrypted_response = "";

                                // Username is encrypted with Guid
                                encrypted_username = SmartEncryptionV2016.DoTheBiz(UserName, guid_key, "", true);
                                // as is the Password
                                encrypted_password = SmartEncryptionV2016.DoTheBiz(PassWord, guid_key, "", true);
                                // and finally the challenge Reponse
                                encrypted_response = SmartEncryptionV2016.DoTheBiz(challenge_time, guid_key, "", true);

                                signinviewmodel.loggedInUser = new Potential();
                                signinviewmodel.jsonReturned = false;

                                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("Input.UserName", encrypted_username),
                                    new KeyValuePair<string, string>("Input.Password", encrypted_password),
                                    new KeyValuePair<string, string>("Input.Raymondo", encrypted_response),     // Wow!  We got there by the skin of our teeth!
                                    new KeyValuePair<string, string>("__RequestVerificationToken", request_verification),
                                    new KeyValuePair<string, string>("Input.RememberMe", "false")
                                };

#if SMARTMAUI
                                setProgress?.Invoke("Authenticating credentials...");
#endif
                                htmlDocument = await SmartBobV2017.HTTPCLIENT_POST_ASYNC_LOGIN(signinviewmodel,
                                                                                                signinCts.Token,
                                                                                                keyValues,
                                                                                                signinviewmodel.TargetUrl,
                                                                                                guid,
                                                                                                loggedin => signinviewmodel.loggedInUser = loggedin);
                                if (!string.IsNullOrEmpty(signinviewmodel.errorMessage))
                                {
                                    signinviewmodel.Led3 = signinviewmodel.redColour;
                                    signinviewmodel.errorMessage = SmartParametersV2016.websiteFailedToRespond;
#if ANDROIDX
                                    LoginLed3.SetTextColor(signinviewmodel.Led3);
                                    ErrorMessageText.Text = signinviewmodel.errorMessage;
#endif
#if WINFORMS
                                    set_errorMessage(signinviewmodel.errorMessage);
#endif
                                    return false;
                                }
                                if (signinviewmodel.jsonReturned)
                                {
                                    //
                                    // This htmlDocument is just the one returned from the POST
                                    // we really need to do a GET on www.keasdon.co.uk to find the Log Off <a> link
                                    // No time for that right now - one day maybe ...
                                    //
                                    signinviewmodel.Led3 = signinviewmodel.greenColour;
#if ANDROIDX
                                    LoginLed3.SetTextColor(signinviewmodel.Led3);
#endif
                                    int random_key = signinviewmodel.loggedInUser.Random_Key;
                                    // Decrypt Time Now (the first key)
                                    string TN = SmartEncryptionV2016.DoTheBiz("", random_key.ToString(), signinviewmodel.loggedInUser.November, false);
                                    // Username is encrypted with 'Time Now'
                                    if (UserName == SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Uniform, false))          // Username
                                    {
                                        signinviewmodel.Led4 = signinviewmodel.greenColour;
#if ANDROIDX
                                        LoginLed4.SetTextColor(signinviewmodel.Led4);
#endif
                                        string slast_login_time = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Lima, false);
                                        last_login_time = SmartTimeV2016.ConvertDateTime(slast_login_time);
                                        trace = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Tango, false);            // Trace                    args[2]
                                        subscriber = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Sierra, false);      // Subscriber               args[8]
                                        multimeter = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Mike, false);        // Multi-Meter              args[9]
                                        administrator = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Alpha, false);    // Administrator            args[10]
                                        expiration1 = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Echo, false);       // Electricity Expiration   args[5]
                                        expiration2 = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Golf, false);       // Gas Expiration           args[6]
                                        expiration3 = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Whisky, false);     // Water Expiration
                                        // Gonna try and use this instead of the Password
                                        // to encrypt our DATA!
                                        userId = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Indigo, false);             // Special param
                                        previousLogon = SmartEncryptionV2016.DoTheBiz("", TN, signinviewmodel.loggedInUser.Papa, false);    // Previous logon
                                        passwordHash = signinviewmodel.loggedInUser.Charlie;
                                        we_are_in = true;
#if SMARTMAUI
                                        setProgress?.Invoke("Login successful, loading data...");
#endif
                                        // Clear down the error message
                                        signinviewmodel.errorMessage = "";

                                        // Usernames SHOULD be:
                                        // a) Non-empty (i.e. they should contain 'something')
                                        // b) Lowercase or Uppercase
                                        // c) Authenticated

                                        defaultDate = SmartRoutinesV2018.DateTimeParseExact(SmartParametersV2016.defaultDates.Substring(0, 10), "d", SmartParametersV2016.defaultCulture);

                                        // We have a name so - we are logged in - so we can clear the 'You need to be logged in' message
                                        // (Doesn't mean we can RUN SMARTSWITCH though, at this stage)

                                        signinviewmodel.LoginStatusMessage = "";

                                        // Electricity
                                        if (expiration1 == SmartParametersV2016.defaultDates)
                                        {
                                            expiration_date1 = defaultDate;
                                        }
                                        else
                                        {
                                            expiration_date1 = SmartTimeV2016.ConvertDateTime(expiration1);
                                        }
                                        // Gas
                                        if (expiration2 == SmartParametersV2016.defaultDates)
                                        {
                                            expiration_date2 = defaultDate;
                                        }
                                        else
                                        {
                                            expiration_date2 = SmartTimeV2016.ConvertDateTime(expiration2);
                                        }
                                        // Water
                                        if (expiration3 == SmartParametersV2016.sqldefaultdates)
                                        {
                                            expiration_date3 = defaultDate;
                                        }
                                        else
                                        {
                                            expiration_date3 = SmartTimeV2016.ConvertDateTime(expiration3);
                                        }
                                        previousLogon_date = SmartTimeV2016.ConvertDateTime(previousLogon);
                                        // Either they are both null or both valid expiry (within 30 days or 1 year)
                                        // If they pay £20 then make expiry to + one year
                                        // If they only pay £10 then make ONLY one (e.g. Electricity) to + one year
                                        // The Gas stays expired at 30 days

                                        // JUST WHAT I NEEDED!!!  After struggling all day to get Firefox working
                                        // They are either:
                                        //      On the 30-day free trial
                                        // or   they have paid a subscription for at least ONE of the resources
                                        // or   their expiration dates never expire (e.g. RAY)
                                        // So set them up to go ... they will see the Meter, but they may not
                                        // see the Costs, 'cos their expiration date(s) is(are) tested in Smart_Analyze
                                        signinviewmodel.LoginKeys = new LoginKeys()
                                        {
                                            userName = UserName,            // Lowercase or uppercase - max 18 characters
                                            passwordHash = passwordHash,    // Might turn this to <blank> .. no, might use Hash
                                            trace = Convert.ToBoolean(trace),                  // Sent as 'true' or 'false'
                                            expirationDate1 = expiration_date1, //oString(SmartParametersV2016.defaultCulture), // sent in 'en-GB' format
                                            expirationDate2 = expiration_date2, //.ToString(SmartParametersV2016.defaultCulture),
                                            expirationDate3 = expiration_date3, //.ToString(SmartParametersV2016.defaultCulture),
                                            subscriber = Convert.ToBoolean(subscriber),// Sent as 'true' or 'false'
                                            multimeter = Convert.ToBoolean(multimeter),                 // Sent as 'true' or 'false'
                                            administrator = Convert.ToBoolean(administrator),              // Sent as 'true' or 'false'
                                            lastLoginTime = last_login_time, //.ToString(SmartParametersV2016.defaultCulture),// Sent as a DateTime
                                            userId = userId,                     // unique netcore-KeasdonEnergy.db Id
                                            previousLogonDate = previousLogon_date //.ToString(SmartParametersV2016.defaultCulture)
                                        };
                                        //signinviewmodel.LoginKeys = Keys;

                                        // Pick up this token which should have been set on the first 'GET'
                                        //string cookie_name = "__AntiXsrfToken";
                                        //signinviewmodel.anti_token = new AntiXsrfToken();

                                        //signinviewmodel.anti_token_string = request_verification;

                                        //if (SmartRoutinesV2018.DecodeCookies(signinviewmodel.website,
                                        //            cookie_name,
                                        //            signinviewmodel.cookies,
                                        //            signinviewmodel.anti_token))
                                        //{
                                        //if (string.IsNullOrEmpty(signinviewmodel.anti_token.cookie_value))
                                        //{
                                        //    return false;
                                        //}
                                        //signinviewmodel.anti_token_string = UserName +
                                        //                        SmartParametersV2016.unitSeparator +
                                        //                        signinviewmodel.anti_token.cookie_value +
                                        //                        SmartParametersV2016.unitSeparator +
                                        //                        signinviewmodel.anti_token.cookie_path +
                                        //                        SmartParametersV2016.unitSeparator +
                                        //                        signinviewmodel.anti_token.cookie_domain +
                                        //                        SmartParametersV2016.unitSeparator +
                                        //                        signinviewmodel.anti_token.cookie_secure.ToString() +
                                        //                        SmartParametersV2016.unitSeparator +
                                        //                        signinviewmodel.anti_token.cookie_timestamp.ToString(SmartParametersV2016.defaultCulture); // en-GB format

                                        signinviewmodel.antiTokenString = UserName +
                                                                SmartParametersV2016.unitSeparator +
                                                                request_verification +
                                                                SmartParametersV2016.unitSeparator +
                                                                DateTime.UtcNow.ToString(SmartParametersV2016.defaultCulture); // UTC time en-GB format


                                        signinviewmodel.Led5 = signinviewmodel.greenColour;
#if ANDROIDX
                                        LoginLed5.SetTextColor(signinviewmodel.Led5);
#endif
                                        // Safe to do this now ... the cookies are initialized and trace is set
                                        // AND the fucking UserName is setup (!!!!! You PLONKER!!)
                                        // This works because we're not updating the main thread

                                        // Moved to the calling program SignIn!
                                        //await SmartRoutinesV2018.CheckTrace_SignIn(signinviewmodel, signinviewmodel.signinToken, "Logged in", true);

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ArgumentNullException)
            {
                signinviewmodel.errorMessage = SmartParametersV2016.loginDatabaseFailed;
            }
            catch (FormatException)
            {
                signinviewmodel.errorMessage = SmartParametersV2016.loginDatabaseFailed;
            }
            catch (ArgumentException)
            {
                signinviewmodel.errorMessage = SmartParametersV2016.loginDatabaseFailed;
            }
            catch (FileNotFoundException)
            {
                signinviewmodel.errorMessage = SmartParametersV2016.loginDatabaseFailed;
            }
            if (!we_are_in)
            {
                signinviewmodel.errorMessage = SmartParametersV2016.unableToAuthenticate;
            }
#if WINFORMS
            set_errorMessage(signinviewmodel.errorMessage);
#endif
            return false;
        }

        internal static async Task<bool> Logout(MainViewModel ourviewmodel)
        {
            try
            {
                ourviewmodel.TargetUrl = new Uri(SmartParametersV2016.localWebsite);

                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument(); ;
                if (!SmartRoutinesV2018.CreateUri(ourviewmodel, ourviewmodel.website, @"/"))
                {
                    return false;
                }

                List<string> headers = new List<string>();

                htmlDocument = await SmartBobV2017.HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                        ourviewmodel.TargetUrl,
                                                                        ourviewmodel.quitCts.Token,
                                                                        headers,
                                                                        "", // Authenticity token
                                                                        new Guid());
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }
                if (htmlDocument.RemainderOffset > 0)
                {
                    string viewstate = "";
                    HtmlAgilityPack.HtmlNode __viewstate = htmlDocument.GetElementbyId("__VIEWSTATE");
                    if (__viewstate != null)
                    {
                        viewstate = __viewstate.GetAttributeValue("value", "");
                    }
                    string viewstategenerator = "";
                    HtmlAgilityPack.HtmlNode __viewstategenerator = htmlDocument.GetElementbyId("__VIEWSTATEGENERATOR");
                    if (__viewstategenerator != null)
                    {
                        viewstategenerator = __viewstategenerator.GetAttributeValue("value", "");
                    }
                    string eventvalidation = "";
                    HtmlAgilityPack.HtmlNode __eventvalidation = htmlDocument.GetElementbyId("__EVENTVALIDATION");
                    if (__eventvalidation != null)
                    {
                        eventvalidation = __eventvalidation.GetAttributeValue("value", "");
                    }

                    List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("__EVENTTARGET", "ctl00$ctl16$ctl02$ctl00"),
                        new KeyValuePair<string, string>("__EVENTARGUMENT", ""),
                        new KeyValuePair<string, string>("__VIEWSTATE", viewstate),
                        new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", viewstategenerator),
                        new KeyValuePair<string, string>("__EVENTVALIDATION", eventvalidation)
                };

                    headers.Clear();
                    await SmartBobV2017.HTTPCLIENT_POST_ASYNC(ourviewmodel,
                                                            ourviewmodel.TargetUrl,
                                                            ourviewmodel.quitCts.Token,
                                                            keyValues,
                                                            headers,
                                                            new Guid());
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return false;
        }
    }
}