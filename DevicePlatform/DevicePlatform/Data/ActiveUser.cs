using DevicePlatform.Models;
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
        public static DeviceCollection Devices { get; set; }

        
        public static void ConfigureActiveUser(User activeUser, Uri uriBase)
        {
            User = activeUser;
            Devices = new DeviceCollection();
            LoggedIn = true;

            Uri deviceUri = new Uri(uriBase, $"api/{User.UserID}/Device/");

            if (User.Devices != null &&
                User.Devices.Count != 0)
            {
                foreach (var device in User.Devices)
                {
                    switch (device.DeviceType)
                    {
                        case "Smart Curtains":
                            Devices.AddNewDevice(device.DeviceID.ToString(), new SmartCurtains.SmartCurtains(deviceUri, device.DeviceID, device.DeviceName, device.DeviceKey));
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
