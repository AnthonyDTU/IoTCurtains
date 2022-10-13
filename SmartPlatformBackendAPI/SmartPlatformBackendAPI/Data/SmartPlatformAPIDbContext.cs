
using Microsoft.EntityFrameworkCore;
using SmartPlatformBackendAPI.Models;

namespace SmartPlatformBackendAPI.Data
{
    public class SmartPlatformAPIDbContext : DbContext
    {
        public SmartPlatformAPIDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
