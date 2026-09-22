using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCubeMobile
{
    internal class SignInAndroidViewModel
    {
        internal Context context { get; set; }
        internal Activity signinActivity { get; set; }
        internal ImageButton English { get; set; }
        internal ImageButton French { get; set; }
        internal ImageButton German { get; set; }
        internal ImageButton Spanish { get; set; }
        internal Button ResetGMT { get; set; }
        internal TextView TimeOffsetMessage { get; set; }
        internal TextView LoginStatusMessage { get; set; }

        // Big Three
        internal EditText UserName { get; set; }
        internal EditText PassWord { get; set; }
        internal Button LoginButton { get; set; }
        // End Big Three

        internal TextView ErrorMessage { get; set; }
        internal TextView CreateAccount { get; set; }
        internal TextView SignInCulture { get; set; }

        internal TextView Version { get; set; }
        internal TextView Platform { get; set; }
        internal TextView LoginLed1 { get; set; }
        internal TextView LoginLed2 { get; set; }
        internal TextView LoginLed3 { get; set; }
        internal TextView LoginLed4 { get; set; }
        internal TextView LoginLed5 { get; set; }
        internal TextView LoginLed6 { get; set; }
        //internal Intent nextIntent { get; set; }
    }

    internal class MainMeterAndroidViewModel
    {
        internal Activity ourActivity { get; set; }

        // Main Meter
        internal ScrollView ScrollViewer { get; set; }
        internal TextView TextBlockBrowser { get; set; }

        internal Button OpenClose { get; set; }
        internal TextView DateTimeNow { get; set; }
        internal TextView Username { get; set; }

        internal FrameLayout MyContent { get; set; }
        internal ImageView MeterTop { get; set; }
        internal ImageView MeterBottom { get; set; }

        internal TextView Led1 { get; set; }
        internal TextView Led2 { get; set; }
        internal TextView Led3 { get; set; }
        internal TextView Led4 { get; set; }
        internal TextView Led5 { get; set; }
        internal TextView Led6 { get; set; }
        internal TextView Led7 { get; set; }
        internal TextView Led8 { get; set; }

        internal Button Quit { get; set; }

        internal Dialog PopupDialog { get; set; }
        internal TextView Confirmation { get; set; }
        internal TextView ReallyLeave { get; set; }
        internal TextView QuitYes { get; set; }
        internal TextView QuitNo { get; set; }

        internal Button ProfileSave { get; set; }

        internal Adapter fuckingadapter { get; set; }

    }
}