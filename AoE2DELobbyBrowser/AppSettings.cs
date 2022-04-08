using System;
using Windows.Storage;

namespace AoE2DELobbyBrowser
{
    internal class AppSettings
    {
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        private static TimeSpan _newLobbyHighlightTime = TimeSpan.Zero;
        public static TimeSpan NewLobbyHighlightTime 
        {
            get 
            {
                if (_newLobbyHighlightTime == TimeSpan.Zero)
                {
                    var seconds = (int)(_localSettings.Values[nameof(NewLobbyHighlightTime)] ?? 30);
                    _newLobbyHighlightTime = TimeSpan.FromSeconds(seconds);
                }
                return _newLobbyHighlightTime;
            }
            set
            {
                _newLobbyHighlightTime = value;
                _localSettings.Values[nameof(NewLobbyHighlightTime)] = (int)value.TotalSeconds;
            } 
        }
    }
}
