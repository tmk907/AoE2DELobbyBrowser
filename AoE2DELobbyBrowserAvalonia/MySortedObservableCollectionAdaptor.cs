using AoE2DELobbyBrowserAvalonia.Models;
using AoE2DELobbyBrowserAvalonia.Services;
using DynamicData;
using DynamicData.Binding;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace AoE2DELobbyBrowserAvalonia
{
    public class MySortedObservableCollectionAdaptor : ISortedObservableCollectionAdaptor<Lobby, string>
    {
        private TimeSpan _newLobbyHighlightTime;
        public MySortedObservableCollectionAdaptor()
        {
            _newLobbyHighlightTime = AppSettings.NewLobbyHighlightTime;
        }

        public void Adapt(ISortedChangeSet<Lobby, string> changes, IObservableCollection<Lobby> collection)
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

        private void DoUpdate(ISortedChangeSet<Lobby, string> updates, IObservableCollection<Lobby> list)
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
