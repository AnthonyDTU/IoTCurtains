using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicePlatform.Models
{
    public class User
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<DeviceParameters> Devices { get; set; }
    }
}
