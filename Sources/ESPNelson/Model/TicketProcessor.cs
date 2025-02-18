using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ESPNelson.Model
{
    public static class TicketProcessor
    {
        private const string ApiKey = "MY_SECRET_API_KEY_12345";

        public static async Task<Ticket> GenerateTicketAsync()
        {
            try
            {
                // Vérifier si APIClient est null, et le réinitialiser si nécessaire
                if (APIHelper.APIClient == null)
                {
                    APIHelper.InitializeClient();
                }

                var request = new HttpRequestMessage(HttpMethod.Post, "tickets/generer");
                request.Headers.Add("ApiKey", ApiKey); // Ajout de la clé d'API

                var response = await APIHelper.APIClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Ticket>(json);
                }
                else
                {
                    Console.WriteLine($"Erreur API : {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de connexion à l’API : {ex.Message}");
                return null;
            }
        }
    }
}
