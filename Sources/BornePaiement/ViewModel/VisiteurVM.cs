using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using BornePaiement.Model;
using System.IO;
using System.Reflection.Metadata;
using System.Windows.Documents;
using ZXing.OneD;
using System;
using System.Windows.Media.Imaging;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using ZXing;
using ZXing.Common;

namespace BornePaiement.ViewModel
{
    public partial class VisiteurVM : ObservableObject
    {
        [ObservableProperty] private string ticketId;
        [ObservableProperty] private string email;
        [ObservableProperty] private ObservableCollection<string> abonnementsDisponibles;
        [ObservableProperty] private string selectedAbonnement;
        [ObservableProperty] private bool abonnementSouscrit;  // ✅ Pour afficher le bouton de téléchargement
        [ObservableProperty] private AbonnementResponse abonnementActuel;



        public IRelayCommand TraiterTicketCommand { get; }
        public IRelayCommand SouscrireAbonnementCommand { get; }
        public IRelayCommand TelechargerBadgeCommand { get; } // ✅ Ajout du bouton de téléchargement

        public VisiteurVM()
        {
            TraiterTicketCommand = new RelayCommand(async () => await TraiterTicket());
            SouscrireAbonnementCommand = new RelayCommand(async () => await SouscrireAbonnement());
            TelechargerBadgeCommand = new RelayCommand(async () => await TelechargerBadge());

            abonnementsDisponibles = new ObservableCollection<string> { "Mensuel", "Trimestriel", "Annuel" };
            AbonnementSouscrit = false;  // ✅ Caché par défaut
        }

        private async Task TraiterTicket()
        {
            if (string.IsNullOrWhiteSpace(TicketId))
            {
                MessageBox.Show("Veuillez scanner un ticket valide avant de souscrire.");
                return;
            }

            var (montant, duree, tarification, dureeDepassee) = await TicketProcessor.CalculerMontantAsync(TicketId);

            if (dureeDepassee)
            {
                MessageBox.Show("⛔ Durée de stationnement dépassée ! Veuillez contacter l'administration.");
                return;
            }

            MessageBox.Show($"Ticket validé : Durée {duree} heures\nTarif : {tarification}");
        }

        private async Task SouscrireAbonnement()
        {
            if (string.IsNullOrWhiteSpace(TicketId))
            {
                MessageBox.Show("Veuillez scanner un ticket valide avant de souscrire.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(SelectedAbonnement))
            {
                MessageBox.Show("Veuillez entrer un email valide et choisir un abonnement.");
                return;
            }

            var abonnementInfo = await AbonnementProcessor.SouscrireAsync(Email, selectedAbonnement, TicketId);
            

            if (abonnementInfo != null)
            {
                MessageBox.Show($"✅ Souscription réussie !\nAbonnement : {abonnementInfo.TypeAbonnement}\nDate début : {abonnementInfo.DateDebut}\nDate fin : {abonnementInfo.DateFin}");

                // 🔹 Affichage du bouton de téléchargement
                abonnementSouscrit = true;

                // 🔹 Stocker les infos pour la génération du badge
                AbonnementActuel = abonnementInfo;
            }
            else
            {
                MessageBox.Show("❌ Échec de la souscription. Vérifiez les informations saisies.");
            }
        }

        private async Task TelechargerBadge()
        {
            if (!AbonnementSouscrit)
            {
                MessageBox.Show("Veuillez souscrire à un abonnement avant de télécharger votre badge.");
                return;
            }

            try
            {
                string dossierBadges = "Badges";
                if (!Directory.Exists(dossierBadges))
                    Directory.CreateDirectory(dossierBadges);

                string pdfFilePath = Path.Combine(dossierBadges, $"Badge_{AbonnementActuel.AbonnmentId}.pdf");

                // Générer un QR Code pour l'abonnement
                System.Drawing.Bitmap qrCodeBitmap = GenerateQrCode(AbonnementActuel.AbonnmentId);
                string qrCodePath = Path.Combine(dossierBadges, $"QR_{AbonnementActuel.AbonnmentId}.png");
                qrCodeBitmap.Save(qrCodePath, System.Drawing.Imaging.ImageFormat.Png);

                // Générer le fichier PDF du badge
                using (PdfWriter writer = new PdfWriter(pdfFilePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    document.Add(new Paragraph("Badge d'Abonnement")
                        .SetFont(boldFont)
                        .SetFontSize(20)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Add(new Paragraph($"ID Abonnement : {AbonnementActuel.AbonnmentId}")
                        .SetFont(regularFont)
                        .SetFontSize(14));

                    document.Add(new Paragraph($"Type : {AbonnementActuel.TypeAbonnement}")
                        .SetFont(regularFont)
                        .SetFontSize(14));

                    document.Add(new Paragraph($"Date Début : {AbonnementActuel.DateDebut:dd/MM/yyyy}")
                        .SetFont(regularFont)
                        .SetFontSize(14));

                    document.Add(new Paragraph($"Date Fin : {AbonnementActuel.DateFin:dd/MM/yyyy}")
                        .SetFont(regularFont)
                        .SetFontSize(14));

                    // Ajouter le QR Code dans le PDF
                    if (File.Exists(qrCodePath))
                    {
                        Image qrCodeImg = new Image(ImageDataFactory.Create(qrCodePath));
                        qrCodeImg.SetWidth(150);
                        document.Add(qrCodeImg);
                    }

                    document.Add(new Paragraph("Présentez ce badge aux bornes pour entrer et sortir du stationnement.")
                        .SetFont(regularFont)
                        .SetFontSize(12)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Close();
                }

                MessageBox.Show($"✅ Badge généré : {pdfFilePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Erreur lors de la génération du badge : {ex.Message}");
            }
        }
    }
}
