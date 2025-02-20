using System.Windows;
using System.Windows.Input;
using BornePaiement.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BornePaiement.ViewModel
{
    public partial class VisiteurVM : ObservableObject
    {
        [ObservableProperty] private bool ticketValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool ticketInvalide = false;
        [ObservableProperty] private string ticketInfo;

        private string ticketScanne = ""; // 🔹 Stocke temporairement le scan

        public IRelayCommand ConfirmerPaiementCommand { get; }

        public VisiteurVM()
        {
            ConfirmerPaiementCommand = new RelayCommand(async () => await ConfirmerPaiement());
        }

        public async void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // 🎯 Lorsque l'utilisateur a scanné son ticket
            {
                await VerifierTicket(ticketScanne);
                ticketScanne = ""; // Réinitialiser le scan après traitement
            }
            else
            {
                ticketScanne += e.Key.ToString().Replace("D", "").Replace("NumPad", ""); // 🔹 Capture les chiffres
            }
        }

        private async Task VerifierTicket(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                return;

            var (montant, duree, tarification, dureeDepassee) = await TicketProcessor.CalculerMontantAsync(ticketId);

            if (dureeDepassee)
            {
                TicketInfo = "⛔ Durée de stationnement dépassée ! Contactez l'administration.";
                TicketInvalide = true;
                TicketValide = false;
            }
            else if (montant > 0)
            {
                TicketInfo = $"✅ Ticket valide !\nMontant : {montant:C}\nDurée : {duree}h\nTarif : {tarification}";
                TicketValide = true;
                TicketInvalide = false;
            }
            else
            {
                TicketInfo = "❌ Ticket invalide ou introuvable.";
                TicketInvalide = true;
                TicketValide = false;
            }
        }

        private async Task ConfirmerPaiement()
        {
            MessageBox.Show("💳 Paiement confirmé !");
        }
    }
}

  
