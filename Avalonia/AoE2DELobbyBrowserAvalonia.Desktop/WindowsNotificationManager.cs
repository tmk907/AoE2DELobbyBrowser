using System;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;
using System.Reactive.Subjects;
using Windows.Data.Xml.Dom;

namespace AoE2DELobbyBrowserAvalonia.Desktop
{
    public class WindowsNotificationManager
    {
        private readonly ToastNotifierCompat _toastNotifier;

        private readonly ReplaySubject<ToastArguments> _notificationsSubject;

        public WindowsNotificationManager()
        {
            _notificationsSubject = new ReplaySubject<ToastArguments>();

            ToastNotificationManagerCompat.OnActivated += OnAppActivated;
            _toastNotifier = ToastNotificationManagerCompat.CreateToastNotifier();
        }

        public IObservable<ToastArguments> Notifications { get { return _notificationsSubject; } }

        public void ShowNotification(XmlDocument xmlContent, DateTimeOffset? expirationTime)
        {
            if (expirationTime < DateTimeOffset.Now)
            {
                throw new ArgumentException(nameof(expirationTime));
            }

            var toastNotification = new ToastNotification(xmlContent)
            {
                ExpirationTime = expirationTime,
                ExpiresOnReboot = true,
            };

            toastNotification.Failed += ToastNotificationOnFailed;

            _toastNotifier.Show(toastNotification);
        }

        private void OnAppActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            Log.Information("OnAppActivated {0}", e.Argument);
            ToastArguments args = ToastArguments.Parse(e.Argument);
            _notificationsSubject.OnNext(args);
        }

        private static void ToastNotificationOnFailed(ToastNotification sender, ToastFailedEventArgs args)
        {
            Log.Error("Toast notification failed {0}", args.ErrorCode);
        }
    }
}