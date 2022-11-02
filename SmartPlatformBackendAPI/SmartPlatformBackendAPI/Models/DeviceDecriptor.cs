
using System.ComponentModel.DataAnnotations;

namespace SmartPlatformBackendAPI.Models
{
    public class DeviceDecriptor
    {
        [Key]
        [Required]
        public Guid DeviceID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [Required]
        public string DeviceName { get; set; } = null!;

        [Required]
        public string DeviceModel { get; set; } = null!;
        
        [Required]
        public string DeviceKey { get; set; } = null!;

        [Required]
        public Uri backendUri { get; set; }
    }
}
