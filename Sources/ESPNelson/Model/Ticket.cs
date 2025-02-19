using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace ESPNelson.Model
{
    public class Ticket
    {
        public string Id { get; set; }  // ID du ticket généré

        [JsonProperty("tempsArrive")] // Correspond exactement à la clé JSON de l'API
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? TempsArrive { get; set; }  // Date et heure d'entrée

        public bool EstPaye { get; set; }  // Indique si le ticket est payé
        public decimal Montant { get; set; }  // Prix total du stationnement
        public DateTime? TempsSortie { get; set; }  // Optionnel : date de sortie

        public Ticket() { }
    }
}
