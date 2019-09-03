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
        public AmarathContext(DbContextOptions<AmarathContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }

    }
}
