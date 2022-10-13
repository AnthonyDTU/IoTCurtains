namespace SmartPlatformBackendAPI.Models
{
    public class User
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public Dictionary<string, Device> Devices { get; set; }
    }
}
