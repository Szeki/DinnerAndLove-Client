using DinnerAndLove.Client.Model;
using System.Windows.Media.Imaging;

namespace DinnerAndLove.Client.Wpf.Component
{
    class UserProfileChangedEvent : ChangedEventBase
    {
        #region Constructor

        public UserProfileChangedEvent(User changedUser, BitmapImage profilePicture)
        {
            ChangedUser = changedUser;
            ProfilePicture = profilePicture;
        }

        #endregion

        #region Properties

        public User ChangedUser
        {
            get;
            private set;
        }

        public BitmapImage ProfilePicture
        {
            get;
            private set;
        }

        #endregion
    }
}
