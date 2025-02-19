using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using BornePaiement.Model;

namespace BornePaiement.ViewModel
{
    public partial class VisiteurVM : ObservableObject
    {
        [ObservableProperty] private string ticketId;
        [ObservableProperty] private string email;
        [ObservableProperty] private ObservableCollection<string> abonnementsDisponibles;
        [ObservableProperty] private string selectedAbonnement;

        public IRelayCommand TraiterTicketCommand { get; }
        public IRelayCommand SouscrireAbonnementCommand { get; }

        public VisiteurVM()
        {
            TraiterTicketCommand = new RelayCommand(async () => await TraiterTicket());
            SouscrireAbonnementCommand = new RelayCommand(async () => await SouscrireAbonnement());

            // Charger les types d'abonnements depuis l'API
            abonnementsDisponibles = new ObservableCollection<string> { "Mensuel", "Trimestriel", "Annuel" };
        }

        private async Task TraiterTicket()
        {
            if (string.IsNullOrWhiteSpace(TicketId))
            {
                MessageBox.Show("Veuillez scanner un ticket valide.");
                return;
            }

            var (montant, duree, tarification, dureeDepassee) = await TicketProcessor.CalculerMontantAsync(TicketId);

            if (dureeDepassee)
            {
                MessageBox.Show("⛔ Durée de stationnement dépassée ! Veuillez contacter l'administration.");
                return;
            }

            MessageBox.Show($"Montant à payer : {montant:C}\nDurée : {duree} heures\nTarif : {tarification}");
        }

        private async Task SouscrireAbonnement()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(SelectedAbonnement))
            {
                MessageBox.Show("Veuillez entrer un email valide et choisir un abonnement.");
                return;
            }

            var success = await AbonnementProcessor.SouscrireAsync(Email, SelectedAbonnement);
            if (success)
            {
                MessageBox.Show("✅ Abonnement souscrit avec succès !");
            }
            else
            {
                MessageBox.Show("❌ Échec de la souscription.");
            }
        }
    }
}
