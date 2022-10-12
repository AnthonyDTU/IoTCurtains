using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartDevice;

namespace SmartCurtains
{
    public class Device : IDevice
    {
        private NodeConfiguration nodeConfiguration;
        public NodeConfiguration NodeConfiguration => nodeConfiguration;


        public IDeviceConfigurator DeviceConfigurator => throw new NotImplementedException();

        public Device()
        {
            
        }

        public Device(NodeConfiguration nodeConfiguration)
        {
            this.nodeConfiguration = nodeConfiguration;
        }


    }
}
