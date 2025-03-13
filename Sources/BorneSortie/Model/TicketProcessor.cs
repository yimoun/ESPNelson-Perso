using BorneSortie.Model;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BorneSortie.Models;

namespace BorneSortie.Model
{
    public static class TicketProcessor
    {
        /// <summary>
        /// Vérifie si un ticket a été payé en interrogeant l'API.
        /// Effectue une requête HTTP GET et retourne un objet `TicketEstPayeResponse`.
        /// En cas d'erreur, un message explicatif est retourné.
        /// </summary>
        /// <param name="ticketId">Identifiant unique du ticket</param>
        /// <returns>Un objet `TicketEstPayeResponse` contenant le statut du paiement.</returns>
        public static async Task<TicketEstPayeResponse> GetTicketPayeAsync(string ticketId)
        {
            using (HttpResponseMessage response = await APIHelper.APIClient.GetAsync($"tickets/{ticketId}/verifier-paiement"))
            {
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Désérialisation en objet TicketEstPayeResponse
                    return JsonSerializer.Deserialize<TicketEstPayeResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    // 🛠 Gérer les erreurs selon le StatutCode
                    string messageErreur = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.NotFound => "❌ Ticket introuvable !",
                        System.Net.HttpStatusCode.BadRequest => "⛔ L'ID du ticket est invalide.",
                        _ => $"⚠️ Erreur inattendue : {json}"
                    };

                    return new TicketEstPayeResponse
                    {
                        TicketId = string.Empty,
                        EstPaye = false,
                        EstConverti = false,
                        Message = messageErreur,
                    };
                }
            }
        }

       
    }
}
