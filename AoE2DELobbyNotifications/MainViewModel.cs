using AoE2DELobbyNotifications.Api;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;

namespace AoE2DELobbyNotifications
{
    [DataContract]
    public class MainViewModel : ReactiveObject
    {
        private readonly Aoe2netApiClient _aoe2netApiClient;
        private readonly NotificationsService _notificationsService;
        public MainViewModel()
        {
            _aoe2netApiClient = new Aoe2netApiClient();
            _notificationsService = new NotificationsService();

            SelectedGameType = GameTypes.First();
            SelectedGameSpeed = GameSpeeds.First();

            Func<Lobby, bool> lobbyFilter(string text) => lobby =>
            {
                return string.IsNullOrEmpty(text) || lobby.Name.ToLower().Contains(text.ToLower());
            };

            Func<Lobby, bool> gameTypeFilter(string gameType) => lobby =>
            {
                return gameType == GameType.All || lobby.GameType == gameType;
            };

            Func<Lobby, bool> gameSpeedFilter(string gameSpeed) => lobby =>
            {
                return gameSpeed == GameType.All || lobby.Speed == gameSpeed;
            };

            var filterPredicate = this
                .WhenAnyValue(x => x.Query)
                .DistinctUntilChanged()
                .Select(lobbyFilter);

            var filterPredicate2 = this
                .WhenAnyValue(x => x.SelectedGameType)
                .DistinctUntilChanged()
                .Select(gameTypeFilter);

            var filterPredicate3 = this
                .WhenAnyValue(x => x.SelectedGameSpeed)
                .DistinctUntilChanged()
                .Select(gameSpeedFilter);

            var filtered = _aoe2netApiClient
                .Connect()
                //.TransformWithInlineUpdate(dto => Lobby.Create(dto), (lobby, dto) => lobby.NumPlayers = dto.NumPlayers)
                .Transform(dto => Lobby.Create(dto))
                .Filter(x => x.Name != "AUTOMATCH")
                .Filter(filterPredicate)
                .Filter(filterPredicate2)
                .Filter(filterPredicate3);

            var newLobbies = filtered
                .SkipInitial()
                .WhereReasonsAre(ChangeReason.Add)
                .Select(changeSet => changeSet.Select(x => x.Current).ToList())
                .Do(list => list.ForEach(x => x.IsNew = true))
                .Do(x => Log.Information($"Added {x.Count} new lobbies"));

            newLobbies
                .Where(_ => ShowNotifications)
                .Do(x => _notificationsService.ShowNotifications(x))
                .Subscribe()
                .DisposeWith(Disposal);

            filtered
                .Sort(SortExpressionComparer<Lobby>.Ascending(t => t.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _lobbies)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Disposal);

            this.RefreshCommand = ReactiveCommand.CreateFromTask(ct => _aoe2netApiClient.Refresh(ct));
            this.WhenAnyObservable(x => x.RefreshCommand.IsExecuting)
                .StartWith(false)
                .DistinctUntilChanged()
                .ToProperty(this, x => x.Loading, out _loading  )
                .DisposeWith(Disposal);

            this
                .WhenAnyValue(x => x.Interval, x => x.IsAutoRefreshEnabled)
                .Where(x => x.Item2)
                .Select(x => x.Item1)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Do(_ => Log.Information("value changed"))
                .Select(interval =>
                    Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(interval), RxApp.TaskpoolScheduler)
                        .Do(x => Log.Information($"Inner observable interval {x}"))
                        .Select(time => Unit.Default)
                    )
                .Switch()
                .Where(_ => _isAutoRefreshEnabled)
                .Do(_ => Log.Information("Refresh enabled"))
                .InvokeCommand(this, x => x.RefreshCommand)
                .DisposeWith(Disposal);
        }

        protected CompositeDisposable Disposal = new CompositeDisposable();
        public void Dispose()
        {
            Disposal.Dispose();
        }

        public ReactiveCommand<Unit,Unit> RefreshCommand { get; }

        private readonly ReadOnlyObservableCollection<Lobby> _lobbies;
        public ReadOnlyObservableCollection<Lobby> Lobbies => _lobbies;

        private string _query;
        [DataMember]
        public string Query
        {
            get => _query;
            set => this.RaiseAndSetIfChanged(ref _query, value);
        }

        private int _interval = 15;
        [DataMember]
        public int Interval
        {
            get => _interval;
            set => this.RaiseAndSetIfChanged(ref _interval, value);
        }

        private bool _isAutoRefreshEnabled;
        [DataMember]
        public bool IsAutoRefreshEnabled
        {
            get => _isAutoRefreshEnabled;
            set => this.RaiseAndSetIfChanged(ref _isAutoRefreshEnabled, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _loading;
        public bool Loading => _loading.Value;

        public List<string> GameTypes { get; } = new GameType().GetAll();
        public List<string> GameSpeeds { get; } = new GameSpeed().GetAll();


        private string selectedGameType;
        [DataMember]
        public string SelectedGameType
        {
            get => selectedGameType;
            set => this.RaiseAndSetIfChanged(ref selectedGameType, value);
        }

        private string selectedGameSpeed;
        [DataMember]
        public string SelectedGameSpeed
        {
            get => selectedGameSpeed;
            set => this.RaiseAndSetIfChanged(ref selectedGameSpeed, value);
        }

        private bool _showNotifications = true;
        public bool ShowNotifications
        {
            get => _showNotifications;
            set => this.RaiseAndSetIfChanged(ref _showNotifications, value);
        }
    }

    public static class Ext
    {
        public static IObservable<IChangeSet<TDestination, TKey>> TransformWithInlineUpdate<TObject, TKey, TDestination>(this IObservable<IChangeSet<TObject, TKey>> source,
        Func<TObject, TDestination> transformFactory,
        Action<TDestination, TObject> updateAction = null)
        {
            return source.Scan((ChangeAwareCache<TDestination, TKey>)null, (cache, changes) =>
            {
                //The change aware cache captures a history of all changes so downstream operators can replay the changes
                if (cache == null)
                    cache = new ChangeAwareCache<TDestination, TKey>(changes.Count);

                foreach (var change in changes)
                {
                    switch (change.Reason)
                    {
                        case ChangeReason.Add:
                            cache.AddOrUpdate(transformFactory(change.Current), change.Key);
                            break;
                        case ChangeReason.Update:
                            {
                                if (updateAction == null) continue;

                                var previous = cache.Lookup(change.Key)
                                    .ValueOrThrow(() => new MissingKeyException($"{change.Key} is not found."));
                                //callback when an update has been received
                                updateAction(previous, change.Current);

                                //send a refresh as this will force downstream operators to filter, sort, group etc
                                cache.Refresh(change.Key);
                            }
                            break;
                        case ChangeReason.Remove:
                            cache.Remove(change.Key);
                            break;
                        case ChangeReason.Refresh:
                            cache.Refresh(change.Key);
                            break;
                        case ChangeReason.Moved:
                            //Do nothing !
                            break;
                    }
                }
                return cache;

            }).Select(cache => cache.CaptureChanges()); //invoke capture changes to return the changeset
        }
    }
}
