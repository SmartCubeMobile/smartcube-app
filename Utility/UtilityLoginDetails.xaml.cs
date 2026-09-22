#if WINFORMS
using System.Windows.Forms;
using SmartDashboard;
#endif

#if WPF
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#endif

#if UWP
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endif

#if ANDROIDX
using Android.Views;
using AndroidX.AppCompat.App;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;
#endif

namespace SmartCubeMobile
{
    // <summary>
    // Interaction logic for UtilityLoginDetails.xaml
    // </summary>
#if WINFORMS
    public partial class UtilityLoginDetails : Form
#endif
#if WPF
    public partial class UtilityLoginDetails : Popup
#endif
#if UWP || WINUI
    public partial class UtilityLoginDetails : ContentDialog
#endif
#if ANDROIDX
    public partial class UtilityLoginDetails : PopupWindow
#endif
#if SMARTMAUI
    public partial class UtilityLoginDetails : ContentPage
#endif
    {
        //string local_password = "";
        private MainViewModel ourviewmodel;
        private UtilityViewModel utilityviewmodel;
        public UtilityLoginDetails(MainViewModel mainvm, UtilityViewModel utilityvm)
        {
            this.ourviewmodel = mainvm;
            this.utilityviewmodel = utilityvm;
            return;
        }
        internal UtilityLoginDetails(List<SmartUtility.BrandConnection> params_found,
                                            UtilityViewModel utilityviewmodel,
                                            string supplier_name,
                                            string password)
        {
#if WPF || UWP || WINUI || SMARTMAUI
            InitializeComponent();
#endif
#if SMARTMAUI
            BindingContext = utilityviewmodel;
#endif
            //FrontEndGUI.SetUtilityPopupDataContext(utilityviewmodel, this);

            //local_password = password;
            utilityviewmodel.LoginDeleteClicked = false;
            if (!utilityviewmodel.found_it)
            {
#if WPF
                utilityviewmodel.LoginCreate = Visibility.Visible;
                utilityviewmodel.LoginAmend = Visibility.Hidden;
                utilityviewmodel.LoginDelete = Visibility.Hidden;
#endif
#if UWP || WINUI || SMARTMAUI
                utilityviewmodel.LoginCreate = true;
                utilityviewmodel.LoginAmend = Visibility.Collapsed;
                utilityviewmodel.LoginDelete = Visibility.Collapsed;
#endif
#if WINFORMS
                utilityviewmodel.LoginCreate = true;
                utilityviewmodel.LoginAmend = false;
                utilityviewmodel.LoginDelete = false;
#endif
#if ANDROIDX
                utilityviewmodel.LoginCreate = true;
                utilityviewmodel.LoginAmend = false;
                utilityviewmodel.LoginDelete = false;
#endif
            }
            else
            {
#if WPF || SMARTMAUI
                utilityviewmodel.LoginCreate = true;
                utilityviewmodel.LoginAmend = Visibility.Visible;
                utilityviewmodel.LoginDelete = Visibility.Visible;
#endif
#if UWP || WINUI
                utilityviewmodel.LoginCreate = Visibility.Collapsed;
                utilityviewmodel.LoginAmend = Visibility.Visible;
                utilityviewmodel.LoginDelete = Visibility.Visible;
#endif
#if WINFORMS
                utilityviewmodel.LoginCreate = false;
                utilityviewmodel.LoginAmend = true;
                utilityviewmodel.LoginDelete = true;
#endif
#if ANDROIDX
                utilityviewmodel.LoginCreate = false;
                utilityviewmodel.LoginAmend = true;
                utilityviewmodel.LoginDelete = true;
#endif
            }
            //local_password = password;
#if WINFORMS
            //LoginStatusMessage.Content = supplier_name;
#endif
#if WPF || UWP || WINUI || SMARTMAUI
            LoginStatusMessage.Text = supplier_name;
#endif
#if ANDROIDX
            Android.Views.View lsm = (Android.Views.View)Resource.Layout.UtilityLoginDetails;
            TextView LoginStatusMessage = (TextView)lsm.FindViewWithTag("LoginStatusMessage");
            LoginStatusMessage.Text = supplier_name;
            EditText UserId = (EditText)lsm.FindViewWithTag("UserId");
            EditText Password = (EditText)lsm.FindViewWithTag("Password");
#endif
#if WPF || UWP || WINUI || SMARTMAUI
            SmartUtilityV2022.UtilityLoginParams(UserId,
                                                Password,
                                                utilityviewmodel.login_info,
                                                params_found);
#endif
#if ANDROIDX
            SmartUtilityV2022.UtilityLoginParams(UserId,
                                                Password,
                                                utilityviewmodel.login_info,
                                                params_found);
#endif
#if WPF
            UserId.GotFocus += OnFocus_UserId;
            Password.GotFocus += OnFocus_Password;
#endif
#if ANDROIDX
            Android.Widget.EditText username = (EditText)lsm.FindViewWithTag("UserId");
            username.Focusable = true; // += OnFocus_Username;
            Android.Widget.EditText passwordx = (EditText)lsm.FindViewWithTag("Password");
            passwordx.Focusable = true;
#endif

            return;
        }

        private void OnFocus_UserId(object sender, EventArgs e)
        {
#if WPF || UWP || WINUI || SMARTMAUI
            UserId.Text = utilityviewmodel.login_info.USER_ID;
            LoginStatusMessage.Text = "";
#endif
#if ANDROIDX
            View lsm = (View)Resource.Layout.UtilityLoginDetails;
            EditText UserId = (EditText)lsm.FindViewWithTag("UserId");
            UserId.Text = utilityviewmodel.login_info.USER_ID;
            TextView LoginStatusMessage = (TextView)lsm.FindViewWithTag("LoginStatusMessage");
            LoginStatusMessage.Text = "";
#endif
            return;
        }

        private void OnFocus_Password(object sender, EventArgs e)
        {
#if WPF || UWP || WINUI || SMARTMAUI
            Password.Text = utilityviewmodel.login_info.USER_PASSWORD;
            LoginStatusMessage.Text = "";
#endif
#if ANDROIDX
            View lsm = (View)Resource.Layout.UtilityLoginDetails;
            EditText Password = (EditText)lsm.FindViewWithTag("Password");
            Password.Text = utilityviewmodel.login_info.USER_PASSWORD;
            TextView LoginStatusMessage = (TextView)lsm.FindViewWithTag("LoginStatusMessage");
            LoginStatusMessage.Text = "";
#endif
            return;
        }

#if WINFORMS
        internal void OnKeyDownHandler(object sender, System.Windows.Forms.KeyEventArgs e)
#endif
#if WPF || UWP || WINUI
        internal void OnKeyDownHandler(object sender, KeyEventArgs e)
#endif
#if ANDROIDX || SMARTMAUI
        internal void OnKeyDownHandler(object sender, EventArgs e)
#endif
        {
#if WPF || UWP || WINUI
#if WPF
            if (e.Key == Key.Return ||
                e.Key == Key.Enter)
#endif
#if UWP || WINUI
            if (e.VirtualKey == VirtualKey.Enter)
#endif
            {
                e.Handled = true;
            }
#endif
            return;
        }

#if WPF
        internal void OnDigitDownHandler(object sender, KeyEventArgs e)
        {
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
            return;
        }
#endif

        //private void OnDigitDownHandler(object sender, EventArgs e)
        //{
        //if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
        //    (e.Key == Key.Back))
        //{
        //    e.Handled = false;
        //}
        //else if ((e.Key >= Key.D0 && e.Key <= Key.D9))
        //{
        //    e.Handled = false;
        //}
        //else
        //{
        //    e.Handled = true;
        //}
        //    return;
        //}

        internal void PopupClosing(object sender, EventArgs e)
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

#if WPF || UWP || WINUI || SMARTMAUI
        internal void MouseLeaving(object sender, EventArgs e)
        {
#if WPF
            this.IsOpen = false;
#endif
#if UWP || WINUI
            this.Hide();
#endif
#if SMARTMAUI
            // Not used or needed as its in the XAML to close it automatically
#endif
            return;
        }
#endif

#if WINFORMS
        private async void LoginDetailsCreate_Click(object sender, EventArgs e, MainProcess process_components)
#endif
#if WPF
        private async void LoginDetailsCreate_Click(object sender, RoutedEventArgs e)
#endif
#if UWP || WINUI
        private async void LoginDetailsCreate_Click(object sender, RoutedEventArgs e)
#endif
#if ANDROIDX 
        private async void LoginDetailsCreate_Click(object sender, EventArgs e,
                                            AppCompatActivity meterActivity)

#endif
#if SMARTMAUI
        private async void LoginDetailsCreate_Click(object sender, EventArgs e)
#endif
        {
            string errorMessage = "";
#if ANDROIDX
            View lsm = (View)Resource.Layout.UtilityLoginDetails;
            EditText UserId = (EditText)lsm.FindViewWithTag("UserId");
            EditText Password = (EditText)lsm.FindViewWithTag("Password");
#endif

#if WPF || UWP || WINUI || SMARTMAUI
            utilityviewmodel.newlogin_info = new SmartUtility.Logins()
            {
                USER_ID = UserId.Text,
                USER_PASSWORD = Password.Text
            };
#endif
#if ANDROIDX
            utilityviewmodel.newlogin_info = new SmartUtility.Logins()
            {
                USER_ID = UserId.Text,
                USER_PASSWORD = Password.Text
            };
#endif
            if (!await SmartUtilityV2022.Utility_LoginSetup(
#if WINFORMS
                                                    process_components,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    //local_password,   // Might need this one day?
                                                    utilityviewmodel.found_it,
                                                    utilityviewmodel.LoginDeleteClicked,
                                                    em => errorMessage = em))
            {
#if WPF || UWP || WINUI || SMARTMAUI
                LoginStatusMessage.Text = errorMessage;
#endif
#if ANDROIDX
                TextView LoginStatusMessage = (TextView)lsm.FindViewWithTag("LoginStatusMessage");
                LoginStatusMessage.Text = errorMessage;
#endif
            }
            else
            {
                //FrontEndGUI.DecodeClosePopup(this);
            }
            return;
        }
        //#endif

#if WINFORMS
        private async void LoginDetailsDelete_Click(object sender, EventArgs e, MainProcess process_components)
#endif
#if WPF
        private async void LoginDetailsDelete_Click(object sender, EventArgs e)
#endif
#if UWP || WINUI
        private async void LoginDetailsDelete_Click(object sender, RoutedEventArgs e)
#endif
#if ANDROIDX 
        private async void LoginDetailsDelete_Click(object sender, EventArgs e,
                                                    AppCompatActivity meterActivity)
#endif
#if SMARTMAUI
        private async void LoginDetailsDelete_Click(object sender, EventArgs e)
#endif
        {
            string errorMessage = "";

            utilityviewmodel.LoginDeleteClicked = true;

            //utilityviewmodel.newlogin_info = new SmartUtility.Logins()
            //{
            //    USER_ID = UserId.Text,
            //    USER_PASSWORD = Password.Text
            //};

            if (!await SmartUtilityV2022.Utility_LoginSetup(
#if WINFORMS
                                                    process_components,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    //local_password,   // Might need this one day?
                                                    utilityviewmodel.found_it,
                                                    utilityviewmodel.LoginDeleteClicked,
                                                    em => errorMessage = em))
            {
#if WPF || UWP || WINUI || SMARTMAUI
                LoginStatusMessage.Text = errorMessage;
#endif
#if ANDROIDX
                View lsm = (View)Resource.Layout.UtilityLoginDetails;
                TextView LoginStatusMessage = (TextView)lsm.FindViewWithTag("LoginStatusMessage");
                LoginStatusMessage.Text = errorMessage;
#endif
            }
            else
            {
                //FrontEndGUI.DecodeClosePopup(this);
            }
            return;
        }

#if WINFORMS
        private void LoginDetailsCancel_Click(object sender, EventArgs e)
#endif
#if WPF
        private void LoginDetailsCancel_Click(object sender, EventArgs e)
#endif
#if UWP || WINUI
        private void LoginDetailsCancel_Click(object sender, RoutedEventArgs e)
#endif
#if ANDROIDX
        private void LoginDetailsCancel_Click(object sender, EventArgs e)
#endif
#if SMARTMAUI
        private void LoginDetailsCancel_Click(object sender, EventArgs e)
#endif
        {
            // 'Cancel' or 'Failed' was returned - do nothing
            // Except turn on all the buttons
            // If the Institution WAS GREEN, then it STAYS GREEN
            // if it was BLACK then it stays BLACK
            
            //FrontEndGUI.DecodeClosePopup(this);
            return;
        }
    }

    //#if WPF
    //    // All this shit just to get default 'watermarks' in the UserName and PassWord boxes ..
    //    public class UtilityPasswordBoxMonitor : DependencyObject
    //    {
    //        public static bool GetIsMonitoring(DependencyObject dependency)
    //        {
    //            if (dependency != null)
    //            {
    //                return (bool)dependency.GetValue(IsMonitoringProperty);
    //            }
    //            // Need to test this!!!!!
    //            return false;
    //        }

    //        public static void SetIsMonitoring(DependencyObject dependency, bool value)
    //        {
    //            // This looks STUPID but it gets rid of IDE0031
    //            if (dependency == null)
    //            {
    //                return;
    //            }
    //            dependency.SetValue(IsMonitoringProperty, value);
    //            return;
    //        }

    //        public static readonly DependencyProperty IsMonitoringProperty =
    //            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasscodeBoxMonitor), new PropertyMetadata(false, OnIsMonitoringChanged));

    //        public static int GetPasswordNumberLength(DependencyObject dependency)
    //        {
    //            if (dependency != null)
    //            {
    //                return (int)dependency.GetValue(UtilityPasswordLengthProperty);
    //            }
    //            // Need to test this!!!!
    //            return 0;
    //        }

    //        public static void UtilitySetPasswordLength(DependencyObject dependency, int value)
    //        {
    //            // This looks STUPID but it gets rid of IDE0031
    //            if (dependency == null)
    //            {
    //                return;
    //            }
    //            dependency.SetValue(UtilityPasswordLengthProperty, value);
    //            return;
    //        }

    //        public static readonly DependencyProperty UtilityPasswordLengthProperty =
    //            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasscodeBoxMonitor), new PropertyMetadata(0));

    //        private static void OnIsMonitoringChanged(DependencyObject depobj, DependencyPropertyChangedEventArgs e)
    //        {
    //            PasswordBox Password = (PasswordBox)depobj;
    //            if (Password != null)
    //            {
    //                if ((bool)e.NewValue)
    //                {
    //                    Password.PasswordChanged += UtilityPasswordChanged;
    //                }
    //                else
    //                {
    //                    Password.PasswordChanged -= UtilityPasswordChanged;
    //                }
    //            }
    //            return;
    //        }

    //        private static void UtilityPasswordChanged(object sender, RoutedEventArgs e)
    //        {
    //            PasswordBox Password = (PasswordBox)sender;
    //            if (Password != null)
    //            {
    //                UtilitySetPasswordLength(Password, Password.Password.Length);
    //            }
    //            return;
    //        }
    //    }
    //#endif
    //#if WPF
    //    // All this shit just to get default 'watermarks' in the UserName and PassWord boxes ..
    //    public class UtilityPasscodeBoxMonitor : DependencyObject
    //    {
    //        public static bool GetIsMonitoring(DependencyObject dependency)
    //        {
    //            if (dependency != null)
    //            {
    //                return (bool)dependency.GetValue(IsMonitoringProperty);
    //            }
    //            // Need to test this!!!!!
    //            return false;
    //        }

    //        public static void SetIsMonitoring(DependencyObject dependency, bool value)
    //        {
    //            //if (dependency != null)
    //            //{
    //            dependency?.SetValue(IsMonitoringProperty, value);
    //            //}
    //            return;
    //        }

    //        public static readonly DependencyProperty IsMonitoringProperty =
    //            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasscodeBoxMonitor), new PropertyMetadata(false, OnIsMonitoringChanged));

    //        public static int GetPasscodeNumberLength(DependencyObject dependency)
    //        {
    //            if (dependency != null)
    //            {
    //                return (int)dependency.GetValue(PasscodeLengthProperty);
    //            }
    //            // Need to test this!!!!
    //            return 0;
    //        }

    //        public static void SetPasscodeLength(DependencyObject dependency, int value)
    //        {
    //            //if (dependency != null)
    //            //{
    //            dependency?.SetValue(PasscodeLengthProperty, value);
    //            //}
    //            return;
    //        }

    //        public static readonly DependencyProperty PasscodeLengthProperty =
    //            DependencyProperty.RegisterAttached("PasscodeLength", typeof(int), typeof(PasscodeBoxMonitor), new PropertyMetadata(0));

    //        private static void OnIsMonitoringChanged(DependencyObject depobj, DependencyPropertyChangedEventArgs e)
    //        {
    //            PasswordBox Passcode = (PasswordBox)depobj;
    //            if (Passcode == null)
    //            {
    //                return;
    //            }
    //            if ((bool)e.NewValue)
    //            {
    //                Passcode.PasswordChanged += PasscodeChanged;
    //            }
    //            else
    //            {
    //                Passcode.PasswordChanged -= PasscodeChanged;
    //            }
    //            return;
    //        }

    //        internal static void PasscodeChanged(object sender, RoutedEventArgs e)
    //        {
    //            PasswordBox Passcode = (PasswordBox)sender;
    //            if (Passcode == null)
    //            {
    //                return;
    //            }
    //            SetPasscodeLength(Passcode, Passcode.Password.Length);
    //            return;
    //        }
    //    }
    //#endif
}