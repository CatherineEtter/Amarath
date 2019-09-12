using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Amarath.Models;

/*
 * Contains the Context for the Amarath Database, required for connection
 */

namespace Amarath.DAL.Data
{
    public class AmarathContext : DbContext
    {
        public AmarathContext(DbContextOptions options) : base(options)
        {

        }
        public AmarathContext(DbContextOptions<AmarathContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer("Server=tcp:amarath.database.windows.net,1433;Initial Catalog=amarath-db;Persist Security Info=False;User ID=dbadmin;Password=CS4320@stmu;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            base.OnConfiguring(builder);
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }

    }
}
