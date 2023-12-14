using Avalonia.Controls.Templates;
using Avalonia.Controls;
using System;
using AoE2DELobbyBrowser.Core.ViewModels;

namespace AoE2DELobbyBrowserAvalonia
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            if (data is null)
            {
                return new TextBlock { Text = "data was null" };
            }

            var name = data.GetType().FullName!
                .Replace("ViewModel", "View")
                .Replace("AoE2DELobbyBrowser.Core", "AoE2DELobbyBrowserAvalonia");
            var type = Type.GetType(name);
            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object? data)
        {
            return data is INavigableViewModel;
        }
    }
}
