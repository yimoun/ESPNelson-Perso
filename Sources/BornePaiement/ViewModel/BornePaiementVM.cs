//using System;
//using System.Threading.Tasks;
//using System.Windows;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using BornePaiement.Model;

//namespace BornePaiement.ViewModel
//{
//    public partial class BornePaiementVM : ObservableObject
//    {
//        [ObservableProperty]
//        private string ticketId;

//        [ObservableProperty]
//        private decimal montantAPayer;

//        [ObservableProperty]
//        private double dureeStationnement;

//        [ObservableProperty]
//        private string tarificationAppliquee;

//        public IRelayCommand ScanTicketCommand { get; }

//        public BornePaiementVM()
//        {
//            ScanTicketCommand = new RelayCommand(async () => await TraiterTicket());
//        }

//        private async Task TraiterTicket()
//        {
//            if (string.IsNullOrWhiteSpace(TicketId))
//            {
//                MessageBox.Show("Veuillez scanner un ticket valide.");
//                return;
//            }

//            var (montant, duree, tarification, dureeDepassee) = await TicketProcessor.CalculerMontantAsync(TicketId);

//            if (dureeDepassee)
//            {
//                MessageBox.Show("La durée de stationnement dépasse les 24h autorisées.\nVeuillez contacter l'administration.",
//                                "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
//                return; // On bloque le processus de paiement
//            }

//            if (montant > 0)
//            {
//                MontantAPayer = montant;
//                DureeStationnement = duree;
//                TarificationAppliquee = tarification;

//                MessageBox.Show($"Montant à payer : {MontantAPayer:C}\n⏳ Durée : {DureeStationnement} heures\n🏷 Tarif : {TarificationAppliquee}",
//                                "Paiement", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//            else
//            {
//                MessageBox.Show("Erreur : Ticket introuvable ou problème avec l'API.",
//                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//    }
//}


using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using BornePaiement.View;

namespace BornePaiement.ViewModel
{
    public partial class BornePaiementVM : ObservableObject
    {
        [ObservableProperty]
        private UserControl selectedView; // Stocke la page actuelle (Visiteur ou Abonné)

        public IRelayCommand SetModeVisiteurCommand { get; }
        public IRelayCommand SetModeAbonneCommand { get; }

        public BornePaiementVM()
        {

            SetModeVisiteurCommand = new RelayCommand(() => SelectedView = new VisiteurPage());
            SetModeAbonneCommand = new RelayCommand(() => SelectedView = new AbonnePage());
        }
    }
}

