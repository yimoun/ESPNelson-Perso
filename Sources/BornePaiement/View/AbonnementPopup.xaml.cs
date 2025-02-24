using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BornePaiement.ViewModel;

namespace BornePaiement.View
{
    public partial class AbonnementPopup : Window
    {
        public AbonnementPopup(string ticketScanne)
        {
            InitializeComponent();
            DataContext = new AbonnementPopupVM(ticketScanne);
        }
    }
}