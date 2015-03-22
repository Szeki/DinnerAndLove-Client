using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinnerAndLove.Client.Model
{
	public class User
	{
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
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		#endregion
	}
}
