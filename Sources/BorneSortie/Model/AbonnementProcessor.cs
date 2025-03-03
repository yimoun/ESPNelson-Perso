using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BorneSortie.Model;



namespace BorneSortie.Model
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
                        AbonnementId = string.Empty,
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

