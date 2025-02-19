using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BornePaiement.Model
{
    public class AbonnementResponse
    {
        public string Message { get; set; }
        public string AbonnmentId { get; set; }
        public string TypeAbonnement { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal MontantPaye { get; set; }

        public AbonnementResponse() { }
    }
}
