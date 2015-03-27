using DinnerAndLove.Client.Wpf.Component;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DinnerAndLove.Client.Wpf.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Members

        LoginViewModel _loginViewModel;
        SearchDinnerViewModel _defaultContent;

        ContentViewModelBase _currentContent;

        BitmapImage _currentUserProfilePicture;

        RelayCommand _showProfileCommand;
        RelayCommand _logoutCommand;
        RelayCommand _showDefaultPageCommand;

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            ActivatePropertyDependencies(typeof(MainWindowViewModel));

            this.PropertyChanged += ViewModelPropertyChanged;

            SubscribeEvent<UserProfileChangedEvent>();

            _defaultContent = new SearchDinnerViewModel();
            LoginViewModel = new LoginViewModel();
        }

        #endregion

        #region Properties

        public LoginViewModel LoginViewModel
        {
            get
            {
                return _loginViewModel;
            }
            private set
            {
                if (_loginViewModel == value) return;

                _loginViewModel = value;

                RaisePropertyChanged(() => LoginViewModel);
            }
        }

        [DependsUpon("LoginViewModel")]
        public bool IsLoginViewModelVisible
        {
            get
            {
                return LoginViewModel != null;
            }
        }

        public BitmapImage CurrentUserProfilePicture
        {
            get
            {
                return _currentUserProfilePicture;
            }
            private set
            {
                if (_currentUserProfilePicture == value) return;

                _currentUserProfilePicture = value;

                RaisePropertyChanged(() => CurrentUserProfilePicture);
            }
        }

        [DependsUpon("CurrentUserProfilePicture")]
        public bool IsCurrentUserProfilePictureVisible
        {
            get
            {
                return CurrentUserProfilePicture != null;
            }
        }

        public ContentViewModelBase CurrentContent
        {
            get
            {
                return _currentContent;
            }
            private set
            {
                if (_currentContent == value) return;

                _currentContent = value;

                RaisePropertyChanged(() => CurrentContent);
            }
        }

        #endregion

        #region Commands

        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new RelayCommand(param => Logout()));
            }
        }

        public ICommand ShowProfileCommand
        {
            get
            {
                return _showProfileCommand ?? (_showProfileCommand = new RelayCommand(param => ShowProfile()));
            }
        }

        public ICommand ShowDefaultPageCommand
        {
            get
            {
                return _showDefaultPageCommand ?? (_showDefaultPageCommand = new RelayCommand(param => ShowDefaultPage()));
            }
        }

        #endregion

        #region Private Methods

        private async void InitializeMainView()
        {
            if(CurrentUser == null)
            {
                CurrentUserProfilePicture = null;

                return;
            }

            LoginViewModel = null;

            ShowLoadingProgress();

            var profileResponse = await ApiService.ExecuteRequestAsync("secured/users/me/profilepicture", true);

            if (profileResponse.Success)
            {
                var imageData = Convert.FromBase64String(profileResponse.Data.ToString());

                CurrentUserProfilePicture = ImageHelper.LoadImage(imageData);
            }

            HideLoadingProgress();

            CurrentContent = _defaultContent;
        }

        private void Logout()
        {
            CurrentUserProfilePicture = null;
            CurrentUser = null;
            CurrentContent = null;

            ApiService.Logout();

            LoginViewModel = new LoginViewModel();
        }

        private void ShowProfile()
        {
            CurrentContent = new ProfileContentViewModel(CurrentUser.Id);
        }

        private void ShowDefaultPage()
        {
            CurrentContent = null;
            CurrentContent = _defaultContent;
        }

        #endregion

        #region Overriden Methods

        protected override void OnEventRaised(ViewModelBase sender, ChangedEventBase eventArgs)
        {
            if(eventArgs.GetType() == typeof(UserProfileChangedEvent))
            {
                var profileChanged = (UserProfileChangedEvent)eventArgs;

                CurrentUser.FirstName = profileChanged.ChangedUser.FirstName;
                CurrentUser.LastName = profileChanged.ChangedUser.LastName;
                CurrentUser.Description = profileChanged.ChangedUser.Description;

                if(profileChanged.ProfilePicture != null)
                {
                    CurrentUserProfilePicture = profileChanged.ProfilePicture;
                }
            }
        }

        #endregion

        #region EventHandlers

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == "CurrentUser")
            {
                InitializeMainView();
            }
        }

        #endregion
    }
}
