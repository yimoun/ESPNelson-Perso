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

        //private void NavigationTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (NavigationTabs.SelectedItem is TabItem selectedTab)
        //    {
        //        // Définir la nouvelle vue
        //        Page newView = null;
        //        switch (selectedTab.Header.ToString())
        //        {
        //            case "Visiteur":
        //                newView = new VisiteurView();
        //                break;
        //            case "Abonné":
        //                newView = new AbonneView();
        //                break;
        //        }

        //        // Naviguer vers la nouvelle vue avec animation
        //        if (newView != null && MainFrame != null) // Vérifier que MainFrame n'est pas null
        //        {
        //            MainFrame.Navigate(newView); // Utiliser Frame.Navigate pour changer la vue
        //            StartFadeInAnimation(); // Déclencher l'animation
        //        }
        //    }
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