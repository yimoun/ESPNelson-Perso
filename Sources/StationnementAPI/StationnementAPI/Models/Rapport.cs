using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationnementAPI.Models
{
    public class Rapport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateGeneration { get; set; } // Stocké en UTC pour éviter les problèmes de fuseaux horaires

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateFin { get; set; }

        public string? Fichier { get; set; } // Chemin du fichier rapport

        // Clé étrangère vers l'Utilisateur qui a généré le rapport
        [Required]
        public int UtilisateurId { get; set; }

        [ForeignKey(nameof(UtilisateurId))]
        public virtual Utilisateur Utilisateur { get; set; } // Relation avec l'utilisateur
    }
}
