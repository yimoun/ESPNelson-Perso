using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationnementAPI.Models
{
    public class Abonnement
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); //Génération automatique de l'ID unique

        [Required]
        public DateTime DateDebut { get; set; } = DateTime.UtcNow; // UTC pour la cohérence

        [Required]
        public DateTime DateFin { get; set; }

        [Required]
        public string Type { get; set; }  // Mensuel, Annuel, etc.

        [Required]
        public int UtilisateurId { get; set; }  // Clé étrangère vers Utilisateur

        [ForeignKey("UtilisateurId")]
        public virtual Utilisateur Utilisateur { get; set; }


    }
}
