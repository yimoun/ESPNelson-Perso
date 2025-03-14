using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Administration.Model
{
    public class Paiement
    {
        [Key]
        public int Id { get; set; }

        public string? TicketId { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? AbonnementId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Montant { get; set; } // Montant total payé (déjà calculé avec les taxes)

        [Required]
        public DateTime DatePaiement { get; set; } = DateTime.UtcNow;

        // Détails de la tarification appliquée au moment du paiement
        [MaxLength(50)]
        public string? TarificationNiveau { get; set; } // Niveau de tarification

        [Column(TypeName = "decimal(10,2)")]
        public decimal? TarificationPrix { get; set; } // Prix appliqué

        public int?TarificationDureeMin { get; set; } // Durée minimale appliquée

        public int? TarificationDureeMax { get; set; } // Durée maximale appliquée

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        [ForeignKey("AbonnementId")]
        public Abonnement? Abonnement { get; set; }
    }
}

