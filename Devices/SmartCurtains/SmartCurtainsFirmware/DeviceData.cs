using System;
using System.Text;

namespace SmartCurtainsFirmware
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
