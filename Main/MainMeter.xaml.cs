//    This program works fine built against XAMASHIT  || WINUI || MEOWI Forms 4.8.0.1826
//    ... but the minute you build it against any of the 5.0 versions
//    it falls over into a fucking useless heap.  Chimps!

// SmartSwitchV2023 Latest and Greatest
// Dedicated to my favourite grand-daughter Amelia Maria Marsh (a.k.a 'Nibby' or 'Moo Bags') and
// my favourite grandson Luke Daniel (a.k.a. 'Wookey')
// (Amelia, poppet - you would never BELIEVE how much 'Ba!' I have cleared out of
// this program since April 2011, and how much I STILL have clear out ..)
//
// In memory of my dear old Mum, who despite all our falling outs, I loved dearly. And whom I miss
// every day.. and to whom I owe everything.  And in memory of my brother Bob to whom I loved all
// my life and to whom I would have donated my kidney if he had needed one and who has left a huge
// void in my life.  And whom I miss every day ... and I think of you every day and you always were
// - and always will be - my hero
// From now on Bob, every blue sky day is a 'Bob day' for me.  I will never forget you. Never.
//
// For reasons which I still don't clearly understand (but I'm not going
// to argue with) Unit Rates need to be passed by REF, but Accounts do not ...
// Go figure - this entire Microshit system is a pile of shit ... these people are ABSOLUTE AND UTTER CHIMPS
//

#if WINFORMS
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SmartDashboard;
#endif

#if WPF
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Web.WebView2.Wpf;
#endif

#if WINUI
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using WinRT.Interop;

#endif

#if ANDROIDX
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using AndroidX.AppCompat.App;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Devices;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using Windows.Graphics;
#endif
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
#if WINUI
    public sealed partial class MainMeter : Page
    {
        
#endif

#if ANDROIDX
    public partial class MainMeter : RelativeLayout
    {
#endif
#if SMARTMAUI
    public sealed partial class MainMeter : ContentPage
    {
        public Frame rootFrame = null;
        public static double currentWidth = 0,
                                    currentHeight = 0;
#endif

        // I know what you are going to say ...  
        // This time is updated by the TimerClock routine every second ...
        // However we pass this value into lots of routines and IT COULD BE that the routine uses a value which is
        // at most ONE SECOND late .  The alternative is to use timeNeow = DateTime Now everywhere ... but that could
        // lead to inconsistencies regarding timeNeow between routines.  We can live with the ONE SECOND potential delay ..
        // Cut me some slack here ...

        // The fucking doors!!
        //   (no more ...)

        private static List<object> vmlist;

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
                                                        TickScheduler scheduler)
#endif
#if WINUI
        // I have wasted too much of my life trying various different (and
        // ultimately unsuccessful) ways of trying to pass a parameter to this
        // WINUI shit. The fucking Chimps have really fucked me over on this one
        // because the OnNavigatedTo event is never fucking called UNTIL ...
        // MainMeter has finished.  And its MAINMETER that needs the fucking
        // signinviewmodel parameter!  What a fucking useless piece of fucking
        // excrement this Chimp-derived system is!! It's FUCKING HOPELESS
        // right from the word go...
        public MainMeter(SignInViewModel signinviewmodel,
                        TickScheduler scheduler)
#endif
#if WPF
        public MainMeter(SignInViewModel signinviewmodel,
                        TickScheduler scheduler)
#endif
#if SMARTMAUI
        public MainMeter(SignInViewModel signinviewmodel,
                        TickScheduler scheduler)
#endif
#if ANDROIDX
        public MainMeter(AppCompatActivity meterActivity,
                            Context meterContext,
                            AndroidX.Fragment.App.FragmentManager fm,
                            AndroidX.Lifecycle.Lifecycle lfc,
                            SignInViewModel signinviewmodel,
                            MainViewModel ourviewmodel,
                            TickScheduler scheduler) : base(meterContext)
#endif
        {
            // Without this next line NONE of the fucking bindings work <= Battling With The Chimps
#if WINFORMS || WPF || WINUI || SMARTMAUI
            // Android does this in MeterActivity
            MainViewModel ourviewmodel = new MainViewModel();

#if WINFORMS
            // This should be "Embedded Resource" aith "Copy always" properties
            //String imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "meter_background.jpg");
            //ourviewmodel.meterbg = new Bitmap.(new Uri(imagesPath, UriKind.Absolute));

            // File must exist in: bin\Debug\net9.0-windows\Images\meter_background.jpg
            string imagesPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images",
                "meter_background.jpg");
            ourviewmodel.meterbg = Image.FromFile(imagesPath);
#endif
#if WPF  || WINUI
            // This should be "Embedded Resource" aith "Copy always" properties
            String imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "meter_background.jpg");
            ourviewmodel.meterbg = new BitmapImage(new Uri(imagesPath, UriKind.Absolute));
#endif
#endif
#if WINFORMS
            ourviewmodel.website = signinviewmodel.website;
            components.MainBindingSource = new();
            components.MainBindingSource.DataSource = ourviewmodel;

            // 
            // ProfilesBindingSource
            // Done later
            //components.profilesCubefacesBindingSource = new();

            if (components.Led1.DataBindings.Count == 0)
            {               
                components.Led1.DataBindings.Add(new Binding("ForeColor", components.MainBindingSource, "Led1", true, DataSourceUpdateMode.OnPropertyChanged));
                components.Led2.DataBindings.Add(new Binding("ForeColor", components.MainBindingSource, "Led2", true, DataSourceUpdateMode.OnPropertyChanged));
                components.Led3.DataBindings.Add(new Binding("ForeColor", components.MainBindingSource, "Led3", true, DataSourceUpdateMode.OnPropertyChanged));
                components.Led4.DataBindings.Add(new Binding("ForeColor", components.MainBindingSource, "Led4", true, DataSourceUpdateMode.OnPropertyChanged));
                components.Led5.DataBindings.Add(new Binding("ForeColor", components.MainBindingSource, "Led5", true, DataSourceUpdateMode.OnPropertyChanged));
                components.Led6.DataBindings.Add(new Binding("ForeColor", components.MainBindingSource, "Led6", true, DataSourceUpdateMode.OnPropertyChanged));
                components.Led7.DataBindings.Add(new Binding("ForeColor", components.MainBindingSource, "Led7", true, DataSourceUpdateMode.OnPropertyChanged));
                components.Led8.DataBindings.Add(new Binding("ForeColor", components.MainBindingSource, "Led8", true, DataSourceUpdateMode.OnPropertyChanged));

                components.ScrollViewer.DataBindings.Add(new System.Windows.Forms.Binding("Visible", components.MainBindingSource, "ScrollViewerVisible", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                components.buttonQuit.Click += new EventHandler((s, e) => ButtonQuitClick(s, e, ourviewmodel));//, profileviewmodel, financeviewmodel, utilityviewmodel));

                components.buttonQuit.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", components.MainBindingSource, "QuitColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.buttonQuit.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", components.MainBindingSource, "QuitEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.OpenCloseButton.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", components.MainBindingSource, "OpenCloseColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.OpenCloseButton.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", components.MainBindingSource, "OpenCloseEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                components.labelUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "UserName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.labelUserName.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", components.MainBindingSource, "UserNameColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                //components.buttonAutoSwitchUtility.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", components.MainBindingSource, "AutoSwitchUtilityColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.labelDateTimeNow.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "DateTimeNowMessage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.WithdrawnDateMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "WithdrawnDateMessage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                components.LastLoginStatusMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "LastLoginStatusMessage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                components.AccountStatusMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", components.MainBindingSource, "AccountStatusMessage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                //components.buttonAutoSwitchFinance.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", components.MainBindingSource, "AutoSwitchFinanceColour", true));
            }

#endif
#if WPF  || WINUI || SMARTMAUI
#if SMARTMAUI
            string _dbg = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_debug.txt");
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: before InitializeComponent\n");
#endif
            InitializeComponent();
#if SMARTMAUI
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: after InitializeComponent\n");
#endif
            FrontEndGUI.SetWindowDataContext(ourviewmodel, this);
#if SMARTMAUI
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: after SetWindowDataContext\n");
#endif
            // Store it in the App in case of a system crash
#endif
            // Add it into our list of viewmodel objects
            vmlist = new List<object>();
            vmlist.Add(ourviewmodel);

            // So these are no longer hard-coded in the viewmodel
#if WINUI
            var _window = ((App)Application.Current).MainWindow;

            var hWnd = WindowNative.GetWindowHandle(_window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Make the window full screen
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
#endif

#if WPF  || WINUI || ANDROIDX || SMARTMAUI
            // WINUI
            // Until I find out how to pass signinviewmodel
            // a parameter ..
            ourviewmodel.website = signinviewmodel.website;
#endif
            ourviewmodel.Platform = signinviewmodel.Platform;
#if WPF  || WINUI
                
            ourviewmodel.ZoomTooltip = SignIn.BesetByChimps(signinviewmodel, "ZoomTooltip");
            ourviewmodel.AutoSwitchTooltip = SignIn.BesetByChimps(signinviewmodel, "AutoSwitchTooltip");
            ourviewmodel.OpenCloseTooltip = SignIn.BesetByChimps(signinviewmodel, "OpenCloseTooltip");
            ourviewmodel.QuitTooltip = SignIn.BesetByChimps(signinviewmodel, "QuitTooltip");
#endif
#if WINUI
            ourviewmodel.Dispatcher = this.DispatcherQueue;
#endif
#if SMARTMAUI
            ourviewmodel.Dispatcher = Microsoft.Maui.Controls.Application.Current.Dispatcher;
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: before Loaded event wire\n");
#endif
#if WPF || WINUI || SMARTMAUI
            this.Loaded += (s, e) => MainMeterLoaded(s, e, ourviewmodel);
#if WPF
            this.Closed += (s, e) => MainMeterClosed(s, e, ourviewmodel);
#endif
#if WINUI
            this.Unloaded += (s, e) => MainMeterClosed(s, e, ourviewmodel);
#endif

#if WPF
            this.ButtonLeft.Click += (s, e) => ButtonLeftClicked(s, e, signinviewmodel, ourviewmodel);
            this.ButtonRight.Click += (s, e) => ButtonRightClicked(s, e, signinviewmodel, ourviewmodel);
#endif
#if SMARTMAUI
            var tapLeft = new TapGestureRecognizer();
            tapLeft.Tapped += (s, e) =>
            {
                // Do something
                ButtonLeftClicked(s, e, signinviewmodel, ourviewmodel);
            };
            this.ButtonLeft.GestureRecognizers.Add(tapLeft);
            
            var tapRight = new TapGestureRecognizer();
            tapRight.Tapped += (s, e) =>
            {
                // Do something
                ButtonRightClicked(s, e, signinviewmodel, ourviewmodel);
            };
            this.ButtonRight.GestureRecognizers.Add(tapRight);
#endif

#endif
#if WPF || WINUI
            this.FullScreen.Click += (s, e) => FullScreenButtonClick(s, e, signinviewmodel, ourviewmodel);

            this.OpenCloseButton.Click += (s, e) => OpenCloseClicked(s, e, signinviewmodel, ourviewmodel);
            //this.FinanceAutoSwitch.Click +=  ButtonAutoSwitchFinanceClick;
            //this.UtilityAutoSwitch.Click +=  ButtonAutoSwitchUtilityClick;
#endif

#if SMARTMAUI
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: before button wiring\n");
            this.FullScreen.Clicked += (s, e) => FullScreenButtonClick(s, e, signinviewmodel, ourviewmodel);
            this.OpenCloseButton.Clicked += (s, e) => OpenCloseClicked(s, e, signinviewmodel, ourviewmodel);
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: button wiring done\n");

#endif
#if ANDROIDX
#pragma warning disable CA1416
            ourviewmodel.Fm = fm;
            ourviewmodel.Lfc = lfc;
            if (meterActivity != null)
            {
                meterActivity.RunOnUiThread(() =>
                {
                    ourviewmodel.ScrollViewer = meterActivity.FindViewById<ScrollView>(Resource.Id.ScrollViewer);
                    ourviewmodel.ADateTimeNow =
                    meterActivity.FindViewById<TextView>(Resource.Id.DateTimeNow);
                    ourviewmodel.TextBlockContent = meterActivity.FindViewById<TextView>(Resource.Id.TextBlockBrowser);
                    ourviewmodel.AutoSwitch = meterActivity.FindViewById<Button>(Resource.Id.AutoSwitch);
                    ourviewmodel.OpenClose = meterActivity.FindViewById<Button>(Resource.Id.OpenClose);
                    ourviewmodel.AUsername = meterActivity.FindViewById<TextView>(Resource.Id.Username);
                    ourviewmodel.MeterTop = meterActivity.FindViewById<ImageView>(Resource.Id.MeterTop);
                    ourviewmodel.MeterBottom = meterActivity.FindViewById<ImageView>(Resource.Id.MeterBottom);
                    ourviewmodel.MyContent = meterActivity.FindViewById<FrameLayout>(Resource.Id.MyContent);
                    // Leds
                    ourviewmodel.ALed1 = meterActivity.FindViewById<TextView>(Resource.Id.Led1);
                    ourviewmodel.ALed2 = meterActivity.FindViewById<TextView>(Resource.Id.Led2);
                    ourviewmodel.ALed3 = meterActivity.FindViewById<TextView>(Resource.Id.Led3);
                    ourviewmodel.ALed4 = meterActivity.FindViewById<TextView>(Resource.Id.Led4);
                    ourviewmodel.ALed5 = meterActivity.FindViewById<TextView>(Resource.Id.Led5);
                    ourviewmodel.ALed6 = meterActivity.FindViewById<TextView>(Resource.Id.Led6);
                    ourviewmodel.ALed7 = meterActivity.FindViewById<TextView>(Resource.Id.Led7);
                    ourviewmodel.ALed8 = meterActivity.FindViewById<TextView>(Resource.Id.Led8);
                    ourviewmodel.AQuit = meterActivity.FindViewById<Button>(Resource.Id.Quit);
                });
            }
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                ourviewmodel.AutoSwitch.TooltipText = SignIn.BesetByChimps(signinviewmodel, "AutoSwitchTooltip");
                ourviewmodel.OpenClose.TooltipText = SignIn.BesetByChimps(signinviewmodel, "OpenCloseTooltip");
                ourviewmodel.AQuit.TooltipText = SignIn.BesetByChimps(signinviewmodel, "QuitTooltip");
            }
#pragma warning restore CA1416
#endif
#if WPF  || WINUI || SMARTMAUI
            ourviewmodel.languageTag = signinviewmodel.languageTag;
#endif
#if SMARTMAUI
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: languageTag set, about to copy lists from Fatah (Fatah is {(signinviewmodel.Fatah == null ? "NULL" : "OK")})\n");
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
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
#if SMARTMAUI
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: lists copied OK\n");
#endif
#if ANDROIDX
            // Copying these here saves us passing SignInViewModel as a parameter
            ourviewmodel.CUBEFACESList = signinviewmodel.Fatah.cubefacesList;
            ourviewmodel.cultureviewList = signinviewmodel.Fatah.cultureviewList;
            ourviewmodel.currenciesList = signinviewmodel.Fatah.currenciesList;
            ourviewmodel.sqliteschemasList = signinviewmodel.Fatah.sqliteschemasList;
            ourviewmodel.sqlitetablesList = signinviewmodel.Fatah.sqlitetablesList;
            ourviewmodel.sqlitefieldsList = signinviewmodel.Fatah.sqlitefieldsList;
#endif
#if WINFORMS
            // Can't use the Meter panel binding here because the
            // Meter control hasn't been set up .. so we have to use 
            // the next line (you couldn't make this bollocks up ...)
            components.Meter.Visible = true;
            components.pictureBoxGUIMeterTop.Visible = true;
            components.pictureBoxGUIMeterBottom.Visible = true;
#endif
            ourviewmodel.Platform = signinviewmodel.Platform;
#if WINUI || SMARTMAUI
            if (signinviewmodel.Platform == SmartParametersV2016.WinUI)
#endif
#if ANDROIDX
            if (signinviewmodel.Platform == SmartParametersV2016.Android)
#endif
#if WINFORMS || WPF
            if ((signinviewmodel.Platform == SmartParametersV2016.WinUI ||
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

#if SMARTMAUI
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: about to call CreateHandlers\n");
#endif
            if (!SmartRoutinesV2018.CreateHandlers(signinviewmodel,
                                                ourviewmodel))
            {
#if WINFORMS
                return false;
#endif
#if WPF  || WINUI || SMARTMAUI

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
            // #if WPF  || WINUI
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
                                vmlist,
                                scheduler);
#endif
#if SMARTMAUI
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: CreateHandlers passed, about to call Bootstrap\n");
#endif
#if WPF  || WINUI || SMARTMAUI
                Bootstrap(signinviewmodel,
                            ourviewmodel,
                            vmlist,
                            scheduler);
#endif
#if ANDROIDX
                Bootstrap(meterActivity,
                            meterContext,
                            signinviewmodel,
                            ourviewmodel,
                            vmlist,
                            scheduler);
#endif
            }
            catch (Exception ex)
            {

#if SMARTMAUI
                System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: Bootstrap EXCEPTION: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                signinviewmodel.errorMessage = "Bootstrap failed: " + ex.Message;
#endif
#if ANDROIDX
                signinviewmodel.errorMessage = "Bootstrap failed: " + ex.Message;
#endif
#if WINFORMS
                return false;
#endif
#if WPF  || WINUI || SMARTMAUI
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
#if SMARTMAUI
            System.IO.File.AppendAllText(_dbg, $"[{DateTime.Now:HH:mm:ss}] MainMeter: constructor ACTUALLY done, about to return\n");
#endif
#if WINFORMS
            return true;
#endif
#if WPF  || WINUI || SMARTMAUI
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
                                                List<object> vmlist,
                                                TickScheduler scheduler)
#endif
#if WPF  || WINUI || SMARTMAUI
        internal async void Bootstrap(SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                List<object> vmlist,
                                                TickScheduler scheduler)
#endif
#if ANDROIDX
        internal async void Bootstrap(AppCompatActivity meterActivity,
                                        Context meterContext,
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                List<object> vmlist,
                                                TickScheduler scheduler)
#endif
        {
#if SMARTMAUI
            string _dbg2 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_debug.txt");
            System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] Bootstrap: ENTERED\n");
#endif
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
#if SMARTMAUI
                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] Bootstrap: inside try\n");
#endif
                ourviewmodel.cookies = signinviewmodel.cookies;
                ourviewmodel.UserName = signinviewmodel.LoginKeys.userName;
#if ANDROIDX
                ourviewmodel.AUsername.Text = ourviewmodel.UserName;
#endif
                ourviewmodel.antiTokenString = signinviewmodel.antiTokenString;

                // Create the all-important DEK to decode incoming data
                // This is based on the User's Password (LoginKey[1]) and their
                // unique netcore-KeasdonEnergy.db Id (LoginKey[10])
#if SMARTMAUI
                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] Bootstrap: cookies+username set, creating DEKs (passwordHash='{signinviewmodel.LoginKeys.passwordHash}', userId='{signinviewmodel.LoginKeys.userId}')\n");
#endif
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
#if SMARTMAUI
                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] Bootstrap: DEKs created\n");
#endif
                ourviewmodel.quitCts = new CancellationTokenSource();
                ourviewmodel.utcOffset = signinviewmodel.utcOffset;
#if WINUI || SMARTMAUI
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
#if WPF  || WINUI
                ourviewmodel.ScrollViewer = FrontEndGUI.FindScrollViewer(this);
                
                ourviewmodel.Border = FrontEndGUI.FindScrollBorder(this);
                //ourviewmodel.TextBlockContent = FrontEndGUI.FindScrollTextBlock(this);
#endif
#if SMARTMAUI
                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] Bootstrap: about to FindScrollViewer/LogLabel/Border\n");
                ourviewmodel.ScrollViewer = FrontEndGUI.FindScrollViewer(this);
                ourviewmodel.LogLabel = FrontEndGUI.FindScrollTextBlock(this);
                ourviewmodel.Border = FrontEndGUI.FindScrollBorder(this);
                //ourviewmodel.TextBlockContent = FrontEndGUI.FindScrollTextBlock(this);



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
                // If you do the following two lines in XAML!!! The entire shit falls over!!
                // **
                // HAVE BEEN WARNED** ==> XamarSHIT Forms is a shit shower <===
#if SMARTMAUI
                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] Bootstrap: FindScroll done, checking ScrollViewer\n");
#endif
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
                ourviewmodel.ShrinkTheDoors.Completed += (s, e) => DoorsAreOpen(s, e, ourviewmodel);
#endif
#if SMARTMAUI
                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] Bootstrap: about to set MeterTop/Bottom\n");
                ourviewmodel.MeterTop = this.MeterTop;
                ourviewmodel.MeterBottom = this.MeterBottom;
                System.IO.File.AppendAllText(_dbg2, $"[{DateTime.Now:HH:mm:ss}] Bootstrap: MeterTop/Bottom set, about to hit first await\n");
#endif
#if WPF
                ourviewmodel.ExpandTheDoors = FrontEndGUI.FindExpandStoryBoard(this);
                ourviewmodel.ExpandTheDoors.Completed += (s, e) => DoorsAreClosed(s, e, ourviewmodel);
#endif
#if WINUI
                ourviewmodel.ExpandTheDoors = FrontEndGUI.FindExpandStoryBoard(this);
                //ourviewmodel.ExpandTheDoors.Commit(this,
                //                                    "ExpandDoors",
                //                                    finished: (finalValue, wasCancelled) =>
                //                                    {
               //                                         DoorsAreClosed(finalValue, wasCancelled, ourviewmodel);
               //                                     });

#endif
                // London eccentric <= London-centric !!!!!

                // This is the expiration date for Analysis costs display                
                // What crap design of an O/S has FOUR FUCKING TYPES OF TIMER???
#if ANDROIDX
                // Won't happen until we dispose of this element
                // which means we have logged-out, closed the doors
                // AND stopped the clock .. so we can then return to 
                // the Sign in page??
#endif
                // Don't enable the Timer Clock until AFTER we have passed Led3 AND Led4 ...
                // Say we have a clock ... which was really started in SignIn
                // but ... hey ho!!
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel, "Clock started");
                // This works because we're not updating the main thread
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Clock started"))
                {
#if WINFORMS
                    return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                    return;
#endif
                }
                ourviewmodel.QuitEnabled = true;
                ourviewmodel.QuitColour = ourviewmodel.yellowColour;
#if WPF || WINUI
                ButtonQuit.Click += (s, e) => ButtonQuitClick(s, e, signinviewmodel, vmlist);
#endif
#if ANDROIDX
                // Android button colour is already set in the MeterView layout ...
                ourviewmodel.AQuit.Click += (s, e) => ButtonQuitClick(s, e,
                                                                    meterActivity,
                                                                    signinviewmodel,
                                                                    vmlist);
#endif
#if SMARTMAUI
                ButtonQuit.Clicked += (s, e) => ButtonQuitClick(s, e, signinviewmodel, vmlist);
#endif
                ourviewmodel.OpenCloseEnabled = true;
#if ANDROIDX
                ourviewmodel.OpenClose.Click += (s, e) => OpenCloseClicked(s, e, signinviewmodel, ourviewmodel);
#endif
                // This can now fail, if it must as we have a timer
                // ticking and we have the Quit button enabled if its needed

                // Create Local if it doesn't already exist.
                // And if it doesn't!! Assume its been DELETED and try
                // and download/rebuild all our stuff!!
                if (!await SmartPhyllV2020.CheckSQLiteNew(
#if ANDROIDX
                                            meterActivity,
#endif
                                            signinviewmodel,
                                            ourviewmodel))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                             ourviewmodel, "SQLite failed 5");
                    // This works because we're not updating the main thread
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 9: SQLite failed: " + ourviewmodel.errorMessage))
                    {
#if WINFORMS
                        return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                        return;
#endif
                    }
#if WINFORMS
                    return false;
#endif
#if WPF  || WINUI || SMARTMAUI
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
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 11: Date conversion failed " + signinviewmodel.LoginKeys.expirationDate1))
                    {
#if WINFORMS
                        return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                        return;
#endif
                    }
#if WINFORMS
                    return false;
#endif
#if WPF  || WINUI || SMARTMAUI
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
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 12: Date conversion failed " + signinviewmodel.LoginKeys.expirationDate2))
                    {
#if WINFORMS
                        return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                        return;
#endif
                    }
#if WINFORMS
                    return false; // <== This may come back to bite you on the bum ...
#endif
#if WPF  || WINUI || SMARTMAUI
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
#if WINFORMS || WPF  || WINUI || SMARTMAUI
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
                //Task<bool> task = GetGoing(... ); doesn't work either
                try
                {
                    if (!await GetGoing(
#if WINFORMS
                                                    processComponents,                                                    
#endif
#if WPF  || WINUI || SMARTMAUI
                                                    this,
#endif
#if ANDROIDX
                                                    meterActivity,
                                                    meterContext,
#endif
                                    signinviewmodel,
                                    ourviewmodel,
                                    vmlist,
                                    scheduler))
                    {
                        
                        // Led8 (might) be red here
#if WINFORMS
                                        return false;
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
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
#if WPF  || WINUI || SMARTMAUI
                    return;
#endif
#if ANDROIDX
                    return;
#endif
                }
            }
            catch (Exception ex)
            {
#if SMARTMAUI
                System.IO.File.AppendAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_debug.txt"), $"[{DateTime.Now:HH:mm:ss}] Bootstrap CATCH: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");
#endif
                signinviewmodel.errorMessage = ex.Message;
#if WINFORMS
                return false;
#endif
#if WPF  || WINUI || SMARTMAUI
                return;
#endif
#if ANDROIDX
                return;
#endif
            }
#if WINFORMS
            return true;
#endif
#if WPF  || WINUI || SMARTMAUI
            return;
#endif
#if ANDROIDX
            return;
#endif
        }

//#if WINDOWS
//        private void OnNavigationFailed(object sender,
//                Microsoft.UI.Xaml.Navigation.NavigationFailedEventArgs e)
//        {
//            Exception ex = e.Exception;

//            Console.WriteLine(ex.ToString());

//            if (Debugger.IsAttached)
//            {
//                Debugger.Break();
//            }
//        }
//#endif
        //public static Animation CreateShrinkAnimation(View view)
        //{
        //    return new Animation(
        //        v => view.Scale = v,
        //        1.0,
        //        0.8);
        //}
#if WINFORMS
        internal static async Task<bool> GetGoing(
#else
        internal async Task<bool> GetGoing(
#endif
#if WINFORMS
                                                    MainProcess processComponents,
#endif
#if WPF  || WINUI || SMARTMAUI
                                                    MainMeter meterComponents,
#endif
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
                                                    Context meterContext,
#endif

                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    List<object> vmlist,
                                                    TickScheduler scheduler)
        {
            ProfilesViewModel profileviewmodel = new ProfilesViewModel();
            vmlist.Add(profileviewmodel);
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
                                                            ourviewmodel.quitCts.Token,
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
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    "Load " + databaseName + " tables failed");
                return false;
            }
            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, "Loaded " + databaseName);

            // Don't clear this yet, we may not even have one to load?
            //ourviewmodel.Hamas.consumersList.Clear();

            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, "Connecting");

            SmartProfile.Groups mmRecord = new SmartProfile.Groups()
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
                ourviewmodel.multiuser.Append(SmartPhyllV2020.DecodeToSQLUsername<SmartProfile.Groups>(group,
                                                                    false,
                                                                    SmartParametersV2016.fieldSeparator));

            }
            // Here is where we say 'Hello' to SmartDBServer
            if (!await SmartBobV2017.ConnectDisconnectAsync(ourviewmodel,
                                                                SmartParametersV2016.connectSymbol,
                                                                ourviewmodel.multiuser,
                                                                ourviewmodel.utcDates,
                                                                SmartParametersV2016.defaultDates))  // No 'lastdate' here
            {
                if (!ourviewmodel.quitCts.IsCancellationRequested)
                {
                    // Set the second Led to Red
                    FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 13: Connecting - " + ourviewmodel.errorMessage))
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
                                            ourviewmodel.quitCts.Token,
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
            if (!await DoWork(
#if WINFORMS
                                processComponents,
#endif
#if WPF  || WINUI || SMARTMAUI
                                meterComponents,
#endif
#if ANDROIDX
                                meterActivity,
                                meterContext,
#endif
                                signinviewmodel,
                                ourviewmodel,
                                vmlist))
            {
                if (!ourviewmodel.quitCts.IsCancellationRequested)
                {
                    // Set the last Led to Red
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 19: DoWork failed"))
                    {
                        return false;
                    }
                }
                return false;
            }
            else
            {
                // Not WINFORMS - yes WINFORMS!!

                try
                {
                    if (!DoViews(
#if WINFORMS
                            processComponents,
#endif
#if WPF  || WINUI || SMARTMAUI
                            meterComponents,
#endif
#if ANDROIDX
                            meterActivity,
#endif
                            signinviewmodel,
                            ourviewmodel))
                    {
                        // SOMETHING has gone amiss ..
                        if (!ourviewmodel.quitCts.IsCancellationRequested)
                        {
                            // Set the last Led to Red
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 20: DoViews failed"))
                            {
                                return false;
                            }
                        }
                        return false;
                    }
                    // Call this - which (should) open the door as the entry for OpenClose should be GREEN
                    // This USED to have an await on it?
#if SMARTMAUI
                    if (!await SmartRoutinesV2018.MoveTheDoorsAsync(ourviewmodel))
#else
                    if (!SmartRoutinesV2018.MoveTheDoorsAsync(ourviewmodel))
#endif
                    {
                        return false;
                    }
                    else
                    {
                        // White disables connects
                        // Orange allows Scrapes
                        // Green we are scraping
                        FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.orangeColour);
                        // This is where we say we are still 'alive'
                        SmartKeepAliveTask task = new SmartKeepAliveTask(
#if WINFORMS
                                                            processComponents,
#endif
#if WPF
                                                            Dispatcher, 
#endif
#if ANDROIDX
                                                            meterActivity,
#endif
#if SMARTMAUI
                                                                Dispatcher,
#endif
                                                                signinviewmodel,
                                                                ourviewmodel,
                                                                vmlist);
                        scheduler.Register(task);
                    }
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                                ourviewmodel,
                                                                "Doors are OPEN");
                    ourviewmodel.Phase1 = false;
                    // Now we can do Phases 2 and 3. Possibly.
                    // So we don't get an immediate Connect
                    ourviewmodel.keepaliveCount = 0;
                    // Phase3 true means Connect not in progress
                    ourviewmodel.Phase3 = true;
                }




                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // From here, we should just see the clock ticking away ...
            // Which is a GOOD sign
            return true;
        }

        internal static async Task<bool> DoWork(
#if WINFORMS
                                                MainProcess processComponents,
#endif
#if WPF  || WINUI || SMARTMAUI
                                                MainMeter meterComponents,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
                                                Context meterContext,
#endif
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                List<object> vmlist)
        {
            // This technique will only FAIL (!!!) IF the user alter's the PC's date/time between when
            // we get the offset (in the website at Keasdon_Energy\SmartSwitch.aspx.cs) and now.
            // This technique assumes that this is not the case (and it will only happen in extremely rare and
            // not worth bothrting about circumstances).  The poorer alternative is to get UTC from the
            // connect to DBServer which I don't think is as elegant as what we have now ...
            //
            // But I might just do that in any case ...
            try
            {
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel, "Databases");
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
#if WPF  || WINUI
                            SmartRoutinesV2018.FullScreenButtonClickActual(meterComponents, signinviewmodel, ourviewmodel);
#endif
#if SMARTMAUI
                            Grid grid = meterComponents.DontDeleteMe;
                            if (grid != null)
                            {
                                SmartRoutinesV2018.FullScreenButtonClickActual(grid, signinviewmodel, ourviewmodel);
                            }
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
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 21: Main adding cubefaces"))
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
                        SmartSpikeV2017.OrderCUBEFACECode(signinviewmodel.Fatah.cubefacesList, ourviewmodel);
                        // If the cubefaces aren't in P, F, U order (?) then everything goes
                        // to rat-shit ...aR
                        foreach (SmartProfile.Cubefaces cubefaceRow in ordered) // Checked
                        {
                            // Are we the logged-in User?
                            if (cubefaceRow.USERNAME == ourviewmodel.UserName)
                            {
                                short screenCode = SmartSpikeV2017.LookupScreenCode(ourviewmodel.CUBEFACESList,
                                                                                        cubefaceRow.CUBEFACE_CODE);
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
                                            ProfilesViewModel profileviewmodel = vmlist.OfType<ProfilesViewModel>().FirstOrDefault();
                                            if (profileviewmodel != null)
                                            {
                                                if (!await SmartRoutinesV2018.ProfileModule(
#if WINFORMS
                                                                                processComponents,
#endif
#if ANDROIDX
                                                                                meterActivity,
                                                                                meterContext,
#endif
                                                                                signinviewmodel,
                                                                                
                                                                                ourviewmodel,
                                                                                profileviewmodel,
                                                                                vmlist,
                                                                                cubefaceRow.CUBEFACE_CODE,
                                                                                cubefaceRow.FACE_CULTURE_CODE,
                                                                                cubefaceRow.FACE_LAST_DISPLAY))
                                                {
                                                    return false;
                                                }
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
                                            FinanceViewModel financeviewmodel = new FinanceViewModel();
                                            vmlist.Add(financeviewmodel);                                            
                                            if (financeviewmodel != null)
                                            {
#if WPF  || WINUI
                                                financeviewmodel.arrowbutton = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\expander_open.png", UriKind.Absolute));
                                                financeviewmodel.cashbag = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\cashbag.jpg", UriKind.Absolute));
                                                financeviewmodel.piggybank = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\piggy_bank.jpg", UriKind.Absolute));
                                                financeviewmodel.stockmarket = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\stock_market.jpg", UriKind.Absolute));
                                                financeviewmodel.bitcoin = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\bitcoin.jpg", UriKind.Absolute));
#endif
                                                if (!await SmartRoutinesV2018.LoadFinanceModule(
#if WINFORMS
                                                                                processComponents,
#endif
#if ANDROIDX
                                                                                meterActivity,
#endif
                                                                                signinviewmodel,
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
                                        }
                                        break;
                                    case SmartParametersV2016.Insurance:
                                        // We *always* need to initialize the
                                        // ScreenCode whether the face is active or not
                                        ourviewmodel.screenCode = SmartSpikeV2017.Lookup_Cubeface_ScreenCode(signinviewmodel,
                                                                                                            SmartParametersV2016.Insurance);

                                        if (cubefaceRow.FACE_ACTIVE)
                                        {
                                            //InsuranceViewModel insuranceviewmodel = vmlist.OfType<InsuranceViewModel>().FirstOrDefault();
                                            //if (insuranceviewmodel != null)
                                            //{
                                                if (!await SmartRoutinesV2018.LoadInsuranceModule(
#if WINFORMS
                                                                                processComponents,
#endif
#if ANDROIDX
                                                                                meterActivity,
#endif
                                                                                ourviewmodel,
                                                                                cubefaceRow.CUBEFACE_CODE,
                                                                                cubefaceRow.FACE_CULTURE_CODE))
                                                {
                                                    return false;
                                                }
                                            //}

                                        }
                                        break;
                                    case SmartParametersV2016.Utility:
                                        // We *always* need to initialize the
                                        // ScreenCode whether the face is active or not
                                        ourviewmodel.screenCode = SmartSpikeV2017.Lookup_Cubeface_ScreenCode(signinviewmodel,
                                                                                                            SmartParametersV2016.Utility);
                                        if (cubefaceRow.FACE_ACTIVE)
                                        {
                                            UtilityViewModel utilityviewmodel = new UtilityViewModel();
                                            vmlist.Add(utilityviewmodel);                                            
                                            if (utilityviewmodel != null)
                                            {
                                                utilityviewmodel.utilityToken = utilityviewmodel.utilityCts.Token;
#if WPF  || WINUI
                                                utilityviewmodel.electricbulb = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\electric_light_bulb.jpg", UriKind.Absolute));
                                                utilityviewmodel.gasflame = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Images\gas_flame.jpg", UriKind.Absolute));
#endif
                                                if (!await SmartRoutinesV2018.LoadUtilityModule(
#if WINFORMS
                                                                                    processComponents,
#endif
#if ANDROIDX
                                                                                    meterActivity,
#endif
                                                                                    signinviewmodel,
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
                                        }
                                        break;
                                    default:
                                        break;
                                }
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
#if WINFORMS || WPF  || WINUI || SMARTMAUI
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
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 22: " + ourviewmodel.errorMessage))
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
#if WPF  || WINUI || SMARTMAUI
                                           MainMeter meterComponents,
#endif
#if ANDROIDX
                                           AppCompatActivity meterActivity,
#endif

                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel)
        {
            // For Swiping go to 
            // https://stackoverflow.com/questions/53755920/how-to-create-left-and-right-swipes-in-wpf-using-mouse
            // and use answer from Shubham Sahu
#if WPF  || WINUI || SMARTMAUI
            try
            {

                if (ourviewmodel.viewCollection.Count > 0)
                {

                    ourviewmodel.CarouselEnabled = true;
                    char cubefaceCode = SmartSpikeV2017.FindCubefaceLastDisplay(ourviewmodel);
                    ourviewmodel.screenCode = SmartSpikeV2017.LookupCubefacesScreenCode(signinviewmodel,
                                                                                        cubefaceCode);

                    ourviewmodel.CurrentPosition = ourviewmodel.screenCode;
                    if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, cubefaceCode))
                    {
                        ourviewmodel.MyContent = SmartRoutinesV2018.CubefaceView(ourviewmodel, cubefaceCode);
                    }
                }
            }            
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message.ToString());
            }
#endif
#if ANDROIDX
            if (ourviewmodel.viewCollection.Count > 0)
            {
                ourviewmodel.CarouselEnabled = true;
                char cubefaceCode = SmartSpikeV2017.FindCubefaceLastDisplay(ourviewmodel);
                ourviewmodel.screenCode = SmartSpikeV2017.LookupCubefacesScreenCode(signinviewmodel,
                                                                                    cubefaceCode);
                ourviewmodel.CurrentPosition = ourviewmodel.screenCode;
                ourviewmodel.MyContent.Touch += (s,e) => MyContentTouch(s, e, meterActivity, signinviewmodel, ourviewmodel);
                if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, cubefaceCode))
                {
                    ourviewmodel.MyContent.AddView(SmartRoutinesV2018.CubefaceView(ourviewmodel, cubefaceCode));
                }

                //ourviewmodel.CurrentPosition = ourviewmodel.screenCode;
                //char cubefaceCode = SmartSpikeV2017.LookupCUBEFACECode(signinviewmodel.Fatah.cubefacesList,
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

        internal static void MyContentTouch(object sender, TouchEventArgs e,
                                            AppCompatActivity meterActivity,
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel)
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
                            Toast.MakeText(meterActivity,
                                        "Swipe right",  // So show previous View 
                                        ToastLength.Short).Show();
                            SmartRoutinesV2018.OnSwipedRightActual(sender, e, meterActivity, signinviewmodel, ourviewmodel);
                        }
                        else
                        {
                            Toast.MakeText(meterActivity,
                                        "Swipe left",   // So show next View
                                        ToastLength.Short).Show();
                            SmartRoutinesV2018.OnSwipedLeftActual(sender, e, meterActivity, signinviewmodel, ourviewmodel);
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

        
#if WINFORMS || WPF || WINUI
        internal void OpenCloseClicked(object sender, object e,
                                        SignInViewModel signinviewmodel,
                                        MainViewModel ourviewmodel)
        {
#endif
#if ANDROIDX
        internal void OpenCloseClicked(object sender, EventArgs e,
                                        SignInViewModel signinviewmodel,
                                        MainViewModel ourviewmodel)
        {
#endif
#if SMARTMAUI
        internal async void OpenCloseClicked(object sender, object e,
                                        SignInViewModel signinviewmodel,
                                        MainViewModel ourviewmodel)
        {
#endif
            if ((sender != null) &&
#if WINFORMS || WPF
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if WINUI
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
#endif
#if ANDROIDX
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if SMARTMAUI
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
            {
#if SMARTMAUI
                await SmartRoutinesV2018.OpenCloseClickedActual(ourviewmodel, signinviewmodel.UserIsLoggedIn);
#else
                SmartRoutinesV2018.OpenCloseClickedActual(ourviewmodel, signinviewmodel.UserIsLoggedIn);
#endif
            }
            return;
        }

#if WINFORMS
        internal void FullScreenButtonClick(object sender, EventArgs e,
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel)
#endif
#if WPF  || WINUI
        internal void FullScreenButtonClick(object sender, object e,
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel)
#endif
#if ANDROIDX
        internal void FullScreenButtonClick(object sender, object e,
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel)
#endif
#if SMARTMAUI
        internal void FullScreenButtonClick(object sender, EventArgs e,
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel)
#endif
        {
            // Need to "hide" the first three lines ?
            if (sender != null &&
#if WINFORMS || WPF
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if WINUI
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
#endif
#if ANDROIDX
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if SMARTMAUI
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
            {
                switch (signinviewmodel.Platform)
                {
#if WPF
                    case SmartParametersV2016.WPF:
                        SmartRoutinesV2018.FullScreenButtonClickActual(this, signinviewmodel, ourviewmodel);
                        break;
#endif
#if WINUI
                    case SmartParametersV2016.WinUI:
                        //Grid greed = this.DontDeleteMe;
                        //SmartRoutinesV2018.FullScreenButtonClickActual(greed, signinviewmodel, ourviewmodel);
                        break;
#endif
#if ANDROIDX
                    case SmartParametersV2016.Android:
                        // Think Android is always full-screen in any case??
                        SmartRoutinesV2018.FullScreenButtonClickActual(this, signinviewmodel, ourviewmodel);
                        break;
#endif
#if SMARTMAUI
                    case SmartParametersV2016.WinUI:
                        Grid greed = this.DontDeleteMe;
                        SmartRoutinesV2018.FullScreenButtonClickActual(greed, signinviewmodel, ourviewmodel);
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
                                     AppCompatActivity meterActivity,
                                     SignInViewModel signinviewmodel,
                                     List<object> vmlist)
        {
            if (sender != null && e != null)
            {
                MainViewModel ourviewmodel = vmlist.OfType<MainViewModel>().FirstOrDefault();
                if (!ourviewmodel.quitinprocess)
                {
                    ourviewmodel.quitinprocess = true;

                    // Create the dialog instance
                    QuitMessageBox quitDialog = new QuitMessageBox(meterActivity, signinviewmodel, vmlist);
                    ourviewmodel.APopupDialog = quitDialog; // optional if you need to reference it elsewhere

                    quitDialog.SetContentView(Resource.Layout.QuitMessageBox);

                    // Bind UI elements
                    quitDialog.FindViewById<TextView>(Resource.Id.Confirmation).Text = ourviewmodel.Confirmation;
                    quitDialog.FindViewById<TextView>(Resource.Id.ReallyLeave).Text = ourviewmodel.ReallyLeave;
                    quitDialog.FindViewById<TextView>(Resource.Id.QuitYes).Text = ourviewmodel.Yes;
                    quitDialog.FindViewById<TextView>(Resource.Id.QuitNo).Text = ourviewmodel.No;

                    quitDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
                    quitDialog.Show();

                    // Hook up buttons to instance methods
                    Button btnPopupYes = quitDialog.FindViewById<Button>(Resource.Id.QuitYes);
                    Button btnPopupNo = quitDialog.FindViewById<Button>(Resource.Id.QuitNo);

                    btnPopupYes.Click += async (s, ev) => await quitDialog.PopupYes(s, e, vmlist);
                    btnPopupNo.Click += (s, ev) => quitDialog.PopupNo(s, e, ourviewmodel);
                }
            }
            return;
        }
#endif

#if WINFORMS
        internal static void ButtonQuitClick(object sender, EventArgs e,
                                                MainViewModel ourviewmodel)
        {
            if (sender != null && e != null)
            {
                QuitConfirmForm popup = new QuitConfirmForm();

                DialogResult dialogresult = popup.ShowDialog();
                // If I put a FormClosed event in for this Form, then
                // it gets called when I click 'yes' or 'No and that
                // is TOO EARLY!!  I want the Close code executed when
                // I choose Yes and not before.
                if (dialogresult == DialogResult.OK)
                {
                    QuitMessageBox.PopupYes(sender, e,
                                        vmlist);

                }
                else if (dialogresult == DialogResult.Cancel)
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
        internal void ButtonQuitClick(object sender, object e,
                                        SignInViewModel signinviewmodel,
                                        List<object> vmlist)
        {
            if (sender != null &&
                (FrontEndGUI.DecodeEventFlags(e) != null))
            {
                QuitMessageBox quitmessageboxPage = new QuitMessageBox(signinviewmodel, vmlist);
                quitmessageboxPage.ShowDialog();
                
            }
            return;
        }
#endif
#if WINUI
        internal async void ButtonQuitClick(object sender, object e,
                                        SignInViewModel signinviewmodel,
                                        List<object> vmlist)
        {
            if (sender != null &&
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
            {
                QuitMessageBox messageboxPage = new QuitMessageBox(signinviewmodel, vmlist)
                {
                    XamlRoot = this.XamlRoot
                };
                await FrontEndGUI.DecodeMessageBox(messageboxPage);
            }
            return;
        }
#endif

#if SMARTMAUI
        internal async void ButtonQuitClick(object sender,
                                            EventArgs e,
                                            SignInViewModel signinviewmodel,
                                            List<object> vmlist)
        {
            if (sender is not Button)
            {
                return;
            }

            
            QuitMessageBox messageboxPage = new QuitMessageBox(signinviewmodel, vmlist);

            //await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushModalAsync(messageboxPage);
            await Microsoft.Maui.Controls.Application.Current.MainPage.ShowPopupAsync(messageboxPage);

            //if (result is bool ok && ok)
            //{
            //    await Shell.Current.GoToAsync("//SignIn");
            //}
            return;
        }
#endif



#if WINFORMS || WPF || WINUI || SMARTMAUI
        internal async void ButtonLeftClicked(object sender,
#if WINFORMS || WPF || SMARTMAUI
                                                EventArgs e,

#endif
#if WINUI
                                                RoutedEventArgs e,
#endif
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel)
        {
            await SmartRoutinesV2018.ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
            return;
        }
#endif
#if WINFORMS || WPF || WINUI || SMARTMAUI
        internal async void ButtonRightClicked(object sender,
#if WINFORMS || WPF || SMARTMAUI
                                                EventArgs e,

#endif
#if WINUI
                                                RoutedEventArgs e,
#endif
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel)
        {
            await SmartRoutinesV2018.ButtonRightClickedActual(signinviewmodel, ourviewmodel);
            return;
        }
#endif
#if WINFORMS || WPF
        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal void SliderValueChanged(object sender, object e)
#endif
#if WPF
        internal async void SliderValueChanged(object sender, object e,
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
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
                                                                        signinviewmodel,
                                                                        ourviewmodel,
                                                                         utilityviewmodel,
                                                                         ourviewmodel.Blanche.vatRatesList,
                                                                         ourviewmodel.Blanche.exchangeRatesList))
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Slider failed"))
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
        //                                                                        signinviewmodel,
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
        internal void DoorsAreOpen(object sender, EventArgs e,
                                    MainViewModel ourviewmodel)
        {
            ourviewmodel.doorDirection = true;
        }
        internal void DoorsAreClosed(object sender, EventArgs e,
                                    MainViewModel ourviewmodel)
        {
            ourviewmodel.doorDirection = false;
        }
#endif
#if WINUI
        internal void DoorsAreOpen(object sender, object e, MainViewModel ourviewmodel)
        {
            ourviewmodel.doorDirection = true;
        }

        internal void DoorsAreClosed(object sender, object e, MainViewModel ourviewmodel)
        {
            ourviewmodel.doorDirection = false;
        }
#endif
#if ANDROIDX
        internal void DoorsAreOpen(object sender, EventArgs e,
                                    MainViewModel ourviewmodel)
        {
            ourviewmodel.doorDirection = true;
        }
        internal void DoorsAreClosed(object sender, EventArgs e,
                                    MainViewModel ourviewmodel)
        {
            ourviewmodel.doorDirection = false;
        }
#endif
        // From Dan - 5th Jan 2012 - this routine loops and won't let me close!!
        // from Dad - 3rd Feb 2012 - But I have fixed it now
        // New version Dan - all because of this Silverligh shit ... sorry
        // But the effect is as NEAT as shit!  This program IS the Dog's Bollocks!!!
#if SMARTMAUI
        internal void DoorsAreOpen(double finalValue, bool wasCancelled, MainViewModel ourviewmodel)
        {
            ourviewmodel.doorDirection = true;
        }        
        internal void DoorsAreClosed(double finalValue, bool wasCancelled, MainViewModel ourviewmodel)
        {
            ourviewmodel.doorDirection = false;
        }
#endif

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
                                                    WebView2 FinanceWebView,
                                                    MainProcess components,
#endif
#if SMARTMAUI
                                                    WebView FinanceWebView,
#endif
#if WPF || WINUI 
                                                    WebView2 FinanceWebView,
#endif
#if ANDROIDX
                                                    WebView FinanceWebView,
                                                    AppCompatActivity meterActivity,
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
                                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Finance Auto-Submit started"))
                                            {
                                                return false;
                                            }
                                            if (!await SmartFinanceV2025.FinanceButtonSubmitClickActual(
#if WINFORMS
                                                                                                        FinanceWebView,
                                                                                                        components.textBoxConsole,
                                                                                                        components,
#endif
#if SMARTMAUI
                                                                                                        FinanceWebView,
#endif
#if WPF  || WINUI
                                                                                                        FinanceWebView,
#endif
#if ANDROIDX
                                                                                                        FinanceWebView,
                                                                                                        meterActivity,
#endif
                                                                                                        ourviewmodel,
                                                                                                        financeviewmodel))
                                            {
                                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                                                        ourviewmodel.quitCts.Token,
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
                                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 31: Finance Next Connection"))
                                                {
                                                    return false;
                                                }
                                                return false;
                                            }
                                            // Succeed or Fail do nothing until this time (unless Submit button pressed)
#if WINFORMS || WPF  || WINUI || SMARTMAUI
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

                                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Utility Auto-Submit started"))
                                            {
                                                return false;
                                            }

                                            if (!await SmartUtilityScrapeV2022.UtilityButtonSubmitClickActual(
#if WINFORMS
                                                                                                    components,
#endif
#if ANDROIDX
                                                                                                    meterActivity,
#endif
                                                                                                    ourviewmodel,
                                                                                                    utilityviewmodel,
                                                                                                    ourviewmodel.Blanche.vatRatesList,
                                                                                                    ourviewmodel.Blanche.exchangeRatesList,
                                                                                                    ourviewmodel.Blanche.postcodesList))
                                            {
                                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                                                        ourviewmodel.quitCts.Token,
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
                                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 32: Utility Next Connection"))
                                                {
                                                    return false;
                                                }
                                                return false;
                                            }
                                            // Success or Fail do nothing until this time (unless Submit button pressed)
#if WINFORMS || WPF  || WINUI || SMARTMAUI
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
#if WPF  || WINUI
        internal void DefaultContextMenuRightMouseButtonDown(object sender, RoutedEventArgs e) //MouseButtonEventArgs e)
#endif
#if ANDROIDX
        internal void DefaultContextMenuRightMouseButtonDown(object sender, object e) //MouseButtonEventArgs e)
#endif
#if SMARTMAUI
        internal void DefaultContextMenuRightMouseButtonDown(object sender, object e) //MouseButtonEventArgs e)
#endif
        {
            // Handle the event so the default context menu is hidden
            if (sender != null &&
#if WINFORMS
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF  || WINUI || SMARTMAUI
                FrontEndGUI.DecodeRoutedEventFlags(e) != null)
#endif
#if ANDROIDX
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
            {
#if WPF
                // No WINUI (stops it bubbling up)
                e.Handled = true;
#endif
            }
            return;
        }

#if WPF
        public void MainMeterLoaded(object sender, RoutedEventArgs e, MainViewModel ourviewmodel)
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
#endif
#if WINUI


    public void MainMeterLoaded(object sender, RoutedEventArgs e, MainViewModel ourviewmodel)
    {
        var window = (Application.Current as App).MainWindow;

        if (window == null)
            return;

        // Get HWND
        var hWnd = WindowNative.GetWindowHandle(window);

        // Get AppWindow
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        // Get display area (screen)
        var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);

        // Screen size
        ourviewmodel.actualWidth = displayArea.WorkArea.Width;
        ourviewmodel.actualHeight = displayArea.WorkArea.Height;

        // Window size
        ourviewmodel.currentWidth = window.Bounds.Width;
        ourviewmodel.currentHeight = window.Bounds.Height;
    }
#endif
#if SMARTMAUI
        public void MainMeterLoaded(object sender,
                                    EventArgs e,
                                    MainViewModel ourviewmodel)
        {
            // Current page/window size
            double screenWidth = Width;
            double screenHeight = Height;

            ourviewmodel.actualWidth = screenWidth;
            ourviewmodel.actualHeight = screenHeight;

            // Current visible page size
            ourviewmodel.currentWidth = Width;
            ourviewmodel.currentHeight = Height;
        }
#endif

#if WPF || WINUI
#if WPF
        public async void MainMeterClosed(object sender, EventArgs e, MainViewModel ourviewmodel)
#endif
#if WINUI
        public async void MainMeterClosed(object sender, RoutedEventArgs e, MainViewModel ourviewmodel)
#endif
        {
            // This is always called, believe it or not!!
            if (FrontEndGUI.CompareLedColour(ourviewmodel, 2, ourviewmodel.greenColour))
            {
                //
                // Clock stop sequence
                //
                //_scheduler.Stop();

                //This last message isn't forced to scroll otherwise we would be trying to
                // update the UI from another thread.  Not good - and it gives an error ...
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Clock stopped");
#endif
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Clock stopped"))
                {
                    return;
                }
                // Cancel ourviewmodel.quitCts?? and quitToken??
                if (FrontEndGUI.GetBorderVisible(ourviewmodel))
                {
                    // Do two invisibles at once
                    FrontEndGUI.SetBorderScrollInvisible(ourviewmodel);
                }
                ShutDown(true);
            }
            return;
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        internal static void ShutDown(bool shutdown_action)
#endif
#if ANDROIDX
        internal static void ShutDown(bool shutdown_action)
#endif
        {
            // Only do this if SHUTDOWN is true
            if (shutdown_action)
            {
                // NOW this is a life-saving routine
                // which will take us back to Default.aspx if we are hosted in a
                // proper browser i.e. when BrowserInteropHelper.HostScript isn't null ...

                // WELL, HOSTING IN A BROWSER IS NEVER GOING TO HAPPEN IS IT ??

                // This saved my life
                // https://ebudur.wordpress.com/2014/07/25/wpf-web-application-throws-system-missingmethodexception-while-calling-javascript-method-in-ie9ie10ie11
                //
                //dynamic hostScript = BrowserInteropHelper.HostScript;
                //if (hostScript != null)
                //{
                //    // This function HAS TO BE in SmartSwitch.aspx or it all falls over!!
                //    hostScript.document.gobacktoDefault();  // Says it all, really ...
                //}
                //else
                //{
                //if (Application.Current == ShutdownMode.OnExplicitShutdown)
                //{



#if WPF
                if (Application.Current.ShutdownMode == ShutdownMode.OnExplicitShutdown)
                {
                    Application.Current.Shutdown();
                }
#endif
#if WINUI || SMARTMAUI
                //if (Application.Current.ShutdownMode == ShutdownMode.OnExplicitShutdown)
                //{
                Application.Current.Exit();
                //}
#endif
#if ANDROIDX
                //Android.Content.Context con = MainActivity.
                //Activity activity = (Activity)MainActivity.Context;
                //this.FinishAffinity();
                //GetActivity.Finish();

                // Exit the Application
                //Finish();

                //Application.Exit(0);


                // All of the above fails to compile so ... I am going with
                // https://stackoverflow.com/questions/17719634/how-to-exit-an-android-app-programmatically
                Process.KillProcess(Process.MyPid());
                // Not sure I can do anything after this??
#endif
            }
            // Only because every routine has to have a RETURN!!
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
            //#if WINUI
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

#if SMARTMAUI
        protected override void OnAppearing()
        {
                base.OnAppearing();

#if WINDOWS
        
        var nativeWindow = App.Current.Windows[0].Handler.PlatformView
            as Microsoft.UI.Xaml.Window;

        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

        var windowId =
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

        var appWindow =
            Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);

        // work area (excludes taskbar)
        var wa = displayArea.WorkArea;

        // 80% size
        int width = (int)(wa.Width * 0.9);
        int height = (int)(wa.Height * 0.9);

        // center it
        int x = wa.X + (wa.Width - width) / 2;
        int y = wa.Y + (wa.Height - height) / 2;

        appWindow.MoveAndResize(new RectInt32(x, y, width, height));

        //appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
#endif
        }
#endif

        
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
}