using BornePaiement.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BornePaiement.Models
{
    public static class TicketProcessor
    {
        private const string ApiKey = "VOTRE_CLE_API_ICI";

        /// <summary>
        /// Cette fonction récupère un ticket existant via l'API
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public static async Task<Ticket> GetTicketAsync(string ticketId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"tickets/{ticketId}");
                request.Headers.Add("ApiKey", ApiKey);

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
