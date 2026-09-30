using System;


#if WINFORMS
using System.Windows.Forms;
using SmartDashboardV2018;
#endif

#if WPF
using System.Windows;
using System.Windows.Controls.Primitives;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#endif

namespace SmartCubeMobile // Leave it here!  It matches the XAML
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
#if WINFORMS
    public partial class MultiMeter : Form
#endif
#if WPF
    public partial class MultiMeter : Popup
#endif
#if WINUI
    public partial class MultiMeter : ContentDialog
#endif
    {
        internal MultiMeter()
        {
#if WPF || UWP
            InitializeComponent();
#endif
        }

        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal static void PopupYes(object sender, EventArgs e)
#endif
#if WPF || UWP || WINUI
        internal void PopupYes(object sender, RoutedEventArgs e)
#endif
        {
            //
            // Yes sequence
            //
            MainMeter.ourviewmodel.multimeteryeschosen = true;

#if WINFORMS
            // At this point for WINFORMS, the Popup is already closed
#endif
#if WPF
            this.IsOpen = false;
#endif
#if WINFORMS || WPF
            MainMeter.ourviewmodel.UserNameVisible = Visibility.Hidden;
#endif
#if WINUI
            MainMeter.ourviewmodel.UserNameVisible = Visibility.Collapsed;
#endif
            MainMeter.ourviewmodel.UserNameListVisible = Visibility.Visible;
            //SmartRoutinesV2018.TextBlockUpdate(Mainwindow.ourviewmodel, "MultiMeter/Yes chosen");
#if WINFORMS
            PopupClosed_Actual();
#endif
            return;
        }

        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal static void PopupNo(object sender, EventArgs e)
#endif
#if WPF || WINUI
        internal void PopupNo(object sender, RoutedEventArgs e)
#endif
        {
            MainMeter.ourviewmodel.multimeteryeschosen = false;
#if WINFORMS
            // At this point for WINFORMS, the Popup is already closed
#endif
#if WPF
            this.IsOpen = false;
#endif
            return;
        }

#if WPF
        internal void MouseLeaving(object sender, EventArgs e)
        {
            this.IsOpen = false;
            return;
        }

#endif
#if WINFORMS
        internal void PopupClosed(object sender, FormClosedEventArgs e)
#endif
#if WPF || WINUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
        {
            PopupClosed_Actual();
            return;
        }

#if WINFORMS
        internal static void PopupClosed_Actual()
#endif
#if WPF || WINUI
        internal void PopupClosed_Actual()
#endif
        {
            //
            // DON'T FORGET TO RUN DANS CODE TO PUT THE SHADES UP
            //
            // Only check the door is we said "Yes" to Quit
            if (MainMeter.ourviewmodel.quityeschosen)
            {

            }
            return;
        }
    }
}