using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BornePaiement.Model
{
    public class PaiementDto
    {
        public string TicketId { get; set; }
        public string Email { get; set; }
        public string TypeAbonnement { get; set; }
    }
}
