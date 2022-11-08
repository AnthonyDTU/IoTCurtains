using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SmartPlatformBackendAPI.Models
{
    public class UserCredentials 
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
