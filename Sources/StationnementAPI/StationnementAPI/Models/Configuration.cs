namespace StationnementAPI.Models
{
    public class Configuration
    {
        public int Id { get; set; }
        public int CapaciteMax { get; set; }
        public decimal TaxeFederal { get; set; }
        public decimal TaxeProvincial { get; set; }
        public DateTime DateModification { get; set; }

        // Clé étrangère vers l'Utilisateur (administrateur qui modifie la configuration)
        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }
    }
}
