using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetCodeBarre.Model
{
    public class Transaction
    {
        public string TicketId { get; set; }    
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public TimeSpan Duration => DepartureTime - ArrivalTime;
        public decimal AmountToPay { get; set; }
        public bool IsPaid { get; set; }

        public Transaction(string ticketId, DateTime arrivalTime)
        {
            TicketId = ticketId;
            ArrivalTime = arrivalTime;
            DepartureTime = DateTime.Now;
            AmountToPay = CalculatePayment();
            IsPaid = false;
        }

        private decimal CalculatePayment()
        {
            double hours = Duration.TotalHours;
            if (hours <= 1)
                return 5.00m; // Tarif horaire
            else if (hours <= 6)
                return 15.00m; // Demi-journée
            else
                return 20.00m; // Journée complète
        }
    }
}
