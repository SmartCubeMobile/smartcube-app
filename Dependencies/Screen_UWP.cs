using SmartCubeMobile;
using System;

#if UWP
using Windows.ApplicationModel.Store;
using Windows.UI.ViewManagement;
using Windows.UI.ViewManagement.Core;
using Windows.UI.Xaml;
#endif

#if WINUI
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
#endif

#if UWP || WINUI
public class ScreenService : IScreen
{
    // Explicit interface member implementation:
    public ScreenService(MainViewModel ourviewmodel)
    { 
    
    }

    void IScreen.SwitchScreen(bool quit, MainMeter components)
    {
#if WINUI
        var _window = ((App)Application.Current).MainWindow;
        // Well ... this is PROBABLY a kludge (Kludge City), but it
        // seems to work ok.
        if (_window.AppWindow.Presenter.Kind == AppWindowPresenterKind.Overlapped ||
            _window.AppWindow.Presenter.Kind == AppWindowPresenterKind.Default)
        {
            _window.AppWindow.SetPresenter(AppWindowPresenterKind.Default);
        }
        else
        {
            if (_window.AppWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
            {
                _window.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            }
        }
#endif

#if UWP
        ApplicationView application_view = ApplicationView.GetForCurrentView();

        if (quit)
        {
            if (application_view.IsFullScreenMode)
            {
                application_view.ExitFullScreenMode();
                MainMeter.ourviewmodel.isfullscreen = false;
            }
            //if (MainMeter.ourviewmodel.isfullscreen)
            //{
            //    components.WindowState = MainMeter.ourviewmodel.state;
            //    MainMeter.ourviewmodel.isfullscreen = false;
            //}
        }
        else
        {
            if (application_view.IsFullScreenMode)
            {
                // Put everything back
                // 16-Oct-2024 I'm commenting this out as I think UWP || WINUI
                // is always in Full Screen mode
                //MainMeter.ourviewmodel.Tossers.RowDefinitions[0].Height = MainMeter.ourviewmodel.rowheight;
                
                //components.WindowState = MainMeter.ourviewmodel.state;

                application_view.ExitFullScreenMode();
                MainMeter.ourviewmodel.isfullscreen = false;

                //MainPage.current_height = App.Current.MainPage.Height;
                //MainPage.current_width = App.Current.MainPage.Width;
            }
            else
            {
                // hide these three
                // 16-Oct-2024 I'm commenting these two out as I think UWP || WINUI
                // is always in Full Screen mode
                //MainMeter.ourviewmodel.rowheight = MainMeter.ourviewmodel.Tossers.RowDefinitions[0].Height;
                //MainMeter.ourviewmodel.Tossers.RowDefinitions[0].Height = new GridLength(0);
                
                //ScaleTransform abc = Calc_ScaleX(ourviewmodel.actual_width, ourviewmodel.actual_height);
                //components.RenderTransform = abc;
                // Re-size window
                //MainMeter.ourviewmodel.state = components.WindowState;
                //components.WindowState = WindowState.Maximized;

                application_view.TryEnterFullScreenMode();
                MainMeter.ourviewmodel.isfullscreen = true;

                //MainPage.current_height = App.Current.MainPage.Height;
                //MainPage.current_width = App.Current.MainPage.Width;
            }
            SmartRoutinesV2018.Record_ScreenSwitch(MainMeter.ourviewmodel);
        }
#endif
        return;
    }
}
#endif