using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logique d'interaction pour TicketView.xaml
    /// </summary>
    public partial class TicketView : Window
    {
        public TicketView()
        {
            InitializeComponent();
            this.DataContext = new TicketVM();
            HiddenScannerInput.Focus(); // toujours en attente des entrées du scanner 
        }

        private void HiddenScannerInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (HiddenScannerInput.Text.Length >= 8) 
            {
                string scannedTicketId = HiddenScannerInput.Text.Trim();
                HiddenScannerInput.Clear(); //Reinitialise après lecture

                // lance la borne de paiement avec l'ID du ticket en paramètre
                OpenPaymentWindow(scannedTicketId);
            }
        }

        private void OpenPaymentWindow(string ticketId)
        {
            var process = new Process();
            process.StartInfo.FileName = "ProjetCodeBarre.exe"; // Nom de l'exécutable
            process.StartInfo.Arguments = ticketId;
            process.Start();
        }
    }
}
