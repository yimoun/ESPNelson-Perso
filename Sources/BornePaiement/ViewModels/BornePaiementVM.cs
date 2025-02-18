using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BornePaiement.Models;
using ZXing;
using ZXing.Common;

namespace BornePaiement.ViewModels
{
    public partial class BornePaiementVM : ObservableObject
    {
        private const string ApiKey = "VOTRE_CLE_API_ICI";
        private const string ApiUrl = "https://stationnementapi.com/api/tickets/";

        [ObservableProperty]
        private string ticketId;

        [ObservableProperty]
        private Ticket ticketActuel;

        [ObservableProperty]
        private decimal montantAPayer;

        public IRelayCommand ScanTicketCommand { get; }
        public IRelayCommand SimulerPaiementCommand { get; }

        public BornePaiementVM()
        {
            ScanTicketCommand = new RelayCommand(async () => await ScanTicket());
            SimulerPaiementCommand = new RelayCommand(async () => await SimulerPaiement());
        }

        private async Task ScanTicket()
        {
            if (string.IsNullOrWhiteSpace(TicketId))
            {
                Console.WriteLine("❌ Aucun ticket scanné !");
                return;
            }

            Console.WriteLine($"📌 Vérification du ticket {TicketId}...");

            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("ApiKey", ApiKey);

                var response = await client.GetAsync(ApiUrl + TicketId);
                if (response.IsSuccessStatusCode)
                {
                    ticketActuel = await response.Content.ReadFromJsonAsync<Ticket>();

                    if (ticketActuel != null && !ticketActuel.EstPaye)
                    {
                        TimeSpan duree = DateTime.Now - ticketActuel.DateHeureEntree;
                        MontantAPayer = CalculerTarif(duree);
                        Console.WriteLine($"✅ Ticket valide ! Montant à payer : {MontantAPayer} €");
                    }
                    else
                    {
                        Console.WriteLine("⚠ Ticket invalide ou déjà payé !");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Ticket introuvable ! Code : {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 Erreur API : {ex.Message}");
            }
        }

        private decimal CalculerTarif(TimeSpan duree)
        {
            decimal tarifHoraire = 2.5m; // Tarif par heure
            return Math.Ceiling((decimal)duree.TotalHours) * tarifHoraire;
        }

        private async Task SimulerPaiement()
        {
            if (ticketActuel == null || ticketActuel.EstPaye)
            {
                Console.WriteLine("❌ Aucun ticket à payer !");
                return;
            }

            Console.WriteLine("📌 Simulation de paiement en cours...");

            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("ApiKey", ApiKey);

                var response = await client.PutAsJsonAsync(ApiUrl + TicketId + "/payer", new { Montant = MontantAPayer });

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Paiement accepté pour {MontantAPayer} € !");
                    ticketActuel.EstPaye = true;
                }
                else
                {
                    Console.WriteLine("❌ Paiement refusé !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 Erreur API : {ex.Message}");
            }
        }
    }
}
