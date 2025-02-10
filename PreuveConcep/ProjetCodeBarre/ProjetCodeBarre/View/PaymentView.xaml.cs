using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProjetCodeBarre.ViewModel;

namespace ProjetCodeBarre.View
{
    /// <summary>
    /// Logique d'interaction pour PaymentView.xaml
    /// </summary>
    public partial class PaymentView : Window
    {
        public PaymentView()
        {
            InitializeComponent();
            var vm = new PaymentVM();

            //Récupération de l'ID du tocket depuis les arguments passés par Process.Start()
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                string ticketId = args[1]; //On Récupére l’ID du ticket
                vm.ScanTicket(ticketId); // 🔹 Mettre à jour le ViewModel
            }

            DataContext = vm;
        }

        public PaymentView(string ticketId) // Ajout du constructeur avec un paramètre
        {
            InitializeComponent();
            var vm = new PaymentVM(); 
            vm.ScanTicket(ticketId); //Mettre à jour le ViewModel avec l'ID du ticket
            DataContext = vm;
        }
    }
}
