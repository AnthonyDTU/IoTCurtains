using DevicePlatform.Data;
using DevicePlatform.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using SmartDevicePlatformPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DevicePlatform.BackendControllers
{
    public class APIController
    {
        private HttpClient backendAPI;
        private Uri uriBase = new Uri("https://localhost:7173/");

        /// <summary>
        /// 
        /// </summary>
        public APIController()
        {
            InitBackendConnection();
        }

        /// <summary>
        /// Initializes the backend connection
        /// </summary>
        private void InitBackendConnection()
        {
            backendAPI = new HttpClient();
            backendAPI.BaseAddress = uriBase;
        }

        /// <summary>
        /// Logs a user in, and gets its data from the backend
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> Login(string username, string password)
        {
            UserCredentials userCredentials = new UserCredentials(username, password);

            try
            {
                var result = await backendAPI.PostAsJsonAsync("api/Users/login", userCredentials);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    User user = await result.Content.ReadFromJsonAsync<User>();
                    await ActiveUser.Instance.ConfigureActiveUser(user);
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
                return HttpStatusCode.Ambiguous;
            }
        }

        /// <summary>
        /// Creates a new user in the backend
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

                var result = await backendAPI.PutAsJsonAsync("api/Users", newUser);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    User user = await result.Content.ReadFromJsonAsync<User>();
                    await ActiveUser.Instance.ConfigureActiveUser(user);
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

        /// <summary>
        /// Updates a user in the backend
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> UpdateUser()
        {
            if (ActiveUser.Instance.LoggedIn == false)
            {
                return HttpStatusCode.PreconditionFailed;
            }

            try
            {
                var result = await backendAPI.PostAsJsonAsync("api/Users", ActiveUser.Instance.User);
                if (result.StatusCode == HttpStatusCode.OK)
                {

                }
                else if (result.StatusCode == HttpStatusCode.NotFound)
                {

                }
                return result.StatusCode;

            }
            catch (Exception)
            {
                return HttpStatusCode.BadRequest;
            }
        }

        /// <summary>
        /// Adds a new device to the user in the backend
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> AddNewDevice(DeviceDescriptor deviceDescriptor)
        {
            string uri = $"api/Users";

            try
            {
                var result = await backendAPI.PostAsJsonAsync(uri, deviceDescriptor);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    User user = await result.Content.ReadFromJsonAsync<User>();
                    //await ActiveUserSingleton.Instance.ConfigureActiveUser(user);
                }

                return result.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode.BadRequest;
            }
        }

        /// <summary>
        /// Deletes a user from the backend
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> DeleteUser(Guid userID)
        {
            string uri = $"api/Users/{userID}";

            try
            {
                var result = await backendAPI.DeleteAsync(uri);
                return result.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode.BadRequest;
            }
        }

        /// <summary>
        /// Deletes a device from the user in the backend
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> DeleteDevice(Guid userID, Guid deviceID)
        {
            string uri = $"api/Users/{userID}/{deviceID}";

            try
            {
                var result = await backendAPI.DeleteAsync(uri);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    User user = await result.Content.ReadFromJsonAsync<User>();
                    ActiveUser.Instance.UpdateActiveUser(user);
                }

                return result.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode.BadRequest;
            }
        }
    }
}
