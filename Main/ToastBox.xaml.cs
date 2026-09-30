#if WINFORMS
using System.Windows.Forms;
#endif

#if WPF
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Threading;
#endif

#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading;
#endif

#if ANDROIDX
using Android.Content;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;
#endif

namespace SmartCubeMobile // Leave it here!  It matches the XAML
{
    // <summary>
    // Interaction logic for MessageBox.xaml
    // </summary>
#if WINFORMS
    public partial class ToastBox : Form
#endif
#if WPF
    public partial class ToastBox : Popup
#endif
#if WINUI
    public partial class ToastBox : UserControl
#endif
#if ANDROIDX
    public partial class ToastBox : Dialog  
#endif
#if SMARTMAUI
    public partial class ToastBox : Popup
#endif
    {
#if WINFORMS || WPF  || WINUI
        private DispatcherTimer toastTimer;
#endif
#if SMARTMAUI
        private IDispatcherTimer toastTimer;
#endif
        public ToastBox(FinanceView financecomponents,
                            string message,
                            int durationSeconds,
                            double tossersWidth = 0) // Not sure I need this
//#if ANDROID
//        internal ToastBox(Context context) : base(context)
//#endif
        {

#if WINFORMS || WPF  || WINUI
            //FrontEndGUI.SetPopupDataContext(this);
#endif
#if WPF  || WINUI || SMARTMAUI
            InitializeComponent();
#endif
            ToastMessage.Text = message;

            // Set a timer to close the toast after the specified duration
#if WINFORMS || WPF  || WINUI
            toastTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(durationSeconds)
            };
            toastTimer.Tick += ToastTimer_Tick;
            
#endif

#if SMARTMAUI
            toastTimer = Application.Current.Dispatcher.CreateTimer();
            toastTimer.Interval = TimeSpan.FromSeconds(durationSeconds);

            toastTimer.Tick += async (s, e) =>
            {
                toastTimer.Stop();
                await FadeOutAndClose();
            };
#endif
            toastTimer.Start();

#if WPF
            this.Placement = PlacementMode.Bottom;
            this.PlacementTarget = financecomponents;
            this.HorizontalOffset = (financecomponents.ActualWidth + ToastMessage.Text.Length) / 2;
            this.VerticalOffset = -this.ActualHeight - 30; // Adjust the -10 to control spacing

            this.IsOpen = true;
#endif
            return;
        }

#if WPF  || WINUI        
#if WPF
        private void ToastTimer_Tick(object sender, EventArgs e)
#endif
#if WINUI 
        private void ToastTimer_Tick(object sender, object e)
#endif
        {
            toastTimer.Stop();
            FadeOutText(); // Start fade-out animation
#if WPF
            this.IsOpen = false;
#endif
            return;
        }
#endif

#if SMARTMAUI
        private async Task FadeOutAndClose()
        {
            await ToastMessage.FadeTo(0, 2000);
            Close();
        }
#endif

#if WPF  || WINUI
        private void FadeOutText()
        {
            // Create a fade-out animation for the TextBlock's opacity
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,    // Fully visible
                To = 0.0,      // Fully transparent
                Duration = TimeSpan.FromSeconds(2) // Fade duration (2 seconds)
            };
#if WPF 
            // Start the animation
            ToastMessage.BeginAnimation(OpacityProperty, fadeOutAnimation);
#endif
            return;
        }
#endif

#if WPF  || WINUI
        internal void MouseLeaving(object sender, EventArgs e)
        {
#if WPF
            FrontEndGUI.DecodeClosePopup(this);
#endif
            return;
        }
#endif
    }
}