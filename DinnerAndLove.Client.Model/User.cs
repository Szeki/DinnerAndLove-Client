using System.ComponentModel;

namespace DinnerAndLove.Client.Model
{
	public class User : INotifyPropertyChanged
    {
        #region Members

        string _firstName;
        string _lastName;
        string _description;

        #endregion

        #region Constructor

        public User(int id, string email)
		{
			Id = id;
			Email = email;
		}

		#endregion

		#region Properties

		public int Id
		{
			get;
			private set;
		}

		public string Email
		{
			get;
			private set;
		}

		public string Salt
		{
			get;
			set;
		}

		public string FirstName
		{
			get
			{
                return _firstName;
			}
            set
            {
                if (_firstName == value) return;

                _firstName = value;

                RaisePropertyChanged("FirstName");
            }
		}

		public string LastName
		{
            get
            {
                return _lastName;
            }
			set
			{
                if (_lastName == value) return;

                _lastName = value;

                RaisePropertyChanged("LastName");
			}
		}

		public string Description
		{
            get
            {
                return _description;
            }
			set
			{
                if (_description == value) return;

                _description = value;

                RaisePropertyChanged("Description");
			}
		}

		#endregion

        #region Private Methods

        private void RaisePropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
