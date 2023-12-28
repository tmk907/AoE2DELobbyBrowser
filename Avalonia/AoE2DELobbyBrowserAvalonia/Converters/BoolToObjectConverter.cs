
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AoE2DELobbyBrowserAvalonia.Converters
{
    public class BoolToObjectConverter: IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }


        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value as bool? ?? false)
            {
                return TrueValue;
            }
            else
            {
                return FalseValue;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
