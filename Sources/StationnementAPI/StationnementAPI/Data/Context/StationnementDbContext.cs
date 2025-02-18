using Microsoft.EntityFrameworkCore;
using StationnementAPI.Models;

namespace StationnementAPI.Data.Context
{
    public class StationnementDbContext : DbContext
    {
        public StationnementDbContext(DbContextOptions<StationnementDbContext> options) : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Tarification> Tarifications { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Abonnement> Abonnements { get; set; }
        public DbSet<Paiement> Paiements { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Rapport> Rapports { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Définition explicite des noms de table
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateur");
            modelBuilder.Entity<Tarification>().ToTable("Tarification");
            modelBuilder.Entity<Ticket>().ToTable("Ticket");
            modelBuilder.Entity<Abonnement>().ToTable("Abonnement");
            modelBuilder.Entity<Paiement>().ToTable("Paiement");
            modelBuilder.Entity<Configuration>().ToTable("Configuration");
            modelBuilder.Entity<Rapport>().ToTable("Rapport"); 

            // Définition des relations
            modelBuilder.Entity<Abonnement>()
                 .HasOne(a => a.Utilisateur)
                 .WithMany(u => u.Abonnements)
                 .HasForeignKey(a => a.UtilisateurId)
                 .OnDelete(DeleteBehavior.Cascade);  // Suppression en cascade si nécessaire

            modelBuilder.Entity<Paiement>()
                .HasOne(p => p.Ticket)
                .WithOne()
                .HasForeignKey<Paiement>(p => p.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Paiement>()
                .HasOne(p => p.Abonnement)
                .WithOne()
                .HasForeignKey<Paiement>(p => p.AbonnementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
               .Property(t => t.TempsArrive)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Rapport>()
                .HasOne(r => r.Utilisateur)
                .WithMany()
                .HasForeignKey(r => r.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();  // ✅ Assure l'unicité de l'email
        }
    }
}
