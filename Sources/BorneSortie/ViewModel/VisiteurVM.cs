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
        [ObservableProperty] private bool peutSabonner = false;
        [ObservableProperty] private bool peutSimuler = false;

        private string ticketScanne = ""; // 🔹 Stocke temporairement le scan

        [ObservableProperty]
        private bool paiementEffectue = false;

        [ObservableProperty]
        private bool afficherBoutonRecu = false;

        private const string PdfSavePath = "Recus";
        private static readonly string LogoPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "img", "logo_ciuss.jpg");

        [ObservableProperty]
        private decimal montantTotal;

        [ObservableProperty]
        private decimal taxes;

        [ObservableProperty]
        private decimal montantAvecTaxes;

        [ObservableProperty]
        private DateTime? tempsArrivee;

        [ObservableProperty]
        private DateTime? tempsSortie;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string typeAbonnement;

        [ObservableProperty]
        private bool afficherBoutonTicketAbonnement;


        public VisiteurVM()
        {
        }

        public async Task VerifierTicket(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                return;

            // Appeler l'API pour calculer le montant
            var (montant, duree, tarification, tempsArrivee, tempsSortie, dureeDepassee, estPaye, estConverti, messageErreur)
                = await TicketProcessor.CalculerMontantAsync(ticketId);

            // Afficher une MessageBox pour les cas spécifiques
            if (estPaye)
            {
                MessageBox.Show("Ce ticket a déjà été payé.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if (estConverti)
            {
                MessageBox.Show("Ce ticket a déjà été converti en abonnement.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!string.IsNullOrEmpty(messageErreur))
            {
                // Cas d'erreur (ticket déjà payé, déjà converti, ou autre erreur)
                TicketInfo = messageErreur;
                TicketInvalide = true;
                TicketValide = false;

                if (dureeDepassee)
                {
                    // Cas de dépassement de durée
                    TicketInfo = "⛔ Durée de stationnement dépassée ! Contactez l'administration.";
                    TicketInvalide = true;
                    TicketValide = false;
                }
                else
                {
                    // Cas d'erreur inconnue
                    TicketInfo = "❌ Ticket invalide ou introuvable.";
                    TicketInvalide = true;
                    TicketValide = false;
                }
            }
            else if (montant >= 0)
            {
                // Cas normal : ticket valide
                TicketInfo = $"Montant : {montant:C} $\n Temps d'arrivée: {tempsArrivee}\nDurée : {duree}h\nTarif : {tarification}";
                TicketValide = true;
                TicketInvalide = false;
                ticketScanne = ticketId;

                //Rendre visble les deux boutons
                PeutSimuler = true;
                PeutSabonner = true;
            }

        }
    }
}
