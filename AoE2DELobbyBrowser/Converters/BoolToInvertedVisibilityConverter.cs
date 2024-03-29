﻿using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;

namespace AoE2DELobbyBrowser.Converters
{
    public class BoolToInvertedVisibilityConverter : BoolToObjectConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        public BoolToInvertedVisibilityConverter()
        {
            TrueValue = Visibility.Collapsed;
            FalseValue = Visibility.Visible;
        }
    }
}
