using BornePaiement.Model;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BornePaiement.Model
{
    public static class TicketProcessor
    {
        public static async Task<Ticket> GetTicketAsync(string ticketId)
        {
            using (HttpResponseMessage response = await APIHelper.APIClient.GetAsync($"tickets/{ticketId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Ticket>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    Console.WriteLine("❌ Erreur : Ticket introuvable !");
                    return null;
                }
            }
        }

        public static decimal CalculerMontant(Ticket ticket)
        {
            if (ticket == null || ticket.TempsArrive == null)
                return 0;

            DateTime heureArrivee = ticket.TempsArrive.Value;
            DateTime heureSortie = DateTime.Now;

            TimeSpan dureeStationnement = heureSortie - heureArrivee;
            decimal tarifHoraire = 2.50m; // Exemple : 2,50$ par heure
            decimal montant = (decimal)dureeStationnement.TotalHours * tarifHoraire;

            return Math.Round(montant, 2);
        }


        public static async Task<(decimal Montant, double Duree, string TarificationAppliquee, bool DureeDepassee)> CalculerMontantAsync(string ticketId)
        {
            using (HttpResponseMessage response = await APIHelper.APIClient.GetAsync($"paiements/calculer-montant/{ticketId}"))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden) // 403 = Durée dépassée
                {
                    return (0, 0, "⛔ La durée de stationnement dépasse les 24h autorisées.", true);
                }

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<MontantResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return (result.Montant, result.DureeStationnement, result.TarificationAppliquee, false);
                }
                else
                {
                    Console.WriteLine($"❌ Erreur API : {response.StatusCode} - {response.ReasonPhrase}");
                    return (0, 0, "Erreur", false);
                }
            }
        }

    }

    // Modèle pour stocker la réponse JSON
    public class MontantResponse
    {
        public decimal Montant { get; set; }
        public double DureeStationnement { get; set; }
        public string TarificationAppliquee { get; set; }

        public MontantResponse() { }
    }

}
