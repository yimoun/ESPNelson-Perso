using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ESPNelson.Model;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using ZXing;
using ZXing.Common;

namespace ESPNelson.ViewModel
{
    public partial class BorneEntreeVM : ObservableObject
    {
        private const string NomHopital = "CIUSS Saguenay Hôpital de Chicoutimi";
        private const string AdresseHopital = "930 Jacques-Cartier Est, Chicoutimi, QC";
        private const string ContactHopital = "Tél: +1 234 567 890 | contact@hopitalchicoutimi.ca";
        private const string LogoPath = "Images/logo_hopital.png"; // À placer dans le projet
        private const string PdfSavePath = "Tickets"; // Dossier pour les PDFs

        [ObservableProperty]
        private Ticket ticketActuel;

        [ObservableProperty]
        private BitmapImage barcodeImage;

        public IRelayCommand GenerateNewTicketCommand { get; }

        public BorneEntreeVM()
        {
            GenerateNewTicketCommand = new RelayCommand(async () => await GenerateNewTicket());
        }

        private async Task GenerateNewTicket()
        {
            Console.WriteLine("📌 Demande de génération d’un ticket via API...");

            var nouveauTicket = await TicketProcessor.GenerateTicketAsync();

            if (nouveauTicket != null)
            {
                Console.WriteLine($"✅ Ticket généré : {nouveauTicket.Id}");

                TicketActuel = null; // 🚀 Force la notification
                OnPropertyChanged(nameof(TicketActuel));

                TicketActuel = nouveauTicket;   // Réaffectation pour déclencher la notification
                OnPropertyChanged(nameof(TicketActuel)); // Notifier manuellement


                System.Drawing.Bitmap barcodeBitmap = GenerateBarcode(nouveauTicket.Id);

                if (barcodeBitmap != null)
                {
                    BarcodeImage = ConvertBitmapToBitmapImage(barcodeBitmap);
                    GenererTicketPDF(nouveauTicket, barcodeBitmap);
                }
            }
            else
            {
                Console.WriteLine("❌ Échec de la génération du ticket.");
            }
        }

        private System.Drawing.Bitmap GenerateBarcode(string text)
        {
            try
            {
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Width = 600,
                        Height = 200,
                        Margin = 5,
                        PureBarcode = true
                    }
                };

                var pixelData = writer.Write(text);

                using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    return new System.Drawing.Bitmap(bitmap);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la génération du code-barres : {ex.Message}");
                return null;
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void GenererTicketPDF(Ticket ticket, System.Drawing.Bitmap barcodeBitmap)
        {
            try
            {
                if (!Directory.Exists(PdfSavePath))
                    Directory.CreateDirectory(PdfSavePath);

                string pdfFilePath = Path.Combine(PdfSavePath, $"Ticket_{ticket.Id}.pdf");

                // Supprimer l'ancien fichier
                if (File.Exists(pdfFilePath))
                {
                    Console.WriteLine($"⚠️ Suppression de l'ancien fichier PDF : {pdfFilePath}");
                    File.Delete(pdfFilePath);
                }

                // Vérifier si on peut créer un fichier
                using (FileStream fsTest = new FileStream(pdfFilePath, FileMode.CreateNew, FileAccess.Write))
                {
                    Console.WriteLine($"✅ Test d'écriture du fichier PDF réussi : {pdfFilePath}");
                }

                // Forcer la création correcte du PDF
                using (FileStream stream = new FileStream(pdfFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (PdfWriter writer = new PdfWriter(stream))
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    document.Add(new Paragraph(NomHopital).SetFont(boldFont).SetFontSize(18)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Add(new Paragraph(AdresseHopital).SetFont(regularFont).SetFontSize(12)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Add(new Paragraph($"📅 Heure d'Arrivée : {ticket.TempsArrive:dd/MM/yyyy HH:mm:ss}")
                        .SetFont(regularFont).SetFontSize(14));

                    document.Add(new Paragraph($"🎫 Numéro du Ticket : {ticket.Id}")
                        .SetFont(boldFont).SetFontSize(16));

                    document.Add(new Paragraph(" "));

                    document.Add(new Paragraph("📢 Présentez ce ticket à la borne de paiement avant de quitter le parking.")
                        .SetFont(regularFont).SetFontSize(12)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    // **Forcer l’écriture et fermer le PDF proprement**
                    document.Flush();
                    pdf.Close();
                }

                Console.WriteLine($"✅ PDF généré avec succès : {pdfFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la génération du PDF : {ex.Message}");
            }
        }



    }
}
