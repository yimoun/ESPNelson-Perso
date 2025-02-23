using System.Windows;
using System.Windows.Input;
using BornePaiement.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BornePaiement.ViewModel
{
    public partial class VisiteurVM : ObservableObject
    {
        [ObservableProperty] private bool ticketValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool ticketInvalide = false;
        [ObservableProperty] private string ticketInfo;

        private string ticketScanne = ""; // 🔹 Stocke temporairement le scan

        public IRelayCommand ConfirmerPaiementCommand { get; }

        public VisiteurVM()
        {
            ConfirmerPaiementCommand = new RelayCommand(async () => await ConfirmerPaiement());
        }

        //public async void KeyPressed(object sender, KeyEventArgs e)
        //{
        //    // Ignorer les touches spéciales (Shift, Ctrl, Alt, etc.)
        //    if (e.Key == Key.LeftShift || e.Key == Key.RightShift ||
        //        e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
        //        e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
        //        e.Key == Key.CapsLock || e.Key == Key.Tab ||
        //        e.Key == Key.Escape || e.Key == Key.Back)
        //    {
        //        return;
        //    }

        //    if (e.Key == Key.Enter) // 🎯 Lorsque l'utilisateur a scanné son ticket
        //    {
        //        await VerifierTicket(ticketScanne);
        //        ticketScanne = ""; // Réinitialiser le scan après traitement
        //    }
        //    else
        //    {
        //        // Capturer uniquement les chiffres
        //        if (e.Key >= Key.D0 && e.Key <= Key.D9) // Chiffres de 0 à 9
        //        {
        //            ticketScanne += e.Key.ToString().Replace("D", "");
        //        }
        //        else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) // Chiffres du pavé numérique
        //        {
        //            ticketScanne += e.Key.ToString().Replace("NumPad", "");
        //        }
        //    }
        //}

        public async Task VerifierTicket(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                return;

            // Appeler l'API pour calculer le montant
            var (montant, duree, tarification, dureeDepassee, estPaye, estConverti, messageErreur) = await TicketProcessor.CalculerMontantAsync(ticketId);

            if (!string.IsNullOrEmpty(messageErreur))
            {
                // Cas d'erreur (ticket déjà payé, déjà converti, ou autre erreur)
                TicketInfo = messageErreur;
                TicketInvalide = true;
                TicketValide = false;

                // Afficher une MessageBox pour les cas spécifiques
                if (estPaye)
                {
                    MessageBox.Show("Ce ticket a déjà été payé.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (estConverti)
                {
                    MessageBox.Show("Ce ticket a déjà été converti en abonnement.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (dureeDepassee)
            {
                // Cas de dépassement de durée
                TicketInfo = "⛔ Durée de stationnement dépassée ! Contactez l'administration.";
                TicketInvalide = true;
                TicketValide = false;
            }
            else if (montant > 0)
            {
                // Cas normal : ticket valide
                TicketInfo = $"✅ Ticket valide !\nMontant : {montant:C}\nDurée : {duree}h\nTarif : {tarification}";
                TicketValide = true;
                TicketInvalide = false;
            }
            else
            {
                // Cas d'erreur inconnue
                TicketInfo = "❌ Ticket invalide ou introuvable.";
                TicketInvalide = true;
                TicketValide = false;
            }
        }

        private async Task ConfirmerPaiement()
        {
            MessageBox.Show("💳 Paiement confirmé !");
        }
    }
}

  
