using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetCodeBarre.Model
{
    public class Ticket
    {
        public string Id { get; set; }
        public string HospitalName { get; set; }
        public string HospitalLogoPath { get; set; }
        public DateTime ArrivalTime { get; set; }


        public Ticket()
        {
            Id = Guid.NewGuid().ToString("N").Substring(0, 8);
            HospitalName = "CIUSSS Saguenay-Lac-Saint-Jean";
            HospitalLogoPath = "Chemin à spécifier plus tard";
            ArrivalTime = DateTime.Now;

        }
    }
}
