using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDevicePlatformPlugin;

namespace DevicePlatform.Data
{
    public class DevicePluginCollection
    {
        private Dictionary<Guid, IPlatformPlugin> plugins = new Dictionary<Guid, IPlatformPlugin>();
        public Dictionary<Guid, IPlatformPlugin> Plugins { get { return plugins; } }

        public int Count => plugins.Count; 

        public DevicePluginCollection()
        {

        }

        /// <summary>
        /// Adds a new platform plugin
        /// </summary>
        /// <param name="newPlugin"></param>
        internal void AddDevicePlugin(IPlatformPlugin newPlugin)
        {
            if (!plugins.ContainsKey(newPlugin.DeviceDescriptor.DeviceID))
            {
                //plugins[newPlugin.DeviceDescriptor.DeviceID] = newPlugin;
                plugins.Add(newPlugin.DeviceDescriptor.DeviceID, newPlugin);
            }
            else
            {
            }



            //plugins.Add(newPlugin.DeviceDescriptor.DeviceID, newPlugin);
            
        }

        /// <summary>
        /// Updates a platform plugin
        /// </summary>
        /// <param name="updatedPlugin"></param>
        internal void UpdateDevicePlugin(IPlatformPlugin updatedPlugin)
        {
            plugins.Remove(updatedPlugin.DeviceDescriptor.DeviceID);
            plugins.Add(updatedPlugin.DeviceDescriptor.DeviceID, updatedPlugin);
        }

        /// <summary>
        /// Removes a platform plugin from the plugin collection
        /// </summary>
        internal void DeleteDevicePlugin(Guid deviceID)
        {
            plugins.Remove(deviceID);
        }

        /// <summary>
        /// Gets a platform plugin
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public IPlatformPlugin GetDevicePlugin(Guid deviceID)
        {
            return plugins.GetValueOrDefault(deviceID);
        }

        
    }
}
