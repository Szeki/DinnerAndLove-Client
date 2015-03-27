using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DinnerAndLove.Client.Model;
using DinnerAndLove.Client.Wpf.Component;
using Newtonsoft.Json;

namespace DinnerAndLove.Client.Wpf.ViewModels
{
    class SearchDinnerViewModel : ContentViewModelBase
    {
        #region Members

        string _dinnerPlace;
        DateTime? _dinnerTime;
        bool _isSearchEnabled;
        List<Dinner> _resultDinners;

        RelayCommand _searchDinnerCommand;

        #endregion

        #region Constructor

        public SearchDinnerViewModel()
        {
            ContentHeader = "Search";

            IsSearchEnabled = true;
        }

        #endregion

        #region Properties

        public string DinnerPlace
        {
            get
            {
                return _dinnerPlace;
            }
            set
            {
                if (_dinnerPlace == value) return;

                _dinnerPlace = value;

                RaisePropertyChanged(() => DinnerPlace);
            }
        }

        public DateTime? DinnerTime
        {
            get
            {
                return _dinnerTime;
            }
            set
            {
                if (_dinnerTime == value) return;

                _dinnerTime = value;

                RaisePropertyChanged(() => DinnerTime);
            }
        }

        public ICommand SearchDinnerCommand
        {
            get
            {
                return _searchDinnerCommand ?? (_searchDinnerCommand = new RelayCommand(param => SearchDinner(), param => IsSearchEnabled));
            }
        }

        public bool IsSearchEnabled
        {
            get
            {
                return _isSearchEnabled;
            }
            private set
            {
                if(_isSearchEnabled == value) return;

                _isSearchEnabled = value;

                RaisePropertyChanged(() => IsSearchEnabled);
            }
        }

        public List<Dinner> ResultDinners
        {
            get
            {
                return _resultDinners;
            }
            private set
            {
                if (_resultDinners == value) return;

                _resultDinners = value;

                RaisePropertyChanged(() => ResultDinners);
            }
        }

        #endregion

        #region Private Methods

        private async void SearchDinner()
        {
            if(string.IsNullOrEmpty(DinnerPlace) && DinnerTime == null)
            {
                return;
            }

            IsSearchEnabled = false;

            var response = await ApiService.ExecuteRequestAsync(string.Format("secured/search{0}{1}",
                string.IsNullOrEmpty(DinnerPlace) ? string.Empty : string.Format("/place/{0}", DinnerPlace),
                DinnerTime == null ? string.Empty : DinnerTime.Value.ToString("yyyy-MM-dd")));

            if(response.Success)
            {
                ResultDinners = JsonConvert.DeserializeObject<List<Dinner>>(response.Data.ToString());
            }

            IsSearchEnabled = true;
        }

        #endregion
    }
}
