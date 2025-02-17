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

        public int? AbonnementId { get; set; }

        [Required]
        public decimal Montant { get; set; }

        [Required]
        public DateTime DatePaiement { get; set; } = DateTime.Now;

        public int? TarificationId { get; set; }

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        [ForeignKey("AbonnementId")]
        public Abonnement Abonnement { get; set; }

        [ForeignKey("TarificationId")]
        public Tarification Tarification { get; set; }
    }
}
