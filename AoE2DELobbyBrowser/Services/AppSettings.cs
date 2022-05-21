using System;
using Windows.Storage;

namespace AoE2DELobbyBrowser.Services
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

        public enum JoinLink
        {
            Aoe2de,
            Steam
        }

        private static JoinLink? _joinLinkType = null;
        public static JoinLink JoinLinkType
        {
            get
            {
                if (_joinLinkType is null)
                {
                    _joinLinkType = (JoinLink)(_localSettings.Values[nameof(JoinLinkType)] ?? 0);
                }
                return _joinLinkType.Value;
            }
            set
            {
                _joinLinkType = value;
                _localSettings.Values[nameof(JoinLinkType)] = (int)value;
            }
        }
    }
}
