//using System.Net.Mail;
using System.ComponentModel;


// In NuGET use 'Data Visualization Toolkit' to find it
//using System.Windows.Controls.Primitives;
//#endif

#if WINFORMS
using OxyPlot.WindowsForms;
#endif

#if WPF
#endif

#if UWP
#endif


#if ANDROID
using Android.Graphics;
using Android.Widget;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics.Drawables;
using OxyPlot.Xamarin.Android;
#endif

namespace SmartCubeMobile
{
    public class UtilityMESModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null &&
                propName != null)                   // Just in case!!
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }


    }
}