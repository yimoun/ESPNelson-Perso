namespace StationnementAPI.Models.ModelsDTO
{
    public class PaiementDto
    {
        public string? TicketId { get; set; }
        public string? Email { get; set; } // Email de l'utilisateur
        public string? TypeAbonnement { get; set; } // Mensuel ou Hebdomadaire

        public Paiement DtoToPaiement(decimal montant, int abonnementId)
        {
            return new Paiement
            {
                TicketId = this.TicketId,
                AbonnementId = abonnementId,
                Montant = montant,
                DatePaiement = DateTime.UtcNow
            };
        }
    }
}
