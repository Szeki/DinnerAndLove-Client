using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DinnerAndLove.Client.Model;

namespace DinnerAndLove.Client.Wpf.ViewModels
{
    class LoginViewModel : ViewModelBase
    {
        #region Members

        bool _isFocused;
        bool _isLoginButtonEnabled;
        string _errorMessage;

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            ActivatePropertyDependencies(typeof(LoginViewModel));

            IsFocused = true;
            IsLoginButtonEnabled = true;
        }

        #endregion

        #region Properties

        public bool IsFocused
        {
            get
            {
                return _isFocused;
            }
            set
            {
                if (_isFocused == value) return;

                _isFocused = value;

                RaisePropertyChanged(() => IsFocused);
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            private set
            {
                if (_errorMessage == value) return;

                _errorMessage = value;

                RaisePropertyChanged(() => ErrorMessage);
            }
        }

        [DependsUpon("ErrorMessage")]
        public bool IsErrorMessageVisible
        {
            get
            {
                return !string.IsNullOrEmpty(ErrorMessage);
            }
        }

        public bool IsLoginButtonEnabled
        {
            get
            {
                return _isLoginButtonEnabled;
            }
            private set
            {
                if(_isLoginButtonEnabled == value) return;

                _isLoginButtonEnabled = value;

                RaisePropertyChanged(() => IsLoginButtonEnabled);
            }
        }

        #endregion

        #region Public Methods

        public async Task InitiateLogin(string username, string password)
        {
            IsLoginButtonEnabled = false;

            User user = null;

            try
            {
                user = await ApiService.LoginAsync(username, password);
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;

                IsLoginButtonEnabled = true;

                return;
            }

            if(user == null)
            {
                ErrorMessage = "User was not found!";

                IsLoginButtonEnabled = true;

                return;
            }

            CurrentUser = user;
        }

        #endregion
    }
}
