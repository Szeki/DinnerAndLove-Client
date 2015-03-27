using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinnerAndLove.Client.Wpf.ViewModels
{
    abstract class ContentViewModelBase : ViewModelBase
    {
        #region Properties

        public string ContentHeader
        {
            get;
            protected set;
        }

        #endregion
    }
}
