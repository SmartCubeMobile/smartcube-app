using Android.Content;
using Xamarin.Essentials;
using AndroidX.AppCompat.App;

namespace SmartCubeMobile
{
    
    [Activity(MainLauncher = true)]
    public class SignInActivity : AppCompatActivity
    {
        public SignInActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignInView);

            Instance = this;
            // ? Removed OverridePendingTransition (deprecated)

            // Continue normal setup
            DevicePlatform platform = DeviceInfo.Platform;
            string version = this.ApplicationContext
                .PackageManager
                .GetPackageInfo(this.ApplicationContext.PackageName, 0)
                .VersionName;

            new SignIn(this, this, platform.ToString(), version);
        }

        //public void NavigateToMeterActivity()
        //{
        //    var intent = new Intent(this, typeof(MeterActivity));

        //    var options = ActivityOptions.MakeCustomAnimation(
        //        this,
        //        Resource.Animation.abc_fade_in,
        //        Resource.Animation.abc_fade_out
        //    );

        //    StartActivity(intent, options.ToBundle());
        //    //Finish(); // Close SignInActivity
        //}

        protected override void OnResume()
        {
            base.OnStart();
        }
    }
}