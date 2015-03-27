namespace DinnerAndLove.Client.Model
{
    public class Profile
    {
        #region Constructor

        public Profile(User user, object picture)
        {
            User = user;
            PictureData = picture;
        }

        #endregion

        #region Properties

        public User User
        {
            get;
            private set;
        }

        public object PictureData
        {
            get;
            private set;
        }

        #endregion
    }
}
