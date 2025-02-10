using System;
using System.Windows.Media.Imaging;
using ProjetCodeBarre.Model;
using ZXing;
using ZXing.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace ProjetCodeBarre.ViewModel
{
    public partial class TicketVM : ObservableObject
    {
        [ObservableProperty]
        private Ticket ticketTest;

        [ObservableProperty]
        private BitmapImage barcodeImage;

        public IRelayCommand GenerateNewTicketCommand { get; }
        public IRelayCommand OpenPaymentWindowCommand { get; }

        public TicketVM()
        {
            GenerateNewTicketCommand = new RelayCommand(GenerateNewTicket);
            OpenPaymentWindowCommand = new RelayCommand(OpenPaymentWindow);
            GenerateNewTicket();
        }

        private void GenerateNewTicket()
        {
            Console.WriteLine("Génération d'un nouveau ticket...");
            TicketTest = new Ticket();
            BarcodeImage = GenerateBarcode(TicketTest.Id);
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

            // Génération des pixels du code-barres
            var pixelData = writer.Write(text);

            // Conversion en Bitmap
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
                bitmapImage.Freeze(); // Évite les problèmes de threading WPF

                return bitmapImage;
            }
        }

        private void OpenPaymentWindow()
        {
            if (TicketTest != null)
            {
                var process = new Process();
                process.StartInfo.FileName = "ProjetCodeBarre.exe";
                process.StartInfo.Arguments = TicketTest.Id;
                process.Start();
            }
        }
    }
}
