using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BorneSortie.Resources;
using BorneSortie.ViewModel;

namespace BorneSortie.View
{
    /// <summary>
    /// Logique d'interaction pour BorneSortie.xaml
    /// </summary>
    public partial class BorneSortieView : Window
    {
        private string _language;
        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged(nameof(Language));
                    Resource.Culture = new CultureInfo(value);
                    LoadLabels(); //à chaque chargement de langue on charge les labels directement

                }
            }
        }

        public BorneSortieView()
        {
            InitializeComponent();
            this.DataContext = new BorneSortieVM();

            // Attendre que la fenêtre soit chargée avant d'initialiser le Frame
            this.Loaded += BorneSortie_Loaded;

            RessourceHelper.SetInitialLanguage();
            LoadLabels();

            //Direct à l'ouverture de la fenêtre !
            Language = ConfigurationManager.AppSettings["language"];
            SelectLanguage();
        }

        /// <summary>
        ///Pour que la langue actuelle(celle configurée dans le fichier App.Config) soit automatiquement sélectionnée 
        ///dans le comboBox Dès l'ouverture de la fenêtre
        /// </summary>
        private void SelectLanguage()
        {
            string lang = ConfigurationManager.AppSettings["language"];

            foreach (ComboBoxItem item in languageComboBox.Items)
            {
                if (item.Tag is string tag && tag == lang)
                {
                    languageComboBox.SelectedItem = item;
                    break;
                }
            }
        }


        //Le code de la fonction suivante n'est pas de moi à 100%...source: ChatGPT
        /// <summary>
        ///cette fonction met à jour la langue de l'application en fonction de la sélection de l'utilisateur dans le ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languageComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Tag is string selectedLanguage)
                {
                    // Affectation propre sans provoquer d'exception
                    _language = selectedLanguage;
                    OnPropertyChanged(nameof(Language));

                    // Mise à jour des ressources linguistiques
                    Resource.Culture = new CultureInfo(selectedLanguage);
                   LoadLabels();

                    // Mise à jour des textes de la VM
                    if (DataContext is BorneSortieVM viewModel)
                    {
                        viewModel.TicketInfo = string.Format(Resource.UnpaidTicket);
                        viewModel.AbonnementInfo = string.Format(Resource.ValidSubscription);

                        viewModel.UpdateTicketInfoLanguage();
                    }

                    // Enregistre la langue sélectionnée dans les paramètres de configuration
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["language"].Value = selectedLanguage;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadLabels()
        {
            label_Language.Content = Resource.Language;
            label_ExitStation.Content = Resource.ExitStation;   
            label_ScanningInstructions.Text = Resource.ScanningInstructions;    
            label_InvalidTicket.Text = Resource.InvalidTicket;  
            label_PaidTicket.Text = Resource.PaidTicket;    
            label_InvalidSeasonTicket.Text = Resource.InvalidSeasonTicket;
            //label_ValidPaiement.Text = Resource.ValidPaiment;
        }

        private StringBuilder _scanBuffer = new StringBuilder(); // Buffeur pour collecter les données du scan

        private void BorneSortie_Loaded(object sender, RoutedEventArgs e)
        {
            // Donne le focus au UserControl pour capturer les événements clavier
            this.Focusable = true;
            this.Focus();
        }

        private void HiddenScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Transmettre l'événement clavier à la méthode principale
            Fenetre_KeyDown(sender, e);
        }

        private async void Fenetre_KeyDown(object sender, KeyEventArgs e)
        
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

            if (e.Key == Key.Enter) //Lorsque l'utilisateur a scanné son ticket
            {
                await Task.Delay(100); // Délai de 100 ms pour s'assurer que le scan est complet
                // Transmettre l'ID du ticket au ViewModel
                if (DataContext is BorneSortieVM viewModel)
                {
                    viewModel.VerifierTicketPaye(_scanBuffer.ToString());
                }
                _scanBuffer.Clear(); // Réinitialiser le buffeur après le traitement
            }
            else
            {
                // Capturer les chiffres (0-9)
                if (e.Key >= Key.D0 && e.Key <= Key.D9) // Chiffres de 0 à 9
                {
                    _scanBuffer.Append(e.Key.ToString().Replace("D", "")); // Supprimer le préfixe "D" pour les chiffres
                }
                // Capturer les chiffres du pavé numérique (0-9)
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) // Chiffres du pavé numérique
                {
                    _scanBuffer.Append(e.Key.ToString().Replace("NumPad", "")); // Supprimer le préfixe "NumPad"
                }
                // Capturer les lettres (A-Z)
                else if (e.Key >= Key.A && e.Key <= Key.Z) // Lettres de A à Z
                {
                    _scanBuffer.Append(e.Key.ToString()); // Conserver la lettre telle quelle
                }
            }
        }

    }   
}
