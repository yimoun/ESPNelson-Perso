using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationnementAPI.Models
{
    public class Configuration
    {
        [Key]
        public int Id { get; set; }
        public int CapaciteMax { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxeFederal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxeProvincial { get; set; }
        public DateTime DateModification { get; set; }

        // Clé étrangère vers l'Utilisateur (administrateur qui modifie la configuration)

        [Required]
        public int UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public Utilisateur Utilisateur { get; set; }
    }
}
