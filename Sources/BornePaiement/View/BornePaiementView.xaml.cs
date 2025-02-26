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
using System.Windows.Shapes;
using BornePaiement.ViewModel;
using System.Windows.Media.Animation;
using BornePaiement.View;

namespace BornePaiement.View
{
    /// <summary>
    /// Logique d'interaction pour BornePaiementView.xaml
    /// </summary>
    public partial class BornePaiementView : Window
    {
        public BornePaiementView()
        {
            InitializeComponent();
            this.DataContext = new BornePaiementVM();

            // Attendre que la fenêtre soit chargée avant d'initialiser le Frame
            this.Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Initialiser le Frame après le chargement de la fenêtre
            MainFrame.Navigate(new VisiteurPage()); // Afficher la vue Visiteur par défaut
            StartFadeInAnimation(); // Déclencher l'animation initiale
        }

        private void BtnVisiteur_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la vue "Visiteur" avec animation
            NavigateToPage(new VisiteurView());
        }

        private void BtnAbonne_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la vue "Abonné" avec animation
            NavigateToPage(new AbonneView());
        }

        private void NavigateToPage(Page newPage)
        {
            if (newPage != null && MainFrame != null) // Vérifier que MainFrame existe
            {
                MainFrame.Navigate(newPage);
                StartFadeInAnimation(); // Déclencher l'animation après le changement de page
            }
        }

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
