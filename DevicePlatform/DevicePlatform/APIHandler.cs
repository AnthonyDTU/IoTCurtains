using DevicePlatform.Data;
using DevicePlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DevicePlatform
{
    public class APIHandler
    {
        Uri uriBase = new Uri("https://localhost:7173/");
        HttpClient backendAPI;

        public APIHandler()
        {
            InitBackendConnection();
        }

        private void InitBackendConnection()
        {
            backendAPI = new HttpClient();
            backendAPI.BaseAddress = uriBase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> Login(string username, string password)
        {
            try
            {
                var result = await backendAPI.GetAsync(($"api/Users/loginAttempt?username={username}&pass={password}").Replace(" ", "%20"));
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    User user = await result.Content.ReadFromJsonAsync<User>();
                    ActiveUser.ConfigureActiveUser(user, uriBase);
                }
                else if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    // Non existing user
                }
                else
                {
                    // Other
                }

                return result.StatusCode;

            }
            catch (Exception ex)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> CreateNewUser(string username, string password)
        {
            NewUser newUser = new NewUser()
            {
                UserName = username,
                Password = password
            };

            try
            {
                var result = await backendAPI.PostAsJsonAsync<NewUser>("api/Users", newUser);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    User user = await result.Content.ReadFromJsonAsync<User>();
                    ActiveUser.ConfigureActiveUser(user, uriBase);
                }
                else if (result.StatusCode == HttpStatusCode.Conflict)
                {

                }
                else
                {
                    // Other error
                }
                return result.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }




    }
}
