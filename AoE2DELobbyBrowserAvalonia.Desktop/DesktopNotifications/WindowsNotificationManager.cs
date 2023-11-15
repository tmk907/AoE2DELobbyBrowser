﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;

namespace DesktopNotifications.Windows
{
    public class WindowsNotificationManager : INotificationManager
    {
        private const int LaunchNotificationWaitMs = 5_000;
        private readonly TaskCompletionSource<string>? _launchActionPromise;
        private readonly Dictionary<ToastNotification, Notification> _notifications;
        private readonly Dictionary<ScheduledToastNotification, Notification> _scheduledNotification;


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

            _notifications = new Dictionary<ToastNotification, Notification>();
            _scheduledNotification = new Dictionary<ScheduledToastNotification, Notification>();
        }

        public NotificationManagerCapabilities Capabilities => NotificationManagerCapabilities.BodyText |
                                                               NotificationManagerCapabilities.BodyImages |
                                                               NotificationManagerCapabilities.Icon |
                                                               NotificationManagerCapabilities.Audio;

        public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;

        public event EventHandler<NotificationDismissedEventArgs>? NotificationDismissed;

        public string? LaunchActionId { get; }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public Task ShowNotification(Notification notification, DateTimeOffset? expirationTime)
        {
            if (expirationTime < DateTimeOffset.Now)
            {
                throw new ArgumentException(nameof(expirationTime));
            }

            var xmlContent = GenerateXml(notification);
            var toastNotification = new ToastNotification(xmlContent)
            {
                ExpirationTime = expirationTime
            };

            toastNotification.Activated += ToastNotificationOnActivated;
            toastNotification.Dismissed += ToastNotificationOnDismissed;
            toastNotification.Failed += ToastNotificationOnFailed;

            _toastNotifier.Show(toastNotification);
            _notifications[toastNotification] = notification;

            return Task.CompletedTask;
        }

        public Task HideNotification(Notification notification)
        {
            if (_notifications.TryGetKey(notification, out var toastNotification))
            {
                _toastNotifier.Hide(toastNotification);
            }

            if (_scheduledNotification.TryGetKey(notification, out var scheduledToastNotification))
            {
                _toastNotifier.RemoveFromSchedule(scheduledToastNotification);
            }

            return Task.CompletedTask;
        }

        public Task ScheduleNotification(
            Notification notification,
            DateTimeOffset deliveryTime,
            DateTimeOffset? expirationTime = null)
        {
            if (deliveryTime < DateTimeOffset.Now || deliveryTime > expirationTime)
            {
                throw new ArgumentException(nameof(deliveryTime));
            }

            var xmlContent = GenerateXml(notification);
            var toastNotification = new ScheduledToastNotification(xmlContent, deliveryTime)
            {
                ExpirationTime = expirationTime
            };

            _toastNotifier.AddToSchedule(toastNotification);
            _scheduledNotification[toastNotification] = notification;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _notifications.Clear();
            _scheduledNotification.Clear();
        }

        private static XmlDocument GenerateXml(Notification notification)
        {
            var builder = new ToastContentBuilder();

            builder.AddText(notification.Title);
            builder.AddText(notification.Body);

            if (notification.BodyImagePath is { } img)
            {
                builder.AddInlineImage(new Uri($"file:///{img}"), notification.BodyImageAltText);
            }

            foreach (var (title, actionId) in notification.Buttons)
            {
                builder.AddButton(title, ToastActivationType.Foreground, actionId);
            }

            return builder.GetXml();
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

        private void ToastNotificationOnDismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            if (!_notifications.TryGetValue(sender, out var notification))
            {
                return;
            }

            _notifications.Remove(sender);

            var reason = args.Reason switch
            {
                ToastDismissalReason.UserCanceled => NotificationDismissReason.User,
                ToastDismissalReason.TimedOut => NotificationDismissReason.Expired,
                ToastDismissalReason.ApplicationHidden => NotificationDismissReason.Application,
                _ => throw new ArgumentOutOfRangeException()
            };

            NotificationDismissed?.Invoke(this, new NotificationDismissedEventArgs(notification, reason));
        }

        private static string GetActionId(string argument)
        {
            return string.IsNullOrEmpty(argument) ? "default" : argument;
        }

        private void ToastNotificationOnActivated(ToastNotification sender, object args)
        {
            if (!_notifications.TryGetValue(sender, out var notification))
            {
                return;
            }

            var activationArgs = (ToastActivatedEventArgs)args;
            var actionId = GetActionId(activationArgs.Arguments);

            NotificationActivated?.Invoke(
                this,
                new NotificationActivatedEventArgs(notification, actionId));
        }
    }
}