﻿using DinnerAndLove.Client.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DinnerAndLove.Client.Wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var profilePicture = GetProfilePicture();

			if (profilePicture != null)
			{
				ProfilePicture.Source = LoadImage(profilePicture);
			}
		}

		private static BitmapImage LoadImage(byte[] imageData)
		{
			if (imageData == null || imageData.Length == 0) return null;
			var image = new BitmapImage();
			using (var mem = new MemoryStream(imageData))
			{
				mem.Position = 0;
				image.BeginInit();
				image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.UriSource = null;
				image.StreamSource = mem;
				image.EndInit();
			}
			image.Freeze();
			return image;
		}

		static byte[] GetProfilePicture()
		{
			var username = "szeky.g@gmail.com";
			var password = "szeki";

			var request = WebRequest.Create(string.Format(@"http://localhost/web/app_dev.php/api/public/users/find/{0}", username));
			User user = null;

			try
			{
				using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
				{
					var result = reader.ReadToEnd();

					user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(result);

					if (user == null)
					{
						return null;
					}
				}

				request = WebRequest.Create(@"http://localhost/web/app_dev.php/api/secured/users/me/profilepicture");
				request.Headers.Add("X-WSSE", CreateAuthenticationHeader(user, password));

				using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
				{
					var result = reader.ReadToEnd();

					var json = Newtonsoft.Json.Linq.JObject.Parse(result);

					Newtonsoft.Json.Linq.JToken token = null;

					var success = (bool)json.SelectToken("success");

					if (success)
					{
						return Convert.FromBase64String(json.SelectToken("data").ToString());
					}
				}
			}
			catch (WebException ex)
			{
				System.Console.WriteLine(ex.Message);
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}

			return null;
		}


		static string EncryptPlainPassword(string plainPassword, string salt)
		{
			using (var hash = new SHA512Managed())
			{
				var plainSalted = string.Format("{0}{{{1}}}", plainPassword, salt);
				var initialValue = Encoding.UTF8.GetBytes(plainSalted);
				var value = hash.ComputeHash(initialValue);

				for (int i = 1; i < 5000; i++)
				{
					value = hash.ComputeHash(value.Concat(initialValue).ToArray());
				}

				return Convert.ToBase64String(value);
			}
		}

		static string CreateAuthenticationHeader(User user, string plainPassword)
		{
			var encryptedPassword = EncryptPlainPassword(plainPassword, user.Salt);
			var nonce = GenerateNonce();

			var createTime = DateTime.Now.ToUniversalTime().ToString(DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern) + "Z";
			var passwordAndTimeBuffer = Encoding.UTF8.GetBytes(createTime + encryptedPassword);
			var diggestBuffer = nonce.Concat(passwordAndTimeBuffer).ToArray();

			//create default SHA1
			var sha1 = new SHA1Managed();

			//make digest string
			string digest = Convert.ToBase64String(sha1.ComputeHash(diggestBuffer));

			return string.Format("UsernameToken Username=\"{0}\", PasswordDigest=\"{1}\", Nonce=\"{2}\", Created=\"{3}\"", user.Email, digest, Convert.ToBase64String(nonce), createTime);
		}

		static byte[] GenerateNonce()
		{
			var nonce = new byte[16];
			var rand = new RNGCryptoServiceProvider();

			rand.GetBytes(nonce);

			return nonce;
		}
	}
}
