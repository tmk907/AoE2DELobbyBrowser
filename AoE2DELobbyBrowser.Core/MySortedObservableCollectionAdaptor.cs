using AoE2DELobbyBrowser.Core.Models;
using AoE2DELobbyBrowser.Core.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using DynamicData.Binding;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace AoE2DELobbyBrowser.Core
{
    public class MySortedObservableCollectionAdaptor : ISortedObservableCollectionAdaptor<LobbyVM, string>
    {
        private TimeSpan _newLobbyHighlightTime;
        public MySortedObservableCollectionAdaptor()
        {
            var settingsService = Ioc.Default.GetRequiredService<AppSettingsService>();
            _newLobbyHighlightTime = settingsService.AppSettings.NewLobbyHighlightTime;
        }

        public void Adapt(ISortedChangeSet<LobbyVM, string> changes, IObservableCollection<LobbyVM> collection)
        {
            if (changes is null)
            {
                throw new ArgumentNullException(nameof(changes));
            }

            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            switch (changes.SortedItems.SortReason)
            {
                case SortReason.InitialLoad:
                case SortReason.ComparerChanged:
                case SortReason.Reset:
                    using (collection.SuspendNotifications())
                    {
                        collection.Load(changes.SortedItems.Select(kv => kv.Value));
                    }

                    break;

                case SortReason.DataChanged:
                    using (collection.SuspendCount())
                    {
                        DoUpdate(changes, collection);
                    }
                    break;

                case SortReason.Reorder:
                    // Updates will only be moves, so apply logic
                    using (collection.SuspendCount())
                    {
                        DoUpdate(changes, collection);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(changes));
            }
        }

        private void DoUpdate(ISortedChangeSet<LobbyVM, string> updates, IObservableCollection<LobbyVM> list)
        {
            foreach (var update in updates)
            {
                switch (update.Reason)
                {
                    case ChangeReason.Add:
                        list.Insert(update.CurrentIndex, update.Current);
                        break;

                    case ChangeReason.Remove:
                        list.RemoveAt(update.CurrentIndex);
                        break;

                    case ChangeReason.Moved:
                        list.Move(update.PreviousIndex, update.CurrentIndex);
                        break;

                    case ChangeReason.Update:
                        if (update.PreviousIndex != update.CurrentIndex)
                        {
                            list.RemoveAt(update.PreviousIndex);
                            list.Insert(update.CurrentIndex, update.Current);
                        }
                        else
                        {
                            var prev = list[update.CurrentIndex];
                            var current = update.Current;

                            if (AreDifferent(prev, current, x => x.NumPlayers) ||
                                AreDifferent(prev, current, x => x.NumSlots))
                            {
                                prev.NumPlayers = current.NumPlayers;
                                prev.NumSlots = current.NumSlots;
                                prev.Players.Clear();
                                prev.Players.AddRange(current.Players.ToList());
                            }
                            if (AreDifferent(prev, current, x => x.GameType) ||
                                AreDifferent(prev, current, x => x.Map) ||
                                AreDifferent(prev, current, x => x.Name) ||
                                AreDifferent(prev, current, x => x.Speed))
                            {
                                current.AddedAt = prev.AddedAt;
                                list[update.CurrentIndex] = current;
                            }

                            current = list[update.CurrentIndex];
                            if (current.IsNew && current.AddedAt + _newLobbyHighlightTime < DateTime.Now)
                            {
                                current.IsNew = false;
                            }
                        }
                        break;
                }
            }
        }

        private static bool AreDifferent<TObject, TValue>(TObject prev, TObject current, Func<TObject, TValue> propSelector)
            where TValue : IComparable
        {
            return propSelector(prev).CompareTo(propSelector(current)) != 0;
        }
    }
}
