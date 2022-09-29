using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities.Concrete;

namespace DataAccess.Concrete
{
    public class BuilderContext : DbContext
    {
        public BuilderContext()
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=SIROWN;Database=Test;Trusted_Connection=True;");
            optionsBuilder.UseLazyLoadingProxies(false);
        }

        public DbSet<User> Users { get; set; }

    }
}
