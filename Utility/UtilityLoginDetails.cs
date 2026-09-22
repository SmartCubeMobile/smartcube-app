using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if WPF
using System.Windows.Controls.Primitives;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;

#endif
namespace SmartCubeMobile
{
#if WINFORMS
    public partial class UtilityLoginDetails : Form
#endif
#if WPF 
    public partial class UtilityLoginDetails : Popup
#endif

#if WINUI
    public partial class UtilityLoginDetails : ContentDialog
#endif
#if SMARTMAUI
    public partial class UtilityLoginDetails : Popup
#endif
    {
        string local_password = string.Empty;

        // Very quick and dirty (but I'm tired after 10 years of slog)
        internal UtilityLoginDetails(List<SmartUtility.BrandConnection> params_found,
                                        UtilityViewModel utilityviewmodel,
                                        string supplier_name,
                                        string password)
        {
            // No idea why this doesn't work (see FinanceLoginDetails.cs)
            //InitializeComponent();

            utilityviewmodel.LoginDeleteClicked = false;
            if (!utilityviewmodel.found_it)
            {
                // No idea why this doesn't work (see FinanceLoginDetails.cs)

                //buttonLoginDetailsCreate.Visible = true;
                //buttonLoginDetailsAmend.Visible = false;
                //buttonLoginDetailsDelete.Visible = false;
            }
            else
            {
                // No idea why this doesn't work (see FinanceLoginDetails.cs)

                //buttonLoginDetailsCreate.Visible = false;
                //buttonLoginDetailsAmend.Visible = true;
                //buttonLoginDetailsDelete.Visible = true;
            }
            local_password = password;
#if WINFORMS
            //LoginStatusMessage.Text = supplier_name;
#endif
#if WPF
            //LoginStatusMessage.Content = supplier_name;
#endif
#if ANDROIDX
            //LoginStatusMessage.Text = supplier_name;
#endif
            // No idea why this doesn't work (see FinanceLoginDetails.cs)

            //SmartUtilityV2022.UtilityLoginParams(textBoxUser_Id,
            //                                    textBoxUser_Password,
            //                                    utilityviewmodel.login_info,
            //                                    params_found);
        }

        private void textBoxUser_Id_TextChanged(object sender, EventArgs e)
        {
            // No idea why this doesn't work (see FinanceLoginDetails.cs)

            //MainMeter.utilityviewmodel.newlogin_info.USER_ID = textBoxUser_Id.Text;
        }

        private void textBoxUser_Password_TextChanged(object sender, EventArgs e)
        {
            // No idea why this doesn't work (see FinanceLoginDetails.cs)

            //MainMeter.utilityviewmodel.newlogin_info.USER_PASSWORD = textBoxUser_Password.Text;
        }

        // The caring sister!  Latch-key < Turnkey!!  Ideris Elbow <= Idris Elboa!!
        private void LoginDetailsCreate_Click(object sender, EventArgs e)
        {
#if WINFORMS
            // Already closed here;
#endif
#if WPF
            //this.IsOpen = false;
#endif
#if ANDROIDX
            //await PopupNavigation.Instance.PopAsync();
#endif
            return;
        }

        private void LoginDetailsAmend_Click(object sender, EventArgs e)
        {
#if WINFORMS
            // Already closed here;
#endif
#if WPF
            //this.IsOpen = false;
#endif
#if ANDROIDX
            //await PopupNavigation.Instance.PopAsync();
#endif
            return;
        }

        private void LoginDetailsDelete_Click(object sender, EventArgs e)
        {
#if WINFORMS
            // Already closed here;
#endif
#if WPF
            //this.IsOpen = false;
#endif
#if ANDROIDX
            //await PopupNavigation.Instance.PopAsync();
#endif
            return;
        }
    }
}
