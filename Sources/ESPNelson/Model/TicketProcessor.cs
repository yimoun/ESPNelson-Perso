using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ESPNelson.Model
{
    /// <summary>
    /// Classe permettant la gestion des tickets, y compris leur génération via l'API.
    /// </summary>
    public static class TicketProcessor
    {
        private const string ApiKey = "CLE_API_BORNE_ENTREE";

        /// <summary>
        /// Envoie une requête à l'API pour générer un nouveau ticket.
        /// </summary>
        /// <returns>Un objet `Ticket` contenant les informations du ticket généré, ou `null` en cas d'échec.</returns>
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
                    Ticket ticketTest = JsonConvert.DeserializeObject<Ticket>(json);
                    return ticketTest;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
