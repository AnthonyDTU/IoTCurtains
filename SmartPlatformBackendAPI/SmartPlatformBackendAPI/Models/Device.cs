
using System.ComponentModel.DataAnnotations;

namespace SmartPlatformBackendAPI.Models
{
    public class Device
    {
        [Key]
        [Required]
        public Guid DeviceID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [Required]
        public string DeviceName { get; set; } = null!;

        [Required]
        public string DeviceType { get; set; } = null!;
        
        [Required]
        public string DeviceKey { get; set; } = null!;
    }
}
