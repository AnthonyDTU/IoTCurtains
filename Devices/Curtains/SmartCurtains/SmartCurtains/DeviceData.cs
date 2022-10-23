using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCurtains
{
    internal class DeviceData
    {
        public Guid DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string DeviceKey { get; set; }

        public int CurrentLevel { get; set; }
        public TimeOnly RollUpTime { get; set; }
        public TimeOnly RollDownTime { get; set; }
        public bool FollowSunset { get; set; }
        public bool FollowSunrise { get; set; }
    }
}
