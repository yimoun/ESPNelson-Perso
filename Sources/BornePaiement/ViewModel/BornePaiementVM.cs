using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BornePaiement.Model;

namespace BornePaiement.ViewModel
{
    public partial class BornePaiementVM : ObservableObject
    {
        [ObservableProperty]
        private string ticketId;

        [ObservableProperty]
        private decimal montantAPayer;

        public IRelayCommand ScanTicketCommand { get; }

        public BornePaiementVM()
        {
            ScanTicketCommand = new RelayCommand(async () => await TraiterTicket());
        }

        private async Task TraiterTicket()
        {
            if (string.IsNullOrWhiteSpace(TicketId))
            {
                MessageBox.Show("Veuillez scanner un ticket valide.");
                return;
            }

            var ticket = await TicketProcessor.GetTicketAsync(TicketId);
            if (ticket != null)
            {
                MontantAPayer = TicketProcessor.CalculerMontant(ticket);
                MessageBox.Show($"Montant à payer : {MontantAPayer:C}");
            }
            else
            {
                MessageBox.Show("Ticket introuvable.");
            }
        }
    }
}
