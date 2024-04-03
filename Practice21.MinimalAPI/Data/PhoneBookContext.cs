using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Practice21.MinimalAPI.Models;

namespace Practice21.MinimalAPI.Data
{
    public class PhoneBookContext : DbContext
    {
        public PhoneBookContext (DbContextOptions<PhoneBookContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Practice21.MinimalAPI.Models.PhoneBookEntry> PhoneBookEntries { get; set; } = default!;
    }
}
