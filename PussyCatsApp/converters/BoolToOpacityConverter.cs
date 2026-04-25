using System;
using Microsoft.UI.Xaml.Data;

namespace PussyCatsApp.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Parameter should be in format "disabledOpacity|enabledOpacity"
            // e.g., "0.6|1.0"
            const double disabledOpacityDefaultValue = 0.5;
            const double enabledOpacityDefaultValue = 1.0;

            const double selectedDisabledOpacityValue = 0.5;
            const double selectedEnabledOpacityValue = 1.0;
            const char delimiterCharacter = '|';

            string[] opacities = parameter?.ToString().Split(delimiterCharacter) ?? new[] { selectedDisabledOpacityValue.ToString(), selectedEnabledOpacityValue.ToString() };

            double disabledOpacity = double.TryParse(opacities[0], out var parsedDisabledOpacity) ? parsedDisabledOpacity : disabledOpacityDefaultValue;
            double enabledOpacity = opacities.Length > 1 && double.TryParse(opacities[1], out var parsedEnabledOpacity) ? parsedEnabledOpacity : enabledOpacityDefaultValue;

            if (value is bool boolValue)
            {
                return boolValue ? disabledOpacity : enabledOpacity;
            }
            return enabledOpacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
