using AoE2DELobbyBrowser.Core;
using AoE2DELobbyBrowser.Core.ViewModels;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;

namespace AoE2DELobbyBrowserAvalonia.ViewModels
{
    public partial class MainWindowViewModel: ObservableObject
    {
        [ObservableProperty]
        private INavigableViewModel _contentViewModel;

        [ObservableProperty]
        private IPageTransition _pageTransition;

        [ObservableProperty]
        private bool _isReversed;

        public MainWindowViewModel()
        {
            WeakReferenceMessenger.Default.Register<NavigateToMessage>(this, (r, m) => NavigateTo(m));
            WeakReferenceMessenger.Default.Register<NavigateBackMessage>(this, (r, m) => GoBack());

            PageTransition = new CrossFade(TimeSpan.FromMilliseconds(250));

            NavigateTo(new NavigateToMessage(typeof(MainViewModel)));
        }

        private Stack<INavigableViewModel> _pages = new Stack<INavigableViewModel>();

        private void NavigateTo(NavigateToMessage message)
        {
            var vm = Activator.CreateInstance(message.Destination) as INavigableViewModel;
            if (vm == null) return;

            _pages.Push(vm);

            IsReversed = false;
            ContentViewModel = vm;
        }

        private void GoBack()
        {
            if (_pages.Count == 1) return;
            _pages.Pop();

            IsReversed = true;
            ContentViewModel = _pages.Peek();
        }
    }
}
