using System.ComponentModel.DataAnnotations;

namespace StationnementAPI.Models.ModelsDTO
{
    public class UtilisateurDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
        public string NomUtilisateur { get; set; }


        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string MotDePasse { get; set; }
    }
}
