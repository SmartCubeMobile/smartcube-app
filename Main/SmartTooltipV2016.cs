using System;
using System.Collections.Generic;
using System.Globalization;


#if WINFORMS
using System.Windows.Data;
#endif

#if WPF
using System.Windows.Data;
#endif

#if UWP
using Windows.UI.Xaml.Data;
#endif

#if WINUI
using Microsoft.UI.Xaml.Data;
#endif

#if ANDROID

#endif

namespace SmartCubeMobile
{
    // All this bollocks just to display the Meter Serial number
    // The ToolTipConverter is defind in the App.xaml file
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
    public class ToolTipConverter : IValueConverter
#endif
#if ANDROID
    public class ToolTipConverter
#endif
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Some comment
            if (value != null)
            {
                KeyValuePair<DateTime, KeyValuePair<decimal, string>> datapoint = (KeyValuePair<DateTime, KeyValuePair<decimal, string>>)value;
                string[] components = datapoint.Value.Value.Trim('[').Trim(']').Split(SmartParametersV2016.bar);
                string tooltip = string.Empty;
                int comp_count = 0;
                while (comp_count < components.Length)
                {
                    switch (comp_count)
                    {
                        case 0:
                            decimal rays = SmartRoutinesV2018.ConvertDecimal(components[comp_count]) / 100;
                            tooltip += string.Format(SmartParametersV2016.currencyFormat, rays);
                            break;
                        case 1:
                            tooltip += Environment.NewLine + components[comp_count];
                            break;
                        default:
                            break;
                    }
                    comp_count++;
                }
                return tooltip;
            }
            return string.Empty;
        }

        public object Convert(object value, Type targetType, object parameter, string language, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#if UWP || WINUI
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
#endif
        public object ConvertBack(object value, Type targetType, object parameter, string language, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#if UWP || WINUI
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
#endif
    }
}