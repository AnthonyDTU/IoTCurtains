
using Microsoft.EntityFrameworkCore;
using SmartPlatformBackendAPI.Controllers;
using SmartPlatformBackendAPI.Models;

namespace SmartPlatformBackendAPI.Data
{
    public class SmartPlatformAPIDbContext : DbContext
    {
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public SmartPlatformAPIDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
