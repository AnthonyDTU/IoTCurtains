using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevice
{
    internal interface IDevice
    {
        public string DeviceModel { get;  }
        public string DeviceName { get; set; }

        public IDeviceConfigurator DeviceConfigurator { get; }

    }
}
