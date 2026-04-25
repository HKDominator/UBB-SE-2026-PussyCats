using System;
using Microsoft.UI.Xaml.Data;

namespace PussyCatsApp.Converters
{
    public class ObjectToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue && parameter is string parameterString && int.TryParse(parameterString, out int parameterInt))
            {
                return intValue == parameterInt;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isChecked && isChecked && parameter is string parameterString && int.TryParse(parameterString, out int parameterInt))
            {
                return parameterInt;
            }
            return null;
        }
    }
}
