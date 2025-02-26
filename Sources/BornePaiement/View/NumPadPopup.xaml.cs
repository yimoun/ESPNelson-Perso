using System.Windows;
using System.Windows.Controls;

namespace BornePaiement.View
{
    public partial class NumPadPopup : Window
    {
        private const string CorrectPin = "999"; // Le NIP valide

        public string EnteredPin { get; private set; } = "";

        public NumPadPopup()
        {
            InitializeComponent();
        }

        private void NumPad_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && PinBox.Password.Length < 3)
            {
                PinBox.Password += button.Content.ToString();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (PinBox.Password.Length > 0)
            {
                PinBox.Password = PinBox.Password.Substring(0, PinBox.Password.Length - 1);
            }
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (PinBox.Password == CorrectPin)
            {
                EnteredPin = PinBox.Password;
                DialogResult = true; // Ferme la fenêtre avec succès
            }
            else
            {
                MessageBox.Show("❌ NIP incorrect. Veuillez réessayer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                PinBox.Password = ""; // Réinitialiser la saisie
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
