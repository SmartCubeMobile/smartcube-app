#if WINFORMS
using System.Windows.Forms;
#endif

#if WPF
using System.Windows.Controls.Primitives;
#endif

#if UWP
using System;
using Windows.UI.Xaml.Controls;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
using System;
#endif

#if ANDROIDX
#endif

namespace SmartCubeMobile
{
#if WINFORMS
    public partial class SupplierDetails : Form
#endif
#if WPF
    public partial class SupplierDetails : Popup
#endif
#if UWP || WINUI
    public partial class SupplierDetails : ContentDialog
#endif
#if ANDROIDX
    public partial class SupplierDetails : PopupWindow
#endif
#if SMARTMAUI
    public partial class SupplierDetails : ContentPage
#endif
    {
        public SupplierDetails()
        {
#if WPF || UWP || WINUI || SMARTMAUI
            InitializeComponent();
#endif
#if ANDROIDX
            //this.SetBindingContext(utilityviewmodel);
#endif
        }

#if WINFORMS
        internal void PopupClosed(object sender, FormClosedEventArgs e)
#endif
#if WPF || UWP || WINUI || SMARTMAUI
        internal void PopupClosed(object sender, EventArgs e)
#endif
#if ANDROIDX
        internal void PopupClosed(object sender, EventArgs e)
#endif
        {
            // Now we don't give a SHIT if this fails or not, because we can't do anything about it even if we could
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
            return;
        }

#if WPF || UWP || WINUI
        private void MouseLeaving(object sender, EventArgs e)
        {
#if WPF
            this.IsOpen = false;
#endif
#if UWP || WINUI
            this.Hide();
#endif
            return;
        }
#endif
    }
}