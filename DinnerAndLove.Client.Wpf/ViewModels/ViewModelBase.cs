using System.Linq;
using DinnerAndLove.Client.Model;
using System;
using System.Collections.Generic;

namespace DinnerAndLove.Client.Wpf.ViewModels
{
    using DinnerAndLove.Client.Service;
    using DinnerAndLove.Client.Wpf.Component;

    class ViewModelBase : NotifyPropertyChanged, IDisposable
    {
        #region Members

        protected static List<ViewModelBase> _viewModels;
        static bool _isLoadingProgressVisible;

        static User _currentUser;
        static List<Tuple<ViewModelBase, Type>> _events;

        #endregion

        #region Static Constructor

        static ViewModelBase()
        {
            _viewModels = new List<ViewModelBase>();
            _events = new List<Tuple<ViewModelBase, Type>>();

            ApiService = new ApiService();
        }

        #endregion

        #region Constructor / Destructor

        public ViewModelBase()
        {
            _viewModels.Add(this);
        }

        ~ViewModelBase()
        {
            _viewModels.Remove(this);
            _events.RemoveAll(item => item.Item1 == this);
        }

        #endregion

        #region Static Properties

        public static ApiService ApiService
        {
            get;
            private set;
        }

        public bool IsLoadingProgressVisible
        {
            get
            {
                return _isLoadingProgressVisible;
            }
            protected set
            {
                if (_isLoadingProgressVisible == value) return;

                _isLoadingProgressVisible = value;

                RaisePropertyChanged(() => IsLoadingProgressVisible);
                RaiseStaticPropertyChanged("IsLoadingProgressVisible");
            }
        }

        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            protected set
            {
                if(_currentUser == value) return;

                _currentUser = value;

                RaisePropertyChanged(() => CurrentUser);
                RaiseStaticPropertyChanged("CurrentUser");
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _viewModels.Remove(this);
            _events.RemoveAll(item => item.Item1 == this);
        }

        #endregion

        #region Protected Methods

        protected static void RaiseStaticPropertyChanged(string propertyName)
        {
            foreach (var viewModel in _viewModels)
            {
                viewModel.RaisePropertyChanged(propertyName);
            }
        }

        protected void RaiseChangedEvent<T>(T eventType) where T : ChangedEventBase
        {
            foreach (var eventItem in _events.Where(item => item.Item2 == typeof(T)))
            {
                eventItem.Item1.OnEventRaised(this, eventType);
            }
        }

        protected void SubscribeEvent<T>() where T : ChangedEventBase
        {
            if (!_events.Any(item => item.Item1 == this && item.Item2 == typeof(T)))
            {
                _events.Add(new Tuple<ViewModelBase, Type>(this, typeof(T)));
            }
        }

        protected virtual void OnEventRaised(ViewModelBase sender, ChangedEventBase eventArgs)
        {
        }

        protected void ShowLoadingProgress()
        {
            IsLoadingProgressVisible = true;
        }

        protected void HideLoadingProgress()
        {
            IsLoadingProgressVisible = false;
        }

        #endregion
    }
}
