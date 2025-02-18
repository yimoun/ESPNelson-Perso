using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ESPNelson.Model;
using ZXing;
using ZXing.Common;

namespace ESPNelson.ViewModel
{
    public partial class BorneEntreeVM : ObservableObject
    {
        private Ticket _ticketActuel;
        public Ticket TicketActuel
        {
            get => _ticketActuel;
            set
            {
                SetProperty(ref _ticketActuel, value);
                OnPropertyChanged(nameof(TicketActuel)); 
            }
        }




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

                TicketActuel = nouveauTicket; // Met à jour la propriété observable
                BarcodeImage = GenerateBarcode(nouveauTicket.Id);
            }
            else
            {
                Console.WriteLine("❌ Échec de la génération du ticket.");
            }
        }


        private BitmapImage GenerateBarcode(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("⚠️ ID de ticket invalide pour génération de code-barres !");
                return null;
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128, // Format lisible par un lecteur standard
                Options = new EncodingOptions
                {
                    Width = 600, // Augmentation de la largeur
                    Height = 200, // Augmentation de la hauteur
                    Margin = 5, // Marge pour éviter que le code-barres touche les bords
                    PureBarcode = true
                }
            };

            try
            {
                var pixelData = writer.Write(text);

                using (Bitmap bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
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
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la génération du code-barres : {ex.Message}");
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
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Empêche les erreurs liées au threading dans WPF

                return bitmapImage;
            }
        }
    }
}
