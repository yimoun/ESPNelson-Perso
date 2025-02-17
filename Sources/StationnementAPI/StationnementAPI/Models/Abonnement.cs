namespace StationnementAPI.Models
{
    public class Abonnement
    {
        public int Id { get; set; }
        public DateTime DateDebut { get; set; } = DateTime.Now;
        public DateTime DateFin { get; set; }
        public string Type { get; set; }  // Mensuel, Annuel, etc.

        //Clé étrangère vers Utilisateur
        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }
    }
}
