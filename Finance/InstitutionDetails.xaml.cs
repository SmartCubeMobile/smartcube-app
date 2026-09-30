#if WINFORMS
using System.Windows.Forms;
#endif

#if WPF
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using static SmartCubeMobile.FinanceViewModel;
using System.Windows.Input;
using System.Windows;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
#endif

#if ANDROIDX
#endif

namespace SmartCubeMobile
{
#if WINFORMS
    public partial class InstitutionDetails : Form
#endif
#if WPF
    public partial class InstitutionDetails : Popup
#endif
#if WINUI
    public partial class InstitutionDetails : ContentDialog
#endif
#if ANDROIDX
    public partial class InstitutionDetails : PopupWindow
#endif
#if SMARTMAUI
    public partial class InstitutionDetails : ContentPage
#endif
    {
        internal InstitutionDetails()
        {
#if WPF || UWP || SMARTMAUI
            InitializeComponent();
#endif
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

#if SMARTMAUI
        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
#endif
    }
}