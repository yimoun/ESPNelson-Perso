using BornePaiement.Model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BornePaiement.Model
{
    public static class PaiementProcessor
    {
        private const string ApiKey = "VOTRE_CLE_API_ICI";

        /// <summary>
        /// Envoie d'une requête PUT à StationnmentAPI pour enregistrer un paiement.
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="montant"></param>
        /// <returns></returns>
        public static async Task<bool> EffectuerPaiementAsync(string ticketId, decimal montant)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, $"tickets/{ticketId}/payer");
                request.Headers.Add("ApiKey", ApiKey);
                request.Content = new StringContent(JsonConvert.SerializeObject(new { Montant = montant }), Encoding.UTF8, "application/json");

                var response = await APIHelper.APIClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Paiement effectué pour {montant} $ !");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Échec du paiement : {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur API : {ex.Message}");
                return false;
            }
        }
    }
}
