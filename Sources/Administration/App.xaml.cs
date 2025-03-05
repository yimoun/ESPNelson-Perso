using Administration.Model;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace Administration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        new public static App Current => (App)Application.Current;

        public Utilisateur? User { get; set; }

        public App()
        {
            // Activer les erreurs de liaison pour le débogage
            Debug.WriteLine("Binding errors will be displayed in the Output window.");
            PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;
        }
    }

}
