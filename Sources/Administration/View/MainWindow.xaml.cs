using Administration.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Administration.ViewModel;
using System.Windows.Media.Animation;
using Microsoft.EntityFrameworkCore;

namespace Administration.View
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NavigationService NavigationService { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            // Attendre que la fenêtre soit chargée avant d'initialiser le Frame
            this.Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Initialiser le Frame après le chargement de la fenêtre
            AfficherLogin(); // Toujours afficher Login au départ
            StartFadeInAnimation(); // Déclencher l'animation initiale
        }

        public void AfficherLogin()
        {
            MainFrame.Navigate(new Login());
            NavButtonsPanel.Visibility = Visibility.Collapsed;
            btnDeconnexion.Visibility = Visibility.Collapsed;
        }

        public void AfficherTableauBord()
        {
            TableauBordVM tableauBordVM = new TableauBordVM();
            TableauBordView tableauBordPage = new TableauBordView { DataContext = tableauBordVM };

            MainFrame.Navigate(tableauBordPage);
            NavButtonsPanel.Visibility = Visibility.Visible;
            btnDeconnexion.Visibility = Visibility.Visible;
        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            App.Current.User = null;
            AfficherLogin();
        }

        private void BtnTableauBord_Click(object sender, RoutedEventArgs e)
        {
            
            NavigateToPage(new TableauBordView());
        }

        private void BtnGestion_Click(object sender, RoutedEventArgs e)
        {
            GestionVM gestionVM = new GestionVM();
            GestionView gestionPage = new GestionView { DataContext = gestionVM };

            NavigateToPage(gestionPage);
        }

        private void BtnRapports_Click(object sender, RoutedEventArgs e)
        {
            RapportsVM rapportsVM = new RapportsVM();
            RapportsView rapportsView = new RapportsView { DataContext = rapportsVM };  

            NavigateToPage(rapportsView);
        }

        public void NavigateToPage(Page newPage)
        {
            if (newPage != null && MainFrame != null) // Vérifier que MainFrame existe
            {
                MainFrame.Navigate(newPage);
                StartFadeInAnimation(); // Déclencher l'animation après le changement de page
            }
        }


        ///// <summary>
        ///// À revoir l'utilité de cette fonction dans ce cadre
        ///// </summary>
        //public void NavigateToLoginPage()
        //{
        //    Page loginPage = new Login();
        //    loginPage.DataContext = new LoginVM();

        //    MainFrame.NavigationService.RemoveBackEntry();
        //    NavigationService.Navigate(loginPage);
        //}

        private void StartFadeInAnimation()
        {
            // Récupérer l'animation depuis les ressources
            Storyboard fadeInAnimation = (Storyboard)FindResource("FadeInAnimation");
            if (fadeInAnimation != null && MainFrame != null) // Vérifier que MainFrame n'est pas null
            {
                fadeInAnimation.Begin(MainFrame); // Déclencher l'animation
            }
        }


    }
}
