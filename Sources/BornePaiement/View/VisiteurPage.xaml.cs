using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BornePaiement.ViewModel;

namespace BornePaiement.View
{
    public partial class VisiteurPage : UserControl
    {
        private StringBuilder _scanBuffer = new StringBuilder(); // Buffeur pour collecter les données du scan

        public VisiteurPage()
        {
            InitializeComponent();
            this.Loaded += VisiteurPage_Loaded; // S'abonner à l'événement Loaded
        }

        private void VisiteurPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Donner le focus au UserControl pour capturer les événements clavier
            this.Focusable = true;
            this.Focus();
        }

        private void HiddenScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Transmettre l'événement clavier à la méthode principale
            UserControl_KeyDown(sender, e);
        }

        private async void UserControl_KeyDown(object sender, KeyEventArgs e)
            {
            // Ignorer les touches spéciales
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.CapsLock || e.Key == Key.Tab ||
                e.Key == Key.Escape || e.Key == Key.Back)
            {
                return;
            }

            if (e.Key == Key.Enter) // 🎯 Lorsque l'utilisateur a scanné son ticket
            {
                // Attendre un court délai pour s'assurer que le scan est complet
                await Task.Delay(100); // Délai de 100 ms

                // Transmettre l'ID du ticket au ViewModel
                if (DataContext is VisiteurVM viewModel)
                {
                    viewModel.VerifierTicket(_scanBuffer.ToString());
                }
                _scanBuffer.Clear(); // Réinitialiser le buffeur après le traitement
            }
            else
            {
                // Capturer les chiffres (0-9) et les lettres (A-Z)
                if ((e.Key >= Key.D0 && e.Key <= Key.D9) || // Chiffres de 0 à 9
                    (e.Key >= Key.A && e.Key <= Key.Z))    // Lettres de A à Z
                {
                    _scanBuffer.Append(e.Key.ToString().Replace("D", "")); // Supprimer le préfixe "D" pour les chiffres
                }
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) // Chiffres du pavé numérique
                {
                    _scanBuffer.Append(e.Key.ToString().Replace("NumPad", "")); // Supprimer le préfixe "NumPad"
                }
            }
        }
    }
}