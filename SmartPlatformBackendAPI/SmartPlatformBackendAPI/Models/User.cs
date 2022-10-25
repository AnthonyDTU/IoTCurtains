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
        public ICollection<Device> Devices { get; set; } = null!;

        //public User(Guid userID, string userName, string password, ICollection<Device> devices)
        //{
        //    UserID = userID;
        //    UserName = userName;
        //    Password = password;
        //    Devices = devices;
        //}
    }
}
