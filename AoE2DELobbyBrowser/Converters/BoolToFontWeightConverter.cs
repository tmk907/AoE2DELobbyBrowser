using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Text;

namespace AoE2DELobbyBrowser.Converters
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
