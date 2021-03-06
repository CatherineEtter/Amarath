﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Amarath.Models;
using Amarath.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

/*
 * Contains the Context for the Amarath Database, required for connection
 */

namespace Amarath.DAL.Data
{
    public class AmarathContext : IdentityDbContext<IdentityUserExt>
    {
        /*public AmarathContext(DbContextOptions options) : base(options)
        {

        }*/
        public AmarathContext(DbContextOptions<AmarathContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer("CONNECTION STRING");
            base.OnConfiguring(builder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<WeaponType> WeaponTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Enemy> Enemies { get; set; }
    }
}
