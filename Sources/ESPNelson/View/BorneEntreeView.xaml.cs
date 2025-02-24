using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ESPNelson.ViewModel;
using ESPNelson.Resources;
using System.ComponentModel;

namespace ESPNelson.View
{
    /// <summary>
    /// Logique d'interaction pour BorneEntreeView.xaml
    /// </summary>
    public partial class BorneEntreeView : Window, INotifyPropertyChanged
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
                    //SetValue(LanguageProperty, value);
                    OnPropertyChanged(nameof(Language));
                    Resource.Culture = new CultureInfo(value);

                    //Changer aussi les labels des autres pages 
                    //à voir plus tard pour la lecture du code barre d'un ticket d'abonnment

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public BorneEntreeView()
        {
            InitializeComponent();
            this.DataContext = new BorneEntreeVM();
            RessourceHelper.SetInitialLanguage();
            //LoadLabels();

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
                if (item.Tag as string == lang)
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
            ComboBoxItem selectedItem = (ComboBoxItem)languageComboBox.SelectedItem;

            if (selectedItem != null)
            {
                string? selectedLanguage = selectedItem.Tag as string;
                Language = selectedLanguage;

                // Enregistrer la langue sélectionnée dans les paramètres de configuration
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["language"].Value = selectedLanguage;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                // Mettre à jour la culture de l'application
                Resource.Culture = new CultureInfo(selectedLanguage);
            }

        }
    }
}
