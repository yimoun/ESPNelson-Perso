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
    }
}
