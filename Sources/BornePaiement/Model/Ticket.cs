using System;
using System.Text.Json.Serialization;

namespace BornePaiement.Model
{
    public class Ticket
    {
        public string Id { get; set; }

        [JsonPropertyName("TempsArrive")]
        public DateTime? TempsArrive { get; set; }

        public bool EstPaye { get; set; }
        public DateTime? TempsSortie { get; set; }
        public decimal Montant { get; set; }

        public Ticket() { } 
    }
}
