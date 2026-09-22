using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
using Microsoft.DotNet.PlatformAbstractions;
using Xamarin.Essentials;

namespace SmartCubeMobile
{

    //[Activity(MainLauncher = true)]
    //public class SignInActivity : AppCompatActivity
    //{

    //    public SignInActivity Instance { get; private set; }

    //    protected override void OnCreate(Bundle savedInstanceState)
    //    {
    //        base.OnCreate(savedInstanceState);
    //        SetContentView(Resource.Layout.SignInView);

    //        Instance = this;
    //        // ? Removed OverridePendingTransition (deprecated)






    //        // Continue normal setup
    //        DevicePlatform platform = DeviceInfo.Platform;
    //        string version = this.ApplicationContext
    //            .PackageManager
    //            .GetPackageInfo(this.ApplicationContext.PackageName, 0)
    //            .VersionName;

    //        new SignIn(this, this, platform.ToString(), version);
    //    }
    //    protected override void OnResume()
    //    {
    //        base.OnResume();
    //    }

    //    protected override void OnPause()
    //    {
    //        base.OnPause();
    //    }        
    //}
    public class AppSession
    {
        private static AppSession _instance;
        public static AppSession Instance => _instance ??= new AppSession();
        public SignInViewModel signInVM { get; set; }
        
        // ADD THIS
        public TickScheduler Scheduler { get; set; }

        // AND THIS (for your task)
        public SmartClockTask ClockTask { get; set; }
    }
    [Activity(MainLauncher = true)]
    public class SignInActivity : AppCompatActivity
    {
        private SignInViewModel signinviewmodel;
        private TextView TimeOffsetMessageText;

        public SignInActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            signinviewmodel = new SignInViewModel();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignInView);

            Instance = this;

            // Get your TextView
            TimeOffsetMessageText = FindViewById<TextView>(Resource.Id.TimeOffsetMessage);

            // Continue normal setup
            DevicePlatform platform = DeviceInfo.Platform;

            string version = this.ApplicationContext
                .PackageManager
                .GetPackageInfo(this.ApplicationContext.PackageName, 0)
                .VersionName;

            // ? CREATE IT FIRST
            signinviewmodel = new SignInViewModel();
            AppSession.Instance.signInVM = signinviewmodel;

            // Ensure ONE scheduler only
            if (AppSession.Instance.Scheduler == null)
            {
                AppSession.Instance.Scheduler = new TickScheduler();
            }
            
            // Create your SignIn logic (this should create the ViewModel)
            SignIn signIn = new SignIn(this, this, platform.ToString(), version, AppSession.Instance.Scheduler, signinviewmodel);

            
            if (AppSession.Instance.ClockTask == null)
            {
                var task = new SmartClockTask(
                    this,
                    signinviewmodel,
                    AppSession.Instance.Scheduler,
                    TimeOffsetMessageText);

                AppSession.Instance.ClockTask = task;

                AppSession.Instance.Scheduler.Register(task);
            }

            // ? Start ONCE only
            AppSession.Instance.Scheduler.Start();
        }

        protected override void OnResume()
        {
            base.OnResume();

            // IMPORTANT: set UI target when screen becomes active
            AppSession.Instance.ClockTask?.SetUiTarget(this, TimeOffsetMessageText, signinviewmodel);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }
    }
}