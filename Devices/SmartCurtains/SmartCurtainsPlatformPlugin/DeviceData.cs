using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCurtainsPlatformPlugin
{
    public class DeviceData
    {
        public int CurrentLevel { get; set; }
        public DateTime RollUpTime { get; set; }
        public DateTime RollDownTime { get; set; }
        public bool FollowSunset { get; set; }
        public bool FollowSunrise { get; set; }
    }
}
