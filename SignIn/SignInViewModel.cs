using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Collections.Generic;
using System;


#if WINFORMS
using System.Drawing;
using System.Collections.Generic;
#endif

#if WPF
using System.Windows.Media;
using System.Windows.Threading;
#endif

#if WINUI
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
#endif

#if ANDROIDX
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
#endif

#if SMARTMAUI
#endif

namespace SmartCubeMobile
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string errormessageDefault = "";
        public string errorMessage
        {
            get
            {
                return errormessageDefault;
            }
            set
            {
                if (errormessageDefault != value)
                {
                    errormessageDefault = value;
                    NotifyPropertyChanged(nameof(errorMessage));
                }
            }
        }

#if !DBSERVER
        internal List<LanguageDictionary> languageDictionaries
                    = new List<LanguageDictionary>();
#endif

        internal Dictionary<string, string> resdict = new Dictionary<string, string>();

        internal string languageTag = "";
        internal ResourceManager resourceMan = new ResourceManager("SmartCubeMobileV2023.Resources", Assembly.GetExecutingAssembly());

        internal TimeSpan utcOffset = SmartParametersV2016.utcdefaultOffset;

        internal DateTime localTime = SmartParametersV2016.defaultDate;
        internal CultureInfo signincultureinfo = SmartParametersV2016.defaultCulture;

        //#if PRODUCTION
        //        internal string website = SmartParametersV2016.website;  // Different project
        //#Xelse
        internal string website = SmartParametersV2016.localWebsite;   // Than this
                                                                       //#endif
        internal Uri listenerUrl = new Uri(SmartParametersV2016.localWebsite);
        internal Uri loadtableUrl = new Uri(SmartParametersV2016.localWebsite);

        internal string antiTokenString = "";
        internal System.Net.CookieContainer cookies = new System.Net.CookieContainer();

        // Don't try and initialize this fucker:
        // If you DO, then expect the Chimps to pull the rug out
        // from under your feet and for the progeam to crash miserably..
        // *You have been warned*
        internal Uri TargetUrl { get; set; }// = new Uri("dummy");

        internal bool UserIsLoggedIn = false;
        internal LoginKeys LoginKeys = new LoginKeys();

        internal TimeSpan timespanTimeout = SmartParametersV2016.timespanTimeout;        // One minute
        internal bool requestTimedOut = false;

        internal bool jsonReturned = false;

        internal Potential loggedInUser = new Potential();
        internal SmartMumList Fatah = new SmartMumList();

        internal int screensCount = 0;

#if WINFORMS
        internal string PasswordHash = "";    // Only for WINFORMS!!

        public Color greenColour = Color.Green;
        public Color orangeColour = Color.Orange;
        public Color redColour = Color.Red;
        public Color blackColour = Color.Black;
#endif

#if WPF  || WINUI
        public Brush greenColour = new SolidColorBrush(Colors.Green);
        public Brush orangeColour = new SolidColorBrush(Colors.Orange);
        public Brush redColour = new SolidColorBrush(Colors.Red);
        public Brush blackColour = new SolidColorBrush(Colors.Black);
#endif

#if ANDROIDX
        public Android.Content.Intent ourintent;
        public Android.Graphics.Color greenColour = Android.Graphics.Color.Green;
        public Android.Graphics.Color orangeColour = Android.Graphics.Color.Orange;
        public Android.Graphics.Color redColour = Android.Graphics.Color.Red;
        public Android.Graphics.Color blackColour = Android.Graphics.Color.Black;
#endif
#if SMARTMAUI
        internal string PasswordHash = "";    // Only for WINFORMS!!

        public Color greenColour = Colors.Green;
        public Color orangeColour = Colors.Orange;
        public Color redColour = Colors.Red;
        public Color blackColour = Colors.Black;
#endif
#if WPF  || WINUI
        public ImageSource kesmall;
        public ImageSource KeasdonEnergySmall
        {
            get => kesmall;
            set
            {
                if (kesmall != value)
                {
                    kesmall = value;
                    NotifyPropertyChanged(nameof(KeasdonEnergySmall));
                }
            }
        }
#endif
#if SMARTMAUI

        internal ImageSource kesmall;

        public ImageSource KeasdonEnergySmall
        {
            get => kesmall;
            set
            {
                if (kesmall != value)
                {
                    kesmall = value;
                    NotifyPropertyChanged(nameof(KeasdonEnergySmall));
                }
            }
        }
#endif
#if WPF  || WINUI
        public ImageSource lightbulbicon;
        public ImageSource LightBulbIcon
        {
            get => lightbulbicon;
            set
            {
                if (lightbulbicon != value)
                {
                    lightbulbicon = value;
                    NotifyPropertyChanged(nameof(LightBulbIcon));
                }
            }
        }
#endif
#if SMARTMAUI
        public ImageSource lightbulbicon;
        public ImageSource LightBulbIcon
        {
            get => lightbulbicon;
            set
            {
                if (lightbulbicon != value)
                {
                    lightbulbicon = value;
                    NotifyPropertyChanged(nameof(LightBulbIcon));
                }
            }
        }
#endif

        private bool resetvisibility = false;
        public bool ResetVisibility
        {
            get
            {
                return resetvisibility;
            }
            set
            {
                if (resetvisibility != value)
                {
                    resetvisibility = value;
                    NotifyPropertyChanged(nameof(ResetVisibility));
                }
            }
        }

        private string username = "";
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (username != value)
                {
                    username = value;
                    NotifyPropertyChanged(nameof(Username));
                }
            }
        }

        private string signincultureDefault = "";
        public string SignInCulture
        {
            get
            {
                return signincultureDefault;
            }
            set
            {
                if (signincultureDefault != value)
                {
                    signincultureDefault = value;

                    NotifyPropertyChanged(nameof(SignInCulture));
                }
            }
        }

        // No Password, so its not stored in memory ...
        private string versionDefault = "";
        public string VersionMessage
        {
            get
            {
                return versionDefault;
            }
            set
            {
                if (versionDefault != value)
                {
                    versionDefault = "Version: " + value;
                    NotifyPropertyChanged(nameof(VersionMessage));
                }
            }
        }

        private string ossystemDefault = "";
        public string Platform
        {
            get
            {
                return ossystemDefault;
            }
            set
            {
                if (ossystemDefault != value)
                {
                    ossystemDefault = value;
                    NotifyPropertyChanged(nameof(Platform));
                }
            }
        }

        private string resetUTCDefault = "";
        public string ResetUTC
        {
            get
            {
                return resetUTCDefault;
            }
            set
            {
                if (resetUTCDefault != value)
                {
                    resetUTCDefault = value;
                    NotifyPropertyChanged(nameof(ResetUTC));
                }
            }
        }

        private string signinDefault = "";
        public string Signin
        {
            get
            {
                return signinDefault;
            }
            set
            {
                if (signinDefault != value)
                {
                    signinDefault = value;
                    NotifyPropertyChanged(nameof(Signin));
                }
            }
        }

        private string createaccountDefault = "";
        public string CreateAccount
        {
            get
            {
                return createaccountDefault;
            }
            set
            {
                if (createaccountDefault != value)
                {
                    createaccountDefault = value;
                    NotifyPropertyChanged(nameof(CreateAccount));
                }
            }
        }
        private string usernameboxDefault = "Username";
        public string UsernameBox
        {
            get
            {
                return usernameboxDefault;
            }
            set
            {
                if (usernameboxDefault != value)
                {
                    usernameboxDefault = value;
                    NotifyPropertyChanged(nameof(UsernameBox));
                }
            }
        }

        private string passwordboxDefault = "Password";
        public string PasswordBox
        {
            get
            {
                return passwordboxDefault;
            }
            set
            {
                if (passwordboxDefault != value)
                {
                    passwordboxDefault = value;
                    NotifyPropertyChanged(nameof(PasswordBox));
                }
            }
        }

        private string loginstatusmessageDefault = "";
        public string LoginStatusMessage
        {
            get
            {
                return loginstatusmessageDefault;
            }
            set
            {
                if (loginstatusmessageDefault != value)
                {
                    loginstatusmessageDefault = value;
                    NotifyPropertyChanged(nameof(LoginStatusMessage));
                }
            }
        }

        private string datetimenowmessage = "";
        public string DateTimeNowMessage
        {
            get
            {
                return datetimenowmessage;
            }
            set
            {
                if (datetimenowmessage != value)
                {
                    datetimenowmessage = value;
                    NotifyPropertyChanged(nameof(DateTimeNowMessage));
                }
            }
        }

        private bool signinenabledDefault = true;
        public bool SignInEnabled
        {
            get
            {
                return signinenabledDefault;
            }
            set
            {
                if (signinenabledDefault != value)
                {
                    signinenabledDefault = value;
                    NotifyPropertyChanged(nameof(SignInEnabled));
                }
            }
        }


#if WINFORMS
        private Color led1Default = Color.Orange;
        public Color Led1
#endif
#if WPF  || WINUI
        private Brush led1Default = new SolidColorBrush(Colors.Orange);
        public Brush Led1
#endif
#if ANDROIDX
        private Android.Graphics.Color led1Default = Android.Graphics.Color.Orange;
        public Android.Graphics.Color Led1
#endif
#if SMARTMAUI
        private Color led1Default = Colors.Orange;
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
                    NotifyPropertyChanged(nameof(Led1));
                }
            }
        }
#endif

#if WINFORMS
        private Color led2Default = Color.Orange;
        public Color Led2
#endif
#if WPF  || WINUI
        private Brush led2Default = new SolidColorBrush(Colors.Orange);
        public Brush Led2
#endif
#if ANDROIDX
        private Android.Graphics.Color led2Default = Android.Graphics.Color.Orange;
        public Android.Graphics.Color Led2
#endif
#if SMARTMAUI
        private Color led2Default = Colors.Orange;
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
                    NotifyPropertyChanged(nameof(Led2));
                }
            }
        }
#endif

#if WINFORMS
        private Color led3Default = Color.Orange;
        public Color Led3
#endif
#if WPF  || WINUI
        private Brush led3Default = new SolidColorBrush(Colors.Orange);
        public Brush Led3
#endif
#if ANDROIDX
        private Android.Graphics.Color led3Default = Android.Graphics.Color.Orange;
        public Android.Graphics.Color Led3
#endif
#if SMARTMAUI
        private Color led3Default = Colors.Orange;
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
                    NotifyPropertyChanged(nameof(Led3));
                }
            }
        }
#endif

#if WINFORMS
        private Color led4Default = Color.Orange;
        public Color Led4
#endif
#if WPF  || WINUI
        private Brush led4Default = new SolidColorBrush(Colors.Orange);
        public Brush Led4
#endif
#if ANDROIDX
        private Android.Graphics.Color led4Default = Android.Graphics.Color.Orange;
        public Android.Graphics.Color Led4
#endif
#if SMARTMAUI
        private Color led4Default = Colors.Orange;
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
                    NotifyPropertyChanged(nameof(Led4));
                }
            }
        }
#endif

#if WINFORMS
        private Color led5Default = Color.Orange;
        public Color Led5
#endif
#if WPF  || WINUI
        private Brush led5Default = new SolidColorBrush(Colors.Orange);
        public Brush Led5
#endif
#if ANDROIDX
        private Android.Graphics.Color led5Default = Android.Graphics.Color.Orange;
        public Android.Graphics.Color Led5
#endif
#if SMARTMAUI
        private Color led5Default = Colors.Orange;
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
                    NotifyPropertyChanged(nameof(Led5));
                }
            }
        }
#endif

#if WINFORMS
        private Color led6Default = Color.Orange;
        public Color Led6
#endif
#if WPF  || WINUI
        private Brush led6Default = new SolidColorBrush(Colors.Orange);
        public Brush Led6
#endif
#if ANDROIDX
        private Android.Graphics.Color led6Default = Android.Graphics.Color.Orange;
        public Android.Graphics.Color Led6
#endif
#if SMARTMAUI
        private Color led6Default = Colors.Orange;
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
                    NotifyPropertyChanged(nameof(Led6));
                }
            }
        }
#endif
    }
}