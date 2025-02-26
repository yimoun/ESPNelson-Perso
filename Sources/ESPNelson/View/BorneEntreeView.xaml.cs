using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ESPNelson.View
{
    public partial class BorneEntreeView : Window
    {
        public BorneEntreeView()
        {
            InitializeComponent();

            // Attendre que la fenêtre soit chargée avant d'initialiser le Frame
            this.Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Initialiser le Frame après le chargement de la fenêtre
            MainFrame.Navigate(new VisiteurView()); // Afficher la vue Visiteur par défaut
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