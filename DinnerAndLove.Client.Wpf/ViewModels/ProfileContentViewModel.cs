using DinnerAndLove.Client.Model;
using DinnerAndLove.Client.Wpf.Component;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace DinnerAndLove.Client.Wpf.ViewModels
{
    class ProfileContentViewModel : ContentViewModelBase
    {
        #region Members

        int _profileUserId;

        Profile _profile;
        BitmapImage _profilePicture;

        RelayCommand _saveProfileCommand;
        RelayCommand _updateAvatarCommand;

        #endregion

        #region Constructor

        public ProfileContentViewModel(int userId)
        {
            ContentHeader = "Profile";

            _profileUserId = userId;

            LoadProfile();
        }

        #endregion

        #region Properties

        public bool IsProfileEditable
        {
            get
            {
                return CurrentUser.Id == _profileUserId;
            }
        }

        public Profile Profile
        {
            get
            {
                return _profile;
            }
            private set
            {
                if (_profile == value) return;

                _profile = value;

                RaisePropertyChanged(() => Profile);
            }
        }

        public BitmapImage ProfilePicture
        {
            get
            {
                return _profilePicture;
            }
            private set
            {
                if (_profilePicture == value) return;

                _profilePicture = value;

                RaisePropertyChanged(() => ProfilePicture);
            }
        }

        public ICommand SaveProfileCommand
        {
            get
            {
                return _saveProfileCommand ?? (_saveProfileCommand = new RelayCommand(param => SaveProfile()));
            }
        }

        public ICommand UpdateAvatarCommand
        {
            get
            {
                return _updateAvatarCommand ?? (_updateAvatarCommand = new RelayCommand(param => UpdateAvatar()));
            }
        }

        #endregion

        #region Private Methods

        private async void LoadProfile()
        {
            ShowLoadingProgress();

            var response = await ApiService.ExecuteRequestAsync(string.Format("secured/users/profile/{0}", _profileUserId), true);

            if(response.Success)
            {
                Profile = JsonConvert.DeserializeObject<Profile>(response.Data.ToString());
                ProfilePicture = ImageHelper.LoadImage(Convert.FromBase64String(Profile.PictureData.ToString()));
            }

            HideLoadingProgress();
        }

        private async void SaveProfile()
        {
            ShowLoadingProgress();

            var response = await ApiService.ExecutePutRequestAsync("secured/users/profile/update", JsonConvert.SerializeObject(Profile.User));
            
            RaiseChangedEvent(new UserProfileChangedEvent(Profile.User, null));

            HideLoadingProgress();
        }

        private async void UpdateAvatar()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg"
            };

            if(dialog.ShowDialog() != true)
            {
                return;
            }

            ShowLoadingProgress();

            var pictureData = System.IO.File.ReadAllBytes(dialog.FileName);

            var jObject = JObject.FromObject(Profile.User);

            jObject.Add("Picture", Convert.ToBase64String(pictureData));

            var response = await ApiService.ExecutePutRequestAsync("secured/users/profile/update", jObject.ToString());

            ProfilePicture = ImageHelper.LoadImage(pictureData);

            RaiseChangedEvent(new UserProfileChangedEvent(Profile.User, ProfilePicture));

            HideLoadingProgress();
        }

        #endregion
    }
}
