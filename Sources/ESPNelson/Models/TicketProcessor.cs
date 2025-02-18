using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ESPNelson.Models
{
    public static class TicketProcessor
    {
        private const string ApiKey = "VOTRE_CLE_API_ICI";

        public static async Task<Ticket> GenerateTicketAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "tickets");
                request.Headers.Add("ApiKey", ApiKey); // Ajout de la clé d'API

                var response = await APIHelper.APIClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Ticket>(json);
                }
                else
                {
                    Console.WriteLine($" Erreur API : {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erreur de connexion à l’API : {ex.Message}");
                return null;
            }
        }
    }
}
