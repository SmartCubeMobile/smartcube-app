#if WPF
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
#endif

#if WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

using WinUIColor = Windows.UI.Color;
using WinUIColors = Microsoft.UI.Colors;
#endif

#if ANDROIDX
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Graphics;
using Android.Widget;
#endif

#if SMARTMAUI

#endif

namespace SmartCubeMobile
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }


#if WPF
    public partial class ToastWindow : Window
    {
        private readonly Window _owner;
        public ToastWindow(Window owner, string message, ToastType type = ToastType.Info)
        {
            InitializeComponent();
            if (owner != null)
            {
                // Remove dark window chrome/background

                _owner = owner;
                Owner = owner;
                MessageText.Text = message;
                ApplyStyle(type);   // 👈 NEW
                Loaded += async (_, __) =>
                {
                    PositionWindow();
                    await ShowToastAsync();
                };
            }
        }

        private void ApplyStyle(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success:
                    Background = new SolidColorBrush(Colors.LightGreen); // green
                    break;
                case ToastType.Error:
                    Background = new SolidColorBrush(Colors.LightPink); // red
                    break;
                case ToastType.Warning:
                    Background = new SolidColorBrush(Colors.LightYellow); // yellow
                    break;
                default:
                    Background = new SolidColorBrush(Colors.LightBlue); // blue
                    break;
            }
        }


        private void PositionWindow()
        {
            //var workingArea = SystemParameters.WorkArea;
            //Left = workingArea.Right - Width - 20;
            //Top = workingArea.Bottom - Height - 20;

            Left = _owner.Left + (_owner.ActualWidth - Width) / 2;
            Top = _owner.Top + (_owner.ActualHeight - Height) / 2;
        }

        private async Task ShowToastAsync()
        {
            // Fade in
            AnimateOpacity(0, 1, 300);
            await Task.Delay(3000); // visible duration
            // Fade out
            AnimateOpacity(1, 0, 300);
            await Task.Delay(300);
            Close();
        }

        private void AnimateOpacity(double from, double to, int durationMs)
        {
            var animation = new DoubleAnimation(from, to,
                new Duration(TimeSpan.FromMilliseconds(durationMs)));
            BeginAnimation(OpacityProperty, animation);
            //var animation = new DoubleAnimation
            //{
            //    From = from,
            //    To = to,
            //    Duration = TimeSpan.FromMilliseconds(durationMs)
            //};
        }
    }
#endif

#if SMARTMAUI

    public partial class ToastWindow : ContentView
    {
        public ToastWindow(string message, ToastType type = ToastType.Info)
        {
            InitializeComponent();
            if (message != null)
            {
                 _ = ShowToastAsync(message, type);
            }
        }

        public async Task ShowToastAsync(string message, ToastType type)
        {                       
            var label = new Label
            {
                Text = message,
                BackgroundColor = ApplyStyle(type), // Remove dark window chrome/background 
                TextColor = Colors.White,
                Padding = 10,
                Opacity = 0
            };

            ToastHost.Children.Add(label);

            // fade in
            await label.FadeTo(1, 250);

            // wait
            await Task.Delay(2000);

            // fade out
            await label.FadeTo(0, 250);

            ToastHost.Children.Remove(label);
        }

        private Color ApplyStyle(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success:
                    BackgroundColor = Colors.LightGreen; // green
                    break;
                case ToastType.Error:
                    BackgroundColor = Colors.LightPink; // red
                    break;
                case ToastType.Warning:
                    BackgroundColor = Colors.LightYellow; // yellow
                    break;
                default:
                    BackgroundColor = Colors.LightBlue; // blue
                    break;
            }
            return BackgroundColor;
        }
    }


#endif

#if ANDROIDX

    public static class ToastService
    {
        public static void Show(Context context, string message, Color backgroundColor)
        {
            var textView = new TextView(context);
            textView.Text = message;
            textView.SetTextColor(Color.White);
            textView.SetPadding(40, 20, 40, 20);

            var drawable = new GradientDrawable();
            drawable.SetColor(backgroundColor);
            drawable.SetCornerRadius(25);

            textView.Background = drawable;

            var toast = new Toast(context);
            toast.Duration = ToastLength.Short;
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                toast.View = textView;
            }
            toast.Show();
        }

        public static void Success(Context context, string message)
        {
            Show(context, message, Color.Rgb(56, 142, 60)); // green
        }

        public static void Error(Context context, string message)
        {
            Show(context, message, Color.Rgb(211, 47, 47)); // red
        }

        public static void Warning(Context context, string message)
        {
            Show(context, message, Color.Rgb(255, 152, 0)); // orange
        }

        public static void Info(Context context, string message)
        {
            Show(context, message, Color.Rgb(33, 150, 243)); // blue
        }
    }
#endif
#if WINUI
    //public partial class ToastWindow : InfoBar
    //{
    //    private static Queue<(string message, ToastType type)> _queue = new();
    //    private static bool _isShowing = false;

    //    public ToastWindow(ContentDialog owner, string message, ToastType type = ToastType.Info)
    //    {
    //        _queue.Enqueue((message, type));
    //        _ = ProcessQueue();
    //    }

    //    private static async Task ProcessQueue()
    //    {
    //        if (_isShowing) return;

    //        _isShowing = true;

    //        while (_queue.Count > 0)
    //        {
    //            var (message, type) = _queue.Dequeue();
    //            await ShowInternal(message, type);
    //        }

    //        _isShowing = false;
    //    }

    //    private static async Task ShowInternal(string message, ToastType type)
    //    {
    //        var root = (FrameworkElement)App.MainWindow.Content;

    //        var container = (Border)root.FindName("ToastContainer");
    //        var text = (TextBlock)root.FindName("ToastText");

    //        if (container == null || text == null)
    //            return;

    //        await App.MainWindow.DispatcherQueue.EnqueueAsync(() =>
    //        {
    //            text.Text = message;
    //            container.Background = new SolidColorBrush(GetColor(type));
    //        });

    //        // 🎨 Apply style
    //        //container.Background = new SolidColorBrush(GetColor(type));

    //        // Reset state
    //        container.Opacity = 0;
    //        if (container.RenderTransform is not TranslateTransform transform)
    //        {
    //            transform = new TranslateTransform();
    //            container.RenderTransform = transform;
    //        }
    //        transform.Y = -20;

    //        // 🔼 Fade + slide in
    //        await Animate(container, 0, 1, -20, 0, 200);

    //        // ⏱️ Stay visible
    //        await Task.Delay(2500);

    //        // 🔽 Fade + slide out
    //        await Animate(container, 1, 0, 0, -20, 200);
    //    }

    //    private static async Task Animate(Border target,
    //        double fromOpacity, double toOpacity,
    //        double fromY, double toY,
    //        int duration)
    //    {
    //        var storyboard = new Storyboard();

    //        var fade = new DoubleAnimation
    //        {
    //            From = fromOpacity,
    //            To = toOpacity,
    //            Duration = TimeSpan.FromMilliseconds(duration)
    //        };

    //        var slide = new DoubleAnimation
    //        {
    //            From = fromY,
    //            To = toY,
    //            Duration = TimeSpan.FromMilliseconds(duration)
    //        };

    //        Storyboard.SetTarget(fade, target);
    //        Storyboard.SetTargetProperty(fade, "Opacity");

    //        Storyboard.SetTarget(slide, target);
    //        Storyboard.SetTargetProperty(slide, "(UIElement.RenderTransform).(TranslateTransform.Y)");

    //        storyboard.Children.Add(fade);
    //        storyboard.Children.Add(slide);

    //        var tcs = new TaskCompletionSource<bool>();
    //        storyboard.Completed += (_, __) => tcs.SetResult(true);

    //        storyboard.Begin();
    //        await tcs.Task;
    //    }

    //    private static Microsoft.UI.Colors GetColor(ToastType type)
    //    {
    //        return type switch
    //        {
    //            ToastType.Success => Colors.LightGreen,
    //            ToastType.Error => Colors.LightPink,
    //            ToastType.Warning => Colors.LightYellow,
    //            _ => Colors.LightBlue
    //        };
    //    }

 

    public partial class ToastWindow
    {
        private readonly string _message;
        private readonly ToastType _type;

        private FrameworkElement _element;

        public ToastWindow(string message, ToastType type = ToastType.Info)
        {
            _message = message;
            _type = type;
        }

        public async void Show()
        {
            var host = GetHost();
            if (host == null) return;

            var _window = ((App)Application.Current).MainWindow;

            await _window.DispatcherQueue.EnqueueAsync(async () =>
            {
                _element = CreateUI();

                host.Children.Add(_element);

                await AnimateIn(_element);

                await Task.Delay(2500);

                await AnimateOut(_element);

                host.Children.Remove(_element);
            });
        }

        private StackPanel GetHost()
        {
            var _window = ((App)Application.Current).MainWindow;

            return (_window.Content as FrameworkElement)?
                .FindName("ToastHost") as StackPanel;
        }        

        private FrameworkElement CreateUI()
        {
            return new InfoBar
            {
                Message = _message,
                Severity = _type switch
                {
                    ToastType.Success => InfoBarSeverity.Success,
                    ToastType.Warning => InfoBarSeverity.Warning,
                    ToastType.Error => InfoBarSeverity.Error,
                    _ => InfoBarSeverity.Informational
                },
                IsOpen = true,
                Opacity = 0,
                RenderTransform = new TranslateTransform { Y = 20 }
            };
        }
        private Task AnimateIn(FrameworkElement target)
            => Animate(target, 0, 1, 20, 0, 200);

        private Task AnimateOut(FrameworkElement target)
            => Animate(target, 1, 0, 0, 20, 200);

        private Task Animate(FrameworkElement target,
            double fromOpacity, double toOpacity,
            double fromY, double toY,
            int duration)
        {
            var storyboard = new Storyboard();

            var fade = new DoubleAnimation
            {
                From = fromOpacity,
                To = toOpacity,
                Duration = TimeSpan.FromMilliseconds(duration)
            };

            var slide = new DoubleAnimation
            {
                From = fromY,
                To = toY,
                Duration = TimeSpan.FromMilliseconds(duration)
            };

            Storyboard.SetTarget(fade, target);
            Storyboard.SetTargetProperty(fade, "Opacity");

            Storyboard.SetTarget(slide, target);
            Storyboard.SetTargetProperty(slide,
                "(UIElement.RenderTransform).(TranslateTransform.Y)");

            storyboard.Children.Add(fade);
            storyboard.Children.Add(slide);

            var tcs = new TaskCompletionSource<bool>();
            storyboard.Completed += (_, __) => tcs.SetResult(true);

            storyboard.Begin();
            return tcs.Task;
        }

        private Windows.UI.Color GetColor(ToastType type)
        {
            return type switch
            {
                ToastType.Success => Colors.SeaGreen,
                ToastType.Error => Colors.IndianRed,
                ToastType.Warning => Colors.DarkOrange,
                _ => Colors.SteelBlue
            };
        }
    }
#endif
}