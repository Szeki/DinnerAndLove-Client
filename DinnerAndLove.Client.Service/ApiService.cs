using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DinnerAndLove.Client.Service
{
    using DinnerAndLove.Client.Model;

    public class ApiService
    {
        #region Members

        const string RequestBaseAddress = @"http://localhost/web/app_dev.php/api";

        string _currentUsername;
        string _currentUserEncodedPassword;

        #endregion

        #region Constructor
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        public User Login(string username, string plainPassword)
        {
            var user = FindUserByUsername(username);

            if(user == null)
            {
                throw new Exception("No such user exists");
            }

            _currentUsername = user.Email;
            _currentUserEncodedPassword = EncryptPassword(plainPassword, user.Salt);

            var result = ExecuteRequest("secured/users/login", true);

            if(result.Success)
            {
                return user;
            }

            throw new Exception("Invalid password");
        }

        public Task<User> LoginAsync(string username, string plainPassword)
        {
            return Task.Factory.StartNew(() => Login(username, plainPassword));
        }

        public void Logout()
        {
            _currentUsername = null;
            _currentUserEncodedPassword = null;
        }

        public ApiResponse ExecuteRequest(string path, bool useAuthentication = true)
        {
            if (useAuthentication && string.IsNullOrEmpty(_currentUsername))
            {
                throw new Exception(string.Format("Need to login first before requesting {0} path", path));
            }

            var request = WebRequest.Create(string.Format("{0}/{1}", RequestBaseAddress, path));

            if (useAuthentication)
            {
                request.Headers.Add("X-WSSE", CreateAuthenticationHeader(_currentUsername, _currentUserEncodedPassword));
            }

            using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                var result = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<ApiResponse>(result);
            }
        }

        public Task<ApiResponse> ExecuteRequestAsync(string path, bool useAuthentication = true)
        {
            return Task.Factory.StartNew(() => ExecuteRequest(path, useAuthentication));
        }

        #endregion

        #region Private Methods

        private User FindUserByUsername(string username)
        {
            var result = ExecuteRequest(string.Format("public/users/find/{0}", username), false);

            if (result.Success)
            {
                return JsonConvert.DeserializeObject<User>(result.Data.ToString());
            }

            return null;
        }

        private string GetUrlEncodedValue(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return WebUtility.UrlEncode(value);
        }

        #endregion

        #region Static Methods

        public static string EncryptPassword(string plainPassword, string salt)
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

        public static string CreateAuthenticationHeader(string username, string password)
        {
            var nonce = GenerateNonce();

            var createTime = DateTime.Now.ToUniversalTime().ToString(DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern) + "Z";
            var passwordAndTimeBuffer = Encoding.UTF8.GetBytes(createTime + password);
            var diggestBuffer = nonce.Concat(passwordAndTimeBuffer).ToArray();

            //create default SHA1
            var sha1 = new SHA1Managed();

            //make digest string
            string digest = Convert.ToBase64String(sha1.ComputeHash(diggestBuffer));

            return string.Format("UsernameToken Username=\"{0}\", PasswordDigest=\"{1}\", Nonce=\"{2}\", Created=\"{3}\"", username, digest, Convert.ToBase64String(nonce), createTime);
        }

        private static byte[] GenerateNonce()
        {
            var nonce = new byte[16];
            var rand = new RNGCryptoServiceProvider();

            rand.GetBytes(nonce);

            return nonce;
        }

        #endregion
    }
}
