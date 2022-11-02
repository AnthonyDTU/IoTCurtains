using DevicePlatform.Models;
using SmartCurtainsPlatformPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Intrinsics.Arm;

namespace DevicePlatform.Data
{
    public static class ActiveUser
    {
        public static bool LoggedIn { get; set; } = false;
        public static User User { get; set; }
        public static DevicePluginCollection DevicesPlugins { get; set; }

        
        public static void ConfigureActiveUser(User activeUser)
        {
            User = activeUser;
            DevicesPlugins = new DevicePluginCollection();
            LoggedIn = true;

            if (User.DeviceDescriptors != null &&
                User.DeviceDescriptors.Count != 0)
            {
                foreach (var device in User.DeviceDescriptors)
                {
                    switch (device.DeviceModel)
                    {
                        case "Smart Curtains":
                            DevicesPlugins.AddNewDevicePlugin(new SmartCurtainsPlatformPlugin.SmartCurtainsPlatformPlugin(device.backendUri, 
                                                                                                                          device.UserID, 
                                                                                                                          device.DeviceID, 
                                                                                                                          device.DeviceName, 
                                                                                                                          device.DeviceKey));
                            break;

                        default:
                            break;
                    }
                }
            }

        }

        public static void UpdateActiveUser(User newActiveUser)
        {
            User = newActiveUser;
        }
    }
}
