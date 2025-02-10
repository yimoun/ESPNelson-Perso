using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjetCodeBarre.Model;

namespace ProjetCodeBarre.ViewModel
{
    public partial class PaymentVM : ObservableObject
    {
        [ObservableProperty]
        private string scannedTicketId;

        [ObservableProperty]
        private Transaction currentTransaction;

        [ObservableProperty]
        private bool isTransactionValid;

        public ICommand ProcessPaymentCommand { get; }

        private readonly ObservableCollection<Ticket> _ticketDatabase;

        public PaymentVM()
        {
            ProcessPaymentCommand = new RelayCommand(ProcessPayment);
            _ticketDatabase = new ObservableCollection<Ticket>();

            // Exemple de tickets déjà créés (Exemple : Tickets stockés en BD)
            _ticketDatabase.Add(new Ticket { Id = "ABC123", ArrivalTime = DateTime.Now.AddHours(-2) });
            _ticketDatabase.Add(new Ticket { Id = "XYZ456", ArrivalTime = DateTime.Now.AddMinutes(-30) });

            //lire l'ID du ticket scanné
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                ScannedTicketId = args[1];
            }
        }

        public void ScanTicket(string ticketId)
        {
            ScannedTicketId = ticketId;

            // Recherche du ticket dans la BD simulée
            var ticket = _ticketDatabase.FirstOrDefault(t => t.Id == ticketId);
            if (ticket != null)
            {
                CurrentTransaction = new Transaction(ticket.Id, ticket.ArrivalTime);
                IsTransactionValid = true;
            }
            else
            {
                IsTransactionValid = false; //Ticket non reconnu 
            }
        }

        private void ProcessPayment()
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.IsPaid = true;
            }
        }
    }
}
