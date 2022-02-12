using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.UI.Text;

namespace AoE2DELobbyNotifications.Converters
{
    public class BoolToFontWeightConverter : BoolToObjectConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        public BoolToFontWeightConverter()
        {
            TrueValue = FontWeights.Bold;
            FalseValue = FontWeights.Normal;
        }
    }
}
