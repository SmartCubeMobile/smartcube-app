//    This program works fine built against XAMASHIT || UWP || WINUI || MEOWI Forms 4.8.0.1826
//    ... but the minute you build it against any of the 5.0 versions
//    it falls over into a fucking useless heap.  Chimps!

// SmartSwitchV2023 Latest and Greatest
// Dedicated to my favourite grand-daughter Amelia Maria Marsh (a.k.a 'Nibby' or 'Moo Bags') and
// my favourite grandson Luke (a.k.a. 'Wookey')
// (Amelia, poppet - you would never BELIEVE how much 'Ba!' I have cleared out of
// this program since April 2011, and how much I STILL have clear out ..)
//
// In memory of my dear old Mum, who despite all our falling outs, I loved dearly. And whom I miss
// every day.. and to whom I owe everything.  And in memory of my brother Bob to whom I loved all my life and to whom I would have
// donated my kidney if he had needed one and who has left a huge void in my life.
// And whom I miss every day ... and I think of you every day and you always were
// - and always will be - my hero
// From now on Bob, every blue sky day is a 'Bob day' for me.  I will never forget you. Never.
//
// For reasons which I still don't clearly understand (but I'm not going
// to argue with) Unit Rates need to be passed by REF, but Accounts do not ...
// Go figure - this entire Microshit system is a pile of shit ... these people are ABSOLUTE AND UTTER CHIMPS
//
// Connections:-
// First Utility    :  MARIA mchapman4 / annidani
// First Utility    :  MARKUS markus.barg@gmail.com Braticku1971fu (yes - its a CAPITAL 'B')
// British Gas      :  maria_chapman53@hotmail.com / password1   £19.03 in the red on her Electric  £0.01 in credit on her Gas
// British Gas      :  BOB bobchapmanuk@yahoo.co.uk / chelsea1
// British Gas      :  markpilbeam@sky.com / gasst3v3n
// British Gas      :  MARTIN frosty@o2email.co.uk / Catherine
// Npower           :  PAT PAT_ODONNELL / npower2011   (mrsdiamondcat@yahoo.co.uk => changed to => ray_chapman48@hotmail.com)
// Npower           :  ricktimps@hotmail.co.uk / lynnette1
// Scottish Power   :  ANNA atmc_21@hotmail.com / Amelia2010
// Scottish Power   :  MICHELLE ts21nan@ntlworld.com 
// EDF Energy       :  LUCY ray_chapman48@hotmail.com LucyAnnCox
// eonenergy.com    :  MARK AKRAWCZYK88 energia5000
// Southern Electric:  RAY ray_chapman48@hotmail.com annidani
//

#if WINFORMS
using System.Text;
using System.Windows.Forms;
using SmartDashboard;
#endif

#if WPF
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

#if UWP
//using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Text;
#endif

#if WINUI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinRT.Interop;
#endif

#if ANDROIDX
using System.Text;
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
using Xamarin.KotlinX.Coroutines;
#endif

namespace SmartCubeMobile
{
#if WINFORMS
    public partial class MainMeter
    {
#endif
#if WPF

    public partial class MainMeter : Window
    {
#endif
#if UWP
    public sealed partial class MainMeter : Page
    {
        public static double currentWidth = 0,
                                currentHeight = 0;
#endif
#if WINUI
    public sealed partial class MainMeter : Page
    {
        
#endif
#if MAUI
    public sealed partial class MainMeter : Page
    {
        public Frame rootFrame = null;
        public static double currentWidth = 0,
                                    currentHeight = 0;
#endif
#if ANDROIDX
    public partial class MainMeter : RelativeLayout
    {
#endif
        internal static string wherewereat;

#if !WINFORMS
        internal static MainViewModel ourviewmodel;// = new MainViewModel();
        internal static ProfilesViewModel profileviewmodel;// = new ProfilesViewModel();
        internal static FinanceViewModel financeviewmodel;// = new FinanceViewModel();
        internal static UtilityViewModel utilityviewmodel;// = new UtilityViewModel();
#endif
        // I know what you are going to say ...  
        // This time is updated by the TimerClock routine every second ...
        // However we pass this value into lots of routines and IT COULD BE that the routine uses a value which is
        // at most ONE SECOND late .  The alternative is to use timeNeow = DateTime Now everywhere ... but that could
        // lead to inconsistencies regarding timeNeow between routines.  We can live with the ONE SECOND potential delay ..
        // Cut me some slack here ...

        // The fucking doors!!
        //   (no more ...)

        // The fucking task Scheduler!!
        private TickScheduler _scheduler;

        // Grouse her up (!!) <= GRASS her up
        // Fractions <= FACTIONS!!

        // The concept of 'Elevated Trust' which applies to Silvershit In-Browser applications
        // does not apply to WPF applications (which is what this one is)
        // So I am going to take-away Led1 from 'Elevated Trust' and give it to 'Trace'
        // And then I am going to give 'Trace' Led7 to 'Switch' to highlight whether an
        // attempted switch worked or not .. Lets seee.... 


        // For the avoidance of doubt
        //  UpdateLed Orange - Cannot update Green - Can update  Red - never shows

        //  Led1 White  - no trace          Green - User is being traced                     Red - never shows
        //  Led2 White  - default           Green - We can connect with our Username         Red - no we can't
        //  Led3 White  - default           Green - We have loaded SmartTariffs/SmartFinance Red - no we haven't
        //  Led4 White  - default           Green - We have loaded User's SmartSwitch data   Red - no we haven't

        //  Led5 White  - network unchecked Green - network detected                         Red - network check failed
        //  Led6 White  - meter inactive    Green / Orange - meter active                    Red - meter problem
        //  Led7 White  - no switch         Green / Orange - Switch succeeded                Red - switch failed
        //  Led8 White  - default           Green - never shows                              Red - some error e.g. CE

        //  Here are a description of the LED lights
        // Led1  TR   (starts as White) Green (user trace in progress)   Red(never shows)
        // Led2  DBS  (starts as White) Green (can connect to HQ with our username)    Red(can't connect to HQ)
        // Led3  SCD  (starts as White) Green (loaded SmartCube data )   Red(can't load SmartCube data)
        // Led4  UD   (starts as White) Green (loaded Username data )    Red(can't load users data)
        // Led5  NW   (starts as White) Green (can detect a Network )    Red(can't find network)
        // Led6  MR   (starts as White) Green/Orange (meter read in progress )  Red(problem reading meter)
        // Led7  SW   (starts as White) Green/Orange (user switch succeeded )   Red(user switch failed)
        // Led8  FAIL (starts as White) Green (never shows)              Red(some internal error)
#if WINFORMS
        public static async Task<bool> MainMeterEntry(MainProcess components,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        ProfilesViewModel profileviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        UtilityViewModel utilityviewmodel)
#endif
#if UWP
        // I have wasted too much of my life trying various different (and
        // ultimately unsuccessful) ways of trying to pass a parameter to this
        // UWP || WINUI shit. The fucking Chimps have really fucked me over on this one
        // because the OnNavigatedTo event is never fucking called UNTIL ...
        // MainMeter has finished.  And its MAINMETER that needs the fucking
        // signinviewmodel parameter!  What a fucking useless piece of fucking
        // excrement this Chimp-derived system is!! It's FUCKING HOPELESS
        // right from the word go...
        public MainMeter()
#endif
#if WINUI
        // I have wasted too much of my life trying various different (and
        // ultimately unsuccessful) ways of trying to pass a parameter to this
        // UWP || WINUI shit. The fucking Chimps have really fucked me over on this one
        // because the OnNavigatedTo event is never fucking called UNTIL ...
        // MainMeter has finished.  And its MAINMETER that needs the fucking
        // signinviewmodel parameter!  What a fucking useless piece of fucking
        // excrement this Chimp-derived system is!! It's FUCKING HOPELESS
        // right from the word go...
        public MainMeter()
#endif
#if WPF

        public MainMeter(SignInViewModel signinviewmodel) // Maybe can take signinviewmodel out?
#endif
#if MAUI
        public MainMeter()
#endif
#if ANDROIDX
        public MainMeter(AppCompatActivity meterActivity,
                            AndroidX.Fragment.App.FragmentManager fm,
                            AndroidX.Lifecycle.Lifecycle lfc,
                            Context meterContext) : base(meterContext)
#endif
        {

#if WINFORMS
            
            //ourviewmodel = new MainViewModel()
            //{
            //    // Until I find out how to pass signinviewmodel
            //    // a parameter ..
            //    website = SignIn.signinviewmodel.website
            //};

            ourviewmodel.website = SignIn.signinviewmodel.website;
            components.MainBindingSource = new();
            components.MainBindingSource.DataSource = ourviewmodel;

            // 
            // ProfilesBindingSource
            // Done later
            //components.profilesCubefacesBindingSource = new();

            if (components.Led1.DataBindings.Count == 0)
            {
                components.Led1.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "Led1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.Led2.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "Led2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.Led3.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "Led3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.Led4.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "Led4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.Led5.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "Led5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.Led6.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "Led6", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.Led7.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "Led7", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.Led8.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "Led8", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                components.ScrollViewer.DataBindings.Add(new System.Windows.Forms.Binding("Visible", components.MainBindingSource, "ScrollViewerVisible", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                components.buttonQuit.Click += new EventHandler((s, e) => ButtonQuitClick(s, e, ourviewmodel, profileviewmodel, financeviewmodel, utilityviewmodel));

                components.buttonQuit.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", components.MainBindingSource, "QuitColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.buttonQuit.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", components.MainBindingSource, "QuitEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.OpenCloseButton.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", components.MainBindingSource, "OpenCloseColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.OpenCloseButton.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", components.MainBindingSource, "OpenCloseEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                components.labelUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "UserName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.labelUserName.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "UserNameColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.buttonAutoSwitchUtility.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", components.MainBindingSource, "AutoSwitchUtilityColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.labelDateTimeNow.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "DateTimeNowMessage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.WithdrawnDateMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "WithdrawnDateMessage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                components.LastLoginStatusMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "LastLoginStatusMessage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.AccountStatusMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "AccountStatusMessage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                components.buttonAutoSwitchFinance.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", components.MainBindingSource, "AutoSwitchFinanceColour", true));
            }

#endif
            // Without this next line NONE of the fucking bindings work <= Battling With The Chimps
            try
            {
#if WPF || UWP || WINUI || MAUI
                InitializeComponent();
#endif
#if WINUI
                var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
                var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                // Make the window full screen
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
#endif

#if WPF || UWP || WINUI || ANDROIDX || MAUI
                ourviewmodel = new MainViewModel()
                {
                    // UWP || WINUI
                    // Until I find out how to pass signinviewmodel
                    // a parameter ..
                    website = SignIn.signinviewmodel.website

                };
#endif
#if WPF || UWP || WINUI
                // So these are no longer hard-coded in the viewmodel
                ourviewmodel.kesmall = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\KeasdonEnergySmall.jpg", UriKind.Absolute));
                ourviewmodel.lightbulbicon = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\LightBulbIcon.ico", UriKind.Absolute));
                ourviewmodel.sslogo = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\SmartSwitchLogo.jpg", UriKind.Absolute));
                ourviewmodel.meterbackground = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\MeterBackground.jpg", UriKind.Absolute));
                ourviewmodel.metertop = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\MeterTop.jpg", UriKind.Absolute));
                ourviewmodel.meterbottom = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\MeterBottom.jpg", UriKind.Absolute));

                ourviewmodel.ZoomTooltip = SignIn.BesetByChimps("ZoomTooltip");
                ourviewmodel.AutoSwitchTooltip = SignIn.BesetByChimps("AutoSwitchTooltip");
                ourviewmodel.OpenCloseTooltip = SignIn.BesetByChimps("OpenCloseTooltip");
                ourviewmodel.QuitTooltip = SignIn.BesetByChimps("QuitTooltip");
#endif
#if WINUI || MAUI
                ourviewmodel.Dispatcher = this.DispatcherQueue;
#endif
#if WPF
                this.Loaded += (s, e) => MainMeterLoaded(s, e, ourviewmodel);
                this.Closed += (s, e) => MainMeterClosed(s, e, ourviewmodel);
                this.ButtonLeft.Click += (s, e) => ButtonLeftClicked(s, e, ourviewmodel);
                this.ButtonRight.Click += (s, e) => ButtonRightClicked(s, e, ourviewmodel);
#endif
#if WPF || WINUI || MAUI

                //this.ShrinkImageStoryboard.Completed += Shrunk;
                //this.ExpandImageStoryboard.Completed += Expanded;

                this.OpenCloseButton.Click += (s, e) => OpenCloseClicked(s, e, ourviewmodel);

                //this.FinanceAutoSwitch.Click +=  ButtonAutoSwitchFinanceClick;
                //this.UtilityAutoSwitch.Click +=  ButtonAutoSwitchUtilityClick;


#endif
#if UWP

                this.OpenCloseButton.Click += (s, e) => OpenCloseClicked(s, e, ourviewmodel);

#endif
#if ANDROIDX
#pragma warning disable CA1416
                //SignIn.signinviewmodel.mvrootView = meterActivity.FindViewById(Android.Resource.Id.Content);

                ourviewmodel.Fm = fm;
                ourviewmodel.Lfc = lfc;
                ourviewmodel.activity = meterActivity;
                ourviewmodel.context = meterContext;
                if (meterActivity != null)
                {
                    meterActivity.RunOnUiThread(() =>
                    {
                        int id0 = (int)typeof(Resource.Id).GetField("ScrollViewer").GetValue(null);
                        ourviewmodel.ScrollViewer = ourviewmodel.activity.FindViewById<ScrollView>(id0);

                        ourviewmodel.ADateTimeNow =
                         ourviewmodel.activity.FindViewById<TextView>(Resource.Id.DateTimeNow);

                        int id1 = (int)typeof(Resource.Id).GetField("TextBlockBrowser").GetValue(null);
                        ourviewmodel.TextBlockContent = ourviewmodel.activity.FindViewById<TextView>(id1);
                        int id2 = (int)typeof(Resource.Id).GetField("AutoSwitch").GetValue(null);
                        ourviewmodel.AutoSwitch = ourviewmodel.activity.FindViewById<Button>(id2);
                        if (OperatingSystem.IsAndroidVersionAtLeast(22))
                        {
                            ourviewmodel.AutoSwitch.TooltipText = SignIn.BesetByChimps("AutoSwitchTooltip");
                        }
                        int id6 = (int)typeof(Resource.Id).GetField("OpenClose").GetValue(null);
                        ourviewmodel.OpenClose = ourviewmodel.activity.FindViewById<Button>(id6);
                        if (OperatingSystem.IsAndroidVersionAtLeast(22))
                        {
                            ourviewmodel.OpenClose.TooltipText = SignIn.BesetByChimps("OpenCloseTooltip");
                        }
                        int id77 = (int)typeof(Resource.Id).GetField("Username").GetValue(null);
                        ourviewmodel.AUsername = ourviewmodel.activity.FindViewById<TextView>(id77);
                        int id3 = (int)typeof(Resource.Id).GetField("MeterTop").GetValue(null);
                        ourviewmodel.MeterTop = ourviewmodel.activity.FindViewById<ImageView>(id3);
                        int id4 = (int)typeof(Resource.Id).GetField("MeterBottom").GetValue(null);
                        ourviewmodel.MeterBottom = ourviewmodel.activity.FindViewById<ImageView>(id4);
                        int idcontent = (int)typeof(Resource.Id).GetField("MyContent").GetValue(null);
                        ourviewmodel.MyContent = ourviewmodel.activity.FindViewById<FrameLayout>(idcontent);
                        // Leds
                        int idled1 = (int)typeof(Resource.Id).GetField("Led1").GetValue(null);
                        ourviewmodel.ALed1 = ourviewmodel.activity.FindViewById<TextView>(idled1);
                        int idled2 = (int)typeof(Resource.Id).GetField("Led2").GetValue(null);
                        ourviewmodel.ALed2 = ourviewmodel.activity.FindViewById<TextView>(idled2);
                        int idled3 = (int)typeof(Resource.Id).GetField("Led3").GetValue(null);
                        ourviewmodel.ALed3 = ourviewmodel.activity.FindViewById<TextView>(idled3);
                        int idled4 = (int)typeof(Resource.Id).GetField("Led4").GetValue(null);
                        ourviewmodel.ALed4 = ourviewmodel.activity.FindViewById<TextView>(idled4);
                        int idled5 = (int)typeof(Resource.Id).GetField("Led5").GetValue(null);
                        ourviewmodel.ALed5 = ourviewmodel.activity.FindViewById<TextView>(idled5);
                        int idled6 = (int)typeof(Resource.Id).GetField("Led6").GetValue(null);
                        ourviewmodel.ALed6 = ourviewmodel.activity.FindViewById<TextView>(idled6);
                        int idled7 = (int)typeof(Resource.Id).GetField("Led7").GetValue(null);
                        ourviewmodel.ALed7 = ourviewmodel.activity.FindViewById<TextView>(idled7);
                        int idled8 = (int)typeof(Resource.Id).GetField("Led8").GetValue(null);
                        ourviewmodel.ALed8 = ourviewmodel.activity.FindViewById<TextView>(idled8);
                        int id5 = (int)typeof(Resource.Id).GetField("Quit").GetValue(null);
                        ourviewmodel.AQuit = ourviewmodel.activity.FindViewById<Button>(id5);
                    });
                }
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {
                    ourviewmodel.AQuit.TooltipText = SignIn.BesetByChimps("QuitTooltip");
                }
#pragma warning restore CA1416
#endif
#if WPF
                ourviewmodel.languageTag = signinviewmodel.languageTag;
#endif
#if UWP || WINUI || MAUI
                ourviewmodel.languageTag = SignIn.signinviewmodel.languageTag;
#endif
#if WINFORMS || WPF
                // Copying these here saves us passing SignInViewModel as a parameter
                ourviewmodel.CUBEFACESList = signinviewmodel.Fatah.cubefacesList;
                ourviewmodel.cultureviewList = signinviewmodel.Fatah.cultureviewList;
                // Only deal with Active currencies
                ourviewmodel.currenciesList = new List<SmartData.Currencies>
                    (from Currency in signinviewmodel.Fatah.currenciesList
                     where Currency.ACTIVEFLAG
                     select Currency);
                ourviewmodel.sqliteschemasList = signinviewmodel.Fatah.sqliteschemasList;
                ourviewmodel.sqlitetablesList = signinviewmodel.Fatah.sqlitetablesList;
                ourviewmodel.sqlitefieldsList = signinviewmodel.Fatah.sqlitefieldsList;
#endif
#if UWP || WINUI || MAUI
                // Copying these here saves us passing SignInViewModel as a parameter
                ourviewmodel.CUBEFACESList = SignIn.signinviewmodel.Fatah.cubefacesList;
                ourviewmodel.cultureviewList = SignIn.signinviewmodel.Fatah.cultureviewList;
                ourviewmodel.currenciesList = SignIn.signinviewmodel.Fatah.currenciesList;
                ourviewmodel.sqliteschemasList = SignIn.signinviewmodel.Fatah.sqliteschemasList;
                ourviewmodel.sqlitetablesList = SignIn.signinviewmodel.Fatah.sqlitetablesList;
                ourviewmodel.sqlitefieldsList = SignIn.signinviewmodel.Fatah.sqlitefieldsList;
#endif
#if ANDROIDX
                // Copying these here saves us passing SignInViewModel as a parameter
                ourviewmodel.CUBEFACESList = SignIn.signinviewmodel.Fatah.cubefacesList;
                ourviewmodel.cultureviewList = SignIn.signinviewmodel.Fatah.cultureviewList;
                ourviewmodel.currenciesList = SignIn.signinviewmodel.Fatah.currenciesList;
                ourviewmodel.sqliteschemasList = SignIn.signinviewmodel.Fatah.sqliteschemasList;
                ourviewmodel.sqlitetablesList = SignIn.signinviewmodel.Fatah.sqlitetablesList;
                ourviewmodel.sqlitefieldsList = SignIn.signinviewmodel.Fatah.sqlitefieldsList;
#endif
#if WINFORMS
                // Can't use the Meter panel binding here because the
                // Meter control hasn't been set up .. so we have to use 
                // the next line (you couldn't make this bollocks up ...)
                components.Meter.Visible = true;
                components.pictureBoxGUIMeterTop.Visible = true;
                components.pictureBoxGUIMeterBottom.Visible = true;
#endif
                ourviewmodel.Platform = SignIn.signinviewmodel.Platform;
#if WPF || UWP || WINUI || MAUI
                FrontEndGUI.SetWindowDataContext(this);
#endif
#if UWP
                if (SignIn.signinviewmodel.Platform == SmartParametersV2016.UWP)
#endif
#if WINUI || MAUI
                if (SignIn.signinviewmodel.Platform == SmartParametersV2016.WINUI)
#endif
#if ANDROIDX
                if (SignIn.signinviewmodel.Platform == SmartParametersV2016.Android)
#endif
#if WINFORMS || WPF
                if ((signinviewmodel.Platform == SmartParametersV2016.Windows ||
                    signinviewmodel.Platform == SmartParametersV2016.WINFORMS ||
                    signinviewmodel.Platform == SmartParametersV2016.WPF) &&
                    FrontEndGUI.FindListener())
#endif
                {
#if WINFORMS
                    components.textBoxConsole.AppendText("Listener supported " +
                                        Environment.NewLine.ToString());
                    components.textBoxConsole.ScrollToCaret();
#endif
                }
                else
                {
                    // Older version of Windows, no Listener
#if WINFORMS
                    components.textBoxConsole.AppendText("Listener *NOT* supported " +
                                        Environment.NewLine.ToString());
                    components.textBoxConsole.ScrollToCaret();
#endif
                }
            }
            catch (Exception exception)
            {
#if WINFORMS || WPF
                signinviewmodel.ErrorMessage = exception.Message;
#endif
#if UWP || WINUI || MAUI
                SignIn.signinviewmodel.ErrorMessage = exception.Message;
#endif
#if ANDROIDX
                SignIn.signinviewmodel.ErrorMessage = exception.Message;
#endif
                // Get SignIn to deal with it
                throw;
            }

#if WINFORMS || WPF
            if (!SmartRoutinesV2018.CreateHandlers(signinviewmodel,
                                                ourviewmodel))
#endif
#if UWP || WINUI || MAUI
            if (!SmartRoutinesV2018.CreateHandlers(SignIn.signinviewmodel,
                                                ourviewmodel))
#endif
#if ANDROIDX
            if (!SmartRoutinesV2018.CreateHandlers(SignIn.signinviewmodel,
                                                    ourviewmodel))
#endif
            {
#if WINFORMS
                return false;
#endif
#if WPF || UWP || WINUI || MAUI

                return;
#endif
#if ANDROIDX
                return;
#endif
            }

            // oooooooh .... ouch! ouch! you cannot Shutdown, Ray.  This might cause a crash!
            // You have to exit gracefully so just - return!
            // You just need to put the above message up and that should be enough!
            // #if WINFORMS
            //    return false;
            // #endif
            // #if WPF || UWP || WINUI
            // Can't do this >=  System.Windows.Application.Current.Shutdown();
            // Have to do this
            //    return;
            // #endif
            // #if ANDROIDX
            //return;     //Application.Current.Quit() in any normal system;
            //#endif

            // Do **NOT** fuck about with this next line!!!!
            // ====> I DON'T KNOW WHY IT WORKS, BUT IT FUCKING DOES  <====
            // If you LEAVE THIS LINE ALONE, all the 'await' calls work
            // If you CHANGE IT or FUCK ABOUT WITH IT then ***NOTHING*** fucking works
            // ====> SO LEAVE IT A-FUCKING-LONE <====
            try
            {
#if WINFORMS
                await Bootstrap(components,
                                signinviewmodel,
                                ourviewmodel,
                                profileviewmodel,
                                financeviewmodel,
                                utilityviewmodel);
#endif
#if WPF
                Bootstrap(signinviewmodel,
                            ourviewmodel);
#endif
#if UWP || WINUI || MAUI
                Bootstrap(SignIn.signinviewmodel,
                            ourviewmodel);
#endif
#if ANDROIDX
                Bootstrap(SignIn.signinviewmodel,
                                ourviewmodel);
#endif
            }
            catch (Exception ex)
            {

#if WINFORMS || WPF
                signinviewmodel.ErrorMessage = "Bootstrap failed: " + ex.Message;
#endif
#if UWP || WINUI || MAUI
                SignIn.signinviewmodel.ErrorMessage = "Bootstrap failed: " + ex.Message;
#endif
#if ANDROIDX
                SignIn.signinviewmodel.ErrorMessage = "Bootstrap failed: " + ex.Message;
#endif
#if WINFORMS
                return false;
#endif
#if WPF || UWP || WINUI || MAUI
                return;
#endif
#if ANDROIDX
                return;
#endif
            }

            // under a delusion <= under an ILLUSION!!!
            // Enhanced = entranced!
            // Marius Piper <= MARIS PIPER!
            // Cate Blanchard <= Cate Blanchett
            // Eileen and Gareth <= Elaine and Gavin!!!!
            // Travelling Willoughbys <= Travelling Wilberries
#if WINFORMS
            return true;
#endif
#if WPF || UWP || WINUI || MAUI
            return;
#endif
#if ANDROIDX
            return;
#endif
        }

        // Just in case I forget how to do it
        //private static void SmartswitchList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    //Console.Write("smartswitchList");
        //}

#if WINFORMS
        internal static async Task<bool> Bootstrap(MainProcess processComponents,
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                ProfilesViewModel profileviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                UtilityViewModel utilityviewmodel)
#endif
#if WPF || UWP || WINUI || MAUI
        internal async void Bootstrap(SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel)
#endif

#if ANDROIDX
        internal async void Bootstrap(SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel)
#endif
        {
#if WINUI
            //App.MaximizeWindow();
            //var window = App.MainWindow;
            //// Get the HWND from the main window
            //nint hWnd = WindowNative.GetWindowHandle(window);
            //WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            //AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            //DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);


            //appWindow.MoveAndResize(displayArea.WorkArea);

            //App.CenterAppWindow(appWindow);

#endif
            try
            {
                ourviewmodel.cookies = signinviewmodel.cookies;
                ourviewmodel.UserName = signinviewmodel.LoginKeys.userName;
#if ANDROIDX
                ourviewmodel.AUsername.Text = ourviewmodel.UserName;
#endif
                ourviewmodel.antiTokenString = signinviewmodel.antiTokenString;

                // Create the all-important DEK to decode incoming data
                // This is based on the User's Password (LoginKey[1]) and their
                // unique netcore-KeasdonEnergy.db Id (LoginKey[10])
                ourviewmodel.PDEK = SmartRoutinesV2018.CreateDEK(signinviewmodel.LoginKeys.passwordHash,
                                                                    signinviewmodel.LoginKeys.userId,
                                                                    SmartParametersV2016.SmartProfileSchema);
                ourviewmodel.FDEK = SmartRoutinesV2018.CreateDEK(signinviewmodel.LoginKeys.passwordHash,
                                                                    signinviewmodel.LoginKeys.userId,
                                                                    SmartParametersV2016.SmartFinanceSchema);
                ourviewmodel.UDEK = SmartRoutinesV2018.CreateDEK(signinviewmodel.LoginKeys.passwordHash,
                                                                    signinviewmodel.LoginKeys.userId,
                                                                    SmartParametersV2016.SmartUtilitySchema);
                // Safe to do this now ... the cookies are initialized and trace is set
                // AND the fucking UserName is setup (!!!!! You PLONKER!!)
                // This works because we're not updating the main thread

                // Setup the cancellation token(s)
                ourviewmodel.userToken = ourviewmodel.quitCts.Token;

                ourviewmodel.utcOffset = signinviewmodel.utcOffset;
#if UWP || WINUI || MAUI
#if PRODUCTION
                var zzz = DeviceDisplay.MainDisplayInfo;
                if (ourviewmodel.appView.IsFullScreenMode)
                {
                    ourviewmodel.appView.ExitFullScreenMode();
                    Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.Auto;
                    // The SizeChanged event will be raised when the exit from full-screen mode is complete.
                }

                else
                {
                    if (ourviewmodel.appView.TryEnterFullScreenMode())
                    {
                        Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.PreferredLaunchViewSize;
                        // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                    }
                }
#endif
#endif
                // This is the seed for GetNeextRandomNew
                ourviewmodel.randomR = new Random();
#if WINFORMS
                ourviewmodel.ScrollViewer = FrontEndGUI.FindScrollViewer(processComponents);
#endif
#if WPF || UWP || WINUI || MAUI
                ourviewmodel.ScrollViewer = FrontEndGUI.FindScrollViewer(this);
                ourviewmodel.Border = FrontEndGUI.FindScrollBorder(this);
                ourviewmodel.TextBlock = FrontEndGUI.FindScrollTextBlock(this);
                //ourviewmodel.Tossers = FrontEndGUI.FindGrid(this);
#endif
                // Bill Nighty (!) (or Nightie??) <= Bill NIGHY
                // Hyundai <= Leylandi !!! (You couldn't make it up!)
                // Font of knowledge <= Fount of knowledge!!
#if ANDROIDX
                ourviewmodel.AutoSwitch0 = ourviewmodel.autoswitch0;
                ourviewmodel.AutoSwitch25 = ourviewmodel.autoswitch25;
                ourviewmodel.AutoSwitch50 = ourviewmodel.autoswitch50;
                ourviewmodel.AutoSwitch75 = ourviewmodel.autoswitch75;

                ourviewmodel.OpenClose0 = ourviewmodel.opengreen0;
                ourviewmodel.OpenClose25 = ourviewmodel.opengreen25;
                ourviewmodel.OpenClose50 = ourviewmodel.opengreen50;
                ourviewmodel.OpenClose75 = ourviewmodel.opengreen75;
#endif
                // All the REDUNDANT CarouselView bollocks
                //ourviewmodel.carouselView = this.MyCarousel; // (CarouselView)FindByName("MyCarousel");

                // If you do the following two lines in XAML!!! The entire shit falls over!!
                // **
                // HAVE BEEN WARNED** ==> XamarSHIT Forms is a shit shower <===

                if (FrontEndGUI.CheckScrollViewer(ourviewmodel))
                {
                    FrontEndGUI.SetBorderVisible(ourviewmodel);
                    FrontEndGUI.SetScrollViewerVisible(ourviewmodel);
                }

#if WINFORMS
                ourviewmodel.MeterTopHeight = processComponents.pictureBoxGUIMeterTop.Height;
                processComponents.pictureBoxGUIMeterTop.DataBindings.Clear();
                processComponents.pictureBoxGUIMeterTop.DataBindings.Add(new Binding("Height", ourviewmodel, "MeterTopHeight"));

                ourviewmodel.MeterBottomHeight = processComponents.pictureBoxGUIMeterBottom.Height;
                processComponents.pictureBoxGUIMeterBottom.DataBindings.Clear();
                processComponents.pictureBoxGUIMeterBottom.DataBindings.Add(new Binding("Height", ourviewmodel, "MeterBottomHeight"));

                ourviewmodel.topActualheight = processComponents.pictureBoxGUIMeterTop.Height;
                ourviewmodel.bottomActualheight = processComponents.pictureBoxGUIMeterBottom.Height;
#endif
#if ANDROIDX
                // MeterTop and MeterBottom have already been set up in
#endif
#if WINFORMS
                ourviewmodel.topIncrement = ourviewmodel.topActualheight / SmartParametersV2016.doorCount;
                ourviewmodel.bottomIncrement = ourviewmodel.bottomActualheight / SmartParametersV2016.doorCount;

                ourviewmodel.timerDoor = new System.Windows.Forms.Timer()
                {
                    Interval = SmartParametersV2016.doorTicks   // Tick every 5 millisecs
                };
                ourviewmodel.timerDoor.Tick += new EventHandler((s, e) => TimerDoorTick(s, e, ourviewmodel));
#endif
#if WPF
                ourviewmodel.ShrinkTheDoors = FrontEndGUI.FindShrinkStoryBoard(this);
                ourviewmodel.ShrinkTheDoors.Completed += new EventHandler(DoorsAreOpen);
#endif
#if UWP || WINUI || MAUI
                ourviewmodel.ShrinkTheDoors = FrontEndGUI.FindShrinkStoryBoard(this);
                ourviewmodel.ShrinkTheDoors.Completed += DoorsAreOpen;
#endif
#if ANDROIDX
                // Done differently in ANDROID
                //ourviewmodel.ShrinkTheDoors = FrontEndGUI.FindShrinkStoryBoard(this);
                //ourviewmodel.ShrinkTheDoors.Completed += DoorsAreOpen;
#endif
#if WPF
                ourviewmodel.ExpandTheDoors = FrontEndGUI.FindExpandStoryBoard(this);
                ourviewmodel.ExpandTheDoors.Completed += new EventHandler(DoorsAreClosed);
#endif
#if UWP || WINUI || MAUI
                ourviewmodel.ExpandTheDoors = FrontEndGUI.FindExpandStoryBoard(this);                
                ourviewmodel.ExpandTheDoors.Completed += DoorsAreClosed;
#endif
#if ANDROIDX
                //ourviewmodel.ExpandTheDoors = FrontEndGUI.FindExpandStoryBoard(this);
#endif
                // London eccentric <= London-centric !!!!!

                // Doesn't look like I actually need this?
                //ourviewmodel.PassWord = signinviewmodel.LoginKeys[1];

                // Start the clock
                // This is the expiration date for Analysis costs display                
                // Main loop happens in the Meter tick section
#if WINFORMS
                ourviewmodel.timerClock.Interval = SmartParametersV2016.clockInt; // 1 Second 
                                                                                  //ourviewmodel.timerClock.Tick += new EventHandler(TimerClockTick);   // Won't happen until we either Open/Close

                ourviewmodel.timerClock.Tick += new EventHandler((s, e) => TimerClockTick(s, e, processComponents, ourviewmodel, financeviewmodel, utilityviewmodel));


                ourviewmodel.timerClock.Disposed += new EventHandler((s, e) => TimerClockDisposed(s, e, ourviewmodel));    // Has to be static for some obscure reason
#endif
#if WPF || UWP || WINUI || MAUI
                //ourviewmodel.timerClock.Interval = TimeSpan.FromSeconds(1);// new TimeSpan(0, 0, 1); // 1 Second 
#if WPF
                //ourviewmodel.timerClock.Tick += (s, e) => TimerClockTick(s, e, ourviewmodel);
#endif
#if UWP || WINUI || MAUI
                //ourviewmodel.timerClock.Tick += new EventHandler<object>((s, e) => TimerClockTick(s, e, ourviewmodel));
#endif

                // Won't happen until we either Open/Close
                // No dispose!  Typical Chimps shit.  It means I have to crash out of
                // the Tick event and back to the Sign In screen.  Usual Microshit bollocks.
                // What crap design of an O/S has FOUR FUCKING TYPES OF TIMER???
#endif
#if ANDROIDX
                //ourviewmodel.timerClock.Interval = SmartParametersV2016.clockInterval; // 1 Second 
                //ourviewmodel.timerClock.Elapsed += ((s, e) => TimerClockTick(s, e, ourviewmodel));   // Won't happen until we either Open/Close
                //ourviewmodel.timerClock.Disposed += TimerClockDisposed;
                // Won't happen until we dispose of this element
                // which means we have logged-out, closed the doors
                // AND stopped the clock .. so we can then return to 
                // the Sign in page??
#endif
                // Don't enable the Timer Clock until AFTER we have passed Led3 AND Led4 ...
                //  Turn on the clock **TURNED OFF**

                //ourviewmodel.timerClock.Start();

                _scheduler = new TickScheduler();

                _scheduler.Register(new SmartClockTask(ourviewmodel,
#if WPF
                    Dispatcher, this, 
#endif
                    _scheduler), false);
                //_scheduler.Register(new CryptoRefreshTask(_vm, Dispatcher));
                _ = _scheduler.StartAsync(); // fire-and-forget
                                             // Works for WPF and Android

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Clock started");
                // This works because we're not updating the main thread
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Clock started"))
                {
#if WINFORMS
                    return false;
#endif
#if WPF || UWP || WINUI || MAUI

                    return;
#endif
                }

                ourviewmodel.QuitEnabled = true;
                ourviewmodel.QuitColour = ourviewmodel.yellowColour;
#if ANDROIDX
                // Android button colour is already set in the MeterView layout ...
                ourviewmodel.AQuit.Click += (s, e) => ButtonQuitClick(s, e, ourviewmodel,
                                                                    profileviewmodel,
                                                                    financeviewmodel,
                                                                    utilityviewmodel);
#endif
                ourviewmodel.OpenCloseEnabled = true;

#if ANDROIDX
                ourviewmodel.OpenClose.Click += OpenCloseClicked;
#endif
                // This can now fail, if it must as we have a timer
                // ticking and we have the Quit button enabled if its needed

                // Create Local if it doesn't already exist.
                // And if it doesn't!! Assume its been DELETED and try
                // and download/rebuild all our stuff!!
                if (!await SmartPhyllV2020.CheckSQLiteNew(ourviewmodel))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SQLite failed");
                    // This works because we're not updating the main thread
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 9: SQLite failed: " + ourviewmodel.errorMessage))
                    {
#if WINFORMS
                        return false;
#endif
#if WPF || UWP || WINUI || MAUI

                        return;
#endif
                    }
#if WINFORMS
                    return false;
#endif
#if WPF || UWP || WINUI || MAUI
                    return;
#endif
#if ANDROIDX
                    return;
#endif
                }

                
                // Fix all the other arguments now its safe to do so ...
                ourviewmodel.trace = signinviewmodel.LoginKeys.trace;
                if (ourviewmodel.trace)
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 1, ourviewmodel.greenColour);
                }

                // Be CAREFUL!  The mainArgs[4] and mainArgs[5] strings MUST BE in
                // culture en-GB format.  Otherwise this ConateTime
                // might throw an error, and we have NO try/catch setup
                // So the DBS Led will show RED and the TR Led will show GREEN
                // (Don't know why this happens)
                // To make sure we don't get this problem in SmartSwitch.aspx.cs
                // we set the local culture to be "en-GB" - irrespective of what
                // it is - so that when we say 'expirationDate1.ToString(SmartParametersV2016.defaultCulture)' and
                // 'expirationDate2.ToString(SmartParametersV2016.defaultCulture)' we get the date strings in en-GB
                // culture format.  But now SmartSwitch might be running with
                // culture 'en-CA' so we have to force it to see the expiration
                // strings in culture 'en-GB' that was passed in

                // Make sure the expiration dates can be understood by this end
                // (which might have any culture set when it is invoked e.g. en-CA)
                // No - this is bollocks - we are ALWAYS working with "en-GB" behind the scenes
                //                
                // The "G" format allows us to parse dd/mmm/yyyy HH:mm:ss formats
                ourviewmodel.expiration[0] = SmartNibbyV2016.ConvertDate(signinviewmodel.LoginKeys.expirationDate1.ToString(), SmartParametersV2016.defaultDate, ourviewmodel);
                if (ourviewmodel.errorMessage != "")
                {
                    // If you don't see any Led lights changing from Orange to Green
                    // then this could be the reason
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 11: Date conversion failed " + signinviewmodel.LoginKeys.expirationDate1))
                    {
#if WINFORMS
                        return false;
#endif
#if WPF || UWP || WINUI || MAUI

                        return;
#endif
                    }
#if WINFORMS
                    return false;
#endif
#if WPF || UWP || WINUI || MAUI
                    return;
#endif
#if ANDROIDX
                    return;
#endif
                }
                ourviewmodel.expiration[1] = SmartNibbyV2016.ConvertDate(signinviewmodel.LoginKeys.expirationDate2.ToString(), SmartParametersV2016.defaultDate, ourviewmodel);
                if (ourviewmodel.errorMessage != "")
                {
                    // If you don't see any Led lights changing from Orange to Green
                    // then this could be the reason
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 12: Date conversion failed " + signinviewmodel.LoginKeys.expirationDate2))
                    {
#if WINFORMS
                        return false;
#endif
#if WPF || UWP || WINUI || MAUI

                        return;
#endif
                    }
#if WINFORMS
                    return false; // <== This may come back to bite you on the bum ...
#endif
#if WPF || UWP || WINUI || MAUI
                    return;
#endif
                }
                // Don't fix expiration3 (mainArgs[5]) yet, but fix these little tinkers
                if (!ourviewmodel.subscriber)
                {
                    ourviewmodel.subscriber = signinviewmodel.LoginKeys.subscriber;
                }
                // Multimeter here just says 'Have you subscribed and are therefore allowed
                // to turn it on and off?
                if (ourviewmodel.multimeter)
                {
                    ourviewmodel.multimeter = signinviewmodel.LoginKeys.multimeter;
                }
                if (!ourviewmodel.administrator)
                {
                    ourviewmodel.administrator = signinviewmodel.LoginKeys.administrator;
                }
                if (ourviewmodel.expiration[0] != SmartParametersV2016.defaultDate)
                {
                    // Show LATEST expiry (could be either Electricity OR Gas)
                    string expirationDisplay = "";
                    DateTime expirationCompare = DateTime.Now + signinviewmodel.utcOffset; // Local time

                    // Work out which less 1 or 2
                    if (SmartRoutinesV2018.DateTimeCompare(ourviewmodel.expiration[1], ourviewmodel.expiration[0]) < 0)
                    {
                        // 2 is less so show 1
                        expirationDisplay = signinviewmodel.LoginKeys.expirationDate1.ToString();
                        expirationCompare = ourviewmodel.expiration[0];
                    }
                    else
                    {
                        // 1 is less so show 2
                        expirationDisplay = signinviewmodel.LoginKeys.expirationDate2.ToString();
                        expirationCompare = ourviewmodel.expiration[1];
                    }
                    string expiryType = "free trial";
                    if (ourviewmodel.subscriber)
                    {
                        expiryType = "subscription";
                    }
                    if (SmartRoutinesV2018.DateTimeCompare(DateTime.Now + signinviewmodel.utcOffset, expirationCompare) <= 0)  // Local time
                    {
                        ourviewmodel.AccountStatusMessage = ourviewmodel.UserName + " - your " + expiryType + " is valid until: " + expirationDisplay;
                    }
                    else
                    {
                        ourviewmodel.AccountStatusMessage = ourviewmodel.UserName + " - your " + expiryType + " was only valid until: " + expirationDisplay;
                    }
#if WINFORMS || WPF || UWP || WINUI || MAUI
                    ourviewmodel.LastLoginStatusMessage = "(You last logged in on " + signinviewmodel.LoginKeys.lastLoginTime + ")"; // + svm.utcOffset?
#endif
                }
                if (signinviewmodel.LoginKeys.lastLoginTime.ToString() == "01/01/0001 00:00:00")
                {
                    ourviewmodel.firstTime = true;
                }
                // Fuck me!  Was this COMPLICATEd or WHAT???
                // Task.Run(() => GetGoing ....() ); doesn't work
                // Oh and
                // Task<bool> task = GetGoing( ... ); doesn't work either
                try
                {
                    if (!await GetGoing(
#if WINFORMS
                                    processComponents,
                                    profileviewmodel,
                                    financeviewmodel,
                                    utilityviewmodel,
#endif
#if WPF || UWP || WINUI || MAUI
                                    this,
#endif
                                    signinviewmodel,
                                    ourviewmodel,
                                    _scheduler

                                    ))
                    {
                        
                        // Led8 (might) be red here
#if WINFORMS
                        return false;
#endif
#if WPF || UWP || WINUI || ANDROIDX || MAUI
                        await ourviewmodel.sqliteDatabase.CloseAsync();

                        ourviewmodel.quityeschosen = true;  // Causes clock to stop
                        ourviewmodel.QuitEnabled = false;
                        ourviewmodel.OpenCloseEnabled = false;
                        return;
#endif
                    }
                }
                catch (Exception ex)
                {
                    ourviewmodel.errorMessage = ex.Message;
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
#if WINFORMS
                    return false;
#endif
#if WPF || UWP || WINUI || MAUI
                    return;
#endif
#if ANDROIDX
                    return;
#endif
                }
            }
            catch (Exception ex)
            {
                signinviewmodel.errorMessage = ex.Message;
#if WINFORMS
                return false;
#endif
#if WPF || UWP || WINUI || MAUI
                return;
#endif
#if ANDROIDX
                return;
#endif
            }
            
#if WINFORMS
            return true;
#endif
#if WPF || UWP || WINUI || MAUI
            return;
#endif
#if ANDROIDX
            return;
#endif
        }

        internal async Task<bool> GetGoing(
#if WINFORMS
                                                    MainProcess processComponents,
                                                    ProfilesViewModel profileviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    UtilityViewModel utilityviewmodel,
#endif
#if WPF || UWP || WINUI || MAUI
                                                    MainMeter meterComponents,
#endif

                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    TickScheduler _scheduler)
        {

           
            profileviewmodel = new ProfilesViewModel();
            try
            {
                ourviewmodel.UserNameBrush = ourviewmodel.whiteColour;
                if (!SmartNibbyV2016.NetworkAvailability())
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.redColour);
                    // Try to carry on ..
                    return false;
                }
                else
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour);
                }

                string databaseName = "SMARTUSERS"; // Load tables for the USER
                if (!await SmartNibbyV2016.MiserableJokeWife(DateTime.Now + signinviewmodel.utcOffset, // Local time
                                                                ourviewmodel,
                                                                signinviewmodel.signinToken,
                                                                ourviewmodel.UserName,
                                                                ourviewmodel.antiTokenString,
                                                                'D',
                                                                SmartParametersV2016.TotalTables,
                                                                databaseName))
                {
#if WINFORMS
                    processComponents.textBoxConsole.AppendText("Load " + databaseName + " tables failed" +
                                    Environment.NewLine.ToString());
                    processComponents.textBoxConsole.ScrollToCaret();
#endif
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                                      "Load " + databaseName + " tables failed");
                    return false;
                }
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Loaded " + databaseName);

                // Don't clear this yet, we may not even have one to load?
                //ourviewmodel.Hamas.consumersList.Clear();

                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Connecting");
                
                SmartProfile.Groups mmRecord = new SmartProfile.Groups()
                {
                    USERNAME = ourviewmodel.UserName,
                    GROUPNAME = ourviewmodel.UserName,
                    ACTIVEFLAG =true,
                    MARKER = Convert.ToDateTime(SmartParametersV2016.maximumDate),
                    PDEK = ourviewmodel.PDEK,
                    SENDF = true,
                    FDEK = ourviewmodel.FDEK,
                    SENDU = true,
                    UDEK = ourviewmodel.UDEK,
                    RECEIVEALL = true,
                    DISPLAYNAME = "Genius"
                };

                ourviewmodel.mmListX.Add(mmRecord);

                ourviewmodel.multiuser = new StringBuilder();
                SmartProfile.Groups nibs = new SmartProfile.Groups()
                {
                    USERNAME = ourviewmodel.UserName,
                    GROUPNAME = ourviewmodel.UserName,
                    ACTIVEFLAG = true,
                    MARKER = Convert.ToDateTime(SmartParametersV2016.maximumDate),
                    PDEK = ourviewmodel.PDEK,
                    SENDF = true,
                    FDEK = ourviewmodel.FDEK,
                    SENDU = true,
                    UDEK = ourviewmodel.UDEK,
                    RECEIVEALL = true,
                    DISPLAYNAME = "Placeholder"
                };

                // Build the schemas we 
                // In here, we now send our dates
                // WE NEVER KEEP OUR OWN STORED IN THE DB - We ALWAYS BUILD IT
                // We send our PDEK, FDEK and UDEK so our data can be
                // decoded IF we have allowed other users to do so...
                ourviewmodel.multiuser.Append(SmartPhyllV2020.DecodeToSQLUsername<SmartProfile.Groups>(nibs, false, SmartParametersV2016.fieldSeparator));
                
                foreach (SmartProfile.Groups group in ourviewmodel.SmartProfile.profilegroupsList)
                {
                    // The PDEK, FDEK and UDEK are the ones we want
                    // RETURNED because they allow us to decode the ANNA data
                    group.PDEK = group.FDEK = group.UDEK = "";
                    ourviewmodel.multiuser.Append(SmartParametersV2016.unitSeparator);
                    ourviewmodel.multiuser.Append(SmartPhyllV2020.DecodeToSQLUsername<SmartProfile.Groups>(group, false, SmartParametersV2016.fieldSeparator));

                }
                if (!await SmartBobV2017.ConnectDisconnectAsync(ourviewmodel,
                                                                    ourviewmodel.userToken,
                                                                    SmartParametersV2016.connectSymbol,
                                                                    ourviewmodel.multiuser,
                                                                    ourviewmodel.utcDates,
                                                                    SmartParametersV2016.defaultDates))  // No 'lastdate' here
                {
                    if (!ourviewmodel.quitCts.IsCancellationRequested)
                    {
                        // Set the second Led to Red
                        FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.redColour);
                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 13: Connecting - " + ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                        // Amazingly (and perfectly correctly) if the SmartSwitch load of
                        // Smart Users has failed, then Led 3 will already be Red here (as well)
                    }
                    return false;
                }

                // Set the second Led to Green
                FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.greenColour);
                // Tell the Listener who we are
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                ourviewmodel.userToken,
                                                0, 0,
                                                "Connected" +
                                                SmartParametersV2016.bar +
                                                ourviewmodel.trace +
                                                SmartParametersV2016.bar +
                                                ourviewmodel.expiration[0].ToString(SmartParametersV2016.defaultCulture) + // en-GB
                                                SmartParametersV2016.bar +
                                                ourviewmodel.expiration[1].ToString(SmartParametersV2016.defaultCulture))) // en-GB
                {
                    return false;
                }

                _scheduler.Register(new SmartKeepAliveTask(
#if WPF
                                                            Dispatcher, 
#endif
#if ANDROIDX
                                                            ourviewmodel.activity,
#endif
                                                            ourviewmodel, financeviewmodel, utilityviewmodel), false);

                // The SmartSwitch database is now loaded in SmartBobV2017 at
                // line 2305 with the Group contents of all the 'T' (true) tokens
                // from SmartDBServer.  These Groups have been put in the tokens
                // when more than one Username accesses SmartDBServer
                // from different devices ... Good fun, eh Ray??!?!
                // NO THIS IS TWADDLE - Multi-User doesn't work like THIS!!!!
                // YES it does.  All we've got to do now is load it dynamically

                //bool primary = true;    // Always have ONE!

                // Set the third Led to Green                                
                FrontEndGUI.SetLedColour(ourviewmodel, 3, ourviewmodel.greenColour);

                // DON'T roll this call into the following one!
                // We need it LIKE THIS so that Hamas and Hezbollah get filled up
                // properly BEFORE continuing ... otherwise we get a null exception
                // This arrangement WORKS.  SO leave it a-fucking-lone. <=THIS MEANS YOU
                // Fuck Me!  Was this COMPLICATED ... or what???
                try
                {
#if WPF
                    financeviewmodel = new FinanceViewModel();
#endif
#if UWP || WINUI || MAUI
                    financeviewmodel = new FinanceViewModel();
#endif
#if ANDROIDX
                    financeviewmodel = new FinanceViewModel();
#endif
#if WPF || UWP || WINUI || MAUI
                    financeviewmodel.arrowbutton = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\ExpanderOpen.jpg", UriKind.Absolute));
                    financeviewmodel.cashbag = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\cashbag.jpg", UriKind.Absolute));
                    financeviewmodel.piggybank = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\piggybank.jpg", UriKind.Absolute));
                    financeviewmodel.stockmarket = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\Stockmarket.jpg", UriKind.Absolute));
                    financeviewmodel.bitcoin = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\Bitcoin.jpg", UriKind.Absolute));
#endif

//                    if (signinviewmodel.Platform == SmartParametersV2016.WINFORMS ||
//                        signinviewmodel.Platform == SmartParametersV2016.WPF ||
//                        signinviewmodel.Platform == SmartParametersV2016.WINUI)
//                    {
//                        // Listener supported!
//#if WINFORMS || WPF || UWP || WINUI || MAUI
//#pragma warning disable CA1416
//                        ourviewmodel.notificationListener = FrontEndGUI.SetListener();
                        
//                        //Listener.notificationListener = Listener.GetCurrent();
//                        // And request access to the user's notifications (must be called from UI thread)
                        
//                        ourviewmodel.accessStatus = await ourviewmodel.notificationListener.RequestAccessAsync();

//                        //Listener.accessStatus = await Listener.RequestAccess(Listener.notificationListener);
//#pragma warning restore CA1416
//#endif
//                        if (ourviewmodel.accessStatus.ToString() == "Denied")

//                        {
//                            // Access denied
//#if WINFORMS
//                        processComponents.textBoxConsole.AppendText("Access *DENIED* " +
//                                    ourviewmodel.accessStatus + " " +
//                                    Environment.NewLine.ToString());
//                        processComponents.textBoxConsole.ScrollToCaret();
//#endif
//                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
//                                                            "Listener access *DENIED*");

//                        }
//                        else
//                        {
//                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
//                                                                    "Listener access ALLOWED");

//                        }
//                    }

#if WPF
                    utilityviewmodel = new UtilityViewModel();
#endif
#if UWP || WINUI || MAUI
                    utilityviewmodel = new UtilityViewModel();
#endif
#if ANDROIDX
                    utilityviewmodel = new UtilityViewModel();
#endif
                    utilityviewmodel.utilityToken = utilityviewmodel.utilityCts.Token;

#if WPF || UWP || WINUI || MAUI
                    utilityviewmodel.electricbulb = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\ElectricLightBulb.jpg", UriKind.Absolute));
                    utilityviewmodel.gasflame = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\GasFlame.jpg", UriKind.Absolute));
#endif
                }

                catch (Exception ex)
                {
                    // Set the last Led to Red
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 18: Model(s) creation failed: " + ex.Message))
                    {
                        return false;
                    }
                    return false;
                }

                if (!await DoWork(
#if WINFORMS
                                    processComponents,
#endif
#if WPF || UWP || WINUI || MAUI
                                    meterComponents,
#endif
                                    signinviewmodel,
                                    ourviewmodel,
                                    profileviewmodel,
                                    financeviewmodel,
                                    utilityviewmodel))
                {
                    if (!ourviewmodel.quitCts.Token.IsCancellationRequested)
                    {
                        // Set the last Led to Red
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 19: DoWork failed"))
                        {
                            return false;
                        }
                    }
                    return false;
                }
//                else
//                {
//                    // Not WINFORMS - yes WINFORMS!!
//                    if (!DoViews(
//#if WINFORMS
//                                            processComponents,
//#endif
//#if WPF || UWP || WINUI || MAUI
//                                            meterComponents,
//#endif
//                                            ourviewmodel))
//                    {
//                        // SOMETHING has gone amiss ..
//                        if (!ourviewmodel.quitCts.Token.IsCancellationRequested)
//                        {
//                            // Set the last Led to Red
//                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
//                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 20: DoViews failed"))
//                            {
//                                return false;
//                            }
//                        }
//                        return false;
//                    }
//                    // Call this - which (should) open the door as the entry for OpenClose should be GREEN
//                    // This USED to have an await on it?
//                    if (!SmartRoutinesV2018.MoveTheDoorsAsync(ourviewmodel))
//                    {
//                        return false;
//                    }
//                    else
//                    {
//                        // White disables connects
//                        // Orange allows Scrapes
//                        // Green we are scraping
//                        FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.orangeColour);
//                    }
//                    // FUCK ME!!! WAS THIS COMPLICATED **OR WHAT***????
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
//                                                                "Doors are OPEN");
//                    ourviewmodel.Phase1 = false;
//                    // Now we can do Phases 2 and 3. Possibly.
//                    // So we don't get an immediate Connect
//                    ourviewmodel.keepaliveCount = 0;
//                    // Phase3 true means Connect not in progress
//                    ourviewmodel.Phase3 = true;
//                }
            }
            catch (Exception ex)
            {
                ourviewmodel.errorMessage = ex.Message;
                return false;
            }
            // From here, we should just see the clock ticking away ...
            

            return true;
        }

        // Fuck Me!  Was this COMPLICATED ... or what???
        internal static async Task<bool> DoWork(
#if WINFORMS
                                                MainProcess processComponents,
#endif
#if WPF || UWP || WINUI || MAUI
                                                MainMeter meterComponents,
#endif
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                ProfilesViewModel profileviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            // This technique will only FAIL (!!!) IF the user alter's the PC's date/time between when
            // we get the offset (in the website at Keasdon_Energy\SmartSwitch.aspx.cs) and now.
            // This technique assumes that this is not the case (and it will only happen in extremely rare and
            // not worth bothrting about circumstances).  The poorer alternative is to get UTC from the
            // connect to DBServer which I don't think is as elegant as what we have now ...
            //
            // But I might just do that in any case ...

            // Fuck Me!  Was this COMPLICATED ... or what???
            try
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Databases");
                if (FrontEndGUI.CompareLedColour(ourviewmodel, 3, ourviewmodel.greenColour))
                {
                    if (ourviewmodel.Hamas.consumersList.Count == 1)
                    {
                        if (ourviewmodel.Hamas.consumersList.First().FULL_SCREEN)
                        {
                            // Make it full screen
#if WINFORMS
                            SmartRoutinesV2018.FullScreenButtonClickActual(ourviewmodel);
#endif
#if WPF || UWP || WINUI || MAUI
                            SmartRoutinesV2018.FullScreenButtonClickActual(meterComponents, ourviewmodel);
#endif
                        }
                    }

                    if (ourviewmodel.SmartProfile.profilecubefacesList.Count == 0)
                    {
                        // Cubefaces is ordered by ascending SCREEN_CODE so
                        // Users/Profile 'P' should come first
                        foreach (SmartData.Cubefaces cubefacesRow in signinviewmodel.Fatah.cubefacesList) // Checked
                        {
                            if (cubefacesRow.ACTIVEFLAG)
                            {
                                SmartProfile.Cubefaces cubeRow = new SmartProfile.Cubefaces()
                                {
                                    USERNAME = ourviewmodel.UserName,
                                    CUBEFACE_CODE = cubefacesRow.CUBEFACE_CODE,
                                    FACE_ACTIVE = SmartParametersV2016.inactive,
                                    CUBEFACE_CREATED = DateTime.UtcNow,     // UTC time
                                    FACE_LAST_DISPLAY = "",
                                    FACE_CULTURE_CODE = "UK",
                                    FACE_AUTOSWITCH = SmartParametersV2016.defaultAutoswitch,      // Not sure wot this means now! (But it needs to be in)
                                    FACE_CURRENCY = 0,  // None is the default
                                    NEXT_CONNECTION = SmartParametersV2016.defaultDate,
                                    Updated = false     // Means its a new record
                                };
                                if (cubeRow.CUBEFACE_CODE == SmartParametersV2016.Profiles)
                                {
                                    cubeRow.FACE_ACTIVE = SmartParametersV2016.active;
                                    cubeRow.FACE_LAST_DISPLAY = SmartParametersV2016.lastChecked;
                                }
                                ourviewmodel.SmartProfile.profilecubefacesList.Add(cubeRow);
                                ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubeRow);
                            }
                        }
                        if (ourviewmodel.SmartProfile.profilecubefacesChangesList.Count > 0)
                        {
                            if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                            SmartParametersV2016.sqliteformat))
                            {
                                FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 21: Main adding cubefaces"))
                                {
                                    return false;
                                }
                                return false;
                            }
                        }
                    }

                    if (ourviewmodel.SmartProfile.profilecubefacesList.Count > 0)
                    {

                        FrontEndGUI.ClearViewCollection(ourviewmodel);
#if WINFORMS
                        processComponents.tabControlCategories.Controls.Clear();
#endif
                        // Because ADDing to the ItemsSource doesn't force it to change in the ViewModel, remember?            

                        // This fucker is SORTED by ascending CUBEFACE_CODE when we
                        // read it from SQLite!!!!!!!!!!!!!!

                        List<SmartProfile.Cubefaces> ordered =
                        SmartSpikeV2017.OrderCUBEFACECode(SignIn.signinviewmodel.Fatah.cubefacesList, ourviewmodel);
                        // If the cubefaces aren't in P, F, U order (?) then everything goes
                        // to rat-shit ...aR
                        foreach (SmartProfile.Cubefaces cubefaceRow in ordered) // Checked
                        {
                            // Are we the logged-in User?
                            if (cubefaceRow.USERNAME == ourviewmodel.UserName)
                            {
                                short screenCode = SmartSpikeV2017.LookupScreenCode(ourviewmodel.CUBEFACESList,
                                                                                        cubefaceRow.CUBEFACE_CODE);
                                //string databaseName = "";

                                switch (cubefaceRow.CUBEFACE_CODE)
                                {
                                    case SmartParametersV2016.Profiles:
                                        // We need this View whether its active or not
                                        // But this one should ALWAYS be active!!!!
                                        //profileviewmodel.ScreenCode = screenCode;
                                        ourviewmodel.screenCode = SmartSpikeV2017.Lookup_Cubeface_ScreenCode(signinviewmodel,
                                                                                                            SmartParametersV2016.Profiles);

                                        if (cubefaceRow.FACE_ACTIVE)
                                        {
                                            if (!await SmartRoutinesV2018.ProfileModule(
#if WINFORMS
                                                                                processComponents,
#endif
                                                                                signinviewmodel,
                                                                                ourviewmodel,
                                                                                profileviewmodel,
                                                                                cubefaceRow.CUBEFACE_CODE,
                                                                                cubefaceRow.FACE_CULTURE_CODE,
                                                                                cubefaceRow.FACE_LAST_DISPLAY))
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case SmartParametersV2016.Finance:
                                        // We *always* need to initialize the
                                        // ScreenCode whether the face is active or not
                                        ourviewmodel.screenCode = SmartSpikeV2017.Lookup_Cubeface_ScreenCode(signinviewmodel,
                                                                                                            SmartParametersV2016.Finance);
                                        if (cubefaceRow.FACE_ACTIVE)
                                        {
                                            if (!await SmartRoutinesV2018.LoadFinanceModule(
#if WINFORMS
                                                                                processComponents,
#endif
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                cubefaceRow.CUBEFACE_CODE,
                                                                                cubefaceRow.FACE_CULTURE_CODE,
                                                                                cubefaceRow.FACE_CURRENCY,     // None is the default
                                                                                cubefaceRow.FACE_LAST_DISPLAY,
                                                                                cubefaceRow.NEXT_CONNECTION))
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case SmartParametersV2016.Insurance:
                                        // We *always* need to initialize the
                                        // ScreenCode whether the face is active or not
                                        ourviewmodel.screenCode = SmartSpikeV2017.Lookup_Cubeface_ScreenCode(signinviewmodel,
                                                                                                            SmartParametersV2016.Insurance);

                                        if (cubefaceRow.FACE_ACTIVE)
                                        {
                                            if (!await SmartRoutinesV2018.LoadInsuranceModule(
#if WINFORMS
                                                                                processComponents,
#endif
                                                                                ourviewmodel,
                                                                                cubefaceRow.CUBEFACE_CODE,
                                                                                cubefaceRow.FACE_CULTURE_CODE))
                                            {
                                                return false;
                                            }

                                        }
                                        break;
                                    case SmartParametersV2016.Utility:
                                        // We *always* need to initialize the
                                        // ScreenCode whether the face is active or not
                                        ourviewmodel.screenCode = SmartSpikeV2017.Lookup_Cubeface_ScreenCode(signinviewmodel,
                                                                                                            SmartParametersV2016.Utility);
                                        if (cubefaceRow.FACE_ACTIVE)
                                        {
                                            if (!await SmartRoutinesV2018.LoadUtilityModule(
#if WINFORMS
                                                                                    processComponents,
#endif
                                                                                    ourviewmodel,
                                                                                    utilityviewmodel,
                                                                                    cubefaceRow.CUBEFACE_CODE,
                                                                                    cubefaceRow.FACE_CULTURE_CODE,
                                                                                    cubefaceRow.FACE_CURRENCY,
                                                                                    cubefaceRow.FACE_LAST_DISPLAY,
                                                                                    cubefaceRow.NEXT_CONNECTION))
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                // Fuck Me!  Was this COMPLICATED ... or what???
                            }
                        }
                        // Set the fourth Led to Green ... and carry on!
                        FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.greenColour);
                    }
                }
                // Now we have loaded in EVERYTHING i.e. every face we possibly
                // can either from Remote OR Local.  But - we will only 'see'
                // the face if its ACTIVE FLAG is set ...
                //ourviewmodel.loadRemote = false; // Even if its already false
#if WINFORMS || WPF || UWP || WINUI || MAUI
                ourviewmodel.originalBackgroundColour = ourviewmodel.OpenCloseColour;  // Gets the background color
#endif
#if ANDROIDX
                ourviewmodel.originalBackgroundColour0 = ourviewmodel.OpenClose0;  // Gets the background color
                ourviewmodel.originalBackgroundColour25 = ourviewmodel.OpenClose25;  // Gets the background color
                ourviewmodel.originalBackgroundColour50 = ourviewmodel.OpenClose50;  // Gets the background color
                ourviewmodel.originalBackgroundColour75 = ourviewmodel.OpenClose75;  // Gets the background color
#endif
            }
            catch (ArgumentNullException exception)
            {
                // SOMETHING has gone amiss ...
                ourviewmodel.errorMessage = exception.Message;
            }
            catch (NullReferenceException exception)
            {
                // SOMETHING has gone seriously amiss ...
                ourviewmodel.errorMessage = exception.Message;
            }
            catch (Exception exception)
            {
                // SOMETHING has gone amazingly amiss ...
                ourviewmodel.errorMessage = exception.Message;
                return false;
            }
            if (ourviewmodel.errorMessage != "")
            {
                // Try and tell HQ
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 22: " + ourviewmodel.errorMessage))
                {
                    return false;
                }
                return false;
            }
            return true;
        }

        internal static bool DoViews(
#if WINFORMS
                                           MainProcess processComponents,
#endif
#if WPF || UWP || WINUI || MAUI
                                           MainMeter meterComponents,
#endif

                                            MainViewModel ourviewmodel)
        {
            // For Swiping go to 
            // https://stackoverflow.com/questions/53755920/how-to-create-left-and-right-swipes-in-wpf-using-mouse
            // and use answer from Shubham Sahu
#if WPF || UWP || WINUI || MAUI
            if (ourviewmodel.viewCollection.Count > 0)
            {
                ourviewmodel.CarouselEnabled = true;
                char cubefaceCode = SmartSpikeV2017.FindCubefaceLastDisplay(ourviewmodel);
                ourviewmodel.screenCode = SmartSpikeV2017.LookupCubefacesScreenCode(SignIn.signinviewmodel,
                                                                                    cubefaceCode);
                ourviewmodel.CurrentPosition = ourviewmodel.screenCode;
                if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, cubefaceCode))
                {
                    ourviewmodel.MyContent = SmartRoutinesV2018.CubefaceView(ourviewmodel, cubefaceCode);
                }
            }
#endif
#if ANDROIDX
            if (ourviewmodel.viewCollection.Count > 0)
            {
                ourviewmodel.CarouselEnabled = true;
                char cubefaceCode = SmartSpikeV2017.FindCubefaceLastDisplay(ourviewmodel);
                ourviewmodel.screenCode = SmartSpikeV2017.LookupCubefacesScreenCode(SignIn.signinviewmodel,
                                                                                    cubefaceCode);
                ourviewmodel.CurrentPosition = ourviewmodel.screenCode;
                ourviewmodel.MyContent.Touch += MyContentTouch;
                if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, cubefaceCode))
                {
                    ourviewmodel.MyContent.AddView(SmartRoutinesV2018.CubefaceView(ourviewmodel, cubefaceCode));
                }

                //ourviewmodel.CurrentPosition = ourviewmodel.screenCode;
                //char cubefaceCode = SmartSpikeV2017.LookupCUBEFACECode(SignIn.signinviewmodel.Fatah.cubefacesList,
                //                                                ourviewmodel,
                //                                                ourviewmodel.screenCode);
                //ourviewmodel.MyContent.Touch += MyContentTouch;
                //if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, cubefaceCode))
                //{
                //    ourviewmodel.MyContent.AddView(SmartRoutinesV2018.CubefaceView(ourviewmodel, cubefaceCode));
                //}
            }
#endif
            // See the notes inside ourviewmodel concerning this
            // I couldn't .. FOR THE FUCKING LIFE OF ME .. get the CarouselView IsEnabled binding
            // to work ... I just had to do it this way (i.e. the fucking hard way like everything
            // else in theis fucking system ..)
            //ourviewmodel.CarouselPosition = ourviewmodel.screen;

#if WINFORMS
            if (ourviewmodel.screenCode != -1)
            {
                try
                {
                    processComponents.tabControlCategories.SelectedIndex = ourviewmodel.screenCode;
                }
                catch (Exception ex)
                {
                    ourviewmodel.errorMessage = ex.Message;
                    return false;
                }
            }
#endif
            return true;
        }

#if ANDROIDX
        internal static System.Single x1;
        internal static System.Single x2;
        internal static bool moveDetected = false;

        internal static void MyContentTouch(object sender, TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    x1 = e.Event.GetX();
                    break;
                case MotionEventActions.Up:
                    if (moveDetected)
                    {
                        x2 = e.Event.GetX();
                        float deltaX = x2 - x1;
                        if (deltaX > 0)
                        {
                            Toast.MakeText(ourviewmodel.activity,
                                        "Swipe right",  // So show previous View 
                                        ToastLength.Short).Show();
                            SmartRoutinesV2018.OnSwipedRightActual(sender, e);
                        }
                        else
                        {
                            Toast.MakeText(ourviewmodel.activity,
                                        "Swipe left",   // So show next View
                                        ToastLength.Short).Show();
                            SmartRoutinesV2018.OnSwipedLeftActual(sender, e);
                        }
                        moveDetected = false;
                    }
                    break;
                case MotionEventActions.Move:
                    moveDetected = true;
                    break;
                default:
                    break;
            }
            return;
        }
#endif

        
        // Do **NOT** fuck about with this next line!!!!
        // ====> I DON'T KNOW WHY IT WORKS, BUT IT FUCKING DOES  <====
        // If you LEAVE THIS LINE ALONE, all the 'await' calls work
        // If you CHANGE IT or FUCK ABOUT WITH IT then ***NOTHING*** fucking works
        // ====> SO LEAVE IT A-FUCKING-LONE <====


#if WINFORMS || WPF || WINUI || MAUI
        internal void OpenCloseClicked(object sender, object e,
                                        MainViewModel ourviewmodel)
        {
#endif
#if UWP
        internal void OpenCloseClicked(object sender, RoutedEventArgs e,
                                        MainViewModel ourviewmodel)
        {
#endif
#if ANDROIDX
        internal void OpenCloseClicked(object sender, EventArgs e)
        {
#endif
            if ((sender != null) &&
#if WINFORMS || WPF
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if UWP || WINUI || MAUI
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
#endif
#if ANDROIDX
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
            {
                SmartRoutinesV2018.OpenCloseClickedActual(ourviewmodel, SignIn.signinviewmodel.UserIsLoggedIn);
                //await SmartRoutinesV2018.OpenCloseClickedActual(ourviewmodel, SignIn.signinviewmodel.UserIsLoggedIn);
            }
            return;
        }

#if WINFORMS
        internal void FullScreenButtonClick(object sender, EventArgs e)
#endif
#if WPF || UWP || WINUI || MAUI
        internal void FullScreenButtonClick(object sender, object e)
#endif
#if ANDROIDX
        internal void FullScreenButtonClick(object sender, object e)
#endif
        {
            // Need to "hide" the first three lines ?
            if (sender != null &&
#if WINFORMS || WPF
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if UWP || WINUI || MAUI
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
#endif
#if ANDROIDX
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
            {
                switch (SignIn.signinviewmodel.Platform)
                {
#if WPF
                    case SmartParametersV2016.WPF:
                        SmartRoutinesV2018.FullScreenButtonClickActual(this, ourviewmodel);
                        break;
#endif
#if UWP
                    case SmartParametersV2016.UWP:
                        SmartRoutinesV2018.FullScreenButtonClickActual(this, ourviewmodel);
                        break;
#endif
#if WINUI || MAUI
                    case SmartParametersV2016.WINUI:
                        SmartRoutinesV2018.FullScreenButtonClickActual(this, ourviewmodel);
                        break;
#endif
#if ANDROIDX
                    case SmartParametersV2016.Android:
                        // Think Android is always full-screen in any case??
                        SmartRoutinesV2018.FullScreenButtonClickActual(this, SignIn.signinviewmodel, ourviewmodel);
                        break;
#endif
                    default:
                        break;
                }
            }
            return;
        }

#if ANDROIDX
        internal static void ButtonQuitClick(object sender, EventArgs e,
                                                MainViewModel ourviewmodel,
                                                ProfilesViewModel profileviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            if (sender != null && e != null)
            {
                if (!ourviewmodel.quitinprocess)
                {
                    // Kill the event
                    ourviewmodel.quitinprocess = true;

                    ourviewmodel.APopupDialog = new QuitMessageBox(ourviewmodel.activity);
                    ourviewmodel.APopupDialog.SetContentView(Resource.Layout.QuitMessageBox);

                    int id1 = (int)typeof(Resource.Id).GetField("Confirmation").GetValue(null);
                    ourviewmodel.AConfirmation = ourviewmodel.APopupDialog.FindViewById<TextView>(id1);
                    int id2 = (int)typeof(Resource.Id).GetField("ReallyLeave").GetValue(null);
                    ourviewmodel.AReallyLeave = ourviewmodel.APopupDialog.FindViewById<TextView>(id2);
                    int id3 = (int)typeof(Resource.Id).GetField("QuitYes").GetValue(null);
                    ourviewmodel.AQuitYes = ourviewmodel.APopupDialog.FindViewById<TextView>(id3);
                    int id4 = (int)typeof(Resource.Id).GetField("QuitNo").GetValue(null);
                    ourviewmodel.AQuitNo = ourviewmodel.APopupDialog.FindViewById<TextView>(id4);

                    ourviewmodel.AConfirmation.Text = ourviewmodel.Confirmation;
                    ourviewmodel.AReallyLeave.Text = ourviewmodel.ReallyLeave;
                    ourviewmodel.AQuitYes.Text = ourviewmodel.Yes;
                    ourviewmodel.AQuitNo.Text = ourviewmodel.No;

                    ourviewmodel.APopupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);

                    ourviewmodel.APopupDialog.Show();

                    // Some Time Layout width not fit with windows size  
                    // but Below lines are not necessery  
                    //Mainourviewmodel.activity.mmviewmodel.PopupDialog.Window.SetLayout(LayoutParams.MatchParent, LayoutParams.WrapContent);
                    //Mainourviewmodel.activity.mmviewmodel.PopupDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);

                    // Access Popup layout fields like below  
                    Button btnPopupNo = ourviewmodel.APopupDialog.FindViewById<Button>(Resource.Id.QuitNo);
                    Button btnPopupYes = ourviewmodel.APopupDialog.FindViewById<Button>(Resource.Id.QuitYes);

                    // Set the dialog Title Property - popupDialog.Window.SetTitle("Alert Title");

                    btnPopupNo.Click += (s, e) => QuitMessageBox.PopupNo(s, e, ourviewmodel);
                    btnPopupYes.Click += async (s, e) => await QuitMessageBox.PopupYes(s, e,
                                                                            ourviewmodel,
                                                                            profileviewmodel,
                                                                            financeviewmodel,
                                                                            utilityviewmodel);
                }
            }
            return;
        }
#endif

#if WINFORMS
        internal static void ButtonQuitClick(object sender, EventArgs e,
                                                MainViewModel ourviewmodel,
                                                ProfilesViewModel profileviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            if (sender != null && e != null)
            {
                QuitConfirmForm popup = new QuitConfirmForm();

                DialogResult dialogresult = popup.ShowDialog();
                // If I put a FormClosed event in for this Form, then
                // it gets called when I click 'yes' or 'No and that
                // is TOO EARLY!!  I want the Close code executed when
                // I choose Yes and not before.
                if (dialogresult == System.Windows.Forms.DialogResult.OK)
                {
                    QuitMessageBox.PopupYes(sender, e,
                                        ourviewmodel,
                                        profileviewmodel,
                                        financeviewmodel,
                                        utilityviewmodel);

                }
                else if (dialogresult == System.Windows.Forms.DialogResult.Cancel)
                {
                    QuitMessageBox.PopupNo(sender, e, ourviewmodel);
                }
                // Popup is already closed here
                popup.Dispose();
            }
            return;
        }
#endif

#if WPF
        internal void ButtonQuitClick(object sender, object e)
        {
            if (sender != null &&
                (FrontEndGUI.DecodeEventFlags(e) != null))
            {
                QuitMessageBox quitmessageboxPage = new QuitMessageBox(profileviewmodel);
                FrontEndGUI.DecodeMessageBox(this, quitmessageboxPage);
            }
            return;
        }
#endif

#if UWP
        internal async void ButtonQuitClick(object sender, object e)
        {
            if (sender != null &&
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
            {
                QuitMessageBox messageboxPage = new QuitMessageBox(profileviewmodel);
                await FrontEndGUI.DecodeMessageBox(messageboxPage);
            }
            return;
        }
#endif
#if WINUI || MAUI
        internal async void ButtonQuitClick(object sender, object e)
        {
            if (sender != null &&
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
            {
                QuitMessageBox messageboxPage = new QuitMessageBox(profileviewmodel)
                {
                    XamlRoot = this.XamlRoot
                };
                await FrontEndGUI.DecodeMessageBox(messageboxPage);
            }
            return;
        }
#endif
        //#if WPF
        //                    // Took me ALL DAY to get this fucker working ...
        //                messageboxPage.PlacementTarget = this;
        //                messageboxPage.Placement = FrontEndGUI.SetPlacement();
        //                messageboxPage.IsOpen = true;
        //#endif
        //                
        //#if UWP || WINUI
        //                await messageboxPage.ShowAsync();
        //#endif
        //}
        //            return;
        //#endif
        //#if ANDROIDX 
        //            // Maybe can do the SAME AS DONE ABOVE????
        //            if (sender != null && e != null)
        //            {
        //                MessageBox messageboxPage = new MessageBox();
        //#if ANDROIDX
        //                View mb = (View)Resource.Layout.MessageBox;
        //                GravityFlags gflags = new GravityFlags();
        //                messageboxPage.ShowAtLocation(mb, gflags, 0, 0);
        //#endif
        //            }
        //            return;
        //#endif
        //        }

        


        

#if WINFORMS || WPF
        internal async void ButtonLeftClicked(object sender, EventArgs e,
                                                MainViewModel ourviewmodel)
        {
            await ButtonLeftClickedActual(ourviewmodel);
            return;
        }
#endif
        internal static async Task<bool> ButtonLeftClickedActual(MainViewModel ourviewmodel)
        {
            short lower = 0;
            short upper = (short)ourviewmodel.SmartProfile.profilecubefacesList.Count;
            short scode = ourviewmodel.screenCode;
            bool limit = true;
            while (true)
            {
                scode--;
                if (scode < lower)
                {
                    if (limit)
                    {
                        break;
                    }
                    scode = upper;
                }
                char oldcubefaceCode = SmartSpikeV2017.LookupCubefacesCubefaceCode(SignIn.signinviewmodel,
                                                                                    ourviewmodel.screenCode);


                char newcubefaceCode = SmartSpikeV2017.LookupCubefacesCubefaceCode(SignIn.signinviewmodel,
                                                                                    scode);
                if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, newcubefaceCode))
                {
#if WINFORMS || WPF || UWP || WINUI || MAUI
                    ourviewmodel.MyContent = SmartRoutinesV2018.CubefaceView(ourviewmodel, newcubefaceCode);
#endif
#if ANDROIDX
                    // WTF are you doing here? Why are you removing all the views
                    // and then adding everything back EXCEPT Profile???
                    ourviewmodel.MyContent.RemoveView(SmartRoutinesV2018.CubefaceView(ourviewmodel, oldcubefaceCode));
                    ourviewmodel.MyContent.AddView(SmartRoutinesV2018.CubefaceView(ourviewmodel, newcubefaceCode));
#endif
                    await SmartRoutinesV2018.RecordCubeSwitch(ourviewmodel, newcubefaceCode);
                    ourviewmodel.screenCode = scode;
                    break;
                }
            }
            // Cuppa tea time Raymondo!!  Think this fucking shit actually works!!!!!
            return true;
        }


#if WINFORMS || WPF
        internal async void ButtonRightClicked(object sender, EventArgs e,
                                                MainViewModel ourviewmodel)
        {
            await ButtonRightClickedActual(ourviewmodel);
            return;
        }
#endif

        internal static async Task<bool> ButtonRightClickedActual(MainViewModel ourviewmodel)
        {
            short lower = 0;
            short upper = (short)ourviewmodel.SmartProfile.profilecubefacesList.Count;
            short scode = ourviewmodel.screenCode;
            bool limit = true;
            while (true)
            {
                scode++;
                if (scode > upper)
                {
                    if (limit)
                    {
                        break;
                    }
                    scode = lower;
                }
                char oldcubefaceCode = SmartSpikeV2017.LookupCubefacesCubefaceCode(SignIn.signinviewmodel,
                                                                                    ourviewmodel.screenCode);

                char newcubefaceCode = SmartSpikeV2017.LookupCubefacesCubefaceCode(SignIn.signinviewmodel,
                                                                                    scode);
                if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, newcubefaceCode))
                {
#if WINFORMS || WPF || UWP || WINUI || MAUI
                    ourviewmodel.MyContent = SmartRoutinesV2018.CubefaceView(ourviewmodel, newcubefaceCode);
#endif
#if ANDROIDX
                    ourviewmodel.MyContent.RemoveView(SmartRoutinesV2018.CubefaceView(ourviewmodel, oldcubefaceCode));
                    ourviewmodel.MyContent.AddView(SmartRoutinesV2018.CubefaceView(ourviewmodel, newcubefaceCode));
#endif
                    await SmartRoutinesV2018.RecordCubeSwitch(ourviewmodel, newcubefaceCode);
                    ourviewmodel.screenCode = scode;
                    break;
                }
            }
            return true;
        }

#if WINFORMS || WPF
        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal void SliderValueChanged(object sender, object e)
#endif
#if WPF
        internal async void SliderValueChanged(object sender, object e)
#endif
        {
            if (sender != null &&
#if WINFORMS
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF
                FrontEndGUI.DecodeRoutedPropertyChangedEventFlags(e) != null)
#endif
            {
#if WPF
                // Find the category given the screen
                if (!await SmartUtilityV2022.SliderValueChangedActual(FrontEndGUI.DecodeSlider(sender),
                                                                        SignIn.signinviewmodel,
                                                                        ourviewmodel,
                                                                         utilityviewmodel,
                                                                         ourviewmodel.Blanche.vatRatesList,
                                                                         ourviewmodel.Blanche.exchangeRatesList))
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Slider failed"))
                    {
                        return;
                    }
                }
#endif
            }
            return;
        }
#endif

#if ANDROIDX
        //        internal async void SliderValueChanged(object sender, EventArgs e)//SeekBar.ProgressChangedEventArgs e)
        //        {
        //            if (sender != null && e != null)
        //            {
        //                Slider slider = sender as Slider;
        //                // Find the category given the screen
        //                if (!await SmartUtilityV2022.SliderValueChangedActual(slider,
        //                                                                        ourviewmodel,
        //                                                                        SignIn.signinviewmodel,
        //                                                                         utilityviewmodel,
        //                                                                         ourviewmodel.Blanche.vatRatesList,
        //                                                                         ourviewmodel.Blanche.exchangeRatesList))
        //                {
        //                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Slider failed");
        //                }
        //            }
        //            return;
        //        }
#endif
#if WPF

        //        internal static void ApplyWindowMouseButtons(UIElement glaze)
        //        {
        //            if (glaze != null)
        //            {
        //                glaze.MouseLeftButtonDown += new MouseButtonEventHandler(SmartRoutinesV2018.PopupMouseLeftButtonDown);
        //                glaze.MouseLeave += new System.Windows.Input.MouseEventHandler(SmartRoutinesV2018.PopupMouseLeave);
        //            }
        //            return;
        //        }

        //        internal static void ApplyGridMouseButtons(UIElement square)
        //        {
        //            if (square != null)
        //            {
        //                square.MouseLeftButtonDown += new MouseButtonEventHandler(SmartRoutinesV2018.PopupMouseLeftButtonDown);
        //            }
        //            return;
        //        }
#endif

#if ANDROIDX

        //        internal static void ApplyWindowMouseButtons(ContentView glaze)
        //        {
        //            if (glaze != null)
        //            {
        //                glaze.PointerPressed += new PointerEventHandler(SmartRoutinesV2018.PopupMouseLeftButtonDown);
        //                glaze.PointerExited += new PointerEventHandler(SmartRoutinesV2018.PopupMouseLeave);
        //            }
        //            return;
        //        }
#endif

#if WPF
        internal void DoorsAreOpen(object sender, EventArgs e)
        {
            ourviewmodel.doorDirection = true;
        }
        internal void DoorsAreClosed(object sender, EventArgs e)
        {
            ourviewmodel.doorDirection = false;
        }
#endif
#if UWP || WINUI || MAUI
        internal void DoorsAreOpen(object sender, object e)
        {
            ourviewmodel.doorDirection = true;
        }
        internal void DoorsAreClosed(object sender, object e)
        {
            ourviewmodel.doorDirection = false;
        }
#endif
#if ANDROIDX
        internal void DoorsAreOpen(object sender, EventArgs e)
        {
            ourviewmodel.doorDirection = true;
        }
        internal void DoorsAreClosed(object sender, EventArgs e)
        {
            ourviewmodel.doorDirection = false;
        }
#endif

#if WINFORMS
        // Static is needed here for WINFORMS
        internal static async void TimerClockTick(object sender, EventArgs e, 
                                                    MainProcess processComponents,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    UtilityViewModel utilityviewmodel)
#endif
#if WPF
        internal async void TimerClockTick(object sender, EventArgs e, MainViewModel ourviewmodel)
#endif
#if UWP || WINUI || MAUI
        internal async void TimerClockTick(object sender, object e, MainViewModel ourviewmodel)
#endif
#if ANDROIDX
        internal async void TimerClockTick(object sender, EventArgs e, MainViewModel ourviewmodel)
#endif
        {
//            if (ourviewmodel.quityeschosen &&
//#if WINFORMS
//                    ourviewmodel.timerDoor.Enabled == false && // Wait for doors to close
//#endif
//                    !ourviewmodel.QuitEnabled &&
//                    !ourviewmodel.OpenCloseEnabled &&
//                    !ourviewmodel.doorDirection)
//                {
//                    //
//                    // Clock stop sequence
//                    //
//                    // Remove the event handler... Oh! we're in it!!
//#if WPF || UWP || WINUI || MAUI
//                    //ourviewmodel.timerClock.Tick -= TimerClockTick;
//#endif
//#if ANDROIDX
//                    //ourviewmodel.timerClock.Elapsed -= TimerClockTick;
//#endif
//                    // Stop the clock. Now, we haven't removed the event handler
//                    // but I think its safe just to stop
//                    // https://stackoverflow.com/questions/7856246/stopping-dispatchertimer-in-its-own-anonymous-tick-event-handler
//                    ourviewmodel.timerClock.Stop();
//                    // Animate is set to false to avoid crashes (this stuff is absolute bollocks)
//                    // This works because we're not updating the main thread
//                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, SignIn.signinviewmodel.signinToken, 0, 0, "Clock stopped"))
//                    {
//                        return;
//                    }
//                    // This last message isn't forced to scroll otherwise we would be trying to
//                    // update the UI from another thread.  Not good - and it gives an error ...
//#if WINFORMS || WPF || UWP || WINUI || MAUI
//                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Clock stopped");
//#endif
//                    await SmartRoutinesV2018.ClosingDown(SignIn.signinviewmodel,
//                                                                ourviewmodel);
//                    //
//                    // Panel hide sequence
//                    //
//                    if (FrontEndGUI.GetBorderVisible(ourviewmodel))
//                    {
//                        // Do two invisibles at once
//                        FrontEndGUI.SetBorderScrollInvisible(ourviewmodel);
//                    }
//#if WINFORMS
//                    ourviewmodel.timerClock.Dispose();

//#endif
//                    // Now we have QUIT SmartSwitch, we have to put SignIn onto
//                    // the Navigation stack and popoff MainMeter.Took me DAYS to get this working
//                    // No ... there is a quicker way
//                    // Windows, Android, IOS - it has to be done on the main thread (go figure)
//#if WPF
//                if (SignIn.signinviewmodel.Platform == SmartParametersV2016.WPF)
//                {
//                    // Cannot use DependencyService in WPF ... or at least I
//                    // can't figure out how the fucking thing works
//                    // Declare an interface instance.
//                    IScreen screenObject = new ScreenService();

//                    // Call the member.
//                    screenObject.SwitchScreen(true, this); // True means we ARE quitting
//                }
//                // Close this MainMeter screen - can't use Dispose Event Handler here because this bollocks
//                // Dispatcher Timer doesn't have one!  And if I use System.Timer.Timers .... the whole
//                // fucking thing falls over!  What a pile of shit Windows is
//                // Only do this if SHUTDOWN is true
//                if (SmartParametersV2016.CrashExit)
//                {
//                    // If ExitToLogin is true, then we try to return to the Sign-In page after logging out
//                    SmartRoutinesV2018.ShutDown(true);
//                }
//                // That's All Folks!! - don't forget the Meter should close after this routine
//                // Should call event handler in SignIn to re-show pop-up
//                this.Close();
//#endif
//#if UWP
//                if (SignIn.signinviewmodel.Platform == SmartParametersV2016.UWP)
//                {
//                    // Cannot use DependencyService in WPF ... or at least I
//                    // can't figure out how the fucking thing works
//                    // Declare an interface instance.
//                    IScreen screenObject = new ScreenService();

//                    // Call the member.
//                    screenObject.SwitchScreen(true, this); // True means we ARE quitting
//                }
//#endif
//#if WINUI || MAUI
//                if (SignIn.signinviewmodel.Platform == SmartParametersV2016.WINUI)
//                {
//                    // Cannot use DependencyService in WPF ... or at least I
//                    // can't figure out how the fucking thing works
//                    // Declare an interface instance.
//                    IScreen screenObject = new ScreenService();

//                    // Call the member.
//                    screenObject.SwitchScreen(true, this); // True means we ARE quitting
//                }
//#endif
//#if WINUI || MAUI
//                // Close this MainMeter screen - can't use Dispose Event Handler here because this bollocks
//                // Dispatcher Timer doesn't have one!  And if I use System.Timer.Timers .... the whole
//                // fucking thing falls over!  What a pile of shit Windows is
//                // Only do this if SHUTDOWN is true
//                if (SmartParametersV2016.CrashExit)
//                {
//                    // If ExitToLogin is true, then we try to return to the Sign-In page after logging out
//                    SmartRoutinesV2018.ShutDown(true);
//                }
//                // That's All Folks!! - don't forget the Meter should close after this routine
//                //this.Frame.GoBack();
//                this.Content = null;        // Its the only thing
//                await Task.Delay(300);      // that seems to work!
//                App.ResizeAndCenter(App.SignInSize);
//                Frame rootFrame = (Frame)App.MainWindow.Content;
//                rootFrame.Navigate(typeof(SignIn), SmartParametersV2016.WINUI);

//#endif
//#if ANDROIDX
//                    // Close this MainMeter screen - can't use Dispose Event Handler here because this bollocks
//                    // Dispatcher Timer doesn't have one!  And if I use System.Timer.Timers .... the whole
//                    // fucking thing falls over!  What a pile of shit Windows is
//                    // Only do this if SHUTDOWN is true
//                    if (SmartParametersV2016.CrashExit)
//                    {
//                        // If ExitToLogin is true, then we try to return to the Sign-In page after logging out
//                        SmartRoutinesV2018.ShutDown(true);
//                    }
//                    // That's All Folks!! - don't forget the Meter should close after this routine
//                    ourviewmodel.timerClock.Dispose();

//#endif
//                    return;
//                }
//                else
//                {
//                    // Time to cancel a Finance scrape?
//                    // Might have to do these for Utility as well ...=:-]
//                    if (financeviewmodel != null)
//                    {
//                        List<string> keysToRemove = new List<string>();
//                        foreach (var (key, timer) in financeviewmodel.timers)
//                        {
//                            timer.RemainingSeconds--;
//                            timer.OnTick.Invoke(key);
//                            if (timer.RemainingSeconds <= 0)
//                            {
//                                timer.OnComplete.Invoke(key, MainViewModel.TimerFinishReason.Completed);
//                                keysToRemove.Add(key);
//                            }
//                        }
//                        if (keysToRemove.Count > 0)
//                        {
//                            foreach (string key in keysToRemove)
//                            {
//                                financeviewmodel.timers.Remove(key);
//                            }
//                        }
//                    }

//                    // Increase the keep alive count - if it hits 60 or more (possibly) then reset it
//                    ourviewmodel.keepaliveCount++;
//                    // And the Crypto Rates
//                    ourviewmodel.cryptoRefreshCount++;
//                    // We ALWAYS send a keep alive whether we are GREEN or RED
//                    // Well ... that's good! It means we can receive the latest
//                    // exchange rates  =:-]   !

//                    // Have introduced a 'connect lock' because we are in danger of calling
//                    // the connect routine ONCE WE ARE ALREADY IN IT, and if that takes
//                    // TOO LONG to execute (!!!) then we could end up inside it repeatedly.
//                    // Not what we want ... 
//                    // So - now - here we go IF its time for a 'Connect' then make sure
//                    // a) we have finished Phase1 (i.e. its false) and
//                    // b) we haven't started Phase2 (i.e its false so we're not doing a Submit)

//                    // If Phase1 is OFF and Led6 is ORANGE and Led8 is ORANGE (i.e. not RED) we can refresh
//                    bool refresh = !ourviewmodel.Phase1 &&
//                                    FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.orangeColour) &&
//                                    FrontEndGUI.CompareLedColour(ourviewmodel, 8, ourviewmodel.orangeColour);

//                    // Crypto refresh
////                    if ((ourviewmodel.cryptoRefreshCount >= SmartParametersV2016.cryptoRatesLimit) &&
////                        refresh)
////                    {
////                        // We ALWAYS send a keep alive whether our UserName is GREEN or RED
////                        ourviewmodel.cryptoRefreshCount = 0;
////                        if (!SmartNibbyV2016.NetworkAvailability())
////                        {
////                            FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.redColour);
////                            // Try to carry on
////                            return; // Pointless carrying on .. wait one minute
////                        }
////                        // Phase3 true means Connect not in progress
////                        if (ourviewmodel.Phase3)
////                        {
////                            ourviewmodel.Phase3 = false; // Block ourselves
////                            FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour);
////                        }
////                        // Get the exchange rates
////                        if (await CryptoBollocksV2025.ExtractExchangeRates(ourviewmodel, financeviewmodel))

////                        {
////#if CRYPTO
////                            Console.WriteLine("Crypto Exchange Rates ");
////#endif
////#if WPF
////                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Crypto Exchange Rates refreshed");
////#endif
////                            SmartDanV2025.RebuildCurrencies(financeviewmodel);

////                        }

////                        ourviewmodel.Phase3 = true; // Unblock ourselves

////                    }
////                    // Main Connect refresh
////                    if ((ourviewmodel.keepaliveCount >= SmartParametersV2016.keepaliveLimit) &&
////                        refresh)
////                    {
////                        // We ALWAYS send a keep alive whether our UserName is GREEN or RED
////                        ourviewmodel.keepaliveCount = 0;
////                        if (!SmartNibbyV2016.NetworkAvailability())
////                        {
////                            FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.redColour);
////                            // Try to carry on
////                            return; // Pointless carrying on .. wait one minute
////                        }
////                        // Phase3 true means Connect not in progress
////                        if (ourviewmodel.Phase3)
////                        {
////                            ourviewmodel.Phase3 = false; // Block ourselves
////                            FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour);

////                            string lastDate = "";
////                            if (ourviewmodel.Blanche.exchangeRatesList.Count > 0)
////                            {
////                                lastDate = ourviewmodel.Blanche.exchangeRatesList.Last().TRANSACTION_DATE.ToString(SmartParametersV2016.sqliteformat);
////#if ANDROIDX
////                                SmartUsers.ExchangeRates exchangerate_row = ourviewmodel.Blanche.exchangeRatesList.Last();
////#endif
////                            }

////                            ourviewmodel.utcDates = "";
////                            if (!await SmartPhyllV2020.GetDates(ourviewmodel))
////                            {
////                                FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
////                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "GetDates failed", true);
////                                // This works because we're not updating the main thread
////                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 25: GetDates failed"))
////                                {
////                                    return;
////                                }
////                                return;
////                            }

////                            // How about a test if it was red to see if this is a rest
////                            if (FrontEndGUI.CompareLedColour(ourviewmodel, 2, ourviewmodel.redColour))
////                            {
////                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Re-connecting", true);
////                            }
////                            else
////                            {
////                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Connecting", true);
////                            }

////                            // We need to know if ANNA or MARIA has changed (when they've been added)
////                            // We will do this by checking the MARKER, but first we need to store
////                            // the current MARKER(s)
////                            List<SmartProfile.Groups> mmCopy = new List<SmartProfile.Groups>();
////                            foreach (SmartProfile.Groups groupRow in ourviewmodel.mmListX) // Checked
////                            {
////                                SmartProfile.Groups mmRow = new SmartProfile.Groups()
////                                {
////                                    USERNAME = groupRow.USERNAME,
////                                    GROUPNAME = groupRow.GROUPNAME,
////                                    ACTIVEFLAG = groupRow.ACTIVEFLAG,
////                                    MARKER = groupRow.MARKER,
////                                    PDEK = groupRow.PDEK,
////                                    SENDF = groupRow.SENDF,
////                                    FDEK = groupRow.FDEK,
////                                    SENDU = groupRow.SENDU,
////                                    UDEK = groupRow.UDEK,
////                                    RECEIVEALL = groupRow.RECEIVEALL,
////                                    DISPLAYNAME = groupRow.DISPLAYNAME
////                                };
////                                mmCopy.Add(mmRow);
////                            }
////                            // Now we have a good copy ...
////                            // In this routine we set Phase3 to false
////                            if (!await SmartBobV2017.ConnectDisconnectAsync(ourviewmodel,
////                                                                                ourviewmodel.userToken,
////                                                                                SmartParametersV2016.connectSymbol,
////                                                                                ourviewmodel.multiuser,
////                                                                                ourviewmodel.utcDates,
////                                                                                lastDate))
////                            {
////                                if (!ourviewmodel.quitCts.IsCancellationRequested)
////                                {
////                                    // Set the second Led to Red
////                                    FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.redColour);
////                                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 26: Connecting"))
////                                    {
////                                        return;
////                                    }
////                                    return;
////                                }
////                            }
////                            else
////                            {
////                                // Set the second Led to Green
////                                FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.greenColour);
////                                if (ourviewmodel.mmListX.Count > 0)
////                                {
////                                    bool primary = false;
////                                    foreach (SmartProfile.Groups mmRow in ourviewmodel.mmListX) // Checked
////                                    {
////                                        // This test means we ignore our own record RAY = RAY
////                                        if (mmRow.GROUPNAME != ourviewmodel.UserName)
////                                        {
////                                            // Does it exist in SmartUsers.Consumers
////                                            bool exists = SmartSpikeV2017.CheckConsumerUser(ourviewmodel,
////                                                                                            mmRow.GROUPNAME);

////                                            if (!exists)
////                                            {
////                                                // We didn't find it in our target list
////                                                if (mmRow.PDEK != "")   // We must be able to DECODE!!! You twat!
////                                                {
////                                                    // It doesn't exist so we need to MB:   SmartUsers
////                                                    //                                    + SmartFinance (if SENDF)
////                                                    //                                    + SmrtUtility  (if SENDU)
////                                                    string ourSchemas = SmartRoutinesV2018.SchemasFromCubeface(SignIn.signinviewmodel,
////                                                                                                ourviewmodel);

////                                                    string mmSchemas = "";
////                                                    mmSchemas = SmartRoutinesV2018.MMFromSchemas(mmSchemas, mmRow);

////                                                    if (!await SmartNibbyV2016.MiserableBitch(SignIn.signinviewmodel,
////                                                                                ourviewmodel,
////                                                                                ourviewmodel.UserName,
////                                                                                mmRow.GROUPNAME,
////                                                                                mmSchemas))
////                                                    {
////                                                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
////                                                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Miserable Bitch failed 1", true);
////                                                        // This works because we're not updating the main thread
////                                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 27: Miserable Bitch failed 1"))
////                                                        {
////                                                            return;
////                                                        }
////                                                        return;
////                                                    }

////                                                    // Pretty sure this next section is USELESS
////                                                    // Because ExtractSmartUsers on extracts CONSUMERS
////                                                    // table and we're not interested in that for
////                                                    // non-primarys e.g. ANNA or MRS_HAPPY


////                                                    //// Now we have SmartProfile and SmartFinance and/or SmartUtility loaded
////                                                    //// we need to extract them.  So do SmartUsers first:
////                                                    //if (!await SmartPhyllV2020.ExtractSmartUsersX(ourviewmodel,
////                                                    //                            ourviewmodel.sqlitetablesList,
////                                                    //                            //ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
////                                                    //                            primary,
////                                                    //                            SmartParametersV2016.SmartUsersSchema,
////                                                    //                            mmRow.GROUPNAME,
////                                                    //                            mmRow.PDEK))
////                                                    //{
////                                                    //    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
////                                                    //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SQLite failed", true);
////                                                    //    // This works because we're not updating the main thread
////                                                    //    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 28: SQLite failed"))
////                                                    //    {
////                                                    //        return;
////                                                    //    }
////                                                    //    return;
////                                                    //}

////                                                    // Now we have SmartProfile and SmartFinance and/or SmartUtility loaded
////                                                    // we need to extract them.  So do SmartProfile first:
////                                                    // All we are interested in here is the AddressesViews btw
////                                                    if (!await SmartPhyllV2020.ExtractSmartProfile(ourviewmodel,
////                                                                                ourviewmodel.sqlitetablesList,
////                                                                                ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
////                                                                                primary,
////                                                                                SmartParametersV2016.SmartProfileSchema,
////                                                                                mmRow.GROUPNAME,
////                                                                                mmRow.PDEK))
////                                                    {
////                                                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
////                                                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SQLite failed", true);
////                                                        // This works because we're not updating the main thread
////                                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 28: SQLite failed"))
////                                                        {
////                                                            return;
////                                                        }
////                                                        return;
////                                                    }

////                                                    // Now we have to do SmartFinance and/or SmartUtility
////                                                    // Check Finance
////                                                    CheckFinanceAdd(
////#if WINFORMS
////                                                                processComponents,
////#endif
////                                                                    ourviewmodel,
////                                                                    financeviewmodel,
////                                                                    mmRow,
////                                                                    primary);
////                                                    // Check Utility
////                                                    CheckUtilityAdd(
////#if WINFORMS
////                                                                processComponents,
////#endif
////                                                                    ourviewmodel,
////                                                                    utilityviewmodel,
////                                                                    mmRow,
////                                                                    primary);
////                                                }
////                                            }
////                                            else
////                                            {
////                                                // It does exist
////                                                // Check to see if we need to remove it
////                                                // But only remove it if it was in before!!
////                                                foreach (SmartProfile.Groups groupRow in mmCopy) // Checked
////                                                {
////                                                    if (groupRow.GROUPNAME == mmRow.GROUPNAME)
////                                                    {
////                                                        // If it was True before, but isn't True now..
////                                                        if (groupRow.SENDF && !mmRow.SENDF)
////                                                        {
////                                                            CheckFinanceRemove(
////#if WINFORMS
////                                                                            processComponents,
////#endif
////                                                                                ourviewmodel,
////                                                                                financeviewmodel,
////                                                                                mmRow,
////                                                                                primary);
////                                                        }
////                                                        if (groupRow.SENDU && !mmRow.SENDU)
////                                                        {
////                                                            CheckUtilityRemove(
////#if WINFORMS
////                                                                                processComponents,
////#endif
////                                                                                    ourviewmodel,
////                                                                                    utilityviewmodel,
////                                                                                    mmRow,
////                                                                                    primary);
////                                                        }
////                                                        break;
////                                                    }
////                                                }
////                                                if (!mmRow.SENDF && !mmRow.SENDU)
////                                                {

////                                                    // Do we REALLY need to RemoveSmartUsers as
////                                                    // we never loaded ANNA or MRS_HAPPY in the first place??
////                                                    SmartPhyllV2020.RemoveSmartUsersX(ourviewmodel,
////                                                                                    ourviewmodel.sqlitetablesList,
////                                                                                    //ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
////                                                                                    SmartParametersV2016.SmartUsersSchema,
////                                                                                    mmRow.GROUPNAME);



////                                                    //int index = 0;
////                                                    //List<SmartUsers.SmartView> temp = new List<SmartUsers.SmartView>();
////                                                    //foreach (SmartUsers.SmartView abc in ourviewmodel.ssListX) // Checked
////                                                    //{
////                                                    //    if (abc.USERNAME != mmRow.GROUPNAME)
////                                                    //    {
////                                                    //        // Remove the one that matches!!!
////                                                    //        // By re-creating all the ones that don't ....
////                                                    //        temp.Add(abc);
////                                                    //        //ourviewmodel.ssList.RemoveAt(index);
////                                                    //        //break;
////                                                    //    }
////                                                    //    index++;
////                                                    //}
////                                                    //ourviewmodel.ssListX = temp;
////                                                }
////                                                else
////                                                {
////                                                    // If there's no need to Remove it
////                                                    // perhaps we should add it in?
////                                                    // Don't forget - SmartUsers is still in at the moment < = NO IT ISN'T
////                                                    // Check Finance against mmCopy: if the PDEK
////                                                    // has changed - add it in
////                                                    foreach (SmartProfile.Groups groupRow in mmCopy) // Checked
////                                                    {
////                                                        if (groupRow.GROUPNAME == mmRow.GROUPNAME)
////                                                        {
////                                                            if (groupRow.PDEK != mmRow.PDEK)
////                                                            {
////                                                                // MB it in. Build the Schemas
////                                                                string ourSchemas = SmartRoutinesV2018.SchemasFromCubeface(SignIn.signinviewmodel,
////                                                                                                ourviewmodel);

////                                                                //string mmSchemas = SmartRoutinesV2018.MMFromSchemas(mmRow);
////                                                                string mmSchemas = "";
////                                                                mmSchemas = SmartRoutinesV2018.MMFromSchemas(mmSchemas, mmRow);

////                                                                if (!await SmartNibbyV2016.MiserableBitch(SignIn.signinviewmodel,
////                                                                                ourviewmodel,
////                                                                                ourviewmodel.UserName,
////                                                                                mmRow.GROUPNAME,
////                                                                                mmSchemas))
////                                                                {
////                                                                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
////                                                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Miserable Bitch failed 2", true);
////                                                                    // This works because we're not updating the main thread
////                                                                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 29: Miserable Bitch failed 2"))
////                                                                    {
////                                                                        return;
////                                                                    }
////                                                                    return;
////                                                                }
////                                                                // Check Finance
////                                                                CheckFinanceAdd(
////#if WINFORMS
////                                                                            processComponents,
////#endif
////                                                                                ourviewmodel,
////                                                                                financeviewmodel,
////                                                                                mmRow,
////                                                                                primary);
////                                                                // Check Utility
////                                                                CheckUtilityAdd(
////#if WINFORMS
////                                                                            processComponents,
////#endif
////                                                                                ourviewmodel,
////                                                                                utilityviewmodel,
////                                                                                mmRow,
////                                                                                primary);
////                                                            }
////                                                            break;
////                                                        }
////                                                    }
////                                                }
////                                            }
////                                        }
////                                    }
////                                }
////                            }
////                        }
////                        ourviewmodel.Phase3 = true;
////                    }
//                    // Date and Time - happens every 1000 ticks or ONE SECOND - is converted to Local Time
//                    // But we display the LOCAL PC (e.g. Yukon) time
//                    if (SignIn.signinviewmodel.signincultureinfo == SmartParametersV2016.defaultCulture)
//                    {
//                        ourviewmodel.DateTimeNowMessage = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.militaryFormat);
//                    }
//                    else
//                    {
//                        ourviewmodel.DateTimeNowMessage = (DateTime.Now + ourviewmodel.utcOffset).ToString(SignIn.signinviewmodel.signincultureinfo);
//                    }
////#if ANDROIDX
////                    ourviewmodel.ADateTimeNow.Text = ourviewmodel.DateTimeNowMessage;
////#endif
////                    if (FrontEndGUI.CompareLedColour(ourviewmodel, 2, ourviewmodel.greenColour) &&
////                        FrontEndGUI.CompareLedColour(ourviewmodel, 3, ourviewmodel.greenColour) &&
////                        FrontEndGUI.CompareLedColour(ourviewmodel, 4, ourviewmodel.greenColour) &&
////                        FrontEndGUI.CompareLedColour(ourviewmodel, 5, ourviewmodel.greenColour))
////                    {
////                        // For reasons best known to the Chimps, parameter e is always 'null' for UWP || WINUI ...
////                        if (!await TimerClockTickCheck(
////#if WINFORMS
////                                                processComponents,
////#endif
////                                                    SignIn.signinviewmodel,
////                                                    ourviewmodel,
////                                                    financeviewmodel,
////                                                    utilityviewmodel))
////                        {
////                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 30: Check Meter failed"))
////                            {
////                                return;
////                            }
////                        }
////                    }
//                }
            
            return;
        }

        // This routine cannot be set up for WPF ...there is no Dispose event
        // for Dispatcher Timer ...  (chimps rule)
//#if WINFORMS
//        internal static void TimerClockDisposed(object sender, object e,
//                                                    MainViewModel ourviewmodel)
//        {
//#endif
//#if UWP || WINUI || MAUI
//        internal void TimerClockDisposed(object sender, object e)
//        {
//#endif
//#if ANDROIDX
//        internal void TimerClockDisposed(object sender, object e)
//        {
//#endif
//#if UWP || WINUI || MAUI
//#if PRODUCTION
//            if (ourviewmodel.appView.IsFullScreenMode)
//            {
//                ourviewmodel.appView.ExitFullScreenMode();
//                Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.Auto;
//                // The SizeChanged event will be raised when the exit from full-screen mode is complete.
//            }
//#endif
//#endif
//#if ANDROIDX
//#if PRODUCTION
//            if (ourviewmodel.appView.IsFullScreenMode)
//            {
//                ourviewmodel.appView.ExitFullScreenMode();
//                Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.Auto;
//                // The SizeChanged event will be raised when the exit from full-screen mode is complete.
//            }
//#endif
//#endif
//#if WINFORMS
//            // We CAN use the binding here, because the Meter panel
//            // control has now been set up
//            ourviewmodel.MeterVisible = false;
//            ourviewmodel.PanelVisibility = false;
//            return;
//#endif
//#if UWP
//            // Now we have QUIT SmartSwitch, we have to put SignIn ONto
//            // the Navigation stack and popoff MainMeter. Took me DAYS to get this working
//            // No ... there is a quicker way
//            // Windows, Android, IOS - it has to be done on the main thread (go figure)
//            if (SignIn.signinviewmodel.Platform == SmartParametersV2016.UWP)
//            {
//                //DependencyService.Get<IScreen>().SwitchScreen(true);    // We are Quitting
//            }
//            return;
//#endif
//#if ANDROIDX
//            // Now we have QUIT SmartSwitch, we have to put SignIn ONto
//            // the Navigation stack and popoff MainMeter. Took me DAYS to get this working
//            // No ... there is a quicker way
//            // Windows, Android, IOS - it has to be done on the main thread (go figure)
//            return;
//#endif

//#if WINFORMS || UWP || WINUI || MAUI
//        }
//#endif
//#if ANDROIDX
//        }
//#endif

        // From Dan - 5th Jan 2012 - this routine loops and won't let me close!!
        // from Dad - 3rd Feb 2012 - But I have fixed it now
        // New version Dan - all because of this Silverligh shit ... sorry
        // But the effect is as NEAT as shit!  This program IS the Dog's Bollocks!!!

#if WINFORMS
        // For this to work !!!!  The Meter Top is DOCKed at the TOP
        // and the Meter Bottom is DOCKed at the BOTTOM
        internal static void TimerDoorTick(object sender, EventArgs e, MainViewModel ourviewmodel)
        {
            if (ourviewmodel.topIncrement > 0 &&
                    ourviewmodel.bottomIncrement > 0)
            {
                if (ourviewmodel.OpenCloseColour == ourviewmodel.redColour)
                {
                    // We are going to SHRINK the fucking doors
                    if ((ourviewmodel.MeterTopHeight - ourviewmodel.topIncrement <= 0) ||
                        (ourviewmodel.MeterBottomHeight - ourviewmodel.bottomIncrement <= 0))
                    {
                        ourviewmodel.MeterTopHeight = ourviewmodel.MeterBottomHeight = 0;
                        ourviewmodel.timerDoor.Stop();
                    }
                    else
                    {
                        ourviewmodel.MeterTopHeight = ourviewmodel.MeterTopHeight - ourviewmodel.topIncrement;
                        ourviewmodel.MeterBottomHeight = ourviewmodel.MeterBottomHeight - ourviewmodel.bottomIncrement;
                    }
                }
                else
                {
                    // We are going to EXPAND the fucking doors
                    if ((ourviewmodel.MeterTopHeight + ourviewmodel.topIncrement >= ourviewmodel.topActualheight) ||
                        (ourviewmodel.MeterBottomHeight + ourviewmodel.bottomIncrement >= ourviewmodel.bottomActualheight))
                    {
                        ourviewmodel.MeterTopHeight = ourviewmodel.topActualheight;
                        ourviewmodel.MeterBottomHeight = ourviewmodel.bottomActualheight;
                        ourviewmodel.timerDoor.Stop();
                    }
                    else
                    {
                        ourviewmodel.MeterTopHeight = ourviewmodel.MeterTopHeight + ourviewmodel.topIncrement;
                        ourviewmodel.MeterBottomHeight = ourviewmodel.MeterBottomHeight + ourviewmodel.bottomIncrement;
                    }
                }
            }
            return;
        }
#endif

        internal static async Task<bool> TimerClockTickCheck(
#if WINFORMS
                                                    MainProcess components,
#endif
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            // Here, we want to check all the cubefaces and see which
            // ones are due for a scrape OR have had the Submit button pressed

            // Nothing happens unless the Username is Green
            if (ourviewmodel.UserNameColour == ourviewmodel.greenColour)
            {
                foreach (SmartProfile.Cubefaces cubefaceRow in ourviewmodel.SmartProfile.profilecubefacesList) // Checked
                {
                    // Are we the logged-in User?
                    if (cubefaceRow.USERNAME == ourviewmodel.UserName)
                    {
                        switch (cubefaceRow.CUBEFACE_CODE) // My cross to bear for using SQLite ...
                        {
                            case SmartParametersV2016.Finance:
                                if (cubefaceRow.FACE_ACTIVE)
                                {
                                    if (cubefaceRow.NEXT_CONNECTION < DateTime.Now + ourviewmodel.utcOffset)  // Local time
                                    {
                                        // Call 'Submit'??
                                        if (FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.whiteColour))
                                        {
                                            // This acts as a 'block'
                                            FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.greenColour);
                                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Finance Auto-Submit started"))
                                            {
                                                return false;
                                            }

                                            if (!await SmartFinanceV2025.FinanceButtonSubmitClickActual(
#if WINFORMS
                                                                                                        components.textBoxConsole,
                                                                                                        components,
#endif
                                                                                                        ourviewmodel,
                                                                                                        financeviewmodel))
                                            {
                                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                                                        ourviewmodel.userToken,
                                                                                        0, 0, "Finance Auto-Submit failed"))
                                                {
                                                    return false;
                                                }
                                            }
                                            // WHATEVER the result!!  Update NEXT|CONNECTION
                                            // otherwise we will get a 'race' condition
                                            cubefaceRow.NEXT_CONNECTION = (DateTime.Now + ourviewmodel.utcOffset).AddDays(1);  // Local time
                                            cubefaceRow.Updated = true;
                                            ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubefaceRow);

                                            // Record the switch
                                            if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                                            SmartParametersV2016.sqliteformat))
                                            {
                                                FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 31: Finance Next Connection"))
                                                {
                                                    return false;
                                                }
                                                return false;
                                            }
                                            // Succeed or Fail do nothing until this time (unless Submit button pressed)
#if WINFORMS || WPF || UWP || WINUI || MAUI
                                            financeviewmodel.NextConnectionMessage = cubefaceRow.NEXT_CONNECTION.ToString(financeviewmodel.CultureINF);
#endif
#if ANDROIDX
                                            financeviewmodel.NextConnectionMessage.Text = cubefaceRow.NEXT_CONNECTION.ToString(financeviewmodel.CultureINF);
#endif
                                            // This acts as an 'unblock'
                                            if (FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.orangeColour))
                                            {
                                                // This acts as a 'block'
                                                FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.whiteColour);
                                            }
                                        }
                                    }
                                }
                                break;
                            case SmartParametersV2016.Utility:
                                if (cubefaceRow.FACE_ACTIVE)
                                {
                                    if (cubefaceRow.NEXT_CONNECTION < DateTime.Now + ourviewmodel.utcOffset)   // Local time
                                    {
                                        // Call 'Submit'??
                                        if (FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.whiteColour))
                                        {
                                            // This acts as a 'block'
                                            FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.greenColour);

                                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Utility Auto-Submit started"))
                                            {
                                                return false;
                                            }

                                            if (!await SmartUtilityScrapeV2022.UtilityButtonSubmitClickActual(
#if WINFORMS
                                                                                                    components,
#endif
                                                                                                    ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    ourviewmodel.Blanche.vatRatesList,
                                                                                                    ourviewmodel.Blanche.exchangeRatesList,
                                                                                                    ourviewmodel.Blanche.postcodesList))
                                            {
                                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                                                        ourviewmodel.userToken,
                                                                                        0, 0, "Utility Auto-Submit failed"))
                                                {
                                                    return false;
                                                }
                                            }
                                            // WHATEVER the result!!  Update NEXT_CONNECTION
                                            // otherwise we will get a 'race' condition
                                            cubefaceRow.NEXT_CONNECTION = (DateTime.Now + ourviewmodel.utcOffset).AddDays(1);  // Local time
                                            cubefaceRow.Updated = true;
                                            ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubefaceRow);

                                            // Record the switch
                                            if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                                            SmartParametersV2016.sqliteformat))
                                            {
                                                FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 32: Utility Next Connection"))
                                                {
                                                    return false;
                                                }
                                                return false;
                                            }
                                            // Success or Fail do nothing until this time (unless Submit button pressed)
#if WINFORMS || WPF || UWP || WINUI || MAUI
                                            utilityviewmodel.NextConnectionMessage = cubefaceRow.NEXT_CONNECTION.ToString(utilityviewmodel.CultureINF);
#endif
#if ANDROIDX
                                            utilityviewmodel.NextConnectionMessage.Text = cubefaceRow.NEXT_CONNECTION.ToString(utilityviewmodel.CultureINF);
#endif
                                            // This acts as an 'unblock'
                                            if (FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.greenColour))
                                            {
                                                // This acts as a 'block
                                                FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.whiteColour);
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return true;
        }

        

#if WINFORMS
        internal void DefaultContextMenuRightMouseButtonDown(object sender, object e) //MouseButtonEventArgs e)
#endif
#if WPF || UWP || WINUI || MAUI
        internal void DefaultContextMenuRightMouseButtonDown(object sender, object e) //MouseButtonEventArgs e)
#endif
#if ANDROIDX
        internal void DefaultContextMenuRightMouseButtonDown(object sender, object e) //MouseButtonEventArgs e)
#endif
        {
            // Handle the event so the default context menu is hidden
            if (sender != null &&
#if WINFORMS
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF || UWP || WINUI || MAUI
                FrontEndGUI.DecodeRoutedEventFlags(e) != null)
#endif
#if ANDROIDX
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
            {
#if WPF
                // No UWP or WINUI
                FrontEndGUI.DecodeRoutedEventFlags(e).Handled = true;
#endif
#if WPF
                //e.Handled = true; ??
#endif
            }
            return;
        }

#if WPF
        public async void MainMeterLoaded(object sender, EventArgs e, MainViewModel ourviewmodel)
        {
            // https://stackoverflow.com/questions/1927540/how-to-get-the-size-of-the-current-screen-in-wpf
            // Thanks to Mikesl!!
            double ScreenHeight = 2 * (Top + 0.5 * Height);
            ourviewmodel.actualHeight = ScreenHeight;
            double ScreenWidth = 2 * (Left + 0.5 * Width);
            ourviewmodel.actualWidth = ScreenWidth;
            // For any Toast popups
            ourviewmodel.currentWidth = this.ActualWidth;
            ourviewmodel.currentHeight = this.ActualHeight;
            return;
        }

        public async void MainMeterClosed(object sender, EventArgs e, MainViewModel ourviewmodel)
        {
            // This is always called, believe it or not!!
            if (FrontEndGUI.CompareLedColour(ourviewmodel, 2, ourviewmodel.greenColour))
            {
                //
                // Clock stop sequence
                //
                _scheduler.Stop();

                //This last message isn't forced to scroll otherwise we would be trying to
                // update the UI from another thread.  Not good - and it gives an error ...
#if WINFORMS || WPF || UWP || WINUI || MAUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Clock stopped");
#endif
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Clock stopped"))
                {
                    return;
                }
                //await SmartRoutinesV2018.ClosingDown(SignIn.signinviewmodel, ourviewmodel);

                // Cancel ourviewmodel.quitCts?? and quitToken??
                if (FrontEndGUI.GetBorderVisible(ourviewmodel))
                {
                    // Do two invisibles at once
                    FrontEndGUI.SetBorderScrollInvisible(ourviewmodel);
                }
                SmartRoutinesV2018.ShutDown(true);
            }
            return;
        }

        public void Shrunk(object sender, EventArgs e)
        {
            if (sender != null)     // EventArgs e is always 'null' !!!!
            {
                ourviewmodel.doorDirection = true;
            }
            return;
        }

        public void Expanded(object sender, EventArgs e)
        {
            if (sender != null)     // EventArgs e is always 'null' !!!!
            {
                ourviewmodel.doorDirection = false;
            }
            return;
        }
#endif

        
        // When you choose a Username Label
        internal void UserNameMouseDoubleClick(object sender, object e)
        {
            if (sender != null)// &&
                               //#if WPF
                               //                FrontEndGUI.DecodeMouseButtonEventFlags(e) != null)
                               //            {
                               //                if (FrontEndGUI.DecodeMouseButtonEventFlags(e).ClickCount == 1)
                               //#endif
                               //#if UWP || WINUI
                               //                FrontEndGUI.DecodeEventFlags(e) != null)
                               //            {
                               //                if (FrontEndGUI.DecodeEventFlags(e).ClickCount == 1)
                               //#endif

            {
                string labelContent = FrontEndGUI.DecodeLabel(sender);
                if (labelContent != null)
                {
                    //SmartRoutinesV2018.FinanceTransactionsTabMouseDoubleClickActual(tablabel,
                    //                                            ourviewmodel,
                    //                                            financeviewmodel);

#if WINFORMS
                    QuitConfirmForm popup = new QuitConfirmForm();

                    DialogResult dialogresult = popup.ShowDialog();
                    // If I put a FormClosed event in for this Form, then
                    // it gets called when I click 'yes' or 'No and that
                    // is TOO EARLY!!  I want the Close code executed when
                    // I choose Yes and not before.
                    if (dialogresult == System.Windows.Forms.DialogResult.OK)
                    {
                        // Not sure wot to do here ... think QuitConformForm above is worng?
                        //MultiMeter.PopupYes(sender, e);                    
                    }
                    else if (dialogresult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        // Not sure wot to do here ... think QuitConformForm above is worng?

                        //MultiMeter.PopupNo(sender, e);
                    }
                    // Popup is already closed here
                    popup.Dispose();
#endif
#if WPF
                    //MultiMeter multimeterPage = new MultiMeter()
                    //{
                    //    // Took me ALL DAY to get this fucker working ...
                    //    PlacementTarget = this,
                    //    Placement = FrontEndGUI.SetPlacement(),
                    //    IsOpen = true
                    //};
#endif
                }
            }

#if WPF
            // Not sure what effect THIS has
            FrontEndGUI.DecodeMouseButtonEventFlags(e).Handled = true;
#endif
            return;
        }        

        //internal Storyboard FindResource(string v)
        //{
        //    throw new NotImplementedException();
        //}
    }

#if ANDROIDX
    //    public static void ShowNotification(string message)
    //    {
    //        // Gonna do this as a Toast
    //        Device.BeginInvokeOnMainThread(() =>
    //        {
    //            Label msg = new Label()
    //            {
    //                Text = $"SMSOTP Received: {message}"
    //            };
    //            MainPage.financeviewmodel.stackLayout.Children.Add(msg);
    //        });
    //    }
    //}
#endif
}