using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AlterEgoEditor.Converters;
public class BooleanToVisibilityConverter : IValueConverter
{
    // True -> Visible, False -> Collapsed
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            // If the boolean is True, return Visibility.Visible. Otherwise, return Visibility.Collapsed (hidden).
            return booleanValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Visible -> True, Collapsed -> False
        return (value is Visibility visibility) && visibility == Visibility.Visible;
    }
}
