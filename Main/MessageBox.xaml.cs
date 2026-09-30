#if WINFORMS
using System.Windows;
using System.Windows.Forms;
#endif

#if WPF
using System.Windows;
#endif

#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class QuitMessageBox : Form
#endif
#if WPF
    public partial class QuitMessageBox : Window
#endif
#if WINUI
    public partial class QuitMessageBox : ContentDialog
#endif
#if ANDROIDX
    public partial class QuitMessageBox : Dialog
#endif
#if SMARTMAUI
    public partial class QuitMessageBox : Popup
#endif
    {
        private SignInViewModel signinviewmodel;
        private List<object> vmlist;
#if WINFORMS || WPF  || WINUI
        public QuitMessageBox(SignInViewModel signinvm, List<object> vms)
#endif
#if ANDROIDX
        public QuitMessageBox(Context context, SignInViewModel signinvm, List<object> vms) : base(context)
#endif
#if SMARTMAUI
        public QuitMessageBox(SignInViewModel signinvm, List<object> vms)
#endif
        {
            this.signinviewmodel = signinvm;
            this.vmlist = vms;
#if WPF  || WINUI || SMARTMAUI
            InitializeComponent();            
#endif
            MainViewModel ourviewmodel = vmlist.OfType<MainViewModel>().FirstOrDefault();
            if (ourviewmodel == null)
            {
                return;
            }
#if WINFORMS || WPF  || WINUI
            DataContext = ourviewmodel;
#endif
#if SMARTMAUI
            BindingContext = ourviewmodel;
#endif
            // Fix the language based on the SignIn choice
            ourviewmodel.Confirmation = SignIn.BesetByChimps(signinviewmodel, "ConfirmationRequired");
            ourviewmodel.ReallyLeave = SignIn.BesetByChimps(signinviewmodel, "ReallyLeave");
            ourviewmodel.Yes = SignIn.BesetByChimps(signinviewmodel, "Yes");
            ourviewmodel.No = SignIn.BesetByChimps(signinviewmodel, "No");
#if WPF
            // Events
            Closed += (s, e) => PopupClosed(s, e, vmlist);
            Yes.Click += (s, e) => PopupYes(s, e, vmlist);
            No.Click += (s, e) => PopupNo(s, e, ourviewmodel);
#endif
#if WINUI
            Closed += new Windows.Foundation.TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs>((s, e) => PopupClosed(s, e, vmlist));
            Yes.Click += new RoutedEventHandler((s, e) => PopupYes(s, e, vmlist));
            No.Click += new RoutedEventHandler((s, e) => PopupNo(s, e, ourviewmodel));
#endif
#if SMARTMAUI
            // Events
            this.Yes.Clicked += (s, e) => PopupYes(s, e, vmlist);
            this.No.Clicked += (s, e) => PopupNo(s, e, ourviewmodel);
#endif
            return;
        }
        
#if ANDROIDX
        //internal QuitMessageBox(Context context)
        //        : this(context, signinviewmodel, ourviewmodel)
        //{
        //}
#endif
        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal static async void PopupYes(object sender, EventArgs e,
                                            List<object> vmlist)
#endif
#if WPF  || WINUI
        internal async void PopupYes(object sender, RoutedEventArgs e,
                                     List<object> vmlist)
#endif
#if ANDROIDX
        internal async Task<bool> PopupYes(object sender, EventArgs e,
                                        List<object> vmlist)
#endif
#if SMARTMAUI
        internal async void PopupYes(object sender, EventArgs e,
                                            List<object> vmlist)
#endif
        {
            //
            // Quit/Yes sequence
            //
            MainViewModel ourviewmodel = vmlist.OfType<MainViewModel>().FirstOrDefault();
            if (ourviewmodel != null)
            {
                await ourviewmodel.sqliteDatabase.CloseAsync();

                ourviewmodel.quityeschosen = true;  // Causes clock to stop
                ourviewmodel.QuitEnabled = false;
                ourviewmodel.OpenCloseEnabled = false;
#if WINFORMS
                // At this point for WINFORMS, the Popup is already closed
#endif
#if WPF  || WINUI
                //FrontEndGUI.DecodeClosePopup(this);
#endif
#if ANDROIDX
                ourviewmodel.APopupDialog?.Dismiss();
                ourviewmodel.APopupDialog?.Hide();
#endif
                ourviewmodel.QuitColour = ourviewmodel.palegoldenrodColour;

              
            }
            // We might be cancelling before this is created
            ProfilesViewModel profileviewmodel = vmlist.OfType<ProfilesViewModel>().FirstOrDefault();
            if (profileviewmodel != null)
            {
                FixProfileToken(profileviewmodel);
            }
            // We might be cancelling before this is created
            FinanceViewModel financeviewmodel = vmlist.OfType<FinanceViewModel>().FirstOrDefault();
            if (financeviewmodel != null)
            {
                // Ignore Cancels
                if (financeviewmodel.financeCts != null)
                {
                    
                        // Don't really cancel - ignore cancels
                        financeviewmodel.financeToken = CancellationToken.None;
                }

#if WPF  || WINUI
                // Close these two down if they're open
                if (financeviewmodel.FCLWindow != null)
                {
#if WPF
                    //if (financeviewmodel.FCLPopup.IsOpen)
                    //{
                    //    financeviewmodel.FCLPopup.IsOpen = false;
                    //}
#endif
#if WINUI
                    if (financeviewmodel.FCLWindow.IsOpen)
                    {
                        financeviewmodel.FCLWindow.IsOpen = false;
                    }
#endif
                }
#endif
            }

            // We might be cancelling before this is created
            UtilityViewModel utilityviewmodel = vmlist.OfType<UtilityViewModel>().FirstOrDefault();
            if (utilityviewmodel != null)
            {
                if (utilityviewmodel.utilityCts != null)
                {
                    utilityviewmodel.utilityCts.Cancel();
                    utilityviewmodel.utilityToken = CancellationToken.None;
                }
            }
            // This next line causes "Task was cancelled" when next we try
            // to Post anything i.e. all/any outstanding communication is
            // cancelled (nothing gets sent/nothing is received) Which is
            // good, because we may have started something and we want to
            // interrupt it. 
            ourviewmodel.quitCts.Cancel();
            // BUT we need to tell our SmartDbServer to
            // 'disconnect' and we can't do that if the above request
            // still says 'cancel'. 
            ourviewmodel.quitCts.Dispose();
            // So we're going to dispose of that
            // request and get a NEW cancellation token ...!!
            ourviewmodel.quitCts = new CancellationTokenSource();
#if WINFORMS
            PopupClosed(sender, e, vmlist);
            return;
#endif
#if WPF
            // No need to call a routine, I think it gets called
            // 'automatically when the Popup is closed ... (??)
            this.Close();
            return;
#endif
#if WINUI
            this.Hide();
            return;
#endif
#if ANDROIDX
            PopupClosed(sender, e, vmlist);
            return true;
#endif
#if SMARTMAUI
            this.Close();
            return;
#endif
        }

        internal static void FixProfileToken(ProfilesViewModel profileviewmodel)
        {
            if (profileviewmodel.profileCts != null)
            {
                profileviewmodel.profileCts.Cancel();
                profileviewmodel.profileToken = CancellationToken.None;
            }
            return;
        }

        //internal void FixFinanceToken(FinanceViewModel financeviewmodel, bool cancel)
        //{
        //    if (financeviewmodel.financeCts != null)
        //    {
        //        if (cancel)
        //        {
        //            financeviewmodel.financeCts = new CancellationTokenSource();
        //        }
        //        else
        //        {
        //            // Don't really cancel - ignore cancels
        //            financeviewmodel.financeToken = CancellationToken.None;
        //        }
        //    }
        //    return;
        //}


        // No async required or necessary(?) on this PopupNo Event Handler
#if WINFORMS
        internal static void PopupNo(object sender, EventArgs e, MainViewModel ourviewmodel)
#endif
#if WPF  || WINUI
        internal void PopupNo(object sender, RoutedEventArgs e, MainViewModel ourviewmodel)
#endif
#if ANDROIDX
        internal void PopupNo(object sender, EventArgs e, MainViewModel ourviewmodel)
#endif
#if SMARTMAUI
        internal void PopupNo(object sender, EventArgs e, MainViewModel ourviewmodel)
#endif
        {
            ourviewmodel.quityeschosen = false;
#if WINFORMS
            // At this point for WINFORMS, the Popup is already closed
#endif
#if WPF  || WINUI
            //FrontEndGUI.DecodeClosePopup(this);
#endif
#if ANDROIDX
            ourviewmodel.APopupDialog.Dismiss();
            ourviewmodel.APopupDialog.Hide();
            // Allow another attempt
            ourviewmodel.quitinprocess = false;
            //MainActivity.actmodel.Quit.Click += (s, e) => ButtonQuitClick(s, e, 
            //                                                    ourviewmodel,
            //                                                    financeviewmodel,
            //                                                    utilityviewmodel);
#endif
            //if (!SmartRoutinesV2018.PopupNo_Actual())
            //{
            //    await SmartBobV2017.ListenerAsync(SignIn.signinviewmodel.Username, ourviewmodel, 0, 0, "PopupNo failed");
            // }
#if WPF
            this.Close();
#endif
#if WINUI
            this.Hide();
#endif
#if SMARTMAUI
            this.Close();

#endif
            return;
        }


#if WPF  || WINUI
        internal void MouseLeaving(object sender, EventArgs e)
        {
            //FrontEndGUI.DecodeClosePopup(this);
#if WPF
            this.Close();
#endif
#if WINUI
            this.Hide();
#endif
            return;
        }
#endif


#if WINFORMS
        internal static void PopupClosed(object sender, EventArgs e, List<object> vmlist)
#endif
#if WPF
        internal void PopupClosed(object sender, EventArgs e, List<object> vmlist)
#endif
#if WINUI
        internal void PopupClosed(ContentDialog sender, ContentDialogClosedEventArgs e, List<object> vmlist)
#endif
#if ANDROIDX
        internal void PopupClosed(object sender, EventArgs e, List<object> vmlist)
#endif
#if SMARTMAUI
        internal void PopupClosed(object sender, CommunityToolkit.Maui.Core.PopupClosedEventArgs e)
#endif
        {
            PopupClosed_Actual(vmlist);
            //
            // DON'T FORGET TO RUN DANS CODE TO PUT THE SHADES UP
            //
            // Only check the door is we said "Yes" to Quit
            //if (ourviewmodel.quityeschosen)
            //{
            //    if (ourviewmodel.doorDirection)
            //    {
#if WINFORMS || WPF
            //                    SmartRoutinesV2018.Move_The_DoorAsync(ourviewmodel);
#endif
            //    }
            //}
            // Now we don't give a SHIT if this fails or not, because we can't do anything about it even if we could
            return;
        }

#if WINFORMS
        internal static void PopupClosed_Actual(List<object> vmlist)
#endif
#if WPF  || WINUI
        internal void PopupClosed_Actual(List<object> vmlist)
#endif

#if ANDROIDX
        internal void PopupClosed_Actual(List<object> vmlist)
#endif
#if SMARTMAUI
        internal async void PopupClosed_Actual(List<object> vmlist)
#endif
        {
            //
            // DON'T FORGET TO RUN DANS CODE TO PUT THE SHADES UP
            //
            // Only check the door is we said "Yes" to Quit
            MainViewModel ourviewmodel = vmlist.OfType<MainViewModel>().FirstOrDefault();
            if (ourviewmodel != null)
            {
                if (ourviewmodel.quityeschosen)
                {
                    if (ourviewmodel.doorDirection)
                    {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                        FinanceViewModel financeviewmodel = vmlist.OfType<FinanceViewModel>().FirstOrDefault();
                        if (financeviewmodel != null)
                        {
//                            if (FinanceWebView != null)
//                            {
//#if WINFORMS
//                                FinanceWebView.Visible = false;
//#endif
//#if WPF  || WINUI
//                                FinanceWebView.Visibility = Visibility.Collapsed;
//#endif
//#if SMARTMAUI
//                                FinanceWebView.IsVisible = false;
//#endif
//                            }
                        }
#endif

#if SMARTMAUI
                        await SmartRoutinesV2018.MoveTheDoorsAsync(ourviewmodel);
#else
                        SmartRoutinesV2018.MoveTheDoorsAsync(ourviewmodel);
#endif
                    }
                }
            }
            return;
        }
    }
}