using System;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;

namespace DesktopNotifications.Windows
{
    public class WindowsNotificationManager
    {
        private const int LaunchNotificationWaitMs = 5_000;
        private readonly TaskCompletionSource<string>? _launchActionPromise;
        private readonly ToastNotifierCompat _toastNotifier;

        /// <summary>
        /// </summary>
        /// <param name="applicationContext"></param>
        public WindowsNotificationManager()
        {
            _launchActionPromise = new TaskCompletionSource<string>();

            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                ToastNotificationManagerCompat.OnActivated += OnAppActivated;

                if (_launchActionPromise.Task.Wait(LaunchNotificationWaitMs))
                {
                    LaunchActionId = _launchActionPromise.Task.Result;
                }
            }

            _toastNotifier = ToastNotificationManagerCompat.CreateToastNotifier();
        }
       
        public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;

        public string? LaunchActionId { get; }

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

            toastNotification.Activated += ToastNotificationOnActivated;
            toastNotification.Failed += ToastNotificationOnFailed;

            _toastNotifier.Show(toastNotification);
        }

        private void OnAppActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            Debug.Assert(_launchActionPromise != null);

            var actionId = GetActionId(e.Argument);
            _launchActionPromise.SetResult(actionId);
        }

        private static void ToastNotificationOnFailed(ToastNotification sender, ToastFailedEventArgs args)
        {
            throw args.ErrorCode;
        }

        private static string GetActionId(string argument)
        {
            return string.IsNullOrEmpty(argument) ? "default" : argument;
        }

        private void ToastNotificationOnActivated(ToastNotification sender, object args)
        {
            var activationArgs = (ToastActivatedEventArgs)args;
            var actionId = GetActionId(activationArgs.Arguments);

            NotificationActivated?.Invoke(
                this,
                new NotificationActivatedEventArgs(null, actionId));
        }
    }
}