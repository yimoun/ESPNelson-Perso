using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationnementAPI.Models
{
    public class Paiement
    {
        [Key]
        public int Id { get; set; }

        public string? TicketId { get; set; }

        [Column(TypeName = "varchar(255)")] // ✅ S'assure que MySQL traite bien cette colonne comme un string
        public string AbonnementId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Montant { get; set; } // Calculé à partir de la tarification applicable

        [Required]
        public DateTime DatePaiement { get; set; } = DateTime.UtcNow; // Enregistre l'instant du paiement

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        [ForeignKey("AbonnementId")]
        public Abonnement? Abonnement { get; set; }
    }
}
