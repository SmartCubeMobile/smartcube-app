using System.Globalization;

#if WINFORMS
using System.Windows.Forms;
using SmartDashboard;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications.Management;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Markup;
#endif

#if WPF
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
//using Windows.Foundation.Metadata;
//using Windows.UI.Notifications.Management;
using System.Windows;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
using Windows.Devices.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.WinUI.UI.Controls;
using System.Linq;
#endif

#if ANDROIDX
using Android.Graphics;
using Android.Views;
using OxyPlot.Xamarin.Android;
using Android.Views.Animations;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;
using iText.Layout.Element;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Core;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;
#endif

namespace SmartCubeMobile
{
    public class FrontEndGUI
    {
#if WPF  || WINUI
        public static Thickness thickness0 = new Thickness(0);
        public static Thickness thickness1 = new Thickness(1);
#endif

#if SMARTMAUI
        public static Thickness thickness0 = new Thickness(0);
        public static Thickness thickness1 = new Thickness(1);
#endif
        //#if WINFORMS
        //        public static void SetWindowDataContext(this)
        //        {
        //#endif

#if WPF || WINUI
        public static void SetWindowDataContext(MainViewModel ourviewmodel, Window whatever)
        {
#endif
#if ANDROIDX
        public static void SetWindowDataContext(MainViewModel ourviewmodel, Window whatever)
        {
#endif
#if WPF || WINUI
#if WPF
            whatever.DataContext = ourviewmodel;
#endif
        }
#endif
#if ANDROIDX
            // Context is read-only
            //whatever.Context = ourviewmodel;
        }
#endif

#if WINUI
        public static void SetWindowDataContext(MainViewModel ourviewmodel, Page whatever)
        {
            whatever.DataContext = ourviewmodel;
        }
#endif
#if SMARTMAUI
        public static void SetWindowDataContext(MainViewModel ourviewmodel, Page whatever)
        {
            whatever.BindingContext = ourviewmodel;
        }
#endif


#if WPF
        public static void SetSignInWindowDataContext(Window whatever,
                                                    SignInViewModel signinviewmodel)
        {
#endif
#if WINUI
        public static void SetSignInWindowDataContext(Grid whatever, 
                                                        SignInViewModel signinviewmodel) // Famous 'root grid' !!!
        {
#endif
#if ANDROIDX
        public static void SetSignInWindowDataContext(Window whatever)
        {
#endif
#if SMARTMAUI
        public static void SetSignInWindowDataContext(ContentView whatever, SignInViewModel signinviewmodel)
        {
#endif
#if WPF  || WINUI
            whatever.DataContext = signinviewmodel;
        }
#endif
#if SMARTMAUI
            whatever.BindingContext = signinviewmodel;
        }
#endif
#if ANDROIDX
            // Can only GET Context - its read-only
            //whatever.Context = SignIn.signinviewmodel;
        }
#endif

#if WINFORMS
        public static void SetPopupDataContext(object dummy)
        {
            return;
        }
#endif

#if WPF
        public static void SetPopupDataContext(MainViewModel ourviewmodel, Popup whatever)
        {
#endif
#if ANDROIDX
        public static void SetPopupDataContext(MainViewModel ourviewmodel, PopupWindow whatever)
        {
#endif
#if WPF
            whatever.DataContext = ourviewmodel;
            return;
        }
#endif
#if ANDROIDX
            // Context is read-only
            //whatever.Context = ourviewmodel;
            return;
        }
#endif

#if WINUI
        public static void SetPopupDataContext(MainViewModel ourviewmodel, ContentDialog whatever)
        {
            whatever.DataContext = ourviewmodel;
            return;
        }
#endif

#if WPF
        public static void SetLabelDataContext(MainViewModel ourviewmodel, Label whatever)
        {
            whatever.DataContext = ourviewmodel;
        }
#endif
#if WINUI
        public static void SetLabelDataContext(MainViewModel ourviewmodel, TextBlock whatever)
        {
            whatever.DataContext = ourviewmodel;
        }
#endif
#if SMARTMAUI
        public static void SetLabelDataContext(MainViewModel ourviewmodel, Label whatever)
        {
            whatever.BindingContext = ourviewmodel;
        }
#endif

#if ANDROIDX
        public static void SetLabelDataContext(TextView whatever)
        {
            // Context is read-only
            Console.WriteLine(whatever.Context);// = ourviewmodel;
        }
#endif

#if WINFORMS
        public static void SetFinanceDataContext(object dummy)
        {
            return;
        }
#endif

#if WPF
        public static void SetFinanceDataContext(Popup whatever, FinanceViewModel financeviewmodel)
        {
            whatever.DataContext = financeviewmodel;
        }
#endif

#if ANDROIDX
        public static void SetFinanceDataContext(PopupWindow whatever)
        {
            // Context is read-only
            Console.WriteLine(whatever.ContentView.Context);// = financeviewmodel;
        }
#endif

#if WINUI
        public static void SetFinanceDataContext(UserControl whatever, FinanceViewModel financeviewmodel)
        {
            whatever.DataContext = financeviewmodel;
            return;
        }
#endif

#if SMARTMAUI
        public static void SetFinanceDataContext(ContentPage whatever, FinanceViewModel financeviewmodel)
        {
            whatever.BindingContext = financeviewmodel;
            return;
        }
#endif

#if WPF
        public static void SetUtilityViewDataContext(UserControl whatever, UtilityViewModel utilityviewmodel)
        {
            whatever.DataContext = utilityviewmodel;
        }
#endif

#if ANDROIDX
        public static void SetUtilityViewDataContext(View whatever)
        {
            // Context is read-only
            Console.WriteLine(whatever.Context);// = utilityviewmodel;
        }
#endif

#if WINUI
        public static void SetUtilityViewDataContext(UserControl whatever, UtilityViewModel utilityviewmodel)
        {
            whatever.DataContext = utilityviewmodel;
        }
#endif
        public static void SetUtilityPopupDataContext(
                                                UtilityViewModel utilityviewmodel,
#if WINFORMS
                                                 object dummy
#endif
#if WPF
                                                 Popup whatever
#endif
#if WINUI
                                                ContentDialog whatever
#endif
#if ANDROIDX
                                                 PopupWindow whatever
#endif
#if SMARTMAUI
                                                 ContentView whatever
#endif
                                                )
        {
#if WPF  || WINUI
            whatever.DataContext = utilityviewmodel;
#endif
#if ANDROIDX
            // Context is read-only
            //whatever.Context = utilityviewmodel;
#endif
        }

#if WINFORMS
        public static EventArgs DecodeEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif

#if WPF
        public static RoutedPropertyChangedEventArgs<double> DecodeRoutedPropertyChangedEventFlags(object eventArgs)
        {
            return (RoutedPropertyChangedEventArgs<double>)eventArgs;
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        public static EventArgs DecodeEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif

#if WPF  || WINUI || SMARTMAUI
#if WPF  || WINUI
        public static RoutedEventArgs DecodeRoutedEventFlags(object eventArgs)
        {
#endif
#if SMARTMAUI
        public static RoutedEventArgs DecodeRoutedEventFlags(object eventArgs)
        {
#endif
            return (RoutedEventArgs)eventArgs;
        }
#endif



#if WINFORMS
        public static EventArgs DecodeSelectionChangedEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif

#if WPF
        public static SelectionChangedEventArgs DecodeSelectionChangedEventFlags(object eventArgs)
        {
            return (SelectionChangedEventArgs)eventArgs;
        }
#endif

#if WINUI
        public static SelectionChangedEventArgs DecodeSelectionChangedEventFlags(object eventArgs)
        {
            return (SelectionChangedEventArgs)eventArgs;
        }
#endif

#if ANDROIDX
        public static EventArgs DecodeSelectionChangedEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif

#if SMARTMAUI
        public static EventArgs DecodeSelectionChangedEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif
#if WINFORMS || WPF  || WINUI
        public static MouseEventArgs DecodeMouseEventFlags(object eventArgs)
        {
            return (MouseEventArgs)eventArgs;
        }
#endif
#if SMARTMAUI
        public static EventArgs DecodeMouseEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif

#if WINUI
        public static RightTappedRoutedEventArgs DecodeRightTappedRoutedEventFlags(object eventArgs)
        {
            return (RightTappedRoutedEventArgs)eventArgs;
        }
#endif

#if WINUI
        public static DoubleTappedRoutedEventArgs DecodeDoubleTappedRoutedEventFlags(object eventArgs)
        {
            return (DoubleTappedRoutedEventArgs)eventArgs;
        }
#endif

#if WPF
        public static MouseButtonEventArgs DecodeMouseButtonEventFlags(object eventArgs)
        {
            return (MouseButtonEventArgs)eventArgs;
        }
#endif
#if SMARTMAUI
        public static EventArgs DecodeMouseButtonEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif
#if ANDROIDX
        public static EventArgs DecodeMouseEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif

#if ANDROIDX
        public static EventArgs DecodeCheckedChangedEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif

#if ANDROIDX
        public static EventArgs DecodeEventFlags(object eventArgs)
        {
            return (EventArgs)eventArgs;
        }
#endif

#if WPF  || WINUI
        public static List<DataGridTextColumn> SetPopupColumns()
        {
            return new List<DataGridTextColumn>();
        }
#endif

#if SMARTMAUI
        public static List<SmartRoutinesV2018.ColumnDefinitionModel> SetPopupColumns()
        {
            return new List<SmartRoutinesV2018.ColumnDefinitionModel>();
        }
#endif

#if WINFORMS || WPF  || WINUI
#pragma warning disable CA1416
        public static bool FindListener()
        {
            //return ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener");
            return true;
        }

        //internal static UserNotificationListener SetListener()
        //{
        //    return UserNotificationListener.Current;
        //}
#pragma warning restore CA1416
#endif

#if ANDROIDX
        public static bool FindListener()
        {
            return false;// ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener");
        }

        //public static UserNotificationListener SetListener()
        //{
        //    return UserNotificationListener.Current;
        //}
#endif

#if WPF  || WINUI
        public static PlacementMode SetPlacement()
        {
#if WPF
            return PlacementMode.Center;
#endif
#if WINUI
            return PlacementMode.Mouse;
#endif
        }
#endif

#if WINFORMS
        public static RichTextBox FindScrollViewer(MainProcess process_components)
        {
            process_components.ScrollViewer.Text = "";
            process_components.ScrollViewer.Visible = true;
            return process_components.ScrollViewer;
        }
#endif

        public static bool CheckScrollViewer(MainViewModel ourviewmodel)
        {
#if SMARTMAUI
            if (ourviewmodel.ScrollViewer != null)
#else
            if (ourviewmodel.ScrollViewer != null)
#endif
            {
                return true;
            }
            return false;
        }


#if WPF  || WINUI
        public static ScrollViewer FindScrollViewer(MainMeter process_components)
        {
            return (ScrollViewer)process_components.FindName("ScrollViewer");
        }
        public static Border FindScrollBorder(MainMeter process_components)
        {
            return (Border)process_components.FindName("Border");
        }
        public static TextBlock FindScrollTextBlock(MainMeter process_components)
        {
            return (TextBlock)process_components.FindName("TextBlock");

        }
        public static Grid FindGrid(UserControl view, string gridname)
        {
            return (Grid)view.FindName(gridname);
        }

        public static DataGrid FindDataGrid(UserControl view, string datagridname)
        {
            return (DataGrid)view.FindName(datagridname);
        }

        //public static ListView FindListView(UserControl view, string listviewname)
        //{
        //    return (ListView)view.FindName(listviewname);
        //}

        //public static TabItem FindTabItem(UserControl view, string tabitemname)
        //{
        //    return (TabItem)view.FindName(tabitemname);
        //}
#endif
#if SMARTMAUI
        public static ScrollView FindScrollViewer(MainMeter process_components)
        {
            return process_components.FindByName<ScrollView>("ScrollViewer");
        }
        public static Border FindScrollBorder(MainMeter process_components)
        {
            return process_components.FindByName<Border>("Border");
        }
        public static Label FindScrollTextBlock(MainMeter process_components)
        {
            return process_components.FindByName<Label>("LogLabel");

        }
        //public static Grid FindGrid(UserControl view, string gridname)
        //{
        //    return (Grid)view.FindName(gridname);
        //}

        //public static DataGrid FindDataGrid(UserControl view, string datagridname)
        //{
        //    return (DataGrid)view.FindName(datagridname);
        //}

        //public static ListView FindListView(UserControl view, string listviewname)
        //{
        //    return (ListView)view.FindName(listviewname);
        //}

        //public static TabItem FindTabItem(UserControl view, string tabitemname)
        //{
        //    return (TabItem)view.FindName(tabitemname);
        //}
#endif




#if SMARTMAUI
        // Jesus Christ was this a fag!!
        public static CollectionView FindDataGrid(View view, string datagridname)
        {
            if (view is not ContentView contentView)
            {
                return null;
            }
            List<CollectionView> all = GetAllCollectionViews(contentView);
            return all.FirstOrDefault(cv => cv.StyleId == datagridname);
        }

        public static List<CollectionView> GetAllCollectionViews(ContentView contentView)
        {
            var result = new List<CollectionView>();

            if (contentView?.Content is not View root)
                return result;

            Find(root, result);

            return result;
        }

        private static void Find(View view, List<CollectionView> result)
        {
            // Only collect CollectionViews
            if (view is CollectionView cv)
            {
                result.Add(cv);
            }
            // Traverse children (required to reach them)
            switch (view)
            {
                case Layout layout:
                    foreach (var child in layout.Children)
                    {
                        if (child is View childView)
                            Find(childView, result);
                    }
                    break;

                case ContentView contentView when contentView.Content is View content:
                    Find(content, result);
                    break;

                case ScrollView scrollView when scrollView.Content is View content:
                    Find(content, result);
                    break;

                case Border border when border.Content is View content:
                    Find(content, result);
                    break;
            }
        }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS || WPF
        public static TabControl FindTabControl(
#endif
#if WINUI
        public static TabView FindTabControl(
#endif
#if SMARTMAUI
        public static Grid FindTabControl(
#endif
#if WINFORMS
                                                MainProcess components,
#endif
#if WPF  || WINUI
                                                UserControl view,
#endif
#if SMARTMAUI
                                                View view,
#endif
                                                string tabcontrolname)
        {
#if WINFORMS
            return (TabControl)components.CurrencyPanel;
#endif
#if WPF
            return (TabControl)view.FindName(tabcontrolname);
#endif
#if WINUI
            return (TabView)view.FindName(tabcontrolname);
#endif
#if SMARTMAUI
            return (Grid)view.FindByName(tabcontrolname);
#endif
        }
#endif

#if ANDROIDX
        //public static Android.Widget.ScrollView FindScrollView(MainMeter process_components)
        //{
        //int id0 = (int)typeof(Resource.Id).GetField("ScrollViewer").GetValue(null);
        //return (Android.Widget.ScrollView)process_components.FindViewById(id0);
        //}

        //public static Border FindScrollBorder(MainMeter process_components)
        //{
        //    return (Border)process_components.FindName("Border");
        //}


        //public static TextView FindScrollTextBlock(MainMeter process_components)
        //{
        //    int id0 = (int)typeof(Resource.Id).GetField("TextBlock").GetValue(null);
        //    return (TextView)process_components.FindViewById(id0);            
        //}


        //public static GridView FindDataGrid(View view, string datagridname)
        //{
        //    int id0 = (int)typeof(Resource.Id).GetField(datagridname).GetValue(null);
        //    return (GridView)view.FindViewById(id0);
        //}
#endif
        public static void SetBorderVisible(MainViewModel ourviewmodel)
        {
#if WINFORMS
            ourviewmodel.BorderVisible = true;
#endif
#if WPF  || WINUI
            ourviewmodel.BorderVisible = Visibility.Visible;
#endif
#if ANDROIDX
            ourviewmodel.BorderVisible = true;
#endif
#if SMARTMAUI
            ourviewmodel.BorderVisible = Visibility.Visible;
#endif
            return;
        }
        public static void SetScrollViewerVisible(MainViewModel ourviewmodel)
        {
#if WINFORMS
            ourviewmodel.ScrollViewerVisible = true;
#endif
#if WPF  || WINUI
            ourviewmodel.ScrollViewerVisible = Visibility.Visible;
#endif
#if ANDROIDX
            ourviewmodel.ScrollViewerVisible = true;
#endif
#if SMARTMAUI
            ourviewmodel.ScrollViewerVisible = Visibility.Visible;
#endif
            return;
        }

        public static bool GetBorderVisible(MainViewModel ourviewmodel)
        {
#if WINFORMS
            if (ourviewmodel.BorderVisible)
#endif
#if WPF  || WINUI || SMARTMAUI
            if (ourviewmodel.BorderVisible == Visibility.Visible)
#endif
#if ANDROIDX
            if (ourviewmodel.BorderVisible)
#endif
            {
                return true;
            }
            return false;
        }

        public static bool GetScrollViewerVisible(MainViewModel ourviewmodel)
        {
#if WINFORMS
            if (ourviewmodel.ScrollViewerVisible)
#endif
#if WPF  || WINUI || SMARTMAUI
            if (ourviewmodel.ScrollViewerVisible == Visibility.Visible)
#endif
#if ANDROIDX
            if (ourviewmodel.ScrollViewerVisible)
#endif
            {
                return true;
            }
            return false;
        }
        public static void SetBorderScrollInvisible(MainViewModel ourviewmodel)
        {
#if WINFORMS
            ourviewmodel.BorderVisible = false;
            ourviewmodel.ScrollViewerVisible = false;
#endif
#if WPF
            ourviewmodel.BorderVisible = Visibility.Hidden;
            ourviewmodel.ScrollViewerVisible = Visibility.Hidden;
#endif
#if WINUI
            ourviewmodel.BorderVisible = Visibility.Collapsed;
            ourviewmodel.ScrollViewerVisible = Visibility.Collapsed;
#endif
#if ANDROIDX
            ourviewmodel.BorderVisible = false;
            ourviewmodel.ScrollViewerVisible = false;
#endif
        }

        public static void SetBorderScrollVisible(MainViewModel ourviewmodel)
        {
#if WINFORMS
            ourviewmodel.BorderVisible = true;
            ourviewmodel.ScrollViewerVisible = true;
#endif
#if WPF  || WINUI
            ourviewmodel.BorderVisible = Visibility.Visible;
            ourviewmodel.ScrollViewerVisible = Visibility.Visible;
#endif
#if ANDROIDX
            ourviewmodel.BorderVisible = true;
            ourviewmodel.ScrollViewerVisible = true;
#endif
        }

#if WPF 
        public static Storyboard FindShrinkStoryBoard(MainMeter components)
        {
            return (Storyboard)components.FindResource("ShrinkImageStoryboard");
        }
        public static Storyboard FindExpandStoryBoard(MainMeter components)
        {
            return (Storyboard)components.FindResource("ExpandImageStoryboard");
        }
#endif
#if WINUI
        public static Storyboard FindShrinkStoryBoard(MainMeter components)
        {
            return (Storyboard)components.FindName("ShrinkImageStoryboard");
        }
        public static Storyboard FindExpandStoryBoard(MainMeter components)
        {
            return (Storyboard)components.FindName("ExpandImageStoryboard");
        }
#endif
#if ANDROIDX
        // Have ABSOLUTELY no fucking clue how to deal with this shit
        public static Animation FindShrinkStoryBoard(MainMeter process_components)
        {
            //int id0 = (int)typeof(Resource.Id).GetField("ShrinkImageStoryboard").GetValue(null);
            //return (Android.Views.Animations.Animation)process_components.FindViewById(id0);
            return null;
        }
        public static Animation FindExpandStoryBoard(MainMeter process_components)
        {
            //return (Animation)process_components.FindByName("ExpandImageStoryboard");
            return null;
        }
#endif
#if SMARTMAUI
        public static Animation FindShrinkStoryBoard(MainMeter components)
        {
            //return (Animation)components.FindByName("ShrinkImageStoryboard");
            return components.FindByName<Animation>("MeterTop");
        }
        public static Animation FindExpandStoryBoard(MainMeter components)
        {
            return (Animation)components.FindByName("ExpandImageStoryboard");
        }
#endif
#if WINFORMS
        public static void SetLedColour(MainViewModel ourviewmodel, int led, Color ledColour)
#endif
#if WPF  || WINUI
        public static void SetLedColour(MainViewModel ourviewmodel, int led, Brush ledColour)
#endif
#if ANDROIDX
        public static void SetLedColour(MainViewModel ourviewmodel, int led, Color ledColour)
#endif
#if SMARTMAUI
        public static void SetLedColour(MainViewModel ourviewmodel, int led, Color ledColour)
#endif
        {
            switch (led)
            {
                case 1:
                    ourviewmodel.Led1 = ledColour;
#if ANDROIDX
                    ourviewmodel.ALed1.SetTextColor(ourviewmodel.Led1);
#endif
                    break;
                case 2:
                    ourviewmodel.Led2 = ledColour;
#if ANDROIDX
                    ourviewmodel.ALed2.SetTextColor(ourviewmodel.Led2);
#endif
                    break;
                case 3:
                    ourviewmodel.Led3 = ledColour;
#if ANDROIDX
                    ourviewmodel.ALed3.SetTextColor(ourviewmodel.Led3);
#endif
                    break;
                case 4:
                    ourviewmodel.Led4 = ledColour;
#if ANDROIDX
                    ourviewmodel.ALed4.SetTextColor(ourviewmodel.Led4);
#endif
                    break;
                case 5:
                    ourviewmodel.Led5 = ledColour;
#if ANDROIDX
                    ourviewmodel.ALed5.SetTextColor(ourviewmodel.Led5);
#endif
                    break;
                case 6:
                    ourviewmodel.Led6 = ledColour;
#if ANDROIDX
                    ourviewmodel.ALed6.SetTextColor(ourviewmodel.Led6);
#endif
                    break;
                case 7:
                    ourviewmodel.Led7 = ledColour;
#if ANDROIDX
                    ourviewmodel.ALed7.SetTextColor(ourviewmodel.Led7);
#endif
                    break;
                case 8:
                    ourviewmodel.Led8 = ledColour;
#if ANDROIDX
                    ourviewmodel.ALed8.SetTextColor(ourviewmodel.Led8);
#endif
                    break;
                default:
                    break;
            }
            return;
        }

#if WINFORMS
        public static bool CompareLedColour(MainViewModel ourviewmodel, int led, Color ledColour)
#endif
#if WPF  || WINUI
        public static bool CompareLedColour(MainViewModel ourviewmodel, int led, Brush ledColour)
#endif
#if ANDROIDX
        public static bool CompareLedColour(MainViewModel ourviewmodel, int led, Color ledColour)
#endif
#if SMARTMAUI
        public static bool CompareLedColour(MainViewModel ourviewmodel, int led, Color ledColour)
#endif
        {
            switch (led)
            {
                case 1:
#if WINFORMS
                    if (ourviewmodel.Led1 == ledColour)
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (ourviewmodel.Led1.ToString() == ledColour.ToString())
#endif
#if ANDROIDX
                        if (ourviewmodel.Led1 == ledColour)
#endif
                    {
                        return true;
                    }
                    break;
                case 2:
#if WINFORMS
                    if (ourviewmodel.Led2 == ledColour)
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (ourviewmodel.Led2.ToString() == ledColour.ToString())
#endif
#if ANDROIDX
                        if (ourviewmodel.Led2 == ledColour)
#endif
                    {
                        return true;
                    }
                    break;
                case 3:
#if WINFORMS
                    if (ourviewmodel.Led3 == ledColour)
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (ourviewmodel.Led3.ToString() == ledColour.ToString())
#endif
#if ANDROIDX
                        if (ourviewmodel.Led3 == ledColour)
#endif
                    {
                        return true;
                    }
                    break;
                case 4:
#if WINFORMS
                    if (ourviewmodel.Led4 == ledColour)
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (ourviewmodel.Led4.ToString() == ledColour.ToString())
#endif
#if ANDROIDX
                        if (ourviewmodel.Led4 == ledColour)
#endif
                    {
                        return true;
                    }
                    break;
                case 5:
#if WINFORMS
                    if (ourviewmodel.Led5 == ledColour)
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (ourviewmodel.Led5.ToString() == ledColour.ToString())
#endif
#if ANDROIDX
                        if (ourviewmodel.Led5 == ledColour)
#endif
                    {
                        return true;
                    }
                    break;
                case 6:
#if WINFORMS
                    if (ourviewmodel.Led6 == ledColour)
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (ourviewmodel.Led6.ToString() == ledColour.ToString())
#endif
#if ANDROIDX
                        if (ourviewmodel.Led6 == ledColour)
#endif
                    {
                        return true;
                    }
                    break;
                case 7:
#if WINFORMS
                    if (ourviewmodel.Led7 == ledColour)
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (ourviewmodel.Led7.ToString() == ledColour.ToString())
#endif
#if ANDROIDX
                    if (ourviewmodel.Led7 == ledColour)
#endif
                    {
                        return true;
                    }
                    break;
                case 8:
#if WINFORMS
                    if (ourviewmodel.Led8 == ledColour)
#endif
#if WPF  || WINUI || SMARTMAUI
                    if (ourviewmodel.Led8.ToString() == ledColour.ToString())
#endif
#if ANDROIDX
                    if (ourviewmodel.Led8 == ledColour)
#endif
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        // Don't do it like this anymore ... WRONG. I think we do
        // Now we have ONE language across ALL screens

        public static void SetFinanceLanguage(FinanceViewModel financeviewmodel, CultureInfo culture_info)
        {
#if WPF
            financeviewmodel.FinanceLanguage = XmlLanguage.GetLanguage(culture_info.Name);
#endif
            // This needs testing!!!!
#if WINUI || SMARTMAUI
            financeviewmodel.FinanceLanguage = financeviewmodel.CultureINF.Name;

            //financeviewmodel.FinanceLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
#endif
            return;
        }

        // Don't do it like this anymore ... Wrong think we do
        // Now we have ONE language across ALL screens

        public static void SetUtilityLanguage(UtilityViewModel utilityviewmodel, CultureInfo culture_info)
        {
#if WPF
            utilityviewmodel.UtilityLanguage = XmlLanguage.GetLanguage(culture_info.Name);//   utilityviewmodel.utility_displayCulture.Name);
#endif
#if WINUI || SMARTMAUI
            // So does THIS!!!!!
            utilityviewmodel.UtilityLanguage = utilityviewmodel.CultureINF.Name;
            //utilityviewmodel.UtilityLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
#endif
            return;
        }

        public static int DecodeSenderIndex(object sender)
        {
#if WINFORMS || WPF  || WINUI
            ComboBox whatever = (ComboBox)sender;
            return whatever.SelectedIndex;
#endif
#if ANDROIDX
            Spinner whatever = (Spinner)sender;
            return whatever.SelectedItemPosition;
#endif
#if SMARTMAUI
            Picker whatever = (Picker)sender;
            return whatever.SelectedIndex;
#endif
        }

        public static UtilityViewModel.SupplierItem DecodeSupplierItem(object sender)
        {
#if WINFORMS || WPF  || WINUI
            ComboBox whatever = (ComboBox)sender;
            return (UtilityViewModel.SupplierItem)whatever.SelectedItem;
#endif
#if ANDROIDX
            Spinner whatever = (Spinner)sender;
            object selectedItem = whatever.SelectedItem;
            return selectedItem as UtilityViewModel.SupplierItem;
#endif
#if SMARTMAUI
            Picker whatever = (Picker)sender;
            return (UtilityViewModel.SupplierItem)whatever.SelectedItem;
#endif
        }

        public static FinanceViewModel.InstitutionItem DecodeInstitutionItem(object sender)
        {
#if WINFORMS || WPF  || WINUI
            ComboBox whatever = (ComboBox)sender;
            return (FinanceViewModel.InstitutionItem)whatever.SelectedItem;
#endif
#if ANDROIDX
            Spinner whatever = (Spinner)sender;
            object selectedItem = whatever.SelectedItem;
            return selectedItem as FinanceViewModel.InstitutionItem;
#endif
#if SMARTMAUI
            Picker whatever = (Picker)sender;
            return (FinanceViewModel.InstitutionItem)whatever.SelectedItem;
#endif
        }

        public static string DecodeLabel(object sender)
        {
            string content = "";
#if WINFORMS || WPF
            Label whatever = (Label)sender;
#if WINFORMS
            content = whatever.Text;
#endif
#if WPF
            content = whatever.Content.ToString();
#endif
#endif
#if WINUI
            TextBlock whatever = (TextBlock)sender;
            content = whatever.Text.ToString();
#endif
#if ANDROIDX
            TextView whatever = (TextView)sender;
            content = whatever.Text.ToString();
#endif
#if SMARTMAUI
            Label whatever = (Label)sender;
            content = whatever.Text;
#endif
            return content;
        }

        // No Slider in ANDROID yet!!
        // Lets just get the fucking thing COMPILED first, can we??!?
#if WPF  || WINUI || SMARTMAUI
        public static Slider DecodeSlider(object sender)
        {
            // No Slider in WINFORMS or XAMARSHIT??? Maybe??!??

            Slider whatever = (Slider)sender;
            return whatever;
        }
#endif

        public static void ClearViewCollection(MainViewModel ourviewmodel)
        {
#if WINFORMS
            ourviewmodel.MyControls = new List<UserControl>();
#endif
#if WPF
            ourviewmodel.viewCollection = new List<SmartUsers.ConsumerViews>();
#endif
#if WINUI || SMARTMAUI
            ourviewmodel.viewCollection = new List<SmartUsers.ConsumerViews>();
#endif
#if ANDROIDX
            ourviewmodel.viewCollection = new List<SmartUsers.ConsumerViews>();
#endif
            return;
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        //public static string DecodeSpinner(ComboBox spinner)
        //{
        //    //ComboBox whatever = (ComboBox)sender;
        //    int index = spinner.SelectedIndex;
        //    MainViewModel.CultureItem selected = (MainViewModel.CultureItem)spinner.Items[index];
        //    return selected.Content;
        //}
#endif

        public static bool DecodeRadioButton(object sender)
        {
            RadioButton whatever = (RadioButton)sender;
#if WINFORMS
            return (bool)whatever.Checked;
#endif
#if WPF  || WINUI || SMARTMAUI
            return (bool)whatever.IsChecked;
#endif
#if ANDROIDX
            return (bool)whatever.Checked;
#endif
        }

        public static string DecodeRadioTag(object sender)
        {
            RadioButton whatever = (RadioButton)sender;
#if WINFORMS
            return "";
#endif
#if WPF  || WINUI
            return whatever.Tag.ToString();
#endif
#if ANDROIDX
            return whatever.Tag.ToString();
#endif
#if SMARTMAUI
            return whatever.Value.ToString(); // FOR THE TIME BEING AS NO TAG
#endif

        }

        public static string DecodeRadioContent(object sender)
        {
            RadioButton whatever = (RadioButton)sender;
#if WINFORMS
            return whatever.Text.ToString();
#endif
#if WPF  || WINUI || SMARTMAUI
            return whatever.Content.ToString();
#endif
#if ANDROIDX
            return whatever.Text.ToString();
#endif
        }

        internal static string[] DecodeTabControlUtility(UtilityView components, object sender)
        {
            string[] tabinfo = new string[2] { "", "" };
#if WINFORMS
            TabControl whatever = (TabControl)sender;
            TabPage tabPage = (TabPage)whatever.SelectedTab;
            tabinfo[0] = tabPage.Tag.ToString();
            tabinfo[1] = tabPage.Text.ToString();
            return tabinfo;
#endif
#if WPF
            TabControl whatever = (TabControl)components.FindName("ChartsTab");
            TabItem tabItem = (TabItem)whatever.SelectedItem;
            tabinfo[0] = tabItem.Tag.ToString();
            tabinfo[1] = tabItem.Header.ToString();
            return tabinfo;
#endif
#if WINUI
            TabView whatever = (TabView)components.FindName("ChartsTab");
            TabViewItem tabItem = (TabViewItem)whatever.SelectedItem;
            tabinfo[0] = tabItem.Tag.ToString();
            tabinfo[1] = tabItem.Header.ToString();
            return tabinfo;
#endif
#if SMARTMAUI
            TabbedPage tabbed = components
                .GetVisualTreeDescendants()
                .OfType<TabbedPage>()
                .FirstOrDefault();
            Page chartsPage = tabbed.Children
                 .FirstOrDefault(p => p.Title == "ChartsTab");

            tabinfo[0] = "TabId";// chartsPage.TabId;   // replaces Tag
            tabinfo[1] = chartsPage.Title;   // replaces Header
            return tabinfo;
#endif
#if ANDROIDX
            // Don't have a fucking CLUE how to do this
            //TabHost whatever = (TabHost)sender;
            ////TabbedPage tabItem = (TabbedPage)whatever.SelectedItem;
            //string tabItem = "No fucking idea";
            //tabinfo[0] = tabItem.ToString();
            //tabinfo[1] = tabItem.ToString();
            return new string[0];// tabinfo;
#endif
        }

        internal static string[] DecodeTabControlFinance(FinanceView components, object sender)
        {
            string[] tabinfo = new string[2] { "", "" };

#if WINFORMS
            TabControl whatever = (TabControl)sender;
            TabPage tabPage = (TabPage)whatever.SelectedTab;
            tabinfo[0] = tabPage.Tag.ToString();
            tabinfo[1] = tabPage.Text.ToString();
            return tabinfo;
#endif
#if WPF
            TabControl whatever = (TabControl)components.FindName("ChartsTab");
            TabItem tabItem = (TabItem)whatever.SelectedItem;
            tabinfo[0] = tabItem.Tag.ToString();
            tabinfo[1] = tabItem.Header.ToString();
            return tabinfo;
#endif
#if WINUI
            TabView whatever = (TabView)components.FindName("ChartsTab");
            TabViewItem tabItem = (TabViewItem)whatever.SelectedItem;
            tabinfo[0] = tabItem.Tag.ToString();
            tabinfo[1] = tabItem.Header.ToString();
            return tabinfo;
#endif
#if ANDROIDX
            // Don't have a fucking CLUE how to do this
            //TabHost whatever = (TabHost)sender;
            ////TabbedPage tabItem = (TabbedPage)whatever.SelectedItem;
            //string tabItem = "No fucking idea";
            //tabinfo[0] = tabItem.ToString();
            //tabinfo[1] = tabItem.ToString();
            return new string[0];// tabinfo;
#endif
#if SMARTMAUI
            TabbedPage tabbed = components
                .GetVisualTreeDescendants()
                .OfType<TabbedPage>()
                .FirstOrDefault();
            Page chartsPage = tabbed.Children
                 .FirstOrDefault(p => p.Title == "ChartsTab");

            tabinfo[0] = "TabId";// chartsPage.TabId;   // replaces Tag
            tabinfo[1] = chartsPage.Title;   // replaces Header
            return tabinfo;
#endif
        }

        public static string DecodeTabLabel(object sender)
        {
#if WINFORMS
            TabControl tabcontrol = (TabControl)sender;
            TabPage tabPage = tabcontrol.SelectedTab;
            return tabPage.Text;
#endif
#if WPF
            TabItem tablabel = (TabItem)sender;
            return tablabel.Header.ToString();
#endif
#if WINUI
            // Not sure whether ANY of this works ...
            TabViewItem tablabel = (TabViewItem)sender;
            return tablabel.Header.ToString();
#endif
#if ANDROIDX
            return "No fucking idea whatsoever";
#endif
#if SMARTMAUI
            Page tablabel = (Page) sender;
            return tablabel.Title;  // Not the header I'm afraid            
#endif
        }

#if WINFORMS
        public static DataGridView DecodeDataGrid(object sender)
        {

            DataGridView whatever = (DataGridView)sender;
            return whatever;
        }
#endif

#if WPF
        public static DataGrid DecodeDataGrid(object sender)
        {
            DataGrid whatever = (DataGrid)sender;
            return whatever;
        }
#endif

#if WINUI
        public static DataGrid DecodeDataGrid(object sender)
        {
            DataGrid whatever = (DataGrid)sender;
            return whatever; //.CurrentColumn;
        }
#endif
#if SMARTMAUI
        public static CollectionView DecodeDataGrid(object sender)
        {
            CollectionView whatever = (CollectionView)sender;
            return whatever; //.CurrentColumn;
        }
#endif
#if ANDROIDX
        public static GridView DecodeDataGrid(object sender)
        {
            GridView whatever = (GridView)sender;
            return whatever;
        }
#endif


#if ANDROIDX
        public static PlotView DecodePlotView(UtilityView components, string header)
        {
            int id0 = (int)typeof(Resource.Id).GetField("CostsPlotView").GetValue(null);
            return (PlotView)components.FindViewById(id0);
        }
#endif

        public static DateTime DecodeDatePicker(object sender)
        {
#if WINFORMS
            DateTimePicker whatever = (DateTimePicker)sender;
            return whatever.Value;
#endif
#if WPF
            DatePicker whatever = (DatePicker)sender;
            return (DateTime)whatever.SelectedDate;
#endif
#if WINUI
            DatePicker whatever = (DatePicker)sender;
            // Thanks to Zen of Kursat https://stackoverflow.com/questions/40702334/how-do-i-get-the-value-from-the-WINUI-datepicker-control
            return whatever.Date.DateTime; // <= DateTimeOffset ... doh! Chimps!!!
#endif
#if ANDROIDX
            Android.Widget.DatePicker andwhatever = (Android.Widget.DatePicker)sender;
            return andwhatever.DateTime;
#endif
#if SMARTMAUI
            DatePicker whatever = (DatePicker)sender;
            // Thanks to Zen of Kursat https://stackoverflow.com/questions/40702334/how-do-i-get-the-value-from-the-WINUI-datepicker-control
            return whatever.Date;
#endif
        }

#if WPF
        public static Boolean DecodeKeyEventArgs(object args)
        {
            KeyEventArgs whatever = args as KeyEventArgs;
            if (whatever.Key == Key.Right)
            {
                // Go right
                return true;
            }
            else
            {
                if (whatever.Key == Key.Left)
                {
                    return false;
                }
            }
            return false;
        }
#endif

        public static void DecodeClosePopup(
#if WINFORMS
                                            object dummy
#endif
#if WPF
                                            Popup whatever
#endif
#if WINUI
                                            ContentDialog whatever
#endif
#if ANDROIDX
                                            PopupWindow whatever
#endif
#if SMARTMAUI
                                            VisualElement whatever
#endif
                                            )
        {
#if WPF
            whatever.IsOpen = false;
#endif
#if WINUI
            whatever.Hide();
#endif
#if ANDROIDX
            whatever.Dismiss();
#endif
#if SMARTMAUI
            whatever.IsVisible = false;
#endif
            return;
        }

#if WPF
        internal static bool DecodeMessageBox(MainMeter components, QuitMessageBox messagebox_page)
        {
#endif
#if WINUI
        internal static async Task<bool> DecodeMessageBox(QuitMessageBox messagebox_page)
        {
#endif
#if WPF
            // Took me ALL DAY to get this fucker working ...
            //messagebox_page.PlacementTarget = components; // this;
            //messagebox_page.Placement = FrontEndGUI.SetPlacement();
            //messagebox_page.IsOpen = true;
            messagebox_page.Owner = Application.Current.MainWindow;
            messagebox_page.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            messagebox_page.Show();
#endif
#if WINUI
            await messagebox_page.ShowAsync();
#endif
#if WPF  || WINUI
            return true;
        }
#endif

#if WINFORMS
        public static void DecodeDataGridViewUsername(DataGridView dataGridView, bool show)
        {
            if (show)
            {
                // Turn the Username column on             
                dataGridView.Columns[0].Width = 60;
                dataGridView.Columns[0].Visible = true;
            }
            else
            {
                // Turn the Username column off                
                dataGridView.Columns[0].Width = 0;
                dataGridView.Columns[0].Visible = false;
            }
            return;
        }
#endif
        //public static void DecodeDataGridViewSortAccount(DataGrid dataGridView, bool show)
        //{
        //    foreach (DataGridColumn col in dataGridView.Columns)
        //    {
        //        if (col.Header.ToString() == "Sort Code" ||
        //            col.Header.ToString() == "Account No")
        //        {
        //            // Turn the Sort Code and Account No columns on or off
        //            if (show)
        //            {
        //                col.Visibility = Visibility.Visible;
        //            }
        //            else
        //            {
        //                col.Visibility = Visibility.Collapsed;
        //            }

        //        }
        //    }
        //    return;
        //}

#if WPF || WINUI
        public static void DecodeDataGridViewUsername(DataGrid dataGridView, bool show)
        {
            if (show)
            {
                // Turn the Username column on
#if WPF
                dataGridView.Columns[0].Width = 60;
#endif
                dataGridView.Columns[0].Visibility = Visibility.Visible;
            }
            else
            {
                // Turn the Username column off
#if WPF
                dataGridView.Columns[0].Width = 0;
#endif
#if WPF
                dataGridView.Columns[0].Visibility = Visibility.Hidden;
#endif
#if WINUI
                dataGridView.Columns[0].Visibility = Visibility.Collapsed;
#endif
            }
            return;
        }
#endif

#if SMARTMAUI
        public static void DecodeDataGridViewUsername(UtilityViewModel utilityviewmodel,
                                                        bool show)
        {
            if (show)
            {
                // Turn the Username column on
                //dataGridView.Columns[0].Width = 60;
                //dataGridView.Columns[0].Visibility = Visibility.Visible;
                utilityviewmodel.ShowUsernameCosts = true;
            }
            else
            {
                // Turn the Username column off
                //dataGridView.Columns[0].Width = 0;
                //dataGridView.Columns[0].Visibility = Visibility.Hidden;
                //dataGridView.Columns[0].Visibility = Visibility.Collapsed;
                utilityviewmodel.ShowUsernameCosts = false;
            }
            return;
        }        
#endif

#if WPF  || WINUI || SMARTMAUI
        public static void DecodeListViewUsername(FinanceViewModel financeviewmodel, bool show)
        {
            if (show)
            {
#if WPF
                if (!financeviewmodel.FuckingGrid.Columns[0].Header.ToString().Contains("User"))
                {
                    // Add the copied column into position 0 of the Columns collection
                    financeviewmodel.FuckingGrid.Columns.Insert(0, financeviewmodel.FuckingGridColumn);

                }
#endif
            }
            else
            {
#if WPF
                if (financeviewmodel.FuckingGrid.Columns.Count > 0)
                {
                    // Remove the first column from the Columns collection
                    financeviewmodel.FuckingGrid.Columns.RemoveAt(0);
                }
#endif
            }
            return;
        }
        public static void DecodeListViewSortAccount(FinanceViewModel financeviewmodel, bool show)
        {
            if (show)
            {
#if SMARTMAUI
                financeviewmodel.SortAccountVisible = true;
#else
                financeviewmodel.SortAccountVisible = Visibility.Visible;
#endif
            }
            else
            {
#if SMARTMAUI
                financeviewmodel.SortAccountVisible = false;
#else
                financeviewmodel.SortAccountVisible = Visibility.Collapsed;
#endif
            }
            return;
        }
#endif

                //#if WINFORMS || WPF  || WINUI
                //        public static void DecodeGroupsDataGrid()
                //        {
                //#if WINFORMS
                //            foreach (DataGridColumn col in ourviewmodel.GroupsDataGrid.Columns)
                //#endif
                //#if WPF  || WINUI
                //            foreach (DataGridColumn col in ourviewmodel.GroupsDataGrid.Columns)
                //#endif
                //            //#if ANDROIDX            
                //            //            foreach (GridViewColumn col in ourviewmodel.GroupsDataGrid.)
                //            //#endif

                //            {
                //                if (col.Header.ToString() == "Receive")
                //                {
                //                    // Turn the Sort Code and Account No columns on or off
                //                    if (ourviewmodel.multimeter)
                //                    {
                //#if WINFORMS || WPF
                //                        col.Visibility = System.Windows.Visibility.Visible;
                //#endif
                //#if ANDROIDX
                //                        col.Visibility = ViewStates.Visible;
                //#endif
                //#if WINUI
                //                        col.Visibility = Visibility.Visible;
                //#endif
                //                    }
                //                    else
                //                    {
                //#if WINFORMS || WPF
                //                        col.Visibility = System.Windows.Visibility.Hidden;
                //#endif
                //#if ANDROIDX
                //                        col.Visibility = ViewStates.Invisible;
                //#endif
                //#if WINUI
                //                        col.Visibility = Visibility.Collapsed;
                //#endif
                //                    }
                //                    break;
                //                }
                //            }
                //            return;
                //        }
                //#endif

                //public static void DecodeTransactionsUsername(bool show)
                //{
                //    // USERNAME is always index 0
                //    if (show)
                //    {
                //        financeviewmodel.TransactionsDataGrid.Columns[0].Visibility = Visibility.Visible;
                //    }
                //    else
                //    {
                //        financeviewmodel.TransactionsDataGrid.Columns[0].Visibility = Visibility.Hidden;
                //    }
                //    return;
                //}


#if WINUI
        public static void DecodeDataGridViewSortAccount(FinanceViewModel financeviewmodel, bool show)
        {
            var nameColumn = financeviewmodel.Datagrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Sort Code");
            if (nameColumn != null)
            {
                if (show)
                {
                    nameColumn.Width = new DataGridLength(60);  // Set a fixed width
                }
                else
                {
                    nameColumn.Width = new DataGridLength(0);
                }
            }
            nameColumn = financeviewmodel.Datagrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Account No");
            if (nameColumn != null)
            {
                if (show)
                {
                    nameColumn.Width = new DataGridLength(60);  // Set a fixed width
                }
                else
                {
                    nameColumn.Width = new DataGridLength(0);
                }
            }
            return;
        }
#endif

#if SMARTMAUI
        public static void DecodeCollectionViewSortAccount(
                                                            List<SmartRoutinesV2018.ColumnDefinitionModel> columns,
                                                            bool show)
        {
            SmartRoutinesV2018.ColumnDefinitionModel nameColumn =
                columns.FirstOrDefault(c => c.Header == "Sort Code");

            if (nameColumn != null)
            {
                if (show)
                {
                    nameColumn.Visible = Visibility.Visible;
                }
                else
                {
                    nameColumn.Visible = Visibility.Hidden;
                }
            }

            nameColumn =
                columns.FirstOrDefault(c => c.Header == "Account No");

            if (nameColumn != null)
            {
                if (show)
                {
                    nameColumn.Visible = Visibility.Visible;
                }
                else
                {
                    nameColumn.Visible = Visibility.Hidden;
                }
            }
            return;
        }

#endif
#if WINUI
        public static void DecodeDataGridViewUsername(FinanceViewModel financeviewmodel, bool show)
        {
            // Yes, I know, these ARE the other way around
            var nameColumn = financeviewmodel.Datagrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Username");
            if (nameColumn != null)
            {
                if (show)
                {
                    nameColumn.Width = new DataGridLength(0);  // Set a fixed width
                }
                else
                {
                    nameColumn.Width = new DataGridLength(60);
                }
            }
            return;
        }
#endif

#if SMARTMAUI
        public static void DecodeCollectionViewUsername(
                                                        List<SmartRoutinesV2018.ColumnDefinitionModel> columns,
                                                        bool show)
        {
            // Yes, these ARE intentionally reversed
            var nameColumn =
                columns.FirstOrDefault(c => c.Header == "Username");

            if (nameColumn != null)
            {
                if (show)
                {
                    nameColumn.Visible = Visibility.Visible;
                }
                else
                {
                    nameColumn.Visible= Visibility.Hidden;
                }
            }
        }
#endif
        public class RoutedEventArgs
        {
        }
    }
}