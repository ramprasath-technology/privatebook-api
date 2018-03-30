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
    }
}
