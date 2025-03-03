using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BorneSortie.Model;

namespace BorneSortie.ViewModel
{
    public partial class VisiteurVM : ObservableObject
    {
        [ObservableProperty] private bool ticketValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool ticketInvalide = false;
        [ObservableProperty] private string ticketInfo;

        private string ticketScanne = ""; // 🔹 Stocke temporairement le scan


        [ObservableProperty]
        private DateTime? tempsArrivee;

        [ObservableProperty]
        private DateTime? tempsSortie;


        public VisiteurVM()
        {
        }

        public async Task VerifierTicketPaye(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                return;

            // Appeler l'API pour vérifier le statut du paiement
            var ticketResponse = await TicketProcessor.GetTicketPayeAsync(ticketId);

            if (ticketResponse == null)
            {
                MessageBox.Show("❌ Erreur de communication avec l'API.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ticketResponse.Message == "NotFound" || ticketResponse.Message == "BadRequest")
            {
                MessageBox.Show("❌ Ticket introuvable ou invalide. Veuillez réessayer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                TicketValide = false;
                TicketInvalide = true;
                TicketInfo = "❌ Ticket introuvable.";
                return;
            }

            if (ticketResponse.EstConverti)
            {
                //TicketInfo = $"✅ Paiement validé !\nHeure d'arrivée : {ticketResponse.TempsArrivee}\nHeure de sortie : {ticketResponse.TempsSortie}";
                MessageBox.Show("Ce ticket a été converti en abonnment donc n'a pas été payé.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                TicketValide = false;
                TicketInvalide = true;
                TicketInfo = "Ticket non payé.";
                return;
            }

            // ✅ Ticket payé, mise à jour de l'affichage
            TicketInfo = $"✅ Paiement validé !\nHeure d'arrivée : {ticketResponse.TempsArrivee}\nHeure de sortie : {ticketResponse.TempsSortie}";
            TicketValide = true;
            TicketInvalide = false;

            // Mise à jour des propriétés pour affichage
            TempsArrivee = ticketResponse.TempsArrivee;
            TempsSortie = ticketResponse.TempsSortie;
        }

    }
}
