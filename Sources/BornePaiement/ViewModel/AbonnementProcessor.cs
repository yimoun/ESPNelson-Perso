using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BornePaiement.Model
{
    public static class AbonnementProcessor
    {
        public static async Task<bool> SouscrireAsync(string email, string abonnementType)
        {
            var data = new
            {
                Email = email,
                TypeAbonnement = abonnementType
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await APIHelper.APIClient.PostAsync("abonnement/souscrire", content))
            {
                return response.IsSuccessStatusCode;
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
