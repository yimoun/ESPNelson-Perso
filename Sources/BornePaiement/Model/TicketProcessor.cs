﻿using BornePaiement.Model;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Reflection.Metadata;

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


        public static async Task<(decimal montant, double duree, string tarification, DateTime? tempsArrivee, DateTime? tempsSortie, bool dureeDepassee,
            bool estPaye, bool estConverti, string messageErreur)> CalculerMontantAsync(string ticketId)
        {
            try
            {
                // Appeler l'API
                var response = await APIHelper.APIClient.GetAsync($"paiements/calculer-montant/{ticketId}");

                // Vérifier si la réponse est réussie
                if (!response.IsSuccessStatusCode)
                {
                    return (0, 0, null, null, null, false, false, false, "Erreur lors de la récupération des informations du ticket.");
                }

                // Désérialiser la réponse JSON
                var result = await response.Content.ReadFromJsonAsync<CalculMontantResponse>();

                // Retourner les résultats
                return (result.Montant, result.DureeStationnement, result.TarificationAppliquee, result.TempsArrivee, result.TempsSortie,
                    result.DureeDepassee, result.EstPaye, result.EstConverti, result.MessageErreur);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs
                return (0, 0, null, null, null, false, false, false, $"Erreur : {ex.Message}");
            }
        }


        public static async Task<(bool success, string message, decimal montantTotal, decimal taxes, decimal montantAvecTaxes, DateTime? tempsArrivee,
            DateTime? tempsSortie)> PayerTicketAsync(string ticketId)
        {
            try
            {
                // Préparer les données pour le paiement
                var paiementDto = new { TicketId = ticketId };

                // Appeler l'API pour effectuer le paiement
                var response = await APIHelper.APIClient.PostAsJsonAsync("paiements/payer-ticket", paiementDto);

                // Vérifier si la réponse est réussie
                if (!response.IsSuccessStatusCode)
                {
                    return (false, "Erreur lors du paiement du ticket.", 0, 0, 0, null, null);
                }

                // Désérialiser la réponse JSON
                var result = await response.Content.ReadFromJsonAsync<PaiementResponse>();

                // Retourner les résultats
                return (true, result.Message, result.MontantTotal, result.Taxes, result.MontantAvecTaxes, result.TempsArrivee, result.TempsSortie);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs
                return (false, $"Erreur : {ex.Message}", 0, 0, 0, null, null);
            }
        }

        public class PaiementResponse
        {
            public string Message { get; set; }
            public decimal MontantTotal { get; set; }
            public decimal Taxes { get; set; }
            public decimal MontantAvecTaxes { get; set; }
            public DateTime? TempsArrivee { get; set; }
            public DateTime? TempsSortie { get; set; }
        }

    }

    public class CalculMontantResponse
    {
        public decimal Montant { get; set; }
        public double DureeStationnement { get; set; }
        public string TarificationAppliquee { get; set; }
        public  DateTime? TempsArrivee { get; set; }
        public DateTime? TempsSortie { get; set; }
        public bool DureeDepassee { get; set; }
        public bool EstPaye { get; set; }
        public bool EstConverti { get; set; }
        public string MessageErreur { get; set; }


       public CalculMontantResponse() { }
    }

}
