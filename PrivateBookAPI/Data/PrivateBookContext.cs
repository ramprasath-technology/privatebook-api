using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateBookAPI.Data
{
    public class PrivateBookContext :DbContext
    {
        public PrivateBookContext(DbContextOptions<PrivateBookContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<UserFeatureMapping> UserFeatureMappings { get; set; }
        public DbSet<Goals> Goals { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}
