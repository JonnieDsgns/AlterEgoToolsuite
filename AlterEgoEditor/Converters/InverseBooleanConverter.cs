using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AlterEgoEditor.Converters;
public class InverseBooleanConverter : IValueConverter
{
    // Converts the source value (bool) to the target value (inverted bool)
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // We ensure the input is a boolean before casting
        if (value is bool booleanValue)
        {
            return !booleanValue; // This is the core logic: True becomes False, False becomes True
        }

        // Return original value or throw an error if the type is incorrect
        return value;
    }

    // Converts back (used for TwoWay bindings, not strictly needed here but required by the interface)
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Since the conversion is symmetrical, we just return the inverted value again
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }

        return value;
    }
}
