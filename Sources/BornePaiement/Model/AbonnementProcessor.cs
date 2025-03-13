using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BornePaiement.Model;
using Microsoft.AspNetCore.Mvc;


namespace BornePaiement.Model
{
    public static class AbonnementProcessor
    {
        public static async Task<AbonnementResponse> GetAbonnementAsync(string abonnementId)
        {
            using (HttpResponseMessage response = await APIHelper.APIClient.GetAsync($"abonnements/actifs/{abonnementId}"))
            {
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // ✅ Désérialisation en AbonnementResponse (cas succès)
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


        public static async Task<(bool Success, string Message, AbonnementResponse Abonnement)> SouscrireAbonnementAsync(string ticketId, string email, string typeAbonnement)
        {
            var paiementDto = new PaiementDto
            {
                TicketId = ticketId,
                Email = email,
                TypeAbonnement = typeAbonnement
            };

            var json = JsonSerializer.Serialize(paiementDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await APIHelper.APIClient.PostAsync("abonnements/souscrire", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var abonnementResponse = JsonSerializer.Deserialize<AbonnementResponse>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return (true, "Abonnement souscrit avec succès.", abonnementResponse);
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    return (false, $"Erreur lors de la souscription : {errorJson}", null);
                }
            }
        }
    
    
    }
    
}

