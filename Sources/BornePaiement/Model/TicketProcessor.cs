using BornePaiement.Model;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

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


        //public static async Task<(decimal Montant, double Duree, string TarificationAppliquee, bool DureeDepassee,
        //    bool estPaye, bool estConverti, string messageErreur)> CalculerMontantAsync(string ticketId)
        //    {
        //        using (HttpResponseMessage response = await APIHelper.APIClient.GetAsync($"paiements/calculer-montant/{ticketId}"))
        //        {

        //            if (!response.IsSuccessStatusCode)
        //            {
        //                return (0, 0, null, false, false, false, "Erreur lors de la récupération des informations du ticket.");
        //            }

        //             if (response.StatusCode == System.Net.HttpStatusCode.Forbidden) // 403 = Durée dépassée
        //            {
        //                return (0, 0, "⛔ La durée de stationnement dépasse les 24h autorisées.", true);
        //            }

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string json = await response.Content.ReadAsStringAsync();
        //                var result = JsonSerializer.Deserialize<MontantResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //                return (result.Montant, result.DureeStationnement, result.TarificationAppliquee, false);
        //            }
        //            else
        //            {
        //                Console.WriteLine($"❌ Erreur API : {response.StatusCode} - {response.ReasonPhrase}");
        //                return (0, 0, "Erreur", false);
        //            }
        //        }
        // }

        public static async Task<(decimal montant, double duree, string tarification, bool dureeDepassee, bool estPaye, bool estConverti, string messageErreur)> CalculerMontantAsync(string ticketId)
        {
            try
            {
                // Appeler l'API
                var response = await APIHelper.APIClient.GetAsync($"paiements/calculer-montant/{ticketId}");

                // Vérifier si la réponse est réussie
                if (!response.IsSuccessStatusCode)
                {
                    return (0, 0, null, false, false, false, "Erreur lors de la récupération des informations du ticket.");
                }

                // Désérialiser la réponse JSON
                var result = await response.Content.ReadFromJsonAsync<CalculMontantResponse>();

                // Retourner les résultats
                return (result.Montant, result.DureeStationnement, result.TarificationAppliquee, result.DureeDepassee, result.EstPaye, result.EstConverti, result.MessageErreur);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs
                return (0, 0, null, false, false, false, $"Erreur : {ex.Message}");
            }
        }

    }

    public class CalculMontantResponse
    {
        public decimal Montant { get; set; }
        public double DureeStationnement { get; set; }
        public string TarificationAppliquee { get; set; }
        public bool DureeDepassee { get; set; }
        public bool EstPaye { get; set; }
        public bool EstConverti { get; set; }
        public string MessageErreur { get; set; }
    }

}
