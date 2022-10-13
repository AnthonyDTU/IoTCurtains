using System.ComponentModel.DataAnnotations;

namespace SmartPlatformBackendAPI.Models
{
    public class User
    {
        public Guid UserID { get; set; }

        [Key]
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public List<Device>? Devices { get; set; }
    }
}
