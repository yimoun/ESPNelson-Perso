
using System;
using System.Linq;
using System.Windows;
using ProjetCodeBarre.View;


namespace ProjetCodeBarre
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Vérifie s'il y a un argument (ex: ID du ticket)
            if (e.Args.Length > 0)
            {
                string ticketId = e.Args[0]; // Récupére l'ID du ticket passé par Process.Start()
                var paymentWindow = new PaymentView(ticketId); // Ouvrir PaymentView.xaml
                paymentWindow.Show();
            }
            else
            {
                var ticketWindow = new TicketView(); // Ouvrir TicketView.xaml par défaut
                ticketWindow.Show();
            }
        }
    }
}
