using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ESPNelson.Model
{
    /// <summary>
    /// Classe permettant de récupérer les informations d'un abonnement actif depuis l'API.
    /// </summary>
    public static class AbonnementProcessor
    {
        /// <summary>
        /// Effectue une requête pour obtenir les informations d'un abonnement via son identifiant.
        /// </summary>
        /// <param name="abonnementId">Identifiant de l'abonnement</param>
        /// <returns>Un objet `AbonnementResponse` contenant les détails de l'abonnement ou un message d'erreur.</returns>
        public static async Task<AbonnementResponse> GetAbonnementAsync(string abonnementId)
        {
            
                using (HttpResponseMessage response = await APIHelper.APIClient.GetAsync($"abonnements/actifs/{abonnementId}"))
                {
                    string json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Désérialisation en AbonnementResponse (cas succès)
                        return JsonSerializer.Deserialize<AbonnementResponse>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    else
                    {
                        // 🛠 Gérer les erreurs selon le StatutCode
                        string messageErreur = response.StatusCode switch
                        {
                            System.Net.HttpStatusCode.NotFound => "❌ Aucun abonnement existant pour ce ticket !",
                            System.Net.HttpStatusCode.BadRequest => "⛔ Cet abonnement n'est plus actif rendu à cette date.",
                            _ => $"⚠️ Erreur inattendue : {json}"
                        };

                        return new AbonnementResponse
                        {
                            Message = messageErreur,
                            AbonnementId = null,
                            TypeAbonnement = null,
                            DateDebut = DateTime.MinValue,
                            DateFin = DateTime.MinValue,
                            MontantPaye = 0
                        };
                    }
                }
           
        }
    }
}