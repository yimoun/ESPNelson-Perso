using System;
using System.ComponentModel.DataAnnotations;


namespace StationnementAPI.Models
{
    public class Ticket
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); //Génération automatique de l'ID unique

        [Required]
        public DateTime TempsArrive { get; set; } = DateTime.Now;
        public bool EstPaye { get; set; } = false;
        public DateTime? TempsSortie { get; set; }

        public bool EstConverti { get; set; } = false;  // Indique si le ticket a été utilisé pour un abonnement

    }

}
