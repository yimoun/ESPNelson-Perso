using System.ComponentModel.DataAnnotations;


namespace StationnementAPI.Models
{
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(205)]
        public string NomUtilisateur { get; set; }

        [Required]
        [MaxLength(205)]
        public string MotDePasse { get; set; }

        [Required]
        [MaxLength(205)]
        public string Role { get; set; } = "visiteur";  //Admin, Abonné, Visiteur (par défaut)

        [Required]
        [MaxLength(205)]
        public string Email { get; set; }

        //Pour les abonnés : un code-barre pour son badge d'abonnement.
        public string? BadgeId { get; set; }

        // Un utilisateur peut avoir plusieurs abonnements
        public ICollection<Abonnement>? Abonnements { get; set; }
    }
}
