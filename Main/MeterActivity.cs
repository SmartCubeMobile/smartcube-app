using System.Diagnostics;
using AndroidX.AppCompat.App;
using static JetBrains.Annotations.Async;

namespace SmartCubeMobile
{    
    [Activity]
    public class MeterActivity : AppCompatActivity
    {
        private MainViewModel ourviewmodel;
        private TextView MeterTimeText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MeterView);

            // Get what we need            
            ourviewmodel = new MainViewModel();
            // IMPORTANT: get your TextView from layout
            MeterTimeText = FindViewById<TextView>(Resource.Id.MeterTimeText);

            // Your existing logic
            new MainMeter(
                this,
                this,
                SupportFragmentManager,
                Lifecycle,
                AppSession.Instance.signInVM,
                ourviewmodel,
                AppSession.Instance.Scheduler
            );
            // ✅ CRITICAL: retarget the existing clock to THIS screen
            AppSession.Instance.ClockTask?.SetUiTarget(this, MeterTimeText, ourviewmodel);
        }

        protected override void OnStart()
        {
            base.OnStart();
            // ✅ SAFETY: Android may recreate UI, so reattach here too
            AppSession.Instance.ClockTask?.SetUiTarget(this, MeterTimeText, ourviewmodel);
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Extra safety (optional but recommended)
            AppSession.Instance.ClockTask?.SetUiTarget(this, MeterTimeText, ourviewmodel);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }
    }
}