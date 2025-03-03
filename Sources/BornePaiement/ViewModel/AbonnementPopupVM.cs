using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BornePaiement.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using ZXing.Common;
using ZXing;
using BornePaiement.View;

namespace BornePaiement.ViewModel
{
    public partial class AbonnementPopupVM : ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string typeAbonnement;
        private DateTime dateDebut;
        private DateTime dateFin;
        private string abonnementId;
        private const string PdfSavePath = "Abonnments";
        private static readonly string LogoPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "img", "logo_ciuss.jpg");
        
        [ObservableProperty]
        private ObservableCollection<string> typesAbonnement = new ObservableCollection<string>();

        [ObservableProperty]
        private bool afficherBoutonTicketAbonnement = false;

        private string _ticketScanne;

        [ObservableProperty] private bool infoAbonnementVisible = true;  
        [ObservableProperty] private bool peutSimuler = true;  
        [ObservableProperty] private bool peutAfficherBoutonGenerer = false;  

        [ObservableProperty]
        private BitmapImage barcodeImage;

        public AbonnementPopupVM(string ticketScanne)
        {
            _ticketScanne = ticketScanne;
            ChargerTypesAbonnement();
        }

        public AbonnementPopupVM()
        {
            // Constructeur sans paramètre requis pour l'inialisation via XAML
            ChargerTypesAbonnement();
        }

        public ICommand ConfirmerCommand => new RelayCommand(Confirmer);
        public ICommand GenererTicketAbonnementCommand => new RelayCommand(GenererTicketAbonnement);

        private void ChargerTypesAbonnement()
        {
            TypesAbonnement.Clear(); // Vide la liste existante avant d'ajouter de nouveaux éléments
            TypesAbonnement.Add("hebdomadaire");
            TypesAbonnement.Add("mensuel");
            OnPropertyChanged(nameof(TypesAbonnement)); // Notifier l'UI d'une mise à jour
        }


        private async void Confirmer()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(TypeAbonnement))
            {
                MessageBox.Show("Veuillez entrer votre email et sélectionner un type d'abonnement.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Afficher le clavier pour simuler le paiement
            // Ouvrir la fenêtre NumPad
            var numPadPopup = new NumPadPopup();
            bool? result = numPadPopup.ShowDialog();

            // Vérifier si l'utilisateur a confirmé un NIP
            if (result == true)
            {
                if (numPadPopup.EnteredPin == "999")
                {
                    var (success, message, abonnement) = await TicketProcessor.SouscrireAbonnementAsync(_ticketScanne, Email, TypeAbonnement);

                    if (success)
                    {
                        MessageBox.Show($"Abonnement souscrit avec succès !\nType : {abonnement.TypeAbonnement}\nDate de début : {abonnement.DateDebut:dd/MM/yyyy}\nDate de fin : {abonnement.DateFin:dd/MM/yyyy}\nMontant : {abonnement.MontantPaye:C}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                        PeutAfficherBoutonGenerer = true;
                        PeutSimuler = false;
                        InfoAbonnementVisible = false;

                        //Les infos à mettre dans le ticket d'abonnment qui sera généré
                        dateDebut = abonnement.DateDebut;
                        dateFin = abonnement.DateFin;
                        abonnementId = abonnement.AbonnementId;


                        //pour l'insérer par la suite dans le ticket d'abonnment
                        Bitmap barcodeBitmap = GenerateBarcode(abonnementId);
                        if (barcodeBitmap != null)
                        {
                            BarcodeImage = ConvertBitmapToBitmapImage(barcodeBitmap);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Erreur lors de la souscription : {message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("❌ NIP incorrect. Veuillez réessayer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private Bitmap GenerateBarcode(string text)
        {
            try
            {
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions { Width = 200, Height = 100 }
                };
                var pixelData = writer.Write(text);
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    bitmap.UnlockBits(bitmapData);
                    return new Bitmap(bitmap);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la génération du code-barres : {ex.Message}");
                return null;
            }
        }


        //private XImage ConvertBitmapToXImage(Bitmap bitmap)
        //{
        //    using (MemoryStream memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
        //        memory.Position = 0;
        //        return XImage.FromStream(memory);
        //    }
        //}


        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(memory.ToArray());
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private void GenererTicketAbonnement()
        {
            // Vérifier si les informations nécessaires sont disponibles
            if (string.IsNullOrEmpty(TypeAbonnement) || dateDebut == null || dateFin == null)
            {
                MessageBox.Show("Les informations de l'abonnement sont incomplètes.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Chemin de sauvegarde du PDF
            string pdfSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TicketsAbonnement");
            if (!Directory.Exists(pdfSavePath))
            {
                Directory.CreateDirectory(pdfSavePath);
            }

            // Nom du fichier PDF
            string pdfFilePath = Path.Combine(PdfSavePath, $"Abonnement_{abonnementId}.pdf");

            // Créer le document PDF
            using (PdfDocument document = new PdfDocument())
            {
                // Ajouter une page au document
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromMillimeter(80); // Format ticket (80 mm de largeur)
                page.Height = XUnit.FromMillimeter(150); // Hauteur du ticket

                // Créer un objet XGraphics pour dessiner sur la page
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Définir les polices
                XFont fontTitle = new XFont("Arial", 14);
                XFont fontNormal = new XFont("Arial", 10);
                XFont fontSmall = new XFont("Arial", 8);

                // Dessiner le logo de l'hôpital
                if (File.Exists(LogoPath))
                {
                    XImage logo = XImage.FromFile(LogoPath);
                    gfx.DrawImage(logo, (page.Width.Point - 80) / 2, 20, 80, 80); // Centrer le logo
                }

                // Titre du ticket
                gfx.DrawString("Ticket d'Abonnement", fontTitle, XBrushes.DarkBlue, new XPoint((page.Width.Point - gfx.MeasureString("Ticket d'Abonnement", fontTitle).Width) / 2, 110));

                // Informations de l'hôpital
                gfx.DrawString("Hôpital de Chicoutimi", fontNormal, XBrushes.Black, new XPoint(20, 130));

                // Dessiner le code-barres au mileu du ticket
                using (MemoryStream memory = new MemoryStream())
                {
                    BarcodeImage.StreamSource.Position = 0;
                    BarcodeImage.StreamSource.CopyTo(memory);
                    memory.Position = 0;
                    XImage barcodeXImage = XImage.FromStream(memory);
                    gfx.DrawImage(barcodeXImage, (page.Width.Point - 300) / 2, 140, 300, 100); // Positionner le code-barres
                }


                // Informations de l'abonnement (déplacées après le code-barres)
                gfx.DrawString($"Type d'abonnement: {TypeAbonnement}", fontNormal, XBrushes.Black, new XPoint(20, 250));
                gfx.DrawString($"Date de début: {dateDebut:dd/MM/yyyy}", fontNormal, XBrushes.Black, new XPoint(20, 270));
                gfx.DrawString($"Date de fin: {dateFin:dd/MM/yyyy}", fontNormal, XBrushes.Black, new XPoint(20, 290));

                // Message de remerciement
                gfx.DrawString("Merci pour votre confiance !", fontNormal, XBrushes.DarkGreen, new XPoint((page.Width.Point - gfx.MeasureString("Merci pour votre confiance !", fontNormal).Width) / 2, 330));

                // Sauvegarder le document
                document.Save(pdfFilePath);
            }

            // Ouvrir le PDF généré
            Process.Start(new ProcessStartInfo(pdfFilePath) { UseShellExecute = true });
            MessageBox.Show("Ticket d'abonnement généré avec succès !", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}