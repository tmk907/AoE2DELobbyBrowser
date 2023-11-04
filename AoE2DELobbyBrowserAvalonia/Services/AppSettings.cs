﻿using System;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    internal class AppSettings
    {
        //private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        private static TimeSpan _newLobbyHighlightTime = TimeSpan.Zero;
        public static TimeSpan NewLobbyHighlightTime
        {
            get
            {
                if (_newLobbyHighlightTime == TimeSpan.Zero)
                {
                    //var seconds = (int)(_localSettings.Values[nameof(NewLobbyHighlightTime)] ?? 30);
                    //_newLobbyHighlightTime = TimeSpan.FromSeconds(seconds);
                }
                return _newLobbyHighlightTime;
            }
            set
            {
                _newLobbyHighlightTime = value;
                //_localSettings.Values[nameof(NewLobbyHighlightTime)] = (int)value.TotalSeconds;
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
                    _joinLinkType = JoinLink.Aoe2de;
                    //_joinLinkType = (JoinLink)(_localSettings.Values[nameof(JoinLinkType)] ?? 0);
                }
                return _joinLinkType.Value;
            }
            set
            {
                _joinLinkType = value;
                //_localSettings.Values[nameof(JoinLinkType)] = (int)value;
            }
        }

        private static string _separator = null;
        public static string Separator
        {
            get
            {
                if (_separator == null)
                {
                    _separator = string.Empty;
                    //_separator = (string)(_localSettings.Values[nameof(Separator)] ?? "/");
                }
                return _separator;
            }
            set
            {
                _separator = value;
                //_localSettings.Values[nameof(Separator)] = value;
            }
        }
    }
}
