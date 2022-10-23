using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCurtains
{
    internal class DeviceData
    {
        string deviceName;
        int currentLevel;
        TimeOnly rollUpTime;
        TimeOnly rollDownTime;
        bool followSunset;
        bool followSunrise;
    }
}
