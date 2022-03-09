using Microsoft.UI.Xaml.Data;
using System;

namespace AoE2DELobbyBrowser.Converters
{
    internal class DateTimeToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dt = (DateTime)value;
            if (dt == DateTimeOffset.FromUnixTimeSeconds(0).ToLocalTime().DateTime || dt == DateTime.MinValue) return "";
            return dt.ToShortTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
