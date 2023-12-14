using System;
using System.Reactive.Disposables;

namespace AoE2DELobbyBrowser.Core
{
    public static class ReactiveExtensions
    {
        public static T DisposeWith<T>(this T item, CompositeDisposable compositeDisposable)
    where T : IDisposable
        {
            if (compositeDisposable is null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            compositeDisposable.Add(item);
            return item;
        }
    }
}