using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESPNelson.Model
{
    public class Ticket
    {
        public string Id { get; set; }  // ID du ticket généré
        public DateTime DateHeureEntree { get; set; }  // Date et heure d'entrée
        public bool EstPayé { get; set; }  // Indique si le ticket est payé
        public decimal Montant { get; set; }  // Prix total du stationnement
        public DateTime? DateHeureSortie { get; set; }  // Optionnel : date de sortie

        public Ticket() { }
    }
    
}
