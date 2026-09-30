using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


#if WINFORMS
using System.Windows;
using SmartDashboard;
//using System.Windows.Threading;
#endif

#if WPF
using System.Windows;
#endif

#if WINUI
using System.Windows;
using Microsoft.UI.Xaml;
#endif

#if ANDROIDX
using Android.Content;
using AndroidX.AppCompat.App;
#endif

#if SMARTMAUI
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
#endif

namespace SmartCubeMobile
{    
    public class SmartClockTask : PeriodicTickTask
    {
#if WINFORMS
        private SmartDashboard.MainProcess components;
        private readonly MainViewModel ourviewmodel;

#endif
#if ANDROIDX
        private AppCompatActivity _activity;
        private TextView _timeOffsetMessage;
        private object _currentViewModel;
#endif

#if !TEST
        private readonly SignInViewModel signinviewmodel;
#endif
        private readonly TickScheduler _scheduler;

        public SmartClockTask(
#if WINFORMS
                            MainProcess Components,
                            MainViewModel mainvm,
#endif
#if ANDROIDX
                            AppCompatActivity signinActivity,
#endif
#if !TEST
                            SignInViewModel signinvm,
#endif
                            TickScheduler scheduler
#if ANDROIDX
                            , TextView TimeOffsetMessageText
#endif
                            )
                            : base(TimeSpan.FromSeconds(1))
        {
#if WINFORMS
            components = Components;
            ourviewmodel = mainvm;
#endif
#if ANDROIDX
            _activity = signinActivity;
            _timeOffsetMessage = TimeOffsetMessageText;
#endif
#if !TEST
            signinviewmodel = signinvm;
#endif
            _scheduler = scheduler;
        }

#if ANDROIDX
        public void SetUiTarget(AppCompatActivity activity, TextView textView, object viewmodel)
        {
            _activity = activity;
            _timeOffsetMessage = textView;
            _currentViewModel = viewmodel;
        }
#endif
        protected async override Task ExecuteAsync()
        {
#if !TEST
            string DateTimeNowMessage = "";

            if (signinviewmodel.signincultureinfo == SmartParametersV2016.defaultCulture)
            {
                DateTimeNowMessage =
                    (DateTime.Now + signinviewmodel.utcOffset)
                    .ToString(SmartParametersV2016.militaryFormat);
            }
            else
            {
                DateTimeNowMessage =
                    (DateTime.Now + signinviewmodel.utcOffset)
                    .ToString(signinviewmodel.signincultureinfo);
            }
#endif
#if WINFORMS
            MainViewModel mainVM = null;

            components.labelDateTimeNow.Invoke(new Action(() =>
            {
                components.labelDateTimeNow.Text = DateTimeNowMessage;
            }));
            if (ourviewmodel != null)
            {
                mainVM = ourviewmodel; // capture for later

                // ✅ NOW you're off the UI thread → safe to await
                if (mainVM != null)
                {
                    if (mainVM.quityeschosen &&
                        !mainVM.QuitEnabled &&
                        !mainVM.OpenCloseEnabled &&
                        !mainVM.doorDirection)
                    {
                        if (mainVM.SmartProfile.profilecubefacesList.Count > 0)
                        {
                            await SmartRoutinesV2018.ClosingDown(
                                signinviewmodel,
                                mainVM);
                        }
                    }
                }
            }
#else
#if !TEST
            MainViewModel mainVM = null;
#endif
#endif
#if ANDROIDX
            _activity?.RunOnUiThread(() =>
            {
                if (_timeOffsetMessage != null)
                {
                    _timeOffsetMessage.Text = DateTimeNowMessage;
                }
            });

            if ((MainViewModel)_currentViewModel != null)
            {
                if (mainVM.quityeschosen &&
                    !mainVM.QuitEnabled &&
                    !mainVM.OpenCloseEnabled &&
                    !mainVM.doorDirection)
                {
                    if (mainVM.SmartProfile.profilecubefacesList.Count > 0)
                    {
                        await SmartRoutinesV2018.ClosingDown(
                            _activity,
                            signinviewmodel,
                            mainVM);
                    }
                }
            }
#endif
#if WPF
            Application.Current.Dispatcher.Invoke(() =>
            {
                var activeWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive);

                if (activeWindow?.DataContext is MainViewModel mvm)
                {
                    mvm.DateTimeNowMessage = DateTimeNowMessage;
                    mainVM = mvm; // capture for later
                }
                else if (activeWindow?.DataContext is SignInViewModel svm)
                {
                    signinviewmodel.DateTimeNowMessage = DateTimeNowMessage;
                }
            });
#endif
#if WINUI
            var _window = ((App)Application.Current).MainWindow;

            _window.DispatcherQueue.TryEnqueue(() =>
            {
                if (_window.Content is FrameworkElement root)
                {
                    if (root.DataContext is MainViewModel mvm)
                    {
                        mvm.DateTimeNowMessage = DateTimeNowMessage;
                        mainVM = mvm;
                    }
                    else if (root.DataContext is SignInViewModel svm)
                    {
                        signinviewmodel.DateTimeNowMessage = DateTimeNowMessage;
                    }
                }
            });
#endif
#if SMARTMAUI
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var page = Shell.Current?.CurrentPage;
#if !TEST
                if (page?.BindingContext is MainViewModel mvm)
                {
                    mvm.DateTimeNowMessage = DateTimeNowMessage;
                    mainVM = mvm;
                }
                else if (page?.BindingContext is SignInViewModel svm)
                {
                    signinviewmodel.DateTimeNowMessage = DateTimeNowMessage;
                }
#endif
            });
#endif
#if (WPF  || WINUI || SMARTMAUI) && !TEST
                // ✅ NOW you're off the UI thread → safe to await
            if (mainVM != null)
            {
                if (mainVM.quityeschosen &&
                    !mainVM.QuitEnabled &&
                    !mainVM.OpenCloseEnabled &&
                    !mainVM.doorDirection)
                {
                    mainVM.quityeschosen = false;
                    if (mainVM.SmartProfile.profilecubefacesList.Count > 0)
                    {
                        await SmartRoutinesV2018.ClosingDown(
                            signinviewmodel,
                            mainVM);
                    }

#if SMARTMAUI
                    // Navigate back to SignIn
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var window = Application.Current?.Windows.FirstOrDefault();

                        if (window != null)
                        {
                            window.Width = App.SignInWidth;
                            window.Height = App.SignInHeight;
                        }
                        await Shell.Current.Navigation.PopAsync();
                    });
#endif

                    // Optional cleanup
                    mainVM = null;
                    //(mainVM.BindingContext as MainViewModel)?.Dispose();
                }
            }
            return;
#endif
        }
    }
}