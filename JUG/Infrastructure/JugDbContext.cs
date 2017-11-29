using System;
using JUG.CRUD;
using JUG.Domain;
using Microsoft.EntityFrameworkCore;

namespace JUG.Infrastructure
{
    public class JugDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<Intervention> Interventions { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
    }
}