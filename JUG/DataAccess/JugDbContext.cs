using System;
using JUG.Model;
using Microsoft.EntityFrameworkCore;

namespace JUG.DataAccess
{
    public class JugDbContext : DbContext
    {
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
    }
}