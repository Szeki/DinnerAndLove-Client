using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinnerAndLove.Client.Model
{
    public class ApiResponse
    {
        #region Constructor

        public ApiResponse(bool success, string failure_message, object data)
        {
            Success = success;
            FailureMessage = failure_message;
            Data = data;
        }

        #endregion

        #region Properties

        public bool Success
        {
            get;
            private set;
        }

        public string FailureMessage
        {
            get;
            private set;
        }

        public object Data
        {
            get;
            private set;
        }

        #endregion
    }
}
