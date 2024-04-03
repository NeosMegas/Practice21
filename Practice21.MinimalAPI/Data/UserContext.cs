using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Practice21.MinimalAPI.Models;

namespace Practice21.MinimalAPI.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Practice21.MinimalAPI.Models.User> Users { get; set; } = default!;
        public DbSet<Practice21.MinimalAPI.Models.Role> Roles { get; set; } = default!;
    }
}
