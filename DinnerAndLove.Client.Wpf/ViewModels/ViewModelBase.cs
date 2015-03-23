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

        static User _currentUser;

        #endregion

        #region Static Constructor

        static ViewModelBase()
        {
            _viewModels = new List<ViewModelBase>();
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
        }

        #endregion

        #region Static Properties

        public static ApiService ApiService
        {
            get;
            private set;
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

        #endregion
    }
}
