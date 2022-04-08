using DynamicData;
using DynamicData.Binding;
using Serilog;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace AoE2DELobbyBrowser
{
    public class MySortedObservableCollectionAdaptor : ISortedObservableCollectionAdaptor<Lobby, string>
    {
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

        private static void DoUpdate(ISortedChangeSet<Lobby, string> updates, IObservableCollection<Lobby> list)
        {
            foreach (var update in updates)
            {
                switch (update.Reason)
                {
                    case ChangeReason.Add:
                        Log.Debug($"Collection add {update.CurrentIndex}");
                        list.Insert(update.CurrentIndex, update.Current);
                        break;

                    case ChangeReason.Remove:
                        Log.Debug($"Collection remove {update.CurrentIndex}");
                        list.RemoveAt(update.CurrentIndex);
                        break;

                    case ChangeReason.Moved:
                        Log.Debug($"Collection move {update.PreviousIndex} {update.CurrentIndex}");
                        list.Move(update.PreviousIndex, update.CurrentIndex);
                        break;

                    case ChangeReason.Update:
                        if (update.PreviousIndex != update.CurrentIndex)
                        {
                            Log.Debug($"Collection update remove insert {update.PreviousIndex} {update.CurrentIndex}");
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
                                Log.Debug($"Update nums {update.CurrentIndex} {prev.NumPlayers} {current.NumPlayers}");
                                prev.NumPlayers = current.NumPlayers;
                                prev.NumSlots = current.NumSlots;
                            }
                            if (AreDifferent(prev, current, x => x.GameType) ||
                                AreDifferent(prev, current, x => x.Map) ||
                                AreDifferent(prev, current, x => x.Name) ||
                                AreDifferent(prev, current, x => x.Speed))
                            {
                                Log.Debug($"Replace lobby {update.CurrentIndex}");
                                current.AddedAt = prev.AddedAt;
                                list[update.CurrentIndex] = current;
                            }

                            current = list[update.CurrentIndex];
                            if (current.IsNew && current.AddedAt + TimeSpan.FromSeconds(30) < DateTime.Now)
                            {
                                current.IsNew = false;
                            }
                        }
                        break;
                }
            }
        }

        private static bool AreDifferent<TObject,TValue>(TObject prev, TObject current, Func<TObject,TValue> propSelector)
            where TValue : IComparable
        {
            return propSelector(prev).CompareTo(propSelector(current)) != 0;
        }
    }
}
