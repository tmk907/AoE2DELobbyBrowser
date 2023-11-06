using CommunityToolkit.Mvvm.DependencyInjection;
using System;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public enum JoinLinkEnum
    {
        Aoe2de,
        Steam
    }

    internal class AppSettings
    {
        private static TimeSpan _newLobbyHighlightTime = TimeSpan.Zero;
        public static TimeSpan NewLobbyHighlightTime
        {
            get
            {
                if (_newLobbyHighlightTime == TimeSpan.Zero)
                {
                    var settings = Ioc.Default.GetRequiredService<IAppSettings>();
                    _newLobbyHighlightTime = TimeSpan.FromSeconds(settings.NewLobbyHighlightTime ?? 30);
                }
                return _newLobbyHighlightTime;
            }
            set
            {
                _newLobbyHighlightTime = value;
                Ioc.Default.GetRequiredService<IAppSettings>().NewLobbyHighlightTime = (int)value.TotalSeconds;
            }
        }

        private static JoinLinkEnum? _joinLinkType = null;
        public static JoinLinkEnum JoinLinkType
        {
            get
            {
                if (_joinLinkType is null)
                {
                    _joinLinkType = Ioc.Default.GetRequiredService<IAppSettings>().JoinLinkType;
                }
                return _joinLinkType.Value;
            }
            set
            {
                _joinLinkType = value;
                Ioc.Default.GetRequiredService<IAppSettings>().JoinLinkType = value;
            }
        }

        private static string _separator = null;
        public static string Separator
        {
            get
            {
                if (_separator == null)
                {
                    _separator = Ioc.Default.GetRequiredService<IAppSettings>().Separator ?? "/";
                }
                return _separator;
            }
            set
            {
                _separator = value;
                Ioc.Default.GetRequiredService<IAppSettings>().Separator = value;
            }
        }
    }
}
