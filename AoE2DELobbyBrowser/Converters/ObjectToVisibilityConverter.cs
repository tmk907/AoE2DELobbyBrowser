using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;

namespace AoE2DELobbyBrowser.Converters
{
    public class ObjectToVisibilityConverter : EmptyObjectToObjectConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        public ObjectToVisibilityConverter()
        {
            EmptyValue = Visibility.Collapsed;
            NotEmptyValue = Visibility.Visible;
        }
    }
}
