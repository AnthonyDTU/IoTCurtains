using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace SmartPlatformDB
{
    public class SmartPlatformDBContext
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
}
