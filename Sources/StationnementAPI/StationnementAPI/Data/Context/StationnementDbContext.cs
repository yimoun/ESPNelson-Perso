using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Emit;

namespace StationnementAPI.Data.Context
{
    public class StationnementDbContext : DbContext
    {
        public StationnementDbContext(DbContextOptions<StationnementDbContext> options) : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Tarification> Tarifications { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Abonnement> Abonnements { get; set; }
        public DbSet<Configuration> Configurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Utilisateur>()
                .HasMany(u => u.Tickets)
                .WithOne(t => t.Utilisateur)
                .HasForeignKey(t => t.UtilisateurId);

            modelBuilder.Entity<Utilisateur>()
                .HasMany(u => u.Abonnements)
                .WithOne(a => a.Utilisateur)
                .HasForeignKey(a => a.UtilisateurId);
        }
    }
}
