using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BornePaiement.Model
{
    public static class AbonnementProcessor
    {
        public static async Task<AbonnementResponse> SouscrireAsync(string email, string abonnementType, string ticketId)
        {
            var data = new
            {
                Email = email,
                TypeAbonnement = abonnementType,
                TicketId = ticketId // Ajout du ticket ID requis pour la souscription
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await APIHelper.APIClient.PostAsync("abonnements/souscrire", content))
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Désérialisation de la réponse JSON
                    var result = JsonSerializer.Deserialize<AbonnementResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
                else
                {
                    Console.WriteLine($"❌ Erreur API : {response.ReasonPhrase} - {responseContent}");
                    return null;
                }
            }
        }


        public static async Task<bool> VerifierAbonnementAsync(string abonnementId)
        {
            using (HttpResponseMessage response = await APIHelper.APIClient.GetAsync($"abonnement/verifier/{abonnementId}"))
            {
                return response.IsSuccessStatusCode;
            }
        }

        

    }
}
