using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BorneSortie.Model
{
    public class AbonnementResponse
    {
        public string Message { get; set; }

        [JsonPropertyName("AbonnementId")]
        public string AbonnementId { get; set; }
        public string TypeAbonnement { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal MontantPaye { get; set; }
    }
}
