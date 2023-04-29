using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            rootFrame.Navigate(typeof(MainPage));
            Title = Windows.ApplicationModel.Package.Current.DisplayName;
            SetIcon();

            WeakReferenceMessenger.Default.Register<NavigateToMessage>(this, (r, m) => NavigateTo(m));
            WeakReferenceMessenger.Default.Register<NavigateBackMessage>(this, (r, m) => GoBack());

            this.Content.ProcessKeyboardAccelerators += Content_ProcessKeyboardAccelerators;
        }

        private void SetIcon()
        {
            var iconPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets/appIcon.ico");
            AppWindow?.SetIcon(iconPath);
        }

        private void Content_ProcessKeyboardAccelerators(UIElement sender, Microsoft.UI.Xaml.Input.ProcessKeyboardAcceleratorEventArgs args)
        {
            if (args.Modifiers == Windows.System.VirtualKeyModifiers.Menu && args.Key == Windows.System.VirtualKey.Left)
            {
                GoBack();
            }
        }

        private void NavigateTo(NavigateToMessage message)
        {
            rootFrame.Navigate(message.Destination);
        }

        private void GoBack()
        {
            rootFrame.GoBack();
        }
    }
}
