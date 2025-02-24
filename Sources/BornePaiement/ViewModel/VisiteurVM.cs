using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using BornePaiement.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Media.Imaging;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace BornePaiement.ViewModel
{
    public partial class VisiteurVM : ObservableObject
    {
        [ObservableProperty] private bool ticketValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool ticketInvalide = false;
        [ObservableProperty] private string ticketInfo;

        private string ticketScanne = ""; // 🔹 Stocke temporairement le scan

        [ObservableProperty]
        private bool paiementEffectue = false;

        [ObservableProperty]
        private bool afficherBoutonRecu = false;

        private const string PdfSavePath = "Recus";
        private static readonly string LogoPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "img", "logo_ciuss.jpg");

        [ObservableProperty]
        private decimal montantTotal;

        [ObservableProperty]
        private decimal taxes;

        [ObservableProperty]
        private decimal montantAvecTaxes;

        [ObservableProperty]
        private DateTime? tempsArrivee;

        [ObservableProperty]
        private DateTime? tempsSortie;

        public IRelayCommand ConfirmerPaiementCommand { get; }
        public IRelayCommand GenererRecuCommand { get; }

        public VisiteurVM()
        {
            ConfirmerPaiementCommand = new RelayCommand(async () => await ConfirmerPaiement());
            GenererRecuCommand = new RelayCommand(GenererRecu);
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
            var (montant, duree, tarification, tempsArrivee, tempsSortie, dureeDepassee, estPaye, estConverti, messageErreur) 
                = await TicketProcessor.CalculerMontantAsync(ticketId);

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
                TicketInfo = $"Montant : {montant:C} $\n Temps d'arrivée: {tempsArrivee}\nDurée : {duree}h\nTarif : {tarification}";
                TicketValide = true;
                TicketInvalide = false;
                ticketScanne = ticketId;
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
            var (success, message, montantTotal, taxes, montantAvecTaxes, tempsArrivee, tempsSortie) = await TicketProcessor.PayerTicketAsync(ticketScanne);

            if (success)
            {
                TicketInfo = $"✅ Paiement effectué !\nMontant : {montantAvecTaxes:C}\nTaxes : {taxes:C}\nDurée : {Math.Round((tempsSortie - tempsArrivee).Value.TotalHours, 2)}h";
                PaiementEffectue = true;
                AfficherBoutonRecu = true;

                //Informations à metrre dans le recu de paiement
                MontantTotal = montantTotal;
                TempsArrivee = tempsArrivee;    
                TempsSortie = tempsSortie;
            }
            else
            {
                TicketInfo = $"❌ Erreur lors du paiement : {message}";
                PaiementEffectue = false;
                AfficherBoutonRecu = false;
            }
        }

        private void GenererRecu()
        {
            if (MontantTotal == 0 || TempsArrivee == null || TempsSortie == null)
            {
                MessageBox.Show("Aucune information de paiement disponible pour générer le reçu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(PdfSavePath))
                Directory.CreateDirectory(PdfSavePath);

            string pdfFilePath = Path.Combine(PdfSavePath, $"Recu_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromMillimeter(80); // Format ticket de stationnement
                page.Height = XUnit.FromMillimeter(150);
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont fontTitle = new XFont("Arial", 12);
                XFont fontNormal = new XFont("Arial", 10);

                // Dessiner le logo de l'hôpital
                if (File.Exists(LogoPath))
                {
                    XImage logo = XImage.FromFile(LogoPath);
                    gfx.DrawImage(logo, (page.Width.Point - 100) / 2, 20, 100, 100); // Ajustez la taille et la position du logo
                }

                // Titre du reçu
                gfx.DrawString("Reçu de Paiement", fontTitle, XBrushes.DarkBlue, new XPoint((page.Width.Point - gfx.MeasureString("Reçu de Paiement", fontTitle).Width) / 2, 130));

                // Informations du ticket
                gfx.DrawString($"Heure d'arrivée: {TempsArrivee:dd/MM/yyyy HH:mm:ss}", fontNormal, XBrushes.Black, new XPoint(20, 150));
                gfx.DrawString($"Heure de sortie: {TempsSortie:dd/MM/yyyy HH:mm:ss}", fontNormal, XBrushes.Black, new XPoint(20, 170));
                gfx.DrawString($"Durée du séjour: {Math.Round((TempsSortie - TempsArrivee).Value.TotalHours, 2)} heures", fontNormal, XBrushes.Black, new XPoint(20, 190));

                // Montant et taxes
                gfx.DrawString($"Montant total: {MontantTotal:C}", fontNormal, XBrushes.Black, new XPoint(20, 210));
                gfx.DrawString($"Taxes (fédéral: {Taxes / MontantTotal * 100}%, provincial: {Taxes / MontantTotal * 100}%): {Taxes:C}", fontNormal, XBrushes.Black, new XPoint(20, 230));
                gfx.DrawString($"Montant avec taxes: {MontantAvecTaxes:C}", fontNormal, XBrushes.Black, new XPoint(20, 250));

                // Message de remerciement
                gfx.DrawString("Merci pour votre visite !", fontNormal, XBrushes.DarkGreen, new XPoint((page.Width.Point - gfx.MeasureString("Merci pour votre visite !", fontNormal).Width) / 2, 280));

                document.Save(pdfFilePath);
            }

            // Ouvrir le PDF généré
            Process.Start(new ProcessStartInfo(pdfFilePath) { UseShellExecute = true });
            MessageBox.Show("Reçu généré avec succès !", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
    



  
