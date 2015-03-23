using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DinnerAndLove.Client.Wpf.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Members

        LoginViewModel _loginViewModel;

        BitmapImage _currentUserProfilePicture;

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            ActivatePropertyDependencies(typeof(MainWindowViewModel));

            this.PropertyChanged += ViewModelPropertyChanged;

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

        #endregion

        #region Private Methods

        private async void InitializeMainView()
        {
            var profileResponse = await ApiService.ExecuteRequestAsync("secured/users/me/profilepicture", true);

            if(profileResponse.Success)
            {
                var imageData = Convert.FromBase64String(profileResponse.Data.ToString());

                CurrentUserProfilePicture = LoadImage(imageData);
            }

            LoginViewModel = null;
        }

        private BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();

            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            image.Freeze();
            return image;
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
