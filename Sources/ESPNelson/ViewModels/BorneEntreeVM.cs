using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ESPNelson.Models;
using ZXing;
using ZXing.Common;

namespace ESPNelson.ViewModels
{
    public partial class BorneEntreeVM : ObservableObject
    {
        [ObservableProperty]
        private Ticket ticketActuel;

        [ObservableProperty]
        private BitmapImage barcodeImage;

        public IRelayCommand GenerateNewTicketCommand { get; }
        public IRelayCommand OpenPaymentWindowCommand { get; }

        public BorneEntreeVM()
        {
            GenerateNewTicketCommand = new RelayCommand(async () => await GenerateNewTicket());
            OpenPaymentWindowCommand = new RelayCommand(OpenPaymentWindow);
        }

        private async Task GenerateNewTicket()
        {
            Console.WriteLine("📌 Demande de génération d’un ticket via API...");

            ticketActuel = await TicketProcessor.GenerateTicketAsync();

            if (ticketActuel != null)
            {
                BarcodeImage = GenerateBarcode(ticketActuel.Id);
                Console.WriteLine($"✅ Ticket généré : {ticketActuel.Id}");
            }
        }

        private BitmapImage GenerateBarcode(string text)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 10
                }
            };

            var pixelData = writer.Write(text);

            using (Bitmap bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                return ConvertBitmapToBitmapImage(bitmap);
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
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        //Pas besoin forcément, à revoir plus tard.
        private void OpenPaymentWindow()
        {
            if (ticketActuel != null)
            {
                Console.WriteLine($"📌 Ouverture de la borne de paiement pour le ticket : {ticketActuel.Id}");
            }
        }
    }
}
