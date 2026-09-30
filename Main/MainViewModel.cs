//using System.ComponentModel;
//using System.Globalization;
//using System.Net;
//using System.Text;


#if DBSERVER
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using SQLite;
#endif

#if WINFORMS
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Text;
using System.Windows;
using SQLite;
#endif

#if WPF
using System.Windows;
using SQLite;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.IO;
#endif

#if WINUI
using System.Collections.Generic;
using System;
using System.Threading;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using SQLite;
using Microsoft.UI.Dispatching;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Globalization;
#endif

#if ANDROIDX
using Android.Graphics;
using Android.Views;
using Android.Content;
using Android.Views.Animations;
using System.ComponentModel;
using System.Text;
using System.Globalization;
using System.Net;
using SQLite;
using AndroidX.AppCompat.App;
#endif

#if SMARTMAUI
using SQLite;
using Microsoft.Maui.Dispatching;
using System.ComponentModel;
using System.Text;
using System.Globalization;
using System.Net;
#endif

namespace SmartCubeMobile
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null && propName != null)    // Once upon a time .. DateTimeNowMessage was null ...
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

#if WINFORMS
        public Image meterbg;
        public Image MeterBackground
        {
            get => meterbg;
            set
            {
                meterbg = value;
                this.NotifyPropertyChanged(nameof(MeterBackground));
            }
        }
#endif
#if WPF  || WINUI || SMARTMAUI
        public ImageSource meterbg;
        public ImageSource MeterBackground
        {
            get => meterbg;
            set
            {
                meterbg = value;
                this.NotifyPropertyChanged(nameof(MeterBackground));
            }
        }
#endif
        private string usernameDefault = "";
        public string UserName
        {
            get
            {
                return usernameDefault;
            }
            set
            {
                if (usernameDefault != value)
                {
                    usernameDefault = value;
                    this.NotifyPropertyChanged(nameof(UserName));
                }
            }
        }

        // Not sure this is really needed??
        internal bool[] Phase2 = new bool[3] { true, false, false };
        
        internal string wherewereat = "";
        internal SmartUsersList Hamas = new SmartUsersList();
        internal SmartProfileList SmartProfile = new SmartProfileList();
        internal StringBuilder temp_stream = new StringBuilder();
        internal int recordCount = 0;   

        internal SmartJoyList Blanche = new SmartJoyList();      // Initialized in DoWork

        internal List<SmartData.Cubefaces> CUBEFACESList = new List<SmartData.Cubefaces>();

        internal System.Random randomR = new System.Random();   // Initial seed

        // These are COPIES from SignIn Fatah so  I don't have to refer to it
        // in Finance and Utility
        internal List<SmartData.CultureView> cultureviewList = new List<SmartData.CultureView>();
        internal List<SmartData.Currencies> currenciesList = new List<SmartData.Currencies>();
        internal List<SmartData.SQLiteSchemas> sqliteschemasList = new List<SmartData.SQLiteSchemas>();
        internal List<SmartData.SQLiteTables> sqlitetablesList = new List<SmartData.SQLiteTables>();
        internal List<SmartData.SQLiteFields> sqlitefieldsList = new List<SmartData.SQLiteFields>();
        public List<SmartUsers.ConsumersSQLite> sqliteConsumerList;

        public enum ChangeTypeEnum { None, Added, Modified, Deleted }

        private bool headless = true;
        public bool Headless
        {
            get
            {
                return headless;
            }
            set
            {
                if (headless != value)
                {
                    headless = value;
                    this.NotifyPropertyChanged(nameof(Headless));
                }
            }
        }

        private string timeNow = "";
        public string TimeNow
        {
            get
            {
                return timeNow;
            }
            set
            {
                if (timeNow != value)
                {
                    timeNow = value;
                    this.NotifyPropertyChanged(nameof(TimeNow));
                }
            }
        }

        private double gridwidth = 0;
        public double GridWidth
        {
            get
            {
                return gridwidth;
            }
            set
            {
                gridwidth = value;
                NotifyPropertyChanged(nameof(GridWidth));
            }
        }

        private bool carouselenabled = false;
        public bool CarouselEnabled
        {
            get
            {
                return carouselenabled;
            }
            set
            {
                carouselenabled = value;
                NotifyPropertyChanged(nameof(CarouselEnabled));
            }
        }
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string zoomtooltip = "";
        public string ZoomTooltip
        {
            get
            {
                return zoomtooltip;
            }
            set
            {
                zoomtooltip = value;
                NotifyPropertyChanged(nameof(ZoomTooltip));
            }
        }

        private string autoswitchtooltip = "";
        public string AutoSwitchTooltip
        {
            get
            {
                return autoswitchtooltip;
            }
            set
            {
                autoswitchtooltip = value;
                NotifyPropertyChanged(nameof(AutoSwitchTooltip));
            }
        }
        private string openclosetooltip = "";
        public string OpenCloseTooltip
        {
            get
            {
                return openclosetooltip;
            }
            set
            {
                openclosetooltip = value;
                NotifyPropertyChanged(nameof(OpenCloseTooltip));
            }
        }

        private string quittooltip = "";
        public string QuitTooltip
        {
            get
            {
                return quittooltip;
            }
            set
            {
                quittooltip = value;
                NotifyPropertyChanged(nameof(QuitTooltip));
            }
        }
#endif
        internal bool Phase1 = true;
        // One for each Cubeface
        internal bool Phase3 = false;

#if ANDROIDX
        internal float isExpanded = 1.0f;        
        internal AndroidX.Fragment.App.FragmentManager Fm { get; set; }
        internal AndroidX.Lifecycle.Lifecycle Lfc { get; set; }
        internal LayoutInflater Inflater { get; set; }
        internal FrameLayout MyContent { get; set; }
        //internal AppCompatActivity activity { get; set; }
        //internal Context context { get; set; }
        internal ScrollView ScrollViewer { get; set; }
        internal TextView TextBlockContent { get; set; }
        internal Button AutoSwitch { get; set; }
        internal Button OpenClose { get; set; }
        internal TextView ADateTimeNow { get; set; }
        internal TextView AUsername { get; set; }
        internal ImageView MeterTop { get; set; }
        internal ImageView MeterBottom { get; set; }
        internal TextView ALed1 { get; set; }
        internal TextView ALed2 { get; set; }
        internal TextView ALed3 { get; set; }
        internal TextView ALed4 { get; set; }
        internal TextView ALed5 { get; set; }
        internal TextView ALed6 { get; set; }
        internal TextView ALed7 { get; set; }
        internal TextView ALed8 { get; set; }
        internal Button AQuit { get; set; }
        internal Dialog APopupDialog { get; set; }
        internal TextView AConfirmation { get; set; }
        internal TextView AReallyLeave { get; set; }
        internal TextView AQuitYes { get; set; }
        internal TextView AQuitNo { get; set; }
        internal short ASupplier_code { get; set; }
        internal short ABrand_code { get; set; }
        //internal GridLength rowheight = new GridLength(0);
        internal Animation ShrinkTheDoors { get; set; }
        internal Animation ExpandTheDoors { get; set; }
#endif
        public class ImageItem
        {
            public char Cubeface { get; set; }
            public string ImageName { get; set; }
            public System.IO.Stream StreamBits { get; set; }
        }

        public enum TimerFinishReason
        {
            Completed,  // finished naturally
            Cancelled   // stopped by user
        }
        public class TimerEntry
        {
            public int RemainingSeconds;
            public Action<string> OnTick;
            public Action<string, TimerFinishReason> OnComplete;
        }
        internal string languageTag = "";
#if WPF  || WINUI || SMARTMAUI
        internal ResourceDictionary rays = new ResourceDictionary();
#endif
#if ANDROIDX
        internal bool quitinprocess = false;
#endif
        internal bool SMARTFINANCE = false;
        internal bool SMARTUTILITY = false;

        internal int tablesCreated = 0;
        internal bool firstTime = false;
        internal int sscount = 0;

        internal string remoteUTCTable = "";

        internal StringBuilder multiuser { get; set; }

#if WINFORMS
        //internal DataGridView GroupsDataGrid = new DataGridView();
#endif
#if WPF  || WINUI || SMARTMAUI
        internal bool webviewLogging = false;
#endif
        internal TimeSpan utcOffset = SmartParametersV2016.utcdefaultOffset;

        internal CancellationTokenSource quitCts = new CancellationTokenSource();
        //internal CancellationToken userToken = new CancellationToken();

        internal CultureInfo[] cultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        internal bool isfullscreen = false;

        internal string utcDates = "";
        internal string utcList = "";

        internal decimal[] exchangeRate = new decimal[4] { 0, 0, 0, 0 };    // Default GBP


#if WINFORMS || WPF  || WINUI || SMARTMAUI

        //internal UserNotificationListener notificationListener = UserNotificationListener.Current;
        //internal UserNotificationListenerAccessStatus accessStatus;
#endif
#if ANDROIDX
        internal string accessStatus = "";
#endif
        internal string antiTokenString = "";
        internal int keepaliveCount = 0;
        internal int cryptoRefreshCount = 0;
#if WPF
        internal WindowState state = WindowState.Normal;
#endif
#if WINUI
        internal Application state = Application.Current;
        internal DispatcherQueue Dispatcher { get; set; }

#endif
#if WPF  || WINUI
        internal GridLength rowheight = new GridLength(0);
        internal Storyboard ShrinkTheDoors { get; set; }
        internal Storyboard ExpandTheDoors { get; set; }
#endif
#if SMARTMAUI
        internal IDispatcher Dispatcher { get; set; }
        internal GridLength rowheight = new GridLength(0);
        internal Animation ShrinkTheDoors { get; set; }
        internal Animation ExpandTheDoors { get; set; }
#endif


        internal CookieContainer cookies = new CookieContainer();

        internal string website = "";
        internal TimeSpan timespanTimeout = SmartParametersV2016.timespanTimeout;        // One minute

        internal DateTime targetDate = SmartParametersV2016.defaultDate;

        internal Uri TargetUrl { get; set; }

        internal Uri webproxyUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri listenerUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri dbserverUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri connectUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri insertcommonUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri storedproceduresUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri lookuplogoUrl = new Uri(SmartParametersV2016.localWebsite);
        //internal Uri lookupdnoUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri loadtableUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri downloadpdfUrl = new Uri(SmartParametersV2016.localWebsite);

        internal string sessionId = "";

        // One of these is passed into Read General Meter and updated therein
        // (these are 'dynamic')
        internal const string defaultDates = SmartParametersV2016.defaultDates,
                                format0Places = SmartParametersV2016.format0Places,
                                format2Places = SmartParametersV2016.format2Places,
                                format3Places = SmartParametersV2016.format3Places;
        internal string currencyFormat = SmartParametersV2016.currencyFormat;

        internal bool doorDirection = false;

        // Use https://quicktype.io/csharp/ for this
        internal string jsonString = @"{ 'Risk': { },
                                    'Data': 
                                    { 
                                        'TransactionToDateTime': '2021-12-31T00:00:00Z',
                                        'ExpirationDateTime': '2022-01-01T00:00:00Z',
                                        'Permissions': [ 'ReadAccountsDetail',
                                                        'ReadBalances',
                                                        'ReadTransactionsCredits',
                                                        'ReadTransactionsDebits',
                                                        'ReadTransactionsDetail'
                                                     ],
                                        'TransactionFromDateTime': '2019-01-01T00:00:00Z'
                                    }
                                }";

#if WINFORMS
        internal Color blackColour = Color.Black;
        internal Color greyColour = Color.LightGray;
        internal Color whiteColour = Color.White;
        internal Color magentaColour = Color.Magenta;
        internal Color cyanColour = Color.Cyan;
        internal Color transparentColour = Color.Transparent;
        internal Color redColour = Color.Red;
        internal Color greenColour = Color.Green;
        internal Color lightgreenColour = Color.LightGreen;
        internal Color orangeColour = Color.Orange;
        internal Color yellowColour = Color.Yellow;
        internal Color palegoldenrodColour = Color.PaleGoldenrod;
        // Autoswitch
        internal Color darkredColour = Color.DarkRed;
        internal Color darkorangeColour = Color.DarkOrange;
        internal Color darkgreenColour = Color.DarkGreen;
#endif
#if WPF  || WINUI
        internal Brush blackColour = new SolidColorBrush(Colors.Black);
        internal Brush greyColour = new SolidColorBrush(Colors.LightGray);
        internal Brush whiteColour = new SolidColorBrush(Colors.White);
        internal Brush magentaColour = new SolidColorBrush(Colors.Magenta);
        internal Brush cyanColour = new SolidColorBrush(Colors.Cyan);
        internal Brush transparentColour = new SolidColorBrush(Colors.Transparent);
        internal Brush greenColour = new SolidColorBrush(Colors.Green);
        internal Brush lightgreenColour = new SolidColorBrush(Colors.LightGreen);
        internal Brush orangeColour = new SolidColorBrush(Colors.Orange);
        internal Brush redColour = new SolidColorBrush(Colors.Red);
        internal Brush yellowColour = new SolidColorBrush(Colors.Yellow);
        internal Brush palegoldenrodColour = new SolidColorBrush(Colors.PaleGoldenrod);
        // Autoswitch
        internal Brush darkredColour = new SolidColorBrush(Colors.DarkRed);
        internal Brush darkorangeColour = new SolidColorBrush(Colors.DarkOrange);
        internal Brush darkgreenColour = new SolidColorBrush(Colors.DarkGreen);
#endif
#if ANDROIDX
        internal Color blackColour = Color.Black;
        internal Color greyColour = Color.LightGray;
        internal Color whiteColour = Color.White;
        internal Color magentaColour = Color.Magenta;
        internal Color cyanColour = Color.Cyan;
        internal Color transparentColour = Color.Transparent;
        internal Color redColour = Color.Red;
        internal Color greenColour = Color.Green;
        internal Color orangeColour = Color.Orange;
        internal Color yellowColour = Color.Yellow;
        internal Color palegoldenrodColour = Color.PaleGoldenrod;
        // Autoswitch
        internal Color darkredColour = Color.DarkRed;
        internal Color darkorangeColour = Color.DarkOrange;
        internal Color darkgreenColour = Color.DarkGreen;

        internal Color autoswitch0 = Color.Purple;
        internal Color autoswitch25 = Color.MediumPurple;
        internal Color autoswitch50 = Color.Violet;
        internal Color autoswitch75 = Color.DarkViolet;

        internal Color opengreen0 = Color.Green;
        internal Color opengreen25 = Color.LawnGreen;
        internal Color opengreen50 = Color.LightGreen;
        internal Color opengreen75 = Color.DarkGreen;

        internal Color openred0 = Color.Pink;
        internal Color openred25 = Color.IndianRed;
        internal Color openred50 = Color.Red;
        internal Color openred75 = Color.DarkRed;
#endif
#if SMARTMAUI
        internal Color blackColour = Colors.Black;
        internal Color greyColour = Colors.LightGray;
        internal Color whiteColour = Colors.White;
        internal Color magentaColour = Colors.Magenta;
        internal Color cyanColour = Colors.Cyan;
        internal Color transparentColour = Colors.Transparent;
        internal Color redColour = Colors.Red;
        internal Color greenColour = Colors.Green;
        internal Color orangeColour = Colors.Orange;
        internal Color yellowColour = Colors.Yellow;
        internal Color palegoldenrodColour = Colors.PaleGoldenrod;
        // Autoswitch
        internal Color darkredColour = Colors.DarkRed;
        internal Color darkorangeColour = Colors.DarkOrange;
        internal Color darkgreenColour = Colors.DarkGreen;

        internal Color autoswitch0 = Colors.Purple;
        internal Color autoswitch25 = Colors.MediumPurple;
        internal Color autoswitch50 = Colors.Violet;
        internal Color autoswitch75 = Colors.DarkViolet;

        internal Color opengreen0 = Colors.Green;
        internal Color opengreen25 = Colors.LawnGreen;
        internal Color opengreen50 = Colors.LightGreen;
        internal Color opengreen75 = Colors.DarkGreen;

        internal Color openred0 = Colors.Pink;
        internal Color openred25 = Colors.IndianRed;
        internal Color openred50 = Colors.Red;
        internal Color openred75 = Colors.DarkRed;

#endif
        //internal bool meterDisplay = false;
#if WINFORMS
        internal System.Windows.Forms.Timer timerClock = new System.Windows.Forms.Timer();
        internal System.Windows.Forms.Timer timerDoor;
        // Main loop happens in the Meter tick section

        internal int topIncrement = 0;
        internal int bottomIncrement = 0;
        internal int topActualheight = 0;
        internal int bottomActualheight = 0;

        internal bool jsonReturned = false;
        internal Potential loggedInUser = new Potential();

#endif
#if WPF  || WINUI
        internal DispatcherTimer timerClock = new DispatcherTimer();

#endif
#if ANDROIDX
        public System.Timers.Timer timerClock = new System.Timers.Timer();
#endif
        internal StringBuilder bollocks = new StringBuilder();
        internal bool trace = false;         // Passed in as Arg 2
        internal bool subscriber = false;    // Passed in as Arg 7
        internal bool multimeter = false;    // Passed in as Arg 8
        internal bool administrator = false; // Passed in as Arg 9
        internal DateTime[] expiration = new DateTime[3] {SmartParametersV2016.defaultDate,
                                                            SmartParametersV2016.defaultDate,
                                                            SmartParametersV2016.defaultDate
                                                            };
        internal DateTime LoginExpirationDate = SmartParametersV2016.defaultDate;

        internal List<SmartSwitchItem> myList = new List<SmartSwitchItem>();

        // 3 is the number of Cubefaces/Schemas

        internal List<SmartUsers.SmartView> ssList = new List<SmartUsers.SmartView>();
        internal int ssCount = 0;

        internal List<SmartProfile.Groups> mmListX = new List<SmartProfile.Groups>();

        internal double actualWidth = 0.0,
                        actualHeight = 0.0;
        internal double currentWidth = 0.0,
                        currentHeight = 0.0;
#if WINFORMS
        internal System.Drawing.Color originalBackgroundColour = Color.Transparent;   // For the Open/Close button
#endif
#if WPF  || WINUI || SMARTMAUI
        internal Brush originalBackgroundColour = new SolidColorBrush(Colors.Transparent);   // For the Open/Close button
#endif
#if ANDROIDX
        internal Color originalBackgroundColour0 = Color.Transparent;   // For the Open/Close button
        internal Color originalBackgroundColour25 = Color.Transparent;   // For the Open/Close button
        internal Color originalBackgroundColour50 = Color.Transparent;   // For the Open/Close button
        internal Color originalBackgroundColour75 = Color.Transparent;   // For the Open/Close button
#endif

        // These are Profile, Finance, Utility and Insurance
        internal bool[] cubeCode = new bool[4] { true, true, true, true };   // Because Remote is always there,
        internal bool localOnly = false;    // Default means we update to Remote                                                                      // but local might not be
        internal short screenCode = -1;

        //internal string errorMessage = "";
        internal string warningMessage = "";
        internal string pdfMessage = "";

#if WINFORMS
        internal RichTextBox ScrollViewer = new();
#endif
#if WPF  || WINUI || SMARTMAUI
        internal Border Border = new Border();
#endif
#if WPF  || WINUI
        internal ScrollViewer ScrollViewer = new ScrollViewer();
#endif
#if SMARTMAUI
        // This replaces ScrollViewer
        public ScrollView ScrollViewer { get; set; }
        public Label LogLabel { get; set; }
#endif
        public class AddressItem    // Used in both Finance and Utility
        {
            public string Content { get; set; }
            public string Value { get; set; }
            public override string ToString()
            {
                return Content;
            }
#if WINFORMS
            public Color Colour { get; set; }
#endif
#if WPF  || WINUI
            public Brush Colour { get; set; }
#endif
#if ANDROIDX
            public Android.Graphics.Color Colour { get; set; }
#endif
#if SMARTMAUI
            public Color Colour { get; set; }
#endif
        }

        public class CultureItem
        {
            public int CultureID { get; set; }
            public string Content { get; set; }

#if WINFORMS
            public Color Colour { get; set; }           // So Binding works
#endif
#if WPF  || WINUI
            public Brush Colour { get; set; }
#endif
#if ANDROIDX
            public Color Colour { get; set; }
#endif
#if SMARTMAUI
            public Color Colour { get; set; }
#endif
            public string Source { get; set; }
            public override string ToString()
            {
                return Content;
            }
        }

        public List<ImageItem> ImageList = new List<ImageItem>();

        private string errMessage = "";
        public string errorMessage
        {
            get
            {
                return errMessage;
            }
            set
            {
                if (errMessage != value)
                {
                    errMessage = value;
                    this.NotifyPropertyChanged(nameof(errorMessage));
                }
            }
        }
        private string ecbdateDefault = String.Empty;
        public string ECBDate
        {
            get
            {
                return ecbdateDefault;
            }
            set
            {
                if (ecbdateDefault != value)
                {
                    ecbdateDefault = value;
                    this.NotifyPropertyChanged(nameof(ECBDate));
                }
            }
        }

        private int ecbvalidDefault = 0;
        public int ECBValid
        {
            get
            {
                return ecbvalidDefault;
            }
            set
            {
                if (ecbvalidDefault != value)
                {
                    ecbvalidDefault = value;
                    this.NotifyPropertyChanged(nameof(ECBValid));
                }
            }
        }

        private string gbprateDefault = "";
        public string GBPRate
        {
            get
            {
                return gbprateDefault;
            }
            set
            {
                if (gbprateDefault != value)
                {
                    gbprateDefault = value;
                    //#if ANDROIDX
                    //// This is Ray's own 'Kludge City' implementation
                    //// of a kind of binding which is nevessary between
                    //// ourviewmodel and financeviewmodel which ISN'T
                    //// available - insofar as I can see - in the current
                    //// implementation of Android ...
                    //if (financeviewmodel != null)
                    //{
                    //    if (financeviewmodel.RBGBP != null)
                    //    {
                    //        financeviewmodel.RBGBP.Text = value;
                    //    }
                    //}
                    //#endif
                    this.NotifyPropertyChanged(nameof(GBPRate));
                }
            }
        }

        private string eurrateDefault = "";
        public string EURRate
        {
            get
            {
                return eurrateDefault;
            }
            set
            {
                if (eurrateDefault != value)
                {
                    eurrateDefault = value;
                    //#if ANDROIDX
                    //#if DEVELOPMENT
                    //                    // This is Ray's own 'Kludge City' implementation
                    //                    // of a kind of binding which is nevessary between
                    //                    // ourviewmodel and financeviewmodel which ISN'T
                    //                    // available - insofar as I can see - in the current
                    //                    // implementation of Android ...
                    //                    if (financeviewmodel != null)
                    //                    {
                    //                        if (financeviewmodel.RBEUR != null)
                    //                        {
                    //                            financeviewmodel.RBEUR.Text = value;
                    //                        }
                    //                    }
                    //#endif
                    //#endif
                    this.NotifyPropertyChanged(nameof(EURRate));
                }
            }
        }

        private string usdrateDefault = "";
        public string USDRate
        {
            get
            {
                return usdrateDefault;
            }
            set
            {
                if (usdrateDefault != value)
                {
                    usdrateDefault = value;
                    //#if ANDROIDX
                    //#if DEVELOPMENT
                    //                    // This is Ray's own 'Kludge City' implementation
                    //                    // of a kind of binding which is nevessary between
                    //                    // ourviewmodel and financeviewmodel which ISN'T
                    //                    // available - insofar as I can see - in the current
                    //                    // implementation of Android ...
                    //                    if (financeviewmodel != null)
                    //                    {
                    //                        if (financeviewmodel.RBUSD != null)
                    //                        {
                    //                            financeviewmodel.RBUSD.Text = value;
                    //                        }
                    //                    }
                    //#endif
                    //#endif
                    this.NotifyPropertyChanged(nameof(USDRate));
                }
            }
        }

        private string jpyrateDefault = "";
        public string JPYRate
        {
            get
            {
                return jpyrateDefault;
            }
            set
            {
                if (jpyrateDefault != value)
                {
                    jpyrateDefault = value;
                    //#if ANDROIDX
                    //#if DEVELOPMENT
                    //                    // This is Ray's own 'Kludge City' implementation
                    //                    // of a kind of binding which is nevessary between
                    //                    // ourviewmodel and financeviewmodel which ISN'T
                    //                    // available - insofar as I can see - in the current
                    //                    // implementation of Android ...
                    //                    if (financeviewmodel != null)
                    //                    {
                    //                        if (financeviewmodel.RBJPY != null)
                    //                        {
                    //                            financeviewmodel.RBJPY.Text = value;
                    //                        }
                    //                    }
                    //#endif
                    //#endif
                    this.NotifyPropertyChanged(nameof(JPYRate));
                }
            }
        }

        private List<SmartUsers.ExchangeRatesView> erDefault = new List<SmartUsers.ExchangeRatesView>();
        public List<SmartUsers.ExchangeRatesView> ExchangeRatesReduced
        {
            get
            {
                return erDefault;
            }
            set
            {
                if (erDefault != value)
                {
                    erDefault = value;
                    this.NotifyPropertyChanged(nameof(ExchangeRatesReduced));
                }
            }
        }

#if WPF  || WINUI || SMARTMAUI

        public ImageSource kesmall;
        public ImageSource KeasdonEnergySmall
        {
            get
            {
                return kesmall;
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        public ImageSource lightbulbicon;
        public ImageSource LightBulbIcon
        {
            get
            {
                return lightbulbicon;
            }
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        // No longer needed - hard-coded in

        //public ImageSource meterbackground;

        //public ImageSource MeterBackground
        //{
        //    get
        //    {
        //        return meterbackground;
        //    }
        //}
#endif

#if WPF  || WINUI
        public ImageSource metertop;
        public ImageSource MeterTop
        {
            get
            {
                return metertop;
            }
        }
#endif

#if WPF  || WINUI
        public ImageSource meterbottom;
        public ImageSource MeterBottom
        {
            get
            {
                return meterbottom;
            }
        }
#endif
#if SMARTMAUI
        internal Image MeterTop { get; set; }
      
        internal Image MeterBottom { get; set; }
        
#endif

        private string todaysdateDefault = "";
        public string TodaysDate
        {
            get
            {
                return todaysdateDefault;
            }
            set
            {
                if (todaysdateDefault != value)
                {
                    todaysdateDefault = value;
                    this.NotifyPropertyChanged(nameof(TodaysDate));
                }
            }
        }
#if WPF  || WINUI || SMARTMAUI
        private double widthDefault = 900.0;
        public double Width
        {
            get
            {
                return widthDefault;
            }
            set
            {
                if (widthDefault != value)
                {
                    widthDefault = value;
                    this.NotifyPropertyChanged(nameof(Width));
                }
            }
        }

        private double heightDefault = 700.0;
        public double Height
        {
            get
            {
                return heightDefault;
            }
            set
            {
                if (heightDefault != value)
                {
                    heightDefault = value;
                    this.NotifyPropertyChanged(nameof(Height));
                }
            }
        }
#endif
#if WINFORMS
        private string Bankingsite;
        public string BankingSite
        {
            get
            {
                return Bankingsite;
            }
            set
            {
                if (Bankingsite != value)
                {
                    Bankingsite = value;
                    this.NotifyPropertyChanged(nameof(BankingSite));
                }
            }
        }
#endif

        private bool metervisible = true;
        public bool MeterVisible
        {
            get
            {
                return metervisible;
            }
            set
            {
                if (metervisible != value)
                {
                    metervisible = value;
                    this.NotifyPropertyChanged(nameof(MeterVisible));
                }
            }
        }



#if WPF  || WINUI || SMARTMAUI
        private Visibility usernamevisibleDefault = Visibility.Visible;
        public Visibility UserNameVisible
        {
            get
            {
                return usernamevisibleDefault;
            }
            set
            {
                if (usernamevisibleDefault != value)
                {
                    usernamevisibleDefault = value;
                    this.NotifyPropertyChanged(nameof(UserNameVisible));
                }
            }
        }

#if WPF
        private Visibility usernamelistvisibleDefault = Visibility.Hidden;
#endif
#if WINUI || SMARTMAUI
        private Visibility usernamelistvisibleDefault = Visibility.Collapsed;
#endif
        public Visibility UserNameListVisible
        {
            get
            {
                return usernamelistvisibleDefault;
            }
            set
            {
                if (usernamelistvisibleDefault != value)
                {
                    usernamelistvisibleDefault = value;
                    this.NotifyPropertyChanged(nameof(UserNameListVisible));
                }
            }
        }
#endif

        private List<SmartProfile.Groups> usernameitem = new List<SmartProfile.Groups>();
        public List<SmartProfile.Groups> UserNamesList
        {
            get
            {
                return usernameitem;
            }
            set
            {
                if (usernameitem != value)
                {
                    usernameitem = value;
                    this.NotifyPropertyChanged(nameof(UserNamesList));
                }
            }
        }

        //internal string PassWord = "";
        //internal string PassWordHash = "";
        internal string PDEK = "",
                        FDEK = "",
                        UDEK = "";

#if WINFORMS
        private Color usernamecolour = Color.Black;
        public Color UserNameColour
#endif
#if WPF  || WINUI
        private Brush usernamecolour = new SolidColorBrush(Colors.Black);
        public Brush UserNameColour
#endif
#if ANDROIDX
        private Color usernamecolour = Color.Black;
        public Color UserNameColour
#endif
#if SMARTMAUI
        private Color usernamecolour = Colors.Black;
        public Color UserNameColour
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return usernamecolour;
            }
            set
            {
                if (usernamecolour != value)
                {
                    usernamecolour = value;
                    this.NotifyPropertyChanged(nameof(UserNameColour));
                }
            }
        }
#endif

#if WINFORMS
        private Color usernamebrushDefault = Color.Transparent;
        public Color UserNameBrush
#endif
#if WPF  || WINUI
        private Brush usernamebrushDefault = new SolidColorBrush(Colors.Transparent);
        public Brush UserNameBrush
#endif
#if ANDROIDX
        private Color usernamebrushDefault = Color.Transparent;
        public Color UserNameBrush
#endif
#if SMARTMAUI
        private Color usernamebrushDefault = Colors.Transparent;
        public Color UserNameBrush
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return usernamebrushDefault;
            }
            set
            {
                if (usernamebrushDefault != value)
                {
                    usernamebrushDefault = value;
                    this.NotifyPropertyChanged(nameof(UserNameBrush));
                }
            }
        }
#endif

#if WINFORMS
        public List<UserControl> MyControls = new List<UserControl>();

#endif
        // Kate Beckinstall < Kate BeckinSALE you numbskull
#if WPF  || WINUI || SMARTMAUI
        public int CurrentPosition
        { get; set; }

        public int PreviousPosition
        { get; set; }
#endif
#if ANDROIDX
        public int CurrentPosition
        { get; set; }

        public int PreviousPosition
        { get; set; }
#endif

        public List<SmartUsers.ConsumerViews> viewCollection = new List<SmartUsers.ConsumerViews>();

#if WINFORMS || WPF  || WINUI
        private UserControl mycontent = new UserControl();
        public UserControl MyContent
        {
            get
            {
                return mycontent;
            }
            set
            {
                mycontent = value;
                NotifyPropertyChanged(nameof(MyContent));
            }
        }
#endif

#if SMARTMAUI
        //private View mycontent = new View();
        private View mycontent;

        public View MyContent
        {
            get => mycontent;
            set
            {
                mycontent = value;
                NotifyPropertyChanged(nameof(MyContent));
            }
        }
#endif



#if WINUI || SMARTMAUI
        private bool carouselvisible = false;
        public bool CarouselVisible
        {
            get
            {
                return carouselvisible;
            }
            set
            {
                if (carouselvisible != value)
                {
                    carouselvisible = value;
                    this.NotifyPropertyChanged(nameof(CarouselVisible));
                }
            }
        }
#endif

#if WINFORMS
        private int metertopDefault = 0;
        public int MeterTopHeight
        {
            get
            {
                return metertopDefault;
            }
            set
            {
                if (metertopDefault != value)
                {
                    metertopDefault = value;
                    this.NotifyPropertyChanged(nameof(MeterTopHeight));
                }
            }
        }

        private int meterbottomDefault = 0;
        public int MeterBottomHeight
        {
            get
            {
                return meterbottomDefault;
            }
            set
            {
                if (meterbottomDefault != value)
                {
                    meterbottomDefault = value;
                    this.NotifyPropertyChanged(nameof(MeterBottomHeight));
                }
            }
        }
#endif

#if WINFORMS
        public ToolTip AutoSwitchToolTip { get; set; }
        public ToolTip OpenCloseToolTip { get; set; }
        public ToolTip QuitToolTip { get; set; }
#endif
#if WINFORMS
        public PictureBox meterTop = new PictureBox();
        public PictureBox meterBottom = new PictureBox();

        private bool panelvisible = true;
        public bool PanelVisibility
        {
            get
            {
                return panelvisible;
            }
            set
            {
                if (panelvisible != value)
                {
                    panelvisible = value;
                    this.NotifyPropertyChanged(nameof(PanelVisibility));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        private string lastloginstatusDefault = "";
        public string LastLoginStatusMessage
        {
            get
            {
                return lastloginstatusDefault;
            }
            set
            {
                if (lastloginstatusDefault != value)
                {
                    lastloginstatusDefault = value;
                    this.NotifyPropertyChanged(nameof(LastLoginStatusMessage));
                }
            }
        }

        private string accountstatusDefault = "";
        public string AccountStatusMessage
        {
            get
            {
                return accountstatusDefault;
            }
            set
            {
                if (accountstatusDefault != value)
                {
                    accountstatusDefault = value;
                    this.NotifyPropertyChanged(nameof(AccountStatusMessage));
                }
            }
        }

        private string withdrawndateDefault = "";
        public string WithdrawnDateMessage
        {
            get
            {
                return withdrawndateDefault;
            }
            set
            {
                if (withdrawndateDefault != value)
                {
                    withdrawndateDefault = value;
                    this.NotifyPropertyChanged(nameof(WithdrawnDateMessage));
                }
            }
        }

        private string confirmationDefault = "";
        public string Confirmation
        {
            get
            {
                return confirmationDefault;
            }
            set
            {
                if (confirmationDefault != value)
                {
                    confirmationDefault = value;
                    this.NotifyPropertyChanged(nameof(Confirmation));
                }
            }
        }

        private string reallyleaveDefault = "";
        public string ReallyLeave
        {
            get
            {
                return reallyleaveDefault;
            }
            set
            {
                if (reallyleaveDefault != value)
                {
                    reallyleaveDefault = value;
                    this.NotifyPropertyChanged(nameof(ReallyLeave));
                }
            }
        }

        private string yesDefault = "";
        public string Yes
        {
            get
            {
                return yesDefault;
            }
            set
            {
                if (yesDefault != value)
                {
                    yesDefault = value;
                    this.NotifyPropertyChanged(nameof(Yes));
                }
            }
        }

        private string noDefault = "";
        public string No
        {
            get
            {
                return noDefault;
            }
            set
            {
                if (noDefault != value)
                {
                    noDefault = value;
                    this.NotifyPropertyChanged(nameof(No));
                }
            }
        }
#endif

#if WINFORMS
        private Color opencloseDefault = Color.Green;
        public Color OpenCloseColour
#endif
#if WPF  || WINUI
        private Brush opencloseDefault = new SolidColorBrush(Colors.Green);
        public Brush OpenCloseColour
#endif
#if SMARTMAUI
        private Color opencloseDefault = Colors.Green;
        public Color OpenCloseColour
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        {
            get
            {
                return opencloseDefault;
            }
            set
            {
                if (opencloseDefault != value)
                {
                    opencloseDefault = value;
                    this.NotifyPropertyChanged(nameof(OpenCloseColour));
                }
            }
        }
#endif
#if ANDROIDX
        private Color opencloseDefault = Color.Green;
        public Color OpenCloseColour
        {
            get
            {
                return opencloseDefault;
            }
            set
            {
                if (opencloseDefault != value)
                {
                    opencloseDefault = value;
                    this.NotifyPropertyChanged(nameof(OpenCloseColour));
                }
            }
        }

        private Color autoswitch0Default;
        public Color AutoSwitch0
        {
            get
            {
                return autoswitch0Default;
            }
            set
            {
                if (autoswitch0Default != value)
                {
                    autoswitch0Default = value;
                    this.NotifyPropertyChanged(nameof(AutoSwitch0));
                }
            }
        }

        private Color autoswitch25Default;
        public Color AutoSwitch25
        {
            get
            {
                return autoswitch25Default;
            }
            set
            {
                if (autoswitch25Default != value)
                {
                    autoswitch25Default = value;
                    this.NotifyPropertyChanged(nameof(AutoSwitch25));
                }
            }
        }

        private Color autoswitch50Default;
        public Color AutoSwitch50
        {
            get
            {
                return autoswitch50Default;
            }
            set
            {
                if (autoswitch50Default != value)
                {
                    autoswitch50Default = value;
                    this.NotifyPropertyChanged(nameof(AutoSwitch50));
                }
            }
        }

        private Color autoswitch75Default;
        public Color AutoSwitch75
        {
            get
            {
                return autoswitch75Default;
            }
            set
            {
                if (autoswitch75Default != value)
                {
                    autoswitch75Default = value;
                    this.NotifyPropertyChanged(nameof(AutoSwitch75));
                }
            }
        }

        private Color openclosestartDefault;
        public Color OpenClose0
        {
            get
            {
                return openclosestartDefault;
            }
            set
            {
                if (openclosestartDefault != value)
                {
                    openclosestartDefault = value;
                    this.NotifyPropertyChanged(nameof(OpenClose0));
                }
            }
        }

        private Color openclosemiddleDefault;
        public Color OpenClose25
        {
            get
            {
                return openclosemiddleDefault;
            }
            set
            {
                if (openclosemiddleDefault != value)
                {
                    openclosemiddleDefault = value;
                    this.NotifyPropertyChanged(nameof(OpenClose25));
                }
            }
        }

        private Color openclose50Default;
        public Color OpenClose50
        {
            get
            {
                return openclose50Default;
            }
            set
            {
                if (openclose50Default != value)
                {
                    openclose50Default = value;
                    this.NotifyPropertyChanged(nameof(OpenClose50));
                }
            }
        }

        private Color opencloseendDefault;
        public Color OpenClose75
        {
            get
            {
                return opencloseendDefault;
            }
            set
            {
                if (opencloseendDefault != value)
                {
                    opencloseendDefault = value;
                    this.NotifyPropertyChanged(nameof(OpenClose75));
                }
            }
        }

        private Color quit0Default = Color.Yellow;
        public Color Quit0
        {
            get
            {
                return quit0Default;
            }
            set
            {
                if (quit0Default != value)
                {
                    quit0Default = value;
                    this.NotifyPropertyChanged(nameof(Quit0));
                }
            }
        }

        private Color quit33Default = Color.LightYellow;
        public Color Quit33
        {
            get
            {
                return quit33Default;
            }
            set
            {
                if (quit33Default != value)
                {
                    quit33Default = value;
                    this.NotifyPropertyChanged(nameof(Quit33));
                }
            }
        }

        private Color quit66Default = Color.Goldenrod;
        public Color Quit66
        {
            get
            {
                return quit66Default;
            }
            set
            {
                if (quit66Default != value)
                {
                    quit66Default = value;
                    this.NotifyPropertyChanged(nameof(Quit66));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        private string datetimenowDefault = "                             ";
        public string DateTimeNowMessage
        {
            get
            {
                return datetimenowDefault;
            }
            set
            {
                if (datetimenowDefault != value)
                {
                    datetimenowDefault = value;
                    this.NotifyPropertyChanged(nameof(DateTimeNowMessage));
                }
            }
        }
#endif

#if WINFORMS
        private string testtimenowDefault = "                             ";
        public string TestTimeNowMessage
        {
            get
            {
                return testtimenowDefault;
            }
            set
            {
                if (testtimenowDefault != value)
                {
                    testtimenowDefault = value;
                    this.NotifyPropertyChanged(nameof(TestTimeNowMessage));
                }
            }
        }
#endif

#if WINFORMS
        private Color led1Default = Color.White;
        public Color Led1
#endif
#if WPF  || WINUI
        private Brush led1Default = new SolidColorBrush(Colors.White);
        public Brush Led1
#endif
#if ANDROIDX
        private Color led1Default = Color.White;
        public Color Led1
#endif
#if SMARTMAUI
        private Color led1Default = Colors.White;
        public Color Led1
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return led1Default;
            }
            set
            {
                if (led1Default != value)
                {
                    led1Default = value;
                    this.NotifyPropertyChanged(nameof(Led1));
                 }
            }
        }
#endif

#if WINFORMS
        private Color led2Default = Color.White;
        public Color Led2
#endif
#if WPF  || WINUI
        private Brush led2Default = new SolidColorBrush(Colors.White);
        public Brush Led2
#endif
#if ANDROIDX
        private Color led2Default = Color.White;
        public Color Led2
#endif
#if SMARTMAUI
        private Color led2Default = Colors.White;
        public Color Led2
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI

        {
            get
            {
                return led2Default;
            }
            set
            {
                if (led2Default != value)
                {
                    led2Default = value;
                    this.NotifyPropertyChanged(nameof(Led2));
                }
            }
        }
#endif

#if WINFORMS
        private Color led3Default = Color.White;
        public Color Led3
#endif
#if WPF  || WINUI
        private Brush led3Default = new SolidColorBrush(Colors.White);
        public Brush Led3
#endif
#if ANDROIDX
        private Color led3Default = Color.White;
        public Color Led3
#endif
#if SMARTMAUI
        private Color led3Default = Colors.White;
        public Color Led3
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI

        {
            get
            {
                return led3Default;
            }
            set
            {
                if (led3Default != value)
                {
                    led3Default = value;
                    this.NotifyPropertyChanged(nameof(Led3));
                }
            }
        }
#endif

#if WINFORMS
        private Color led4Default = Color.White;
        public Color Led4
#endif
#if WPF  || WINUI
        private Brush led4Default = new SolidColorBrush(Colors.White);
        public Brush Led4
#endif
#if ANDROIDX
        private Color led4Default = Color.White;
        public Color Led4
#endif
#if SMARTMAUI
        private Color led4Default = Colors.White;
        public Color Led4
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI

        {
            get
            {
                return led4Default;
            }
            set
            {
                if (led4Default != value)
                {
                    led4Default = value;
                    this.NotifyPropertyChanged(nameof(Led4));
                }
            }
        }
#endif

#if WINFORMS
        private Color led5Default = Color.White;
        public Color Led5
#endif
#if WPF  || WINUI
        private Brush led5Default = new SolidColorBrush(Colors.White);
        public Brush Led5
#endif
#if ANDROIDX
        private Color led5Default = Color.White;
        public Color Led5
#endif
#if SMARTMAUI
        private Color led5Default = Colors.White;
        public Color Led5
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return led5Default;
            }
            set
            {
                if (led5Default != value)
                {
                    led5Default = value;
                    this.NotifyPropertyChanged(nameof(Led5));
                }
            }
        }
#endif

#if WINFORMS
        private Color led6Default = Color.White;
        public Color Led6
#endif
#if WPF  || WINUI
        private Brush led6Default = new SolidColorBrush(Colors.White);
        public Brush Led6
#endif
#if ANDROIDX
        private Color led6Default = Color.White;
        public Color Led6
#endif
#if SMARTMAUI
        private Color led6Default = Colors.White;
        public Color Led6
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI

        {
            get
            {
                return led6Default;
            }
            set
            {
                if (led6Default != value)
                {
                    led6Default = value;
                    this.NotifyPropertyChanged(nameof(Led6));
                }
            }
        }
#endif

#if WINFORMS
        private Color led7Default = Color.White;
        public Color Led7
#endif
#if WPF  || WINUI
        private Brush led7Default = new SolidColorBrush(Colors.White);
        public Brush Led7
#endif
#if ANDROIDX
        private Color led7Default = Color.White;
        public Color Led7
#endif
#if SMARTMAUI
        private Color led7Default = Colors.White;
        public Color Led7
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI

        {
            get
            {
                return led7Default;
            }
            set
            {
                if (led7Default != value)
                {
                    led7Default = value;
                    this.NotifyPropertyChanged(nameof(Led7));
                }
            }
        }
#endif

#if WINFORMS
        private Color led8Default = Color.White;
        public Color Led8
#endif
#if WPF  || WINUI
        private Brush led8Default = new SolidColorBrush(Colors.White);
        public Brush Led8
#endif
#if ANDROIDX
        private Color led8Default = Color.White;
        public Color Led8
#endif
#if SMARTMAUI
        private Color led8Default = Colors.White;
        public Color Led8
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI

        {
            get
            {
                return led8Default;
            }
            set
            {
                if (led8Default != value)
                {
                    led8Default = value;
                    this.NotifyPropertyChanged(nameof(Led8));
                }
            }
        }
#endif

#if WINFORMS
        private Color quitcolorDefault = Color.Transparent;
        public Color QuitColour
#endif
#if WPF  || WINUI
        private Brush quitcolorDefault = new SolidColorBrush(Colors.Transparent);
        public Brush QuitColour
#endif
#if ANDROIDX
        private Color quitcolorDefault = Color.Transparent;
        public Color QuitColour
#endif
#if SMARTMAUI
        private Color quitcolorDefault = Colors.Transparent;
        public Color QuitColour

#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return quitcolorDefault;
            }
            set
            {
                if (quitcolorDefault != value)
                {
                    quitcolorDefault = value;
                    this.NotifyPropertyChanged(nameof(QuitColour));
                }
            }
        }
#endif

#if WINFORMS
        private bool receivevisibilityDefault = false;
        public bool ReceiveVisibility
#endif
#if WPF
        private Visibility receivevisibilityDefault = Visibility.Hidden;
        public Visibility ReceiveVisibility
#endif
#if WINUI || SMARTMAUI
        private bool receivevisibilityDefault = false;
        public bool ReceiveVisibility
#endif
#if ANDROIDX
        private bool receivevisibilityDefault = false;
        public bool ReceiveVisibility
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return receivevisibilityDefault;
            }
            set
            {
                if (receivevisibilityDefault != value)
                {
                    receivevisibilityDefault = value;
                    this.NotifyPropertyChanged(nameof(ReceiveVisibility));
                }
            }
        }

        internal bool multimeteryeschosen = false;
        internal bool quityeschosen = false;

        private bool quitenabledDefault;
        public bool QuitEnabled
        {
            get
            {
                return quitenabledDefault;
            }
            set
            {
                if (quitenabledDefault != value)
                {
                    quitenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(QuitEnabled));
                }
            }
        }

        private bool opencloseenabledDefault;
        public bool OpenCloseEnabled
        {
            get
            {
                return opencloseenabledDefault;
            }
            set
            {
                if (opencloseenabledDefault != value)
                {
                    opencloseenabledDefault = value;
                    this.NotifyPropertyChanged(nameof(OpenCloseEnabled));
                }
            }
        }

        private bool openclosebuttonfocusDefault = false;
        public bool OpenCloseButtonFocus
        {
            get
            {
                return openclosebuttonfocusDefault;
            }
            set
            {
                if (openclosebuttonfocusDefault != value)
                {
                    openclosebuttonfocusDefault = value;
                    this.NotifyPropertyChanged(nameof(OpenCloseButtonFocus));
                }
            }
        }
#endif

#if WINFORMS
        private System.Windows.Forms.RichTextBox consoleDefault;
        public System.Windows.Forms.RichTextBox Console
        {
            get
            {
                return consoleDefault;
            }
            set
            {
                if (consoleDefault != value)
                {
                    consoleDefault = value;
                    this.NotifyPropertyChanged(nameof(Console));
                }
            }
        }
#endif

#if WINFORMS
        private bool borderVisibleDefault = false;
        public bool BorderVisible
#endif
#if WPF
        private Visibility borderVisibleDefault = Visibility.Hidden;
#endif
#if WINUI || SMARTMAUI
        private Visibility borderVisibleDefault = Visibility.Collapsed;
#endif
#if WPF  || WINUI || SMARTMAUI
        public Visibility BorderVisible
#endif
#if ANDROIDX
        private bool borderVisibleDefault = false;
        public bool BorderVisible
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {
                return borderVisibleDefault;
            }
            set
            {
                if (borderVisibleDefault != value)
                {
                    borderVisibleDefault = value;
                    this.NotifyPropertyChanged(nameof(BorderVisible));
                }
            }
        }
#endif

#if WINFORMS
        private bool scrollviewerDefault = false;
        public bool ScrollViewerVisible
#endif
#if WPF
        private Visibility scrollviewerDefault = Visibility.Hidden;
#endif
#if WINUI || SMARTMAUI
        private Visibility scrollviewerDefault = Visibility.Collapsed;
#endif
#if WPF  || WINUI || SMARTMAUI
        public Visibility ScrollViewerVisible
#endif
#if ANDROIDX
        private bool scrollviewerDefault = false;
        public bool ScrollViewerVisible
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            get
            {

                return scrollviewerDefault;
            }
            set
            {
                if (scrollviewerDefault != value)
                {
                    scrollviewerDefault = value;
                    this.NotifyPropertyChanged(nameof(ScrollViewerVisible));
                }
            }
        }
#endif


#if WPF  || WINUI
        private TextBlock textBlockContentDefault = new TextBlock();
        public TextBlock TextBlockContent
#endif
#if ANDROIDX
        // Its in as TextBlockContent further up!
#endif
#if SMARTMAUI
        private string textBlockContentDefault = "";
        public string TextBlockContent
#endif
#if WPF  || WINUI || SMARTMAUI
        {
            get
            {
                return textBlockContentDefault;
            }
            set
            {
                if (textBlockContentDefault != value)
                {
                    textBlockContentDefault = value;
                    this.NotifyPropertyChanged(nameof(TextBlockContent));
                }
            }
        }
        
#endif

#if WPF  || WINUI
        public TextBlock textBoxBrowserFinance = new TextBlock();
#endif

#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        private string textboxbrowserfinance = "";
        public string TextBoxBrowserFinance
        {
            get
            {
                return textboxbrowserfinance;
            }
            set
            {
                if (textboxbrowserfinance != value)
                {
                    textboxbrowserfinance = value;
                    this.NotifyPropertyChanged(nameof(TextBoxBrowserFinance));
                }
            }
        }
#endif

#if WPF  || WINUI
        public TextBlock textBoxBrowserUtility = new TextBlock();
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        private string textboxbrowserutility = "";
        public string TextBoxBrowserUtility
        {
            get
            {
                return textboxbrowserutility;
            }
            set
            {
                if (textboxbrowserutility != value)
                {
                    textboxbrowserutility = value;
                    this.NotifyPropertyChanged(nameof(TextBoxBrowserUtility));
                }
            }
        }
#endif

#if WINUI || SMARTMAUI
        internal double topheight = 0;
        internal double bottomheight = 0;
#endif
#if ANDROIDX
        internal double topheight = 0;
        internal double bottomheight = 0;
#endif

        //
        // Now this ? MUST include the Package this WINUI is built under; don't forget that
        // because your WINUI app is 'closed', it has no access to any storage area or
        // files OUTSIDE of it's Package.  So you can create a database INSIDE the Package
        // but you can't create one outside in say 'MyDocuments' or 'C:\Users\Ray\AppData\SmartCube'
        // You can only create one INSIDE e.g on a path like this:
        // "C:\Users\Ray\AppData\Local\Packages\0b594570-a584-40c3-8cf4-79b52c21723e|st3526g18r730\LocalState\SmartCube"
        //
        // AND this same bollocks applies to Android devices as well .. you can
        // create files inside the APK but not outside.  See the Notes in Things Done
        // regarding the Local storage and my attempts to 'see' the SQLite database
        // ... what a pile of excrement this is for Developers...

        internal SQLiteAsyncConnection sqliteDatabase = new SQLiteAsyncConnection("");
        internal SQLiteAsyncConnection remotesqliteDatabase = new SQLiteAsyncConnection("");
        internal string fullfilepath = "";
        internal SQLite.SQLiteOpenFlags CreateFlags =
                    // Open the database in read/write mode
                    SQLite.SQLiteOpenFlags.ReadWrite |
                    // Create the database if it doesn't exist
                    SQLite.SQLiteOpenFlags.Create;// Enable multi-threaded database access

        internal SQLite.SQLiteOpenFlags ReadWriteFlags =
                                    // Open the database in read/write mode 
                                    SQLite.SQLiteOpenFlags.ReadWrite;

        internal string Platform = "";
        public string DataBasePath
        {
            get
            {
                string basePath = "";
                // DON'T forget this!!!! (You dork)
#if WINFORMS
                basePath = System.IO.Path.Combine(@"C:\Users\Ray\Documents\Visual Studio 2022\Projects\SmartDBServerV2023",
                                                SmartParametersV2016.SmartSwitchFilename);
#else

                switch (Platform)
                {
                    case SmartParametersV2016.WPF:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                    case SmartParametersV2016.WinUI:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                    case SmartParametersV2016.Android:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                    case SmartParametersV2016.WINFORMS:
                        // Winforms?
                        //basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        basePath = System.IO.Path.Combine(@"C:\Users\Ray\Documents\Visual Studio 2022\Projects\SmartDBServerV2023",
                                                    SmartParametersV2016.SmartSwitchFilename);


                        break;
                    case SmartParametersV2016.MacCatalyst:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                    case SmartParametersV2016.Tizen:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                    default:    // DBSERVER
                        var exePath = AppContext.BaseDirectory;
                        var projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(exePath, @"..\..\..\"));
                        basePath = projectPath;
                        break;
                }
#endif
                return basePath;
            }
        }

        // Here begins the SQLite strings for encrypted tables, and here they are:
        // SmartUsers.Details   (always stored locally so no need to encrypt)
        // SmartFinance.Accounts
        // SmartFinance.Logins
        // SmartFinance.Switches
        // SmartUtility.Accounts
        // SmartUtility.BankDetails
        // SmartUtility.Logins
        // SmartFinance.Transactions
        // SmartFinance.TransactionsCategories

        // Note that we only REALLY need to keep SmartFinance.Logins and SmartUtility.Logins
        // LOCALLY as they contain the connection data; all the others can be stored REMOTELY
        // (and yes, they are encrypted as you can see)


        // Here ENDS the SQLite statements for encrypted tables
        // All of this stuff below can be commented out ...
        // (We'll see, eh, Ray???)
    }
}