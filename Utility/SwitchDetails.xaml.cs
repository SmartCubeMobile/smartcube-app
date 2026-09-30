#if WINFORMS
using System.Windows.Forms;

#endif

#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#endif

#if UWP
using Windows.UI.Core;
using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;
#endif

#if WINUI
using Windows.UI.Core;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
#endif

#if ANDROID
using Android.Views;
using Android.Text;
using Android.Widget;
using Android;
#endif

namespace SmartCubeMobile
{
    /// <summary>
    /// Interaction logic for SwitchDetails.xaml
    /// </summary>
#if WINFORMS
    public partial class SwitchDetails //: Popup
#endif
#if WPF
    public partial class SwitchDetails : Popup
#endif
#if UWP || WINUI
    public partial class SwitchDetails : ContentDialog
#endif
#if ANDROID
    public partial class SwitchDetails : PopupWindow
#endif
#if SMARTMAUI
    public partial class SwitchDetails : ContentPage
#endif
    {

        private static SignInViewModel signinviewmodel;
        private static List<object> vmlist;
        public SwitchDetails(SignInViewModel signinvm, List<object> vms)
        {
            signinviewmodel = signinvm;
            vmlist = vms;
        }
        internal SwitchDetails(UtilityViewModel utilityviewmodel)
        {
#if WPF  || UWP || WINUI || SMARTMAUI
            InitializeComponent();
#endif
#if WPF
            SwitchStatusMessage2.Content = utilityviewmodel.switch_brand_name;
#endif
#if UWP || WINUI
            SwitchStatusMessage2.Text = utilityviewmodel.switch_brand_name;
#endif
#if ANDROIDX
            // Is this shit going to work???
            View ssm2 = (View)Resource.Layout.SwitchDetails;
            TextView SwitchStatusMessage2 = (TextView)ssm2.FindViewWithTag("SwitchStatusMessage2");            
            SwitchStatusMessage2.Text = utilityviewmodel.switch_brand_name;
#endif
#if WPF
            SwitchStatusMessage5.Content = utilityviewmodel.switch_tariff_name + SwitchStatusMessage5.Content;
#endif
#if UWP || WINUI
            SwitchStatusMessage5.Text = utilityviewmodel.switch_tariff_name + SwitchStatusMessage5.Text;
#endif
#if ANDROIDX
            View ssm5 = (View)Resource.Layout.SwitchDetails;
            TextView SwitchStatusMessage5 = (TextView)ssm5.FindViewWithTag("SwitchStatusMessage5");
            SwitchStatusMessage5.Text = utilityviewmodel.switch_tariff_name + SwitchStatusMessage5.Text;
#endif
#if WPF || UWP || WINUI
            AccountName.Text = "Bank Account Name";
            AccountSortCode.Text = "Bank Sort Code";
#if WPF
            AccountNumber.Password = "Bank Account Number";
#endif
#if UWP || WINUI
            AccountNumber.Text = "Bank Account Number";
#endif

#endif
#if ANDROIDX
            View acc = (View)Resource.Layout.SwitchDetails;
            TextView AccountName = (TextView)acc.FindViewWithTag("AccountName");
            AccountName.Hint = "Bank Account Name";
            TextView AccountSortCode = (TextView)acc.FindViewWithTag("AccountSortCode");
            AccountSortCode.Hint = "Bank Sort Code";
            TextView AccountNumber = (TextView)acc.FindViewWithTag("AccountNumber");
            AccountNumber.Hint = "Bank Account Number";
#endif
#if WPF || UWP || WINUI
            AccountName.Text = utilityviewmodel.XRAY.bank_account_name;
            AccountSortCode.Text = utilityviewmodel.XRAY.bank_account_sort_code;
#endif
#if ANDROIDX
            AccountName.Text = utilityviewmodel.XRAY.bank_account_name;
            AccountSortCode.Text = utilityviewmodel.XRAY.bank_account_sort_code;
#endif
#if WPF
            AccountNumber.Password = utilityviewmodel.XRAY.bank_account_number;
#endif
#if UWP || WINUI
            AccountNumber.Text = utilityviewmodel.XRAY.bank_account_number;
#endif
#if ANDROIDX
            AccountNumber.Text = utilityviewmodel.XRAY.bank_account_number;
#endif
            //// Ha ha !  The old code is the BEST!!
            //int count = 0;
            //foreach (char c in global_account_number)
            //{
            //    if (c == '*')
            //    {
            //        count++;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
#if WPF
            AccountNumber.MaxLength = 8 - AccountNumber.Password.Length;
            AccountNumber.ToolTip = "Please enter the first " +
                                            AccountNumber.MaxLength.ToString() +
                                            " digits for ";// +
                                                           //global_account_number;

            AccountName.GotFocus += (s, e) => OnFocus_AccountName(s, e, utilityviewmodel);
            AccountSortCode.GotFocus += (s, e) => OnFocus_AccountSortCode(s, e, utilityviewmodel);
            AccountNumber.GotFocus += (s, e) => OnFocus_AccountNumber(s, e, utilityviewmodel);
#endif

#if ANDROIDX
            utilityviewmodel.accountnumber_maxlength = 8 - AccountNumber.Text.Length;
#endif
            return;
        }

        private void OnFocus_AccountName(object sender, EventArgs e,
                                        UtilityViewModel utilityviewmodel)
        {
#if WPF  || UWP || WINUI
            AccountName.Text = utilityviewmodel.XRAY.bank_account_name;
#endif
#if ANDROIDX
            View acc = (View)Resource.Layout.SwitchDetails;
            TextView AccountName = (TextView)acc.FindViewWithTag("AccountName");
            AccountName.Text = utilityviewmodel.XRAY.bank_account_name;
#endif
#if WPF
            SwitchStatusMessage.Content = "";
#endif
#if UWP || WINUI
            SwitchStatusMessage.Text = "";
#endif
#if ANDROIDX
            View ssm = (View)Resource.Layout.SwitchDetails;
            TextView SwitchStatusMessage = (TextView)ssm.FindViewWithTag("SwitchStatusMessage");
            SwitchStatusMessage.Text = "";
#endif
            return;
        }

        private void OnFocus_AccountSortCode(object sender, EventArgs e,
                                            UtilityViewModel utilityviewmodel)
        {
#if WPF  || UWP || WINUI
            AccountSortCode.Text = utilityviewmodel.XRAY.bank_account_sort_code;
#endif
#if ANDROIDX
            View acc = (View)Resource.Layout.SwitchDetails;
            TextView AccountSortCode = (TextView)acc.FindViewWithTag("AccountSortCode");
            AccountSortCode.Text = utilityviewmodel.XRAY.bank_account_sort_code;
#endif
#if WPF
            SwitchStatusMessage.Content = "";
#endif
#if UWP || WINUI
            SwitchStatusMessage.Text = "";
#endif
#if ANDROIDX
            View ssm = (View)Resource.Layout.SwitchDetails;
            TextView SwitchStatusMessage = (TextView)ssm.FindViewWithTag("SwitchStatusMessage");
            SwitchStatusMessage.Text = "";
#endif
            return;
        }

        private void OnFocus_AccountNumber(object sender, EventArgs e,
                                            UtilityViewModel utilityviewmodel)
        {
#if WPF
            AccountNumber.Password = utilityviewmodel.XRAY.bank_account_number;
#endif
#if UWP || WINUI
            AccountNumber.Text = utilityviewmodel.XRAY.bank_account_number;
#endif
#if ANDROIDX
            View acc = (View)Resource.Layout.SwitchDetails;
            TextView AccountNumber = (TextView)acc.FindViewWithTag("AccountNumber");
            AccountNumber.Text = utilityviewmodel.XRAY.bank_account_number;
#endif
#if WPF
            SwitchStatusMessage.Content = "";
#endif
#if UWP || WINUI
            SwitchStatusMessage.Text = "";
#endif
#if ANDROIDX
            View ssm = (View)Resource.Layout.SwitchDetails;
            TextView SwitchStatusMessage = (TextView)ssm.FindViewWithTag("SwitchStatusMessage");
            SwitchStatusMessage.Text = "";
#endif
            return;
        }

        // She cannot do ANYTHING quietly ... its an unrelenting BASH, CRASH, CRASH BASH around
        // the kitchen.  Little wonder the fucking thing is falling apart under her reletless onslaught.
        //#if WPF
        //        private void OnFocus_AccountName(object sender, EventArgs e)
        //        {
        //            BankAccountName.Text = global_account_name;
        //            SwitchStatusMessage.Text = "";
        //            return;
        //        }

        //        private void OnFocus_AccountSortCode(object sender, EventArgs e)
        //        {
        //            BankAccountSortCode.Text = global_sort_code;
        //            SwitchStatusMessage.Text = "";
        //            return;
        //        }
        //#endif

        //        private void OnFocus_AccountNumber(object sender, EventArgs e)
        //        {
        //#if WPF
        //            AccountNumber.Password = "";
        //#endif
        //#if ANDROID
        //            AccountNumber.Text = "";
        //#endif
        //            SwitchStatusMessage.Text = "";
        //            return;
        //        }

#if WPF || UWP || WINUI
        internal void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
#if WPF
            if (e.Key == Key.Return ||
                e.Key == Key.Enter)
#endif
#if UWP || WINUI
            if (e.VirtualKey == Windows.System.VirtualKey.Enter)
#endif
            {
                e.Handled = true;
            }
            return;
        }
#endif

#if WPF || UWP || WINUI
        internal void OnDigitDownHandler(object sender, KeyEventArgs e)
        {
#if WPF
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                (e.Key == Key.Back))
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
#endif
            return;
        }
#endif


        //private void OnDigitDownHandler(object sender, EventArgs e)
        //{

        //if ((e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9) ||
        //    (e.Key == VirtualKey.Back))
        //{
        //    e.Handled = false;
        //}
        //else if ((e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9))
        // {
        //    e.Handled = false;
        //}
        //else
        //{
        //    e.Handled = true;
        //}
        //  return;
        //}

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

#if WINFORMS || WPF
        private void SubmitClick(object sender, EventArgs e)
#endif
#if UWP || WINUI
        private void SubmitClick(object sender, RoutedEventArgs e)
#endif
#if ANDROIDX || SMARTMAUI
        private void SubmitClick(object sender, EventArgs e)
#endif
        {
            UtilityViewModel utilityviewmodel = vmlist.OfType<UtilityViewModel>().FirstOrDefault();
#if WPF
            string combined_account_number = AccountNumber.Password.Trim() + utilityviewmodel.XRAY.bank_account_number.Replace("*", "").Trim();
#endif
#if UWP || WINUI
            string combined_account_number = AccountNumber.Text.Trim() + utilityviewmodel.XRAY.bank_account_number.Replace("*", "").Trim();
#endif
#if ANDROIDX
            View acc = (View)Resource.Layout.SwitchDetails;
            TextView AccountNumber = (TextView)acc.FindViewWithTag("AccountNumber");
            string combined_account_number = AccountNumber.Text.Trim() + utilityviewmodel.XRAY.bank_account_number.Replace("*", "").Trim();
#endif
#if WPF || UWP || WINUI || ANDROIDX

#if WPF || UWP || WINUI
            if (string.IsNullOrEmpty(AccountName.Text.Trim()) ||
                (AccountSortCode.Text.Trim().Length != 6) ||
                (combined_account_number.Length != 8))
            {
#endif
#if ANDROIDX
            TextView AccountName = (TextView)acc.FindViewWithTag("AccountName");
            TextView AccountSortCode = (TextView)acc.FindViewWithTag("AccountSortCode");

            if (string.IsNullOrEmpty(AccountName.Text.Trim()) ||
                (AccountSortCode.Text.Trim().Length != 6) ||
                (combined_account_number.Length != 8))
            {
#endif
#if WPF
                SwitchStatusMessage.Content = "Switchdetailsincomplete";
#endif
#if UWP || WINUI
                SwitchStatusMessage.Text = "Switchdetailsincomplete";
#endif
#if ANDROIDX
                View ssm = (View)Resource.Layout.SwitchDetails;
                TextView SwitchStatusMessage = (TextView)ssm.FindViewWithTag("SwitchStatusMessage");
                SwitchStatusMessage.Text = "Switchdetailsincomplete";
#endif
            }
            else
#endif
            {
#if WPF || UWP || WINUI || ANDROIDX
                utilityviewmodel.XRAY.bank_account_name = AccountName.Text.Trim();
                utilityviewmodel.XRAY.bank_account_sort_code = AccountSortCode.Text.Trim();
                utilityviewmodel.XRAY.bank_account_number = combined_account_number;
                utilityviewmodel.XRAY.status = "";

                FrontEndGUI.DecodeClosePopup(this);
#endif
                utilityviewmodel.switch_info.bank_account_name = utilityviewmodel.XRAY.bank_account_name;
                utilityviewmodel.switch_info.bank_account_sort_code = utilityviewmodel.XRAY.bank_account_sort_code;
                utilityviewmodel.switch_info.bank_account_number = utilityviewmodel.XRAY.bank_account_number;
            }
            return;
        }

#if WINFORMS || WPF
        private void CancelClick(object sender, EventArgs e)
#endif
#if UWP || WINUI
        private void CancelClick(object sender, RoutedEventArgs e)
#endif
#if ANDROIDX || SMARTMAUI
        private void CancelClick(object sender, EventArgs e)
#endif
        {
            UtilityViewModel utilityviewmodel = vmlist.OfType<UtilityViewModel>().FirstOrDefault();
            utilityviewmodel.XRAY.bank_account_name = "";
            utilityviewmodel.XRAY.bank_account_sort_code = "";
            utilityviewmodel.XRAY.bank_account_number = "";
            utilityviewmodel.XRAY.status = "Cancel";

            FrontEndGUI.DecodeClosePopup(this);
            return;
        }
    }

#if WPF
    // All this shit just to get default 'watermarks' in the UserName and PassWord boxes ..
    public class AccountNumberBoxMonitor : DependencyObject
    {
        public static bool GetIsMonitoring(DependencyObject dependency)
        {
            if (dependency != null)
            {
                return (bool)dependency.GetValue(IsMonitoringProperty);
            }
            // Need to test this!!!!!
            return false;
        }

        public static void SetIsMonitoring(DependencyObject dependency, bool value)
        {
            if (dependency == null)
            {
                return;
            }
            dependency.SetValue(IsMonitoringProperty, value);
            return;
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(AccountNumberBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static int GetAccountNumberLength(DependencyObject dependency)
        {
            if (dependency != null)
            {
                return (int)dependency.GetValue(AccountNumberLengthProperty);
            }
            // Need to test this!!!!
            return 0;
        }
        public static void SetAccountNumberLength(DependencyObject dependency, int value)
        {
            // This looks STUPID but it gets risd of IDE0031
            if (dependency == null)
            {
                return;
            }
            dependency.SetValue(AccountNumberLengthProperty, value);
            return;
        }

        public static readonly DependencyProperty AccountNumberLengthProperty =
            DependencyProperty.RegisterAttached("AccountNumberLength", typeof(int), typeof(AccountNumberBoxMonitor), new UIPropertyMetadata(0));

        private static void OnIsMonitoringChanged(DependencyObject depobj, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox BankAccountNumber = (PasswordBox)depobj;
            if (BankAccountNumber != null)
            {
                if ((bool)e.NewValue)
                {
                    BankAccountNumber.PasswordChanged += AccountNumberChanged;
                }
                else
                {
                    BankAccountNumber.PasswordChanged -= AccountNumberChanged;
                }
            }
            return;
        }

        private static void AccountNumberChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox BankAccountNumber = (PasswordBox)sender;
            if (BankAccountNumber != null)
            {
                SetAccountNumberLength(BankAccountNumber, BankAccountNumber.Password.Length);
            }
            return;
        }

    }
#endif
}