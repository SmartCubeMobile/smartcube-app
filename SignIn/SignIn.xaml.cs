using System.Globalization;

#if WINFORMS
using System.Windows.Forms;
using System.Threading.Tasks;
using SmartDashboard;
using System.Windows;
#endif

#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.IO;
#endif

#if UWP
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.System;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Threading;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
#endif

#if WINUI
using Windows.System;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Threading;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.IO;
using Microsoft.UI.Xaml.Media.Imaging;
#endif
// This churning bitch NEVER shits the fuck up never.  Churn, churn, churn, churn churn ,..

#if ANDROIDX
// Here we fucking go again ....  =:-[
using Android.Content.Res;
using Android.Content;
using Android.Views;
using Android.Views.Animations;
using AndroidX.AppCompat.App;
using Android.App.AppSearch;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
#endif

#if SMARTMAUI
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace SmartCubeMobile
{
    public class PageSizeX
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }

    // <summary>
    // Interaction logic for SignIn.xaml
    // </summary>
#if WINFORMS
    public partial class SignIn : Form
    {
        public SignInViewModel signinviewmodel;
        public string key;
        public CancellationTokenSource signinCts = new CancellationTokenSource();
        private static TickScheduler scheduler;
#endif
#if WPF
    public partial class SignIn : Window
    {
        public static SignInViewModel signinviewmodel;
        private CancellationTokenSource signinCts = new CancellationTokenSource();
        private static TickScheduler scheduler;

#endif
#if UWP || WINUI 
    public sealed partial class SignIn : Page
    {
        public static SignInViewModel signinviewmodel;
        private CancellationTokenSource signinCts = new CancellationTokenSource();
        private static TickScheduler scheduler;

        public SignIn()
        {
            this.InitializeComponent();
        }
#endif
#if SMARTMAUI
    public sealed partial class SignIn : ContentView
    {
        public static SignInViewModel signinviewmodel;
        private CancellationTokenSource signinCts = new CancellationTokenSource();
        private static TickScheduler scheduler;
#endif
#if ANDROIDX    
    public partial class SignIn : LinearLayout
    {
        public static SignInViewModel signinviewmodel;
        public string key = "";
        CancellationTokenSource signinCts = new CancellationTokenSource();

        private TickScheduler scheduler;

        // ✅ Store handler so it can be removed later (no lambda leak)
        private EventHandler loginHandler;

        // ✅ UI controls
        ImageButton English;
        ImageButton French;
        ImageButton German;
        ImageButton Spanish;
        
        Button ResetUTCButton;
        TextView TimeOffsetMessageText;
        TextView LoginStatusMessageText;
        
        EditText UserName;
        EditText PassWord;
        Button LoginButton;
        TextView ErrorMessageText;
        TextView CreateAccountText;
        TextView SignInCultureText;
        TextView VersionMessageText;
        TextView PlatformText;
        
        TextView LoginLed1;
        TextView LoginLed2;
        TextView LoginLed3;
        TextView LoginLed4;
        TextView LoginLed5;
        TextView LoginLed6;
#endif
#if WINFORMS
        public static System.Windows.Forms.BindingSource signinBindingSource = new System.Windows.Forms.BindingSource();
#endif

#if WINFORMS || WPF || UWP
        public SignIn(string WhatAmI, SignInViewModel signinVM, TickScheduler Scheduler)
        {
#endif
#if WINUI || SMARTMAUI
        public void Initialize(SignInViewModel signinVM)     // WINUI - cannot provide a parameter here - Thanks Chimps!!
        {
#endif
            // =========================================================
            // ✅ ANDROID CONSTRUCTOR (NOT async!)
            // =========================================================
#if ANDROIDX
        public SignIn(AppCompatActivity activity, Context context, string whatAmI, string version, TickScheduler Scheduler, SignInViewModel signinVM)
            : base(activity)
        {   
#endif
            bool keepresdict = false;
            try
            {
#if WPF || UWP || SMARTMAUI
                InitializeComponent();
#endif

                signinviewmodel = signinVM;
                scheduler = new TickScheduler();
#if ANDROIDX
                // Bind UI
                BindViews(activity);

                // Wire events safely
                WireEvents(activity);

                // Set platform info
                PlatformText.Text = whatAmI;
                VersionMessageText.Text += version;

                signinviewmodel.Platform = whatAmI;

                // 🚀 Run async init safely
                _ = InitializeAsync(activity);
#endif

#if WPF || UWP || WINUI
                DataContext = signinviewmodel;
#endif
#if SMARTMAUI
                BindingContext = signinviewmodel;
#endif
#if WPF || UWP || WINUI
                //signinviewmodel.kesmall = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\keasdon_energy_small.jpg", UriKind.Absolute));
                //signinviewmodel.lightbulbicon = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\light_bulb_icon.png", UriKind.Absolute));
                String imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "keasdon_energy_small.jpg");
                signinviewmodel.kesmall = new BitmapImage(new Uri(imagesPath, UriKind.Absolute));
                String iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "light_bulb_icon.png");
                signinviewmodel.lightbulbicon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));

#endif
#if SMARTMAUI
                signinviewmodel.kesmall = ImageSource.FromFile("keasdon_energy_small.jpg");
                signinviewmodel.lightbulbicon = ImageSource.FromFile("light_bulb_icon.png");
#endif
#if WPF || UWP || SMARTMAUI
                FrontEndGUI.SetSignInWindowDataContext(this, signinviewmodel);
#endif
#if WINUI
                // As you might expect from the Chimps, the Window does not
                // have a DataContext property so, if you want to see text
                // on your screen !!!! you have to bind the SignIn view model
                // to the first Grid (if you've got one of course) What complete
                // and utter bollocks this entire shit is
                FrontEndGUI.SetSignInWindowDataContext(this.RootGrid, signinviewmodel);
#endif
                // For all - WINFORMS, WPF and ANDROID but *not* UWP || WINUI
#if WINFORMS || WPF
                signinviewmodel.Platform = WhatAmI;
#endif
#if SMARTMAUI
                signinviewmodel.Platform = "MAUI";
#endif
#if WPF || UWP || WINUI || SMARTMAUI
                signinviewmodel.VersionMessage = Assembly.GetEntryAssembly().GetName().Version.ToString();
#endif
                // Platform for UWP and WINUI gets set in the 'OnNavigatedTo' event
                switch (signinviewmodel.Platform)
                {
                    case SmartParametersV2016.WINFORMS:
                        break;
                    case SmartParametersV2016.WPF:
                        break;
                    case SmartParametersV2016.WinUI:
                        break;
                    case SmartParametersV2016.Android:
                        // This is because this Android shite doesn't 'know'
                        // or understand 'localhost' even tho everything else
                        // in the Universe does.  Typical Garble/Andrip shite
                        signinviewmodel.website = SmartParametersV2016.localWebsite;// windowsWebsite;
                                                                                    // Couldn't FOR THE LIFE OF ME
                                                                                    // get the android:onClick="OnLogInButtonClicked" shit
                                                                                    // to work, so I've had to resort to this ....
                                                                                    // (anything to save my sanity)

                        break;
                    case SmartParametersV2016.iOS:
                        break;
                    default:
                        // Use default local signinviewmodel.website = SmartParametersV2016.windowsWebsite;
                        break;
                }
                if (signinviewmodel.languageDictionaries.Count == 0)
                {
                    // Make sure the .xml files have "Embedded Resource"
                    // and "Copy always" attributes ...
                    // With thanks to: https://www.wpfsharp.com/2012/01/26/how-to-load-a-dictionarystyle-xaml-file-at-run-time/
                    // string Directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                    // But not setting the Platform doesn't stop us
                    // from loading the dictionaries for UWP || WINUI
                    signinviewmodel.languageDictionaries = SmartRoutinesV2018.BuildLanguageDictionaries(@"Resources");
#endif
#if ANDROIDX
                    signinviewmodel.languageDictionaries = SmartRoutinesV2018.BuildLanguageDictionaries("");// @"Resources/values");
#endif
                    if (signinviewmodel.languageDictionaries.Count == 0)
                    {
                        signinviewmodel.errorMessage = SmartParametersV2016.catastrophe;
                        return;
                    }
                }
                else
                {
                    keepresdict = true;
                }
            }
            catch (Exception ex)
            {
                //
                // If you get fucking XAML parse errors from this fucking
                // bollocks twaddle then make sure:
                // keasdon_energy_small.jpg and light_bulb_icon.png have properties
                // Build action:             EmbeddedResource
                // Copy to output directory: Copy always
                //
                // Ha ha!  In your dreams!  This bollocks can't even FIND
                // Images/keasdon_energy_small.jpg let alone copy it!!!  Chimps!!
                // Because we might not have any Language Dictionaries to look up ..
                if (signinviewmodel.languageDictionaries.Count > 0)
                {
                    DisplayError("FatalError", signinviewmodel);
                }
                signinviewmodel.errorMessage = "FatalError" + ": " + ex.Message;
                return;
            }
#if WINFORMS
            signinBindingSource.DataSource = signinviewmodel;
#endif
            // For WPF               - CultureInfo.CurrentCulture is "en-GB"
            // For Android emulator  - CultureInfo.CurrentCulture is "en-US"  !!!!
            // What a complete pile of inconsistent bollocks!!!
            if (!keepresdict)
            {
#if WINFORMS || WPF
                // Using CultureInfo.CurrentCulture HERE returns 'en-CA'            
                signinviewmodel.signincultureinfo = CultureInfo.CurrentCulture;
#endif
#if UWP || WINUI
                // ... but using CultureInfo.CurrentCulture HERE returns 'en-GB' !!!!
                // CultureInfo returns-en-GB = Thread.CurrentThread.CurrentCulture;
                // CultureInfo returns-en-GB = CultureInfo.CurrentCulture;
                // so we use this
                signinviewmodel.signincultureinfo = CultureInfo.InstalledUICulture;
#endif
#if ANDROIDX
                // ... but using CultureInfo.CurrentCulture HERE returns 'en-GB' !!!!
                // CultureInfo returns-en-GB = Thread.CurrentThread.CurrentCulture;
                // CultureInfo returns-en-GB = CultureInfo.CurrentCulture;
                // so we use this
                signinviewmodel.signincultureinfo = CultureInfo.InstalledUICulture;
#endif
#if ANDROIDX
                // ... but using CultureInfo.CurrentCulture HERE returns 'en-US' !!!!
                // CultureInfo.InstalledUICulture HERE returns 'en-US
#endif
                FixLanguage(signinviewmodel, signinviewmodel.signincultureinfo.TwoLetterISOLanguageName);
                signinviewmodel.SignInCulture = "Local: " + signinviewmodel.signincultureinfo.ToString();
#if ANDROIDX
                SignInCultureText.Text = signinviewmodel.SignInCulture;
#endif
            }
            // Set these two just in case            
            if (signinCts == null)
            {
                signinCts = new CancellationTokenSource();
            }

            // How ON EARTH can I possibly think with Mrs Bullshit banging on as usual???

            //#if IOS
            // https://forums.xamarshit.com/discussion/104922/how-to-set-get-the-app-version-number-and-build-for-cross-platform
            //
            //[assembly: XamarShit.Forms.Dependency(typeof(Your.Namespace.iOS.Version_iOS))]
            //namespace Your.Namespace.iOS
            //{
            //    public class Version_iOS : IAppVersion
            //    {
            //        public string GetVersion()
            //        {
            //            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
            //        }
            //        public int GetBuild()
            //        {
            //            return int.Parse(NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString());
            //        }
            //    }
            //}
            //#endif


#if WPF || UWP
            this.Loaded += async (s, e) => await OnLoaded(s, e);
            UserName.GotFocus += OnFocusUsername;
            PassWord.GotFocus += OnFocusPassword;
            ResetUTC.Click += OnResetButtonClicked;
            LoginButton.Click += async (s, e) => await OnLogInButtonClicked(s, e);
#endif
#if SMARTMAUI
            LoginButton.Clicked += async (s, e) => await OnLogInButtonClicked(s, e);
            UserName.Focus();
#endif
#if WPF
            UserName.Focus();
#endif
#if ANDROIDX
            UserName.RequestFocus();
#endif
            return;
        }
        // =========================================================
        // ✅ VIEW BINDING
        // =========================================================
#if ANDROIDX
        private void BindViews(AppCompatActivity activity)
        {
            English = activity.FindViewById<ImageButton>(Resource.Id.English);
            French = activity.FindViewById<ImageButton>(Resource.Id.French);
            German = activity.FindViewById<ImageButton>(Resource.Id.German);
            Spanish = activity.FindViewById<ImageButton>(Resource.Id.Spanish);

            ResetUTCButton = activity.FindViewById<Button>(Resource.Id.ResetUTC);

            TimeOffsetMessageText = activity.FindViewById<TextView>(Resource.Id.TimeOffsetMessage);
            LoginStatusMessageText = activity.FindViewById<TextView>(Resource.Id.LoginStatusMessage);

            UserName = activity.FindViewById<EditText>(Resource.Id.UserName);
            PassWord = activity.FindViewById<EditText>(Resource.Id.PassWord);
            LoginButton = activity.FindViewById<Button>(Resource.Id.LoginButton);

            ErrorMessageText = activity.FindViewById<TextView>(Resource.Id.ErrorMessage);
            CreateAccountText = activity.FindViewById<TextView>(Resource.Id.CreateAccount);
            SignInCultureText = activity.FindViewById<TextView>(Resource.Id.SignInCulture);
            VersionMessageText = activity.FindViewById<TextView>(Resource.Id.VersionMessage);
            PlatformText = activity.FindViewById<TextView>(Resource.Id.Platform);

            LoginLed1 = activity.FindViewById<TextView>(Resource.Id.LoginLed1);
            LoginLed2 = activity.FindViewById<TextView>(Resource.Id.LoginLed2);
            LoginLed3 = activity.FindViewById<TextView>(Resource.Id.LoginLed3);
            LoginLed4 = activity.FindViewById<TextView>(Resource.Id.LoginLed4);
            LoginLed5 = activity.FindViewById<TextView>(Resource.Id.LoginLed5);
            LoginLed6 = activity.FindViewById<TextView>(Resource.Id.LoginLed6);

            // All clock shit now done BEFORE we hit this routine
        }
#endif
        // =========================================================
        // ✅ EVENT WIRING (NO LAMBDA LEAKS)
        // =========================================================
#if ANDROIDX
        private void WireEvents(AppCompatActivity activity)
        {
            English.Click += OnClickLanguage;
            French.Click += OnClickLanguage;
            German.Click += OnClickLanguage;
            Spanish.Click += OnClickLanguage;

            ResetUTCButton.Click += async (s, e) =>
                await CheckStartupConditions(activity);

            //// ✅ SAFE handler (stored reference)
            loginHandler = async (s, e) =>
                await OnLogInButtonClicked(s, e, activity);

            LoginButton.Click += loginHandler;
        }
#endif

        // =========================================================
        // ✅ ASYNC INITIALIZATION (REPLACES async ctor)
        // =========================================================
#if ANDROIDX
        private async Task InitializeAsync(AppCompatActivity activity)
        {
            try
            {
                await CheckStartupConditions(activity);

                UserName?.RequestFocus();
            }
            catch (Exception ex)
            {
                signinviewmodel.errorMessage = ex.Message;
                DisplayError("FatalError", signinviewmodel);
            }
        }
#endif
#if WINFORMS
        internal static void DisplayError(string key, SignInViewModel signinviewmodel)
#endif
#if WPF || UWP || WINUI || SMARTMAUI
        internal void DisplayError(string key, SignInViewModel signinviewmodel)
#endif
#if ANDROIDX
        internal void DisplayError(string key, SignInViewModel signinviewmodel)
#endif
        {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
            signinviewmodel.errorMessage = BesetByChimps(signinviewmodel, key);
#endif
#if ANDROIDX
            ErrorMessageText.Text = BesetByChimps(signinviewmodel, key);
#endif
            return;
        }
        internal async Task CheckStartupConditions(
#if ANDROIDX
                                                    AppCompatActivity signinActivity
#endif
            )
        {
            signinviewmodel.SignInEnabled = false;
            if (!SmartNibbyV2016.NetworkAvailability())
            {
                DisplayError("EnsureConnectedToInternet", signinviewmodel);
                // They have been told, and if they enter a Username and Password and hit return
                // we have to check that nothing happens ...
            }
            else
            {
                // Nothing for WINFORMS ... yet (!)
                DateTime? networkTime = await SmartTimeV2016.GetNetworkTimeAsync();
                if (networkTime != null)
                {
                    DateTime UTC = networkTime.Value;

                    signinviewmodel.utcOffset = UTC - DateTime.UtcNow;  // Usually only about 3ms
                    // HERE DESPITE BEING FUCKING INTERRUPTED!!

                    // I think  I need to add the utcOffset in here to get the PROPER local time
                    // Yes, because you can change the Local Time of this PC to something dumb
                    // The utcOffset should be milliseconds btw
                    DisplayError("PleaseSignin", signinviewmodel);
                    FullReset(signinviewmodel);
                    signinviewmodel.SignInEnabled = true;
                }
                else
                {
                    DisplayError("Problem UTC", signinviewmodel);
#if WPF
                    ResetUTC.Visibility = Visibility.Visible;
#endif
#if UWP || WINUI || SMARTMAUI
                    signinviewmodel.ResetVisibility = true;
#endif
#if ANDROIDX
                    signinviewmodel.ResetVisibility = true;
#endif
#if ANDROIDX
                    ResetUTCButton.Visibility = Android.Views.ViewStates.Visible;
#endif
                }
            }
            return;
        }

        
#if WPF
        // Has no effect for UWP || WINUI?
        internal async Task OnLoaded(object sender, EventArgs e)
        {
            // Hope to fuck this works ...
            await CheckStartupConditions();
            return;
        }
#endif

#if WINUI
        private async Task OnLoaded(object sender, RoutedEventArgs e)
        {
            // There is no Window 'OnLoaded' event so you have to
            // tie the event to the 'RootGrid' if you have one of course
            // Complete and utter shit, as always. Chimp-infested bollocks
            // Hope to fuck this works ...
            await CheckStartupConditions();
            return;
        }
#endif


#if WPF || UWP || WINUI
        internal void OnClickLanguage(object sender, RoutedEventArgs e)
#endif
#if SMARTMAUI
        internal void OnClickLanguage(object sender, EventArgs e)
#endif
#if ANDROIDX
        internal void OnClickLanguage(object sender, EventArgs e)
#endif
#if WPF || UWP || WINUI || SMARTMAUI
        {
            // Hope to fuck this works ...
            if (sender != null)
            {
                Button button = (Button)sender;
                if (button != null)
                {
#if SMARTMAUI
                    FixLanguage(signinviewmodel, (string)button.StyleId);
#else
                    FixLanguage(signinviewmodel, (string)button.Tag);
#endif
                }
            }
            //FullReset(signinviewmodel);
            signinviewmodel.LoginStatusMessage = BesetByChimps(signinviewmodel, "LoginStatusMessage");
            DisplayError("PleaseSignin", signinviewmodel);
            return;
        }
#endif
#if ANDROIDX
        {
            // Hope to fuck this works ...
            if (sender != null)
            {
                ImageButton button = (ImageButton)sender;
                if (button != null)
                {
                    FixLanguage(signinviewmodel, (string)button.Tag);
                }
            }
            FullReset(signinviewmodel);
            DisplayError("PleaseSignin", signinviewmodel);
            return;
        }
#endif

        internal static void FixLanguage(SignInViewModel signinviewmodel, string tag)
        {
            foreach (LanguageDictionary languageDictionary in signinviewmodel.languageDictionaries)
            {
                if (languageDictionary.languageCode == tag)
                {
                    // Clear any previous dictionaries loaded
#if WINFORMS
                    //Resources.MergedDictionaries.Clear();
                    // Add in newly loaded Resource Dictionary
                    //if (Resources.MergedDictionaries.Count == 0)
                    //{
                    //    Resources.MergedDictionaries.Add(languageDictionary.resourceDictionary);
                    //}
#endif
#if WPF || UWP || WINUI || SMARTMAUI
                    signinviewmodel.resdict = languageDictionary.resourceDictionary;
#endif
#if ANDROIDX
                    signinviewmodel.resdict = languageDictionary.resourceDictionary;
#endif
                    signinviewmodel.languageTag = tag;
                    signinviewmodel.ResetUTC = BesetByChimps(signinviewmodel, "ResetUTC");

                    signinviewmodel.LoginStatusMessage = BesetByChimps(signinviewmodel, "LoginStatusMessage");
                    signinviewmodel.UsernameBox = BesetByChimps(signinviewmodel, "UsernameBox");
                    signinviewmodel.PasswordBox = BesetByChimps(signinviewmodel, "PasswordBox");
#if UWP || WINUI
                    int len = signinviewmodel.PasswordBox.Length;
                    signinviewmodel.PasswordBox = signinviewmodel.PasswordBox.PadLeft(len + 8, Convert.ToChar(SmartParametersV2016.space));
#endif

                    signinviewmodel.Signin = BesetByChimps(signinviewmodel, "Signin");
                    signinviewmodel.CreateAccount = BesetByChimps(signinviewmodel, "CreateAccount");
                    if (signinviewmodel.errorMessage != "")
                    {
                        signinviewmodel.errorMessage = BesetByChimps(signinviewmodel, signinviewmodel.errorMessage);
                    }
#if ANDROIDX
                    // PUT THESE BACK!!


                    //signinviewmodel.ResetUTCButton.Text = signinviewmodel.ResetUTC;
                    //signinviewmodel.LoginStatusMessageText.Text = signinviewmodel.LoginStatusMessage;
                    //signinviewmodel.ErrorMessageText.Text = signinviewmodel.ErrorMessage;
#endif
                    break;
                }
            }
            return;
        }

#if WINFORMS
        internal static string BesetByChimps(SignInViewModel signinviewmodel, string key)
#endif
#if WPF || UWP || WINUI || SMARTMAUI
        internal static string BesetByChimps(SignInViewModel signinviewmodel, string key)
#endif
#if ANDROIDX
        internal static string BesetByChimps(SignInViewModel signinviewmodel, string key)
#endif
        {
            if (key != "")
            {
                // Nothing for WINFORMS as yet!
#if WPF || UWP || WINUI || SMARTMAUI
                return signinviewmodel.resdict[key];
#endif
#if ANDROIDX
                return signinviewmodel.resdict[key];
#endif
            }
           return "";
        }

#if UWP || WINUI
        //protected override async void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    
        //    // Because we can't pass them over to SignIn from App.xaml.cs
        //    signinviewmodel.Platform = e.Parameter.ToString();            
        //    await CheckStartupConditions();
        //    return;
        // }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            //    base.OnNavigatedTo(e);
            //    // Read e.Parameter to retrieve the parameter
            //    if (e.Parameter is SignInViewModel vm)
            //    {
            //        signinviewmodel = vm;
            //        DataContext = signinviewmodel;
            //    }

            //    _ = CheckStartupConditions();

            base.OnNavigatedTo(e);

            if (DataContext == null)
            {
                var vm = (SignInViewModel)Application.Current.Resources["SignInVM"];

                Initialize(vm);   // ✅ your old method still works
                DataContext = vm;
            }

            _ = CheckStartupConditions();

        }
        
#endif

#if WPF || UWP || WINUI
        internal void OnFocusUsername(object sender, RoutedEventArgs e)
        {
#endif
#if SMARTMAUI
        internal void OnFocusUsername(object sender, EventArgs e)
        {
#endif
#if ANDROIDX
        internal void OnFocusUsername(object sender, EventArgs e)
        {
#endif
#if WPF || UWP || WINUI || SMARTMAUI
            signinviewmodel.Username = "";
            return;
        }
#endif
#if ANDROIDX
            UserName.Text = "";
            return;
        }
#endif

#if WPF || UWP || WINUI
        internal void OnFocusPassword(object sender, RoutedEventArgs e)
        {
            PassWord.Password = "";
        }
#endif
#if SMARTMAUI
        internal void OnFocusPassword(object sender, EventArgs e)
        {
            PassWord.Text = "";
        }
#endif
#if ANDROIDX
        internal void OnFocusPassword(object sender, EventArgs e)
        {
            // Optional: clear text
            //PassWord.Text = "";
        }
#endif

#if ANDROIDX
        public async Task OnSignUpButtonClicked(object sender, EventArgs args)
        {
            // Your code here
            // Example:
            await Xamarin.Essentials.Launcher.OpenAsync(new Uri("http://www.keasdon.co.uk/Identity/Account/Register"));
            return;
        }
#endif

#if WPF
        internal async void OnResetButtonClicked(object sender, RoutedEventArgs args)
#endif
#if UWP || WINUI
        internal async void OnResetButtonClicked(object sender, RoutedEventArgs args)
#endif
#if ANDROIDX
        internal async void OnResetButtonClicked(object sender, EventArgs args,
                                        AppCompatActivity signinActivity)
#endif
#if WPF || UWP || WINUI || ANDROIDX
        {
#if WPF
            ResetUTC.Visibility = Visibility.Hidden;
            await CheckStartupConditions();
#endif
#if UWP || WINUI
            //ResetUTC.IsVisible = false;
            signinviewmodel.ResetVisibility = false;
            await CheckStartupConditions();
#endif
#if ANDROIDX
            //ResetUTC.IsVisible = false;
            signinviewmodel.ResetVisibility = false;
            await CheckStartupConditions(signinActivity);
#endif
            return;
        }
#endif

#if WINFORMS || WPF || UWP || WINUI || ANDROIDX || SMARTMAUI
#if WINFORMS
        internal static async Task<bool> OnLogInButtonClicked(RichTextBox textBoxConsole,
                                                                Action<string> setErrorMessage,
                                                                CancellationTokenSource signinCts,
                                                                SignInFormX signinComponents,
                                                                MainProcess processComponents,
                                                                string UserName,
                                                                string PassWord,
                                                                LoginKeys loginKeys,
                                                                SmartMumList signinFatah,
                                                                SignInViewModel signinviewmodel)
#endif
#if WPF || SMARTMAUI
        internal async Task OnLogInButtonClicked(object sender, EventArgs args)
#endif
#if ANDROIDX
        internal async Task OnLogInButtonClicked(object sender, EventArgs e, AppCompatActivity activity)
#endif
#if UWP || WINUI
        internal async void OnLogInButtonClicked(object sender, RoutedEventArgs args)
#endif
        {
#if WINFORMS
            bool consoleLogin = true;
#endif
            if (!signinviewmodel.SignInEnabled)
            {
                DisplayError("CannotSignin", signinviewmodel);
#if WINFORMS
                return false;
#endif
#if WPF || UWP || WINUI || SMARTMAUI
                //return;
#endif
#if ANDROIDX
                return;
#endif
            }
            else
            {
                // If we're not enabled
#if WINFORMS
                if (string.IsNullOrEmpty(UserName) ||   // or the Username is empty
                string.IsNullOrEmpty(PassWord))
#endif
#if WPF || UWP || WINUI
                if (string.IsNullOrEmpty(UserName.Text) ||   // or the Username is empty
                string.IsNullOrEmpty(PassWord.Password))            // or the Password is empty
#endif
#if SMARTMAUI
                if (string.IsNullOrEmpty(UserName.Text) ||   // or the Username is empty
                string.IsNullOrEmpty(PassWord.Text))                 // or the Password is empty (MAUI Entry uses .Text)
#endif
#if ANDROIDX
                string username = UserName.Text;
                string password = PassWord.Text;
                if (string.IsNullOrEmpty(username) ||   // or the Username is empty
                    string.IsNullOrEmpty(password))
#endif
                {
                    DisplayError("EnterUsernameAndPassword", signinviewmodel);
#if WINFORMS
                    return false;
#endif
#if WPF || UWP || WINUI || SMARTMAUI
                    //return;
#endif
#if ANDROIDX
                    return;
#endif
                }
                else
                {
                    signinviewmodel.SignInEnabled = false;
                    // Your code here
                    DisplayError("", signinviewmodel);
                    signinviewmodel.UserIsLoggedIn = false;
#if SMARTMAUI
                    LoginStatusMessageText.Text = "Connecting to server...";
                    LoginStatusMessageText.TextColor = Colors.LightGray;
                    LoginButton.IsEnabled = false;
                    LoginButton.Opacity = 0.6;
                    await Task.Delay(50);
#endif

                    //ReadOnlyCollection<TimeZoneInfo> tzCollection;
                    //tzCollection = TimeZoneInfo.GetSystemTimeZones();
                    //this.timeZoneList.DataSource = tzCollection;

                    // You couldn't make this up!! TextTransform DOESN'T uppercase
                    // the last character of a piece of text!!!  What absolute BOLLOCKS
                    // this entire Xamarshit is!!!
#if WINFORMS
                    signinviewmodel.Username = UserName; // Removed 16th July 2023.ToUpper();
#endif
#if WPF || UWP || WINUI || SMARTMAUI
                    signinviewmodel.Username = UserName.Text; // Removed 16th July 2023 .ToUpper();
#endif
#if ANDROIDX
                    signinviewmodel.Username = UserName.Text; // Removed 16th July 2023 .ToUpper();
#endif
                    // Password UpperCasing also removed!
                    if (signinCts.IsCancellationRequested)
                    {
                        DisplayError("CancellationRequested", signinviewmodel);
#if WINFORMS
                        return false;
#else
                        //return;
#endif
                    }

                    bool success = false;
                    try
                    {
                    success = await SmartLoginV2016.Login(
#if WINFORMS
                                                    textBoxConsole,
                                                    setErrorMessage,
#endif
                                                    signinviewmodel,
                                                    signinCts,
                                                    signinviewmodel.website,
                                                    signinviewmodel.Username
#if WINFORMS
                                                    ,PassWord
#endif
#if WPF || UWP || WINUI
                                                    ,PassWord.Password
#endif
#if SMARTMAUI
                                                    ,PassWord.Text
                                                    ,msg => MainThread.BeginInvokeOnMainThread(() => LoginStatusMessageText.Text = msg)
#endif
#if ANDROIDX
                                                    ,PassWord.Text
                                                    ,LoginLed1
                                                    ,LoginLed2
                                                    ,LoginLed3
                                                    ,LoginLed4
                                                    ,LoginLed5
                                                    ,LoginLed6
                                                    ,ErrorMessageText
#endif
                                                    );
                    }
                    catch (Exception ex)
                    {
                        signinviewmodel.errorMessage = "Connection failed: " + ex.Message;
#if SMARTMAUI
                        LoginStatusMessageText.Text = "";
                        LoginButton.IsEnabled = true;
                        LoginButton.Opacity = 1.0;
#endif
                        FullReset(signinviewmodel);
#if WINFORMS
                        return false;
#else
                        return;
#endif
                    }
                    if (!success)
                    {
                        //
                        // Have you rebuilt KeasdonEnergy after a backup??
                        // Is SmartDBserver running?
                        // Have you started the KeasdonEnergy website in IIS?
                        // Is the SQLServer (MSSQLServer) bollocks started??
                        // Is the DefaultAppPool started?
                        //
#if ANDROIDX
                        LoginLed1.SetTextColor(signinviewmodel.Led1);
                        ErrorMessageText.Text = signinviewmodel.errorMessage;
#endif
                        if (string.IsNullOrEmpty(signinviewmodel.LoginKeys.userName))
                        {
                            DisplayError("InvalidSignin", signinviewmodel);
#if SMARTMAUI
                            if (!string.IsNullOrEmpty(signinviewmodel.errorMessage) && signinviewmodel.errorMessage.Contains("Problem with Mother"))
                                signinviewmodel.errorMessage = "Server returned no data. Please check the server is running.";
                            ErrorMessageText.Text = signinviewmodel.errorMessage;
#endif
                        }
                        else
                        {
                            // Login failed - FOR POSSIBLE LOTS OF REASONS *DON'T TELL 'EM WHY!!*
                            DisplayError("UnableToAuthenticate", signinviewmodel);
                        }
                        FullReset(signinviewmodel);
#if SMARTMAUI
                        LoginStatusMessageText.Text = "";
                        LoginButton.IsEnabled = true;
                        LoginButton.Opacity = 1.0;
#endif
#if WINFORMS
                        return false;
#else
                        return;
#endif
                    }
                    if (!SmartRoutinesV2018.CreateUriLogin(signinviewmodel, signinviewmodel.website, @"/Handlers/Listener.ashx"))
                    {
                        signinviewmodel.SignInEnabled = false;
                        DisplayError("CreateHandlerFailure", signinviewmodel);
#if WINFORMS
                        return false;
#else
                        //return;
#endif
                    }
                    signinviewmodel.listenerUrl = signinviewmodel.TargetUrl;
                    if (!SmartRoutinesV2018.CreateUriLogin(signinviewmodel, signinviewmodel.website, @"/Handlers/LoadTable.ashx"))
                    {
                        signinviewmodel.SignInEnabled = false;
                        DisplayError("CreateHandlerFailure", signinviewmodel);
#if WINFORMS
                        return false;
#else
                        //return;
#endif

                    }
                    signinviewmodel.loadtableUrl = signinviewmodel.TargetUrl;
                    signinviewmodel.UserIsLoggedIn = true;

                    // Safe to do this now ... the cookies are initialized and trace is set
                    // AND the fucking UserName is setup (!!!!! You PLONKER!!)
                    // This works because we're not updating the main thread
#if SMARTMAUI
                    LoginStatusMessageText.Text = "Logging trace...";
                    await Task.Delay(50);
#endif
#if !WINFORMS
                    await SmartRoutinesV2018.CheckTraceSignIn(signinviewmodel, signinCts.Token, "Logged in", true);
#endif
                    try
                    {
#if UWP || WINUI || SMARTMAUI
                        // This pushes MainMeter onto the stack as the one we see
                        // However when we QUIT SmartSwitch, we have to pop MainMeter off the stack
                        // This happens in TimerClockTick when we find out signinviewmodel.IsLoggedIn is false.
                        // Took me DAYS to get this working

                        // Fuck Me!  Was this COMPLICATED ... or what???

#endif
                        // Login succeeded
#if SMARTMAUI
                        LoginStatusMessageText.Text = "Loading database tables...";
#endif
                        string databaseName = "SMARTMUM";   // Load tables for the PROGRAM
                        if (!await SmartNibbyV2016.MiserableJokeSister((DateTime.Now + signinviewmodel.utcOffset),    // Local time
                                                                        signinviewmodel,
                                                                        signinCts.Token,
                                                                        signinviewmodel.Username,
                                                                        signinviewmodel.antiTokenString,
                                                                        'D',
                                                                        databaseName,
                                                                        SmartParametersV2016.TotalTables))
                        {
                            // Set the sixth Led to Red ... and carry on!
                            signinviewmodel.Led6 = signinviewmodel.redColour;
#if ANDROIDX
                            LoginLed6.SetTextColor(signinviewmodel.redColour);
#endif
                            DisplayError("CannotLoadFromMothership", signinviewmodel);
                            await SmartRoutinesV2018.CheckTraceSignIn(signinviewmodel,
                                                                    signinCts.Token,
                                                                    "Problem 41: " + signinviewmodel.errorMessage, true);
                            FullReset(signinviewmodel);
#if WINFORMS
                            return false;
#else
                            //return;
#endif
                        }
#if SMARTMAUI
                        _ = SmartRoutinesV2018.CheckTraceSignIn(signinviewmodel,
                                                                    signinCts.Token,
                                                                    "SMARTMUM loaded", true);
                        LoginStatusMessageText.Text = "Preparing main screen...";
#else
                        await SmartRoutinesV2018.CheckTraceSignIn(signinviewmodel,
                                                                    signinCts.Token,
                                                                    "SMARTMUM loaded", true);
#endif

                        // Set the sixth Led to Green ... and carry on!
                        signinviewmodel.Led6 = signinviewmodel.greenColour;
#if ANDROIDX
                        LoginLed6.SetTextColor(signinviewmodel.greenColour);
#endif
                        // Now get the currency codes for all the cultures we support
                        // We use SPECIFIC CULTURES because a Currency Code goes against a COUNTRY
                        // whereas a Culture could cross more than one country with different
                        // currency symbols ...
                        CultureInfo[] cultInf = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                        signinviewmodel.Fatah.cultureviewList = new List<SmartData.CultureView>();

                        foreach (SmartData.Cultures ci in signinviewmodel.Fatah.culturesList)
                        {
                            SmartData.CultureView cultureView = SmartSpikeV2017.GetCurrencyInfo(signinviewmodel,
                                                                                                    cultInf,
                                                                                                    new CultureInfo(ci.CULTUREINFO));
                            if (string.IsNullOrEmpty(signinviewmodel.errorMessage))
                            {
                                cultureView.CULTURE_CODE = ci.CULTURE_CODE;
                                cultureView.DESCRIPTION = ci.DESCRIPTION;

                                signinviewmodel.Fatah.cultureviewList.Add(cultureView);
                            }
                        }
                        // Believe it or not (!) signinviewmodel.Fatah.cultureviewList is
                        // already sorted on CULTURE_CODE ... so no need to 're-sort' it
                        signinviewmodel.screensCount = signinviewmodel.Fatah.cubefacesList.Count;
#if WINFORMS
                        if (consoleLogin)
                        {
                            try
                            {
                                signinComponents.Close();
                                // Took me DAYS to get this working
                                string[] args = new string[0];
                                loginKeys.subscriber = signinviewmodel.LoginKeys.subscriber;
                                loginKeys.multimeter = signinviewmodel.LoginKeys.multimeter;
                                loginKeys.trace = signinviewmodel.LoginKeys.trace;
                                loginKeys.lastLoginTime = signinviewmodel.LoginKeys.lastLoginTime;
                                loginKeys.passwordHash = signinviewmodel.LoginKeys.passwordHash;
                                loginKeys.previousLogonDate = signinviewmodel.LoginKeys.previousLogonDate;
                                loginKeys.expirationDate1 = signinviewmodel.LoginKeys.expirationDate1;
                                loginKeys.expirationDate2 = signinviewmodel.LoginKeys.expirationDate2;
                                loginKeys.expirationDate3 = signinviewmodel.LoginKeys.expirationDate3;

                                loginKeys.userId = signinviewmodel.LoginKeys.userId;
                                loginKeys.userName = signinviewmodel.LoginKeys.userName;
                                loginKeys.administrator = signinviewmodel.LoginKeys.administrator;

                                signinFatah.cubefacesList = signinviewmodel.Fatah.cubefacesList;
                                signinFatah.cultureviewList = signinviewmodel.Fatah.cultureviewList;
                                signinFatah.currenciesList = signinviewmodel.Fatah.currenciesList;
                                signinFatah.handlersList = signinviewmodel.Fatah.handlersList;
                                signinFatah.sqliteschemasList = signinviewmodel.Fatah.sqliteschemasList;
                                signinFatah.sqlitetablesList = signinviewmodel.Fatah.sqlitetablesList;
                                signinFatah.sqlitefieldsList = signinviewmodel.Fatah.sqlitefieldsList;
                            }
                            catch (Exception ex)
                            {
                                signinviewmodel.errorMessage = ex.Message;
                            }
                        }
#endif
#if WPF || UWP || WINUI || SMARTMAUI
#if WPF
                        Window main = new MainMeter(signinviewmodel, scheduler);
#endif
#if UWP
                        Frame rootFrame = Window.Current.Content as Frame;
                        //
                        // Cannot figure out how to pass signinviewmodel as a parameter ..
                        //
                        rootFrame.Navigate(typeof(MainMeter));
#endif
#if WINUI
                        //
                        // Cannot figure out how to pass signinviewmodel as a parameter ..
                        //
                        // This is the only way I can make the SignIn
                        // screen clear before MainMeter
                        var _window = ((App)Application.Current).MainWindow;

                        Frame rootFrame = (Frame)_window.Content;
                        rootFrame.Content = null;
                        await Task.Delay(300); // Wait for 30 msec (1/3 second)
                        rootFrame.Navigate(typeof(MainMeter));
#endif
#if SMARTMAUI
                        await Task.Delay(300);
                        string _dbg2 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_debug.txt");
                        System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] SignIn: creating MainMeter\n");
                        try
                        {
                            if (scheduler == null) scheduler = new TickScheduler();
                            var mainMeter = new MainMeter(signinviewmodel, scheduler);
                            System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] SignIn: MainMeter created, about to PushAsync\n");
                            var navPage = Application.Current.Windows[0].Page as NavigationPage;
                            if (navPage != null)
                            {
                                await navPage.PushAsync(mainMeter, false);
                                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] SignIn: PushAsync completed\n");
                            }
                            else
                            {
                                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] SignIn: navPage is NULL!\n");
                            }
                        }
                        catch (Exception navEx)
                        {
                            System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] SignIn: EXCEPTION: {navEx.GetType().Name}: {navEx.Message}\n{navEx.StackTrace}\n");
                        }
#endif
                        if (signinviewmodel.errorMessage == "")
                        {
                            signinviewmodel.LoginStatusMessage = "";
#if WPF
                            main.Closed += MeterClosed;    // Event handler for when its closed in MainMeter
                            this.Hide();                    // Put our SignIn behind the settee!
#endif
                            signinviewmodel.errorMessage = BesetByChimps(signinviewmodel, "PleaseSignin");
                            // Show the Meter screen
#if WPF
                            main.Show();
#endif
                        }
                        else
                        {
                            FullReset(signinviewmodel);
                        }
#endif
#if ANDROIDX
                        if (string.IsNullOrEmpty(signinviewmodel.errorMessage))
                        {
                            try
                            {
                                // So the way this works, is that SmartSwitch starts off
                                // with SigninActivity which calls SignIn.xaml.cs i.e the
                                // routine we're in now, which in turn calls MeterActivity,
                                // which we hold in as a variable in 'ouractivity'.
                                // Now MeterActivity has MainMeter.xaml.cs built-in to it
                                // so the screen will show AND all the routines are called
                                // (and hopefully) work. However the 'FullReset();' code
                                // below is executed immediately ... but the user can't see it
                                // as the MainMeter screen obscures it.
                                // When we 'Quit' from MainMeter, part of the TimerClockTick
                                // routine(s) which checks to see if the clock has stopped AND
                                // a few other parameters besides and if this IS the case, well
                                // it shuts down, disconnects, disposes of the clock and THEN ..
                                // invokes ouractivity.Finish() to 'kill off' MeterActivity.
                                // At that point the MainMeter disappears to be replaced by
                                // SignIn which is part of SignInActivity and has always been there
                                // behind the MainMeter screen.  So it's kind of like a 'navigation'
                                // but nothing gets 'Pushed' or 'Popped' as such. Its all complete
                                // and utter BOLLOCKS ...

                                // ✅ navigate
                                AppSession.Instance.signInVM = signinviewmodel;
                                AppSession.Instance.Scheduler = scheduler; 
                                Intent intent = new Intent(activity, typeof(MeterActivity));

                                ActivityOptions options = ActivityOptions.MakeCustomAnimation(
                                    activity,
                                    Resource.Animation.abc_fade_in,
                                    Resource.Animation.abc_fade_out
                                );

                                activity.StartActivity(intent, options.ToBundle());
                                FullReset(signinviewmodel);            // This happens immediately
                                DisplayError("PleaseSignin", signinviewmodel);      // AND this, ready for the next SignIn
                            }
                            catch (Exception ex)
                            {
                                signinviewmodel.errorMessage = ex.Message;
                            }
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        signinviewmodel.errorMessage = ex.Message;
                    }
                    finally
                    {
                        signinviewmodel.SignInEnabled = true;
                        FullReset(signinviewmodel);
                    }
#if WINFORMS
                    return true;
#endif
#if WPF || UWP || WINUI || SMARTMAUI
                    //return;
#endif
#if ANDROIDX
                    return;
#endif
                }
            }
        }
#endif

#if WINFORMS
        internal static void FullReset(SignInViewModel signinviewmodel)
#endif
#if WPF || UWP || WINUI || SMARTMAUI
        internal void FullReset(SignInViewModel signinviewmodel)
#endif
#if ANDROIDX
        internal void FullReset(SignInViewModel signinviewmodel)
#endif
        {
#if WINFORMS
            signinviewmodel.LoginStatusMessage = BesetByChimps(signinviewmodel, "LoginStatusMessage");
            signinviewmodel.Led1 =
                signinviewmodel.Led2 =
                signinviewmodel.Led3 =
                signinviewmodel.Led4 =
                signinviewmodel.Led5 =
                signinviewmodel.Led6 = signinviewmodel.orangeColour;
#endif
#if WPF || UWP || WINUI || SMARTMAUI
            signinviewmodel.LoginStatusMessage = BesetByChimps(signinviewmodel, "LoginStatusMessage");
            signinviewmodel.Led1 =
            signinviewmodel.Led2 =
            signinviewmodel.Led3 =
            signinviewmodel.Led4 =
            signinviewmodel.Led5 =
            signinviewmodel.Led6 = signinviewmodel.orangeColour;
#endif
#if ANDROIDX
            LoginStatusMessageText.Text = BesetByChimps(signinviewmodel, "LoginStatusMessage");
            LoginLed1.SetTextColor(signinviewmodel.orangeColour);
            LoginLed2.SetTextColor(signinviewmodel.orangeColour);
            LoginLed3.SetTextColor(signinviewmodel.orangeColour);
            LoginLed4.SetTextColor(signinviewmodel.orangeColour);
            LoginLed5.SetTextColor(signinviewmodel.orangeColour);
            LoginLed6.SetTextColor(signinviewmodel.orangeColour);            
#endif
#if WPF || UWP || WINUI || SMARTMAUI
            //PassWord.Password = "";
#endif
#if ANDROIDX
            UserName.Text = "";
            PassWord.Text = "";
            // Take this OUT for PRODUCTION
            UserName.Text = SmartParametersV2016.myUsername;
            PassWord.Text = SmartParametersV2016.myPassword;
#endif
            // I don't think we need to do this anymore. Yes we do!
            //signinviewmodel.SignInEnabled = true;
            signinviewmodel.errorMessage = "";
            return;
        }

#if WPF
        internal async void MeterClosed(object sender, EventArgs e)
        {
            // Clear the last User and Pwd
            UserName.Text = "";
            PassWord.Password = "";
            // Allow users to log in again .. Not sure we need to do this now?
            // We don't - its all done in CheckStartupCOnditions ... signinviewmodel.SignInEnabled = true;            
            // Meter Window has closed so now show the SignIn screen once more
            this.Show();
            await CheckStartupConditions();
            UserName.Focus();
            return;
        }
#endif

#if WINUI
        internal void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            //if (e.Key != null)    // Always true
            //{
                return;
            //}            
        }
#endif

#if WPF || UWP || SMARTMAUI
#if WPF
        internal async void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
#endif
#if UWP
        internal async void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Enter)
#endif
#if SMARTMAUI
        internal async void OnKeyDownHandler(object sender, EventArgs e)
        {
            try
            {
                if (true)
#endif
                {
#if !SMARTMAUI
                    e.Handled = true;
#endif
                    await OnLogInButtonClicked(sender, e);
                }
            }
            catch (Exception ex)
            {
                signinviewmodel.errorMessage = ex.Message;
            }
            return;
        }
#endif


#if SMARTMAUI
        internal async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync("http://www.keasdon.co.uk/Account/Register");
        }

#endif
#if WINFORMS || WPF
        internal void SignInClosed(object sender, EventArgs e)
        {
            // Don't think I need the next line, but I do need the one after that!
            // Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
#if WINFORMS
            System.Windows.Forms.Application.Exit(); // Byeeeeeeeeeeeeeeeeee
#endif
#if WPF
            Application.Current.Shutdown(); // Byeeeeeeeeeeeeeeeeee
#endif
            return;
        }

#if WPF || UWP || WINUI || SMARTMAUI
        internal void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
            return;
        }
#endif

#endif
    }

#if WPF
    // All this shit just to get default 'watermarks' in the UserName and PassWord boxes ..
    public class PasswordBoxMonitor : DependencyObject
    {
        public static bool GetIsMonitoring(DependencyObject depobj)
        {
            return (bool)depobj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject depobj, bool value)
        {
            depobj.SetValue(IsMonitoringProperty, value);
            return;
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static int GetPasswordLength(DependencyObject depobj)
        {
            return (int)depobj.GetValue(PasswordLengthProperty);
        }

        public static void SetPasswordLength(DependencyObject depobj, int value)
        {
            depobj.SetValue(PasswordLengthProperty, value);
            return;
        }

        public static readonly DependencyProperty PasswordLengthProperty =
            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxMonitor), new UIPropertyMetadata(0));

        public static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox PassWord = (PasswordBox)d;
            if (PassWord == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                PassWord.PasswordChanged += PasswordChanged;
            }
            else
            {
                PassWord.PasswordChanged -= PasswordChanged;
            }
            return;
        }

        public static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox PassWord = sender as PasswordBox;
            if (PassWord == null)
            {
                return;
            }
            SetPasswordLength(PassWord, PassWord.Password.Length);
            return;
        }
    }
#endif
}