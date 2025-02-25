using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ESPNelson.Model;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Windows.Media.Imaging;
using System.Windows;
using ZXing.Common;
using ZXing;

namespace ESPNelson.ViewModel
{
    public partial class VisiteurVM : ObservableObject
    {
        private const string PdfSavePath = "Tickets";
        private static readonly string LogoPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "img", "logo_ciuss.jpg");

        [ObservableProperty]
        private Ticket ticketActuel;

        [ObservableProperty]
        private BitmapImage barcodeImage;

        public IRelayCommand GenerateNewTicketCommand { get; }
        public IRelayCommand DownloadTicketPDFCommand { get; }

        public VisiteurVM()
        {
            GenerateNewTicketCommand = new RelayCommand(GenerateNewTicket);
            DownloadTicketPDFCommand = new RelayCommand(DownloadTicketPDF);
        }

        private async void GenerateNewTicket()
        {
            var nouveauTicket = await TicketProcessor.GenerateTicketAsync();
            if (nouveauTicket != null)
            {
                TicketActuel = null;
                OnPropertyChanged(nameof(TicketActuel));
                TicketActuel = nouveauTicket;
                OnPropertyChanged(nameof(TicketActuel));

                Bitmap barcodeBitmap = GenerateBarcode(nouveauTicket.Id);
                if (barcodeBitmap != null)
                {
                    BarcodeImage = ConvertBitmapToBitmapImage(barcodeBitmap);
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
                    Options = new EncodingOptions { Width = 300, Height = 100, Margin = 5 }
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

        private void DownloadTicketPDF()
        {
            if (TicketActuel == null || BarcodeImage == null)
            {
                MessageBox.Show("Aucun ticket généré");
                return;
            }

            if (!Directory.Exists(PdfSavePath))
                Directory.CreateDirectory(PdfSavePath);

            string pdfFilePath = Path.Combine(PdfSavePath, $"Ticket_{TicketActuel.Id}.pdf");
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromMillimeter(80); // Format ticket de stationnement
                page.Height = XUnit.FromMillimeter(150);
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont fontTitle = new XFont("Arial", 12);
                XFont fontNormal = new XFont("Arial", 10);

                // Charger et dessiner le logo
                if (File.Exists(LogoPath))
                {
                    XImage logo = XImage.FromFile(LogoPath);
                    gfx.DrawImage(logo, (page.Width.Point - 150) / 2, (page.Height.Point / 2) - 90, 150, 150);
                }



                gfx.DrawString($"Date et Heure d'Arrivée: {TicketActuel.TempsArrive:dd/MM/yyyy HH:mm:ss}", fontNormal, XBrushes.DarkBlue, new XPoint((page.Width.Point - gfx.MeasureString($"Date et Heure d'Arrivée: {TicketActuel.TempsArrive:dd/MM/yyyy HH:mm:ss}", fontNormal).Width) / 2, (page.Height.Point / 2) + 60));

                // Dessiner le code-barres en haut et en bas du ticket
                using (MemoryStream memory = new MemoryStream())
                {
                    BarcodeImage.StreamSource.Position = 0;
                    BarcodeImage.StreamSource.CopyTo(memory);
                    memory.Position = 0;
                    XImage barcodeXImage = XImage.FromStream(memory);
                    gfx.DrawImage(barcodeXImage, (page.Width.Point - 300) / 2, 20, 300, 100);
                    gfx.DrawImage(barcodeXImage, (page.Width.Point - 300) / 2, page.Height.Point - 120, 300, 100);
                }

                document.Save(pdfFilePath);
            }
            Console.WriteLine($"✅ PDF généré avec succès : {pdfFilePath}");
            Process.Start(new ProcessStartInfo(pdfFilePath) { UseShellExecute = true });
        }
    }
}
