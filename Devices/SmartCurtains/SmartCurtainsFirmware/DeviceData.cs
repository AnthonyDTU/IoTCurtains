﻿using System;
using System.Text;

namespace SmartCurtainsFirmware
{
    public class DeviceData
    {
        public int CurrentLevel { get; set; } = 0;
        public string RollUpTime { get; set; } = "08:00";
        public string RollDownTime { get; set; } = "22:00";
        public bool FollowSunset { get; set; } = false;
        public bool FollowSunrise { get; set; } = false;
    }
}
