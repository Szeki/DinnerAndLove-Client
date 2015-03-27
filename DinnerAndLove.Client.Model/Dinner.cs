using System;

namespace DinnerAndLove.Client.Model
{
    public class Dinner
    {
        #region Constructor

        public Dinner(int id, User host, DateTime date_time)
        {
            Id = id;
            Host = host;
            DateTime = date_time;
        }

        #endregion

        #region Properties

        public int Id
        {
            get;
            private set;
        }

        public User Host
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public DateTime DateTime
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public int MaxGuest
        {
            get;
            set;
        }

        public int PricePerGuest
        {
            get;
            set;
        }

        #endregion
    }
}
