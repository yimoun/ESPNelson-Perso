using Administration.Data;
using Administration.Data.Context;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Administration.ViewModel
{
    public partial class TableauBordVM : ObservableObject
    {
        private readonly AdministrationContext _dbContext;

        [ObservableProperty]
        private string dateDuJour;

        [ObservableProperty]
        private string heureActuelle;

        [ObservableProperty]
        private int placesOccupees;

        [ObservableProperty]
        private int placesDisponibles;

        [ObservableProperty]
        private SeriesCollection etatStationnementSeries;

        [ObservableProperty]
        private SeriesCollection revenusSeries = new SeriesCollection();

        [ObservableProperty]
        private List<string> joursLabels = new List<string>();

        public Func<double, string> YFormatter { get; private set; } = value => value.ToString("C");

        [ObservableProperty]
        private FiltreType _filtreActif = FiltreType.Tous; // par défaut

        public bool FiltreTous
        {
            get => FiltreActif == FiltreType.Tous;
            set
            {
                if (value) FiltreActif = FiltreType.Tous;
                ChargerGraphiqueRevenus();
            }
        }

        public bool FiltreTickets
        {
            get => FiltreActif == FiltreType.Tickets;
            set
            {
                if (value) FiltreActif = FiltreType.Tickets;
                ChargerGraphiqueRevenus();
            }
        }

        public bool FiltreAbonnements
        {
            get => FiltreActif == FiltreType.Abonnements;
            set
            {
                if (value) FiltreActif = FiltreType.Abonnements;
                ChargerGraphiqueRevenus();
            }
        }

        [ObservableProperty]
        private bool afficherDiagramme = true;  // Par défaut on montre le graphique

        [ObservableProperty]
        private string messageAlerte;  // Pour le message d'alerte

        public IRelayCommand RefreshCommand { get; }

        public TableauBordVM()
        {
            AdministrationContextFactory factory = new AdministrationContextFactory();
            _dbContext = factory.CreateDbContext(new string[0]);

            DateDuJour = DateTime.Now.ToString("yyyy/MM/dd");
            HeureActuelle = DateTime.Now.ToString("HH:mm");

            RefreshCommand = new RelayCommand(ChargerDonnees);

            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            DateDuJour = DateTime.Now.ToString("yyyy/MM/dd");
            HeureActuelle = DateTime.Now.ToString("HH:mm");

            ChargerEtatStationnement();
            ChargerGraphiqueRevenus();
        }

        private void ChargerEtatStationnement()
        {
            // Récupérer la capacité max de la configuration la plus récente
            var derniereConfig = _dbContext.Configurations
                .OrderByDescending(c => c.DateModification)
                .FirstOrDefault();

            if (derniereConfig == null || derniereConfig.CapaciteMax <= 0)
            {
                // Aucun paramètre de capacité trouvé, on informe l'utilisateur
                AfficherDiagramme = false;  // Cache le diagramme
                MessageAlerte = "⚠️ Aucune capacité maximale définie dans la configuration.\nVeuillez configurer la capacité dans la console de gestion.";
                
                return;
            }

            int totalPlaces = derniereConfig.CapaciteMax;

            // Filtrer les tickets non payés pour aujourd'hui seulement
            DateTime dateDuJour = DateTime.Today;

            var ticketsNonPayesAujourdHui = _dbContext.Tickets
                .Where(t => !t.EstPaye && !t.EstConverti && t.TempsArrive.Date == dateDuJour)
                .Count();

            PlacesOccupees = ticketsNonPayesAujourdHui;
            PlacesDisponibles = totalPlaces - ticketsNonPayesAujourdHui;

            EtatStationnementSeries = new SeriesCollection
            {
                new PieSeries { Title = "Occupées", Values = new ChartValues<double> { PlacesOccupees }, Fill = Brushes.Orange },
                new PieSeries { Title = "Disponibles", Values = new ChartValues<double> { PlacesDisponibles }, Fill = Brushes.LightGreen }
            };
        }

        private void ChargerGraphiqueRevenus()
        {
            DateTime aujourdHui = DateTime.Now.Date;
            DateTime ilYA7Jours = aujourdHui.AddDays(-7);

            var paiements = _dbContext.Paiements
                .Where(p => p.DatePaiement.Date >= ilYA7Jours && p.DatePaiement.Date <= aujourdHui)
                .ToList();

            switch (FiltreActif)
            {
                case FiltreType.Tickets:
                    paiements = paiements.Where(p => !string.IsNullOrEmpty(p.TicketId)).ToList();
                    break;
                case FiltreType.Abonnements:
                    paiements = paiements.Where(p => !string.IsNullOrEmpty(p.AbonnementId)).ToList();
                    break;
                case FiltreType.Tous:
                default:
                    break; // Ne filtre rien
            }

            // Initialisation et agrégation par jour
            var revenusParJour = Enumerable.Range(0, 7)
                .Select(i => new
                {
                    Date = ilYA7Jours.AddDays(i).ToString("dd/MM"),
                    Montant = paiements
                                .Where(p => p.DatePaiement.Date == ilYA7Jours.AddDays(i))
                                .Sum(p => p.Montant)
                })
                .ToDictionary(x => x.Date, x => x.Montant);

            // Mise à jour de la VM
            RevenusSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Revenus",
                    Values = new ChartValues<decimal>(revenusParJour.Values),
                    Fill = new SolidColorBrush(Color.FromRgb(63, 81, 181)) // Couleur bleue Material Design
                }
            };

            JoursLabels = revenusParJour.Keys.ToList();

            // Notifier
            OnPropertyChanged(nameof(RevenusSeries));
            OnPropertyChanged(nameof(JoursLabels));
        }


    }

    public enum FiltreType
    {
        Tous,
        Tickets,
        Abonnements
    }
}
