using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Json;

namespace SmartCurtainsPlatformPlugin
{
    internal class APIHandler
    {
        HttpClient backendAPI;

        public APIHandler(Uri backendDeviceUri)
        {
            backendAPI = new HttpClient();
            backendAPI.BaseAddress = backendDeviceUri;
        }



        /// <summary>
        /// Tries to get a devices current state
        /// </summary>
        /// <param name="deviceParameters"></param>
        /// <returns></returns>
        public async Task<DeviceData> GetCurrentDeviceState(DeviceParameters deviceParameters)
        {
            try
            {
                var result = await backendAPI.GetAsync($"getRequestedState?guid={deviceParameters.DeviceID}&key={deviceParameters.DeviceKey}");
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    DeviceData currentRequestedState = await result.Content.ReadFromJsonAsync<DeviceData>();
                    return currentRequestedState;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        /// <summary>
        /// Tries to set a devices current state
        /// </summary>
        /// <param name="deviceParameters"></param>
        /// <param name="deviceData"></param>
        public async void SetCurrentDeviceState(DeviceParameters deviceParameters, DeviceData deviceData)
        {
            try
            {
                var result = await backendAPI.PutAsJsonAsync<DeviceData>($"setRequestedState?guid={deviceParameters.DeviceID}&key={deviceParameters.DeviceKey}", deviceData);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
