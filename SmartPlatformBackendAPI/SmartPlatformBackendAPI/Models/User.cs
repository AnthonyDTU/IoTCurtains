using System.ComponentModel.DataAnnotations;

namespace SmartPlatformBackendAPI.Models
{
    public class User
    {
        [Key]
        [Required]
        public Guid UserID { get; set; }

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [ConcurrencyCheck]
        [Required]
        public ICollection<DeviceDecriptor> DeviceDescriptors { get; set; } = null!;
        
    }
}
