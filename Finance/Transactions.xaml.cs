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
//using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
#endif

#if ANDROIDX
using Android.Widget;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
#endif

namespace SmartCubeMobile // Leave it here!  It matches the XAML
{
    /// <summary>
    /// Interaction logic for Transactions.xaml
    /// </summary>
#if WINFORMS
    public partial class Transactions : Form
#endif
#if WPF
    public partial class Transactions : Popup
#endif
#if WINUI
    public partial class Transactions : ContentDialog
#endif
#if ANDROIDX
    public partial class Transactions : PopupWindow
#endif
#if SMARTMAUI
    public partial class Transactions : Popup
#endif
    {
        internal Transactions()
        {
#if WPF  || WINUI
            InitializeComponent();
#endif
        }

        // Its ok to use async void on Event Handlers

#if WINFORMS
        internal void PopupClosed(object sender, FormClosedEventArgs e)
#endif
#if WPF  || WINUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if ANDROIDX
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if SMARTMAUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
        {
            return;
        }

        private void PopupClosing(object sender, EventArgs e)
        {
            //    if (e.Key == Key.Return ||
            //        e.Key == Key.Enter)
            //    {
            //        e.Handled = true;
            //    //    //await SubmitClick(sender, e);
            //    }
            //
            // this.IsOpen = false; ????
            return;
        }

#if WPF  || WINUI
        private void MouseLeaving(object sender, EventArgs e)
        {
#if WPF
            this.IsOpen = false;
#endif
#if WINUI
            this.Hide();
#endif
            return;
        }
#endif

#if WINFORMS || WPF || ANDROIDX
        private void OnPrintButtonClick(object sender, EventArgs e)
#endif
#if WINUI
        private void OnPrintButtonClick(object sender, RoutedEventArgs e)
#endif
#if SMARTMAUI
        internal void OnPrintButtonClick(object sender, EventArgs e)
#endif
        {
            if (sender != null && e != null)
            {
                return;
            }
        }
    }
}