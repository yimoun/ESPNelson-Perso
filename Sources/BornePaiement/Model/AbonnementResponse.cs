using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BornePaiement.Model
{
    public class AbonnementResponse
    {
        public string Message { get; set; }

        /*pour forcer la correspondance dans le noms de propriétés, pour éviter des erreurs de désérialisation
         * dans le cas où les noms des propriétés ne sont pas les meme avec ceux de l'objet retourné par L'API
         * */
        [JsonPropertyName("AbonnementId")]
        public string AbonnementId { get; set; }
        public string TypeAbonnement { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal MontantPaye { get; set; }
    }
}
