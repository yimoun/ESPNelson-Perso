using Administration.Data;
using Administration.Data.Context;
using Administration.Model;
using Administration.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Administration.ViewModel
{
    public partial class GestionVM : ObservableObject
    {
        private readonly AdministrationContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<Utilisateur> administrateurs;

        [ObservableProperty]
        private Utilisateur? utilisateurSelectionne;

        [ObservableProperty]
        private ObservableCollection<Tarification> tarifications;

        [ObservableProperty]
        private Tarification tarificationSelectionnee;

        [ObservableProperty]
        private bool boutonsVisible;

        [ObservableProperty]
        private ObservableCollection<Configuration> configurations;

        [ObservableProperty]
        private Configuration? configurationSelectionnee;

        public GestionVM()
        {
            AdministrationContextFactory factory = new AdministrationContextFactory();
            _dbContext = factory.CreateDbContext(new string[0]);

            ChargerUtilisateurs();
            ChargerTarifications();
            ChargerConfigurations();
        }

        private void ChargerUtilisateurs()
        {
            var admins = _dbContext.Utilisateurs
                .Where(u => u.Role == "admin")
                .ToList();

            Administrateurs = new ObservableCollection<Utilisateur>(admins);
            BoutonsVisible = false;
        }

        private void ChargerTarifications()
        {
            Tarifications = new ObservableCollection<Tarification>(_dbContext.Tarifications.ToList());
        }

        private void ChargerConfigurations()
        {
            var configs = _dbContext.Configurations
                .Include(c => c.Utilisateur) 
                .ToList();

            Configurations = new ObservableCollection<Configuration>(configs);
        }

        [RelayCommand]
        private void AjouterConfiguration()
        {
            var nouvelleConfiguration = new Configuration
            {
                DateModification = DateTime.Now,
                UtilisateurId = App.Current.User.Id
            };

            if (AfficherDialogConfiguration(nouvelleConfiguration))
            {
                _dbContext.Configurations.Add(nouvelleConfiguration);
                _dbContext.SaveChanges();
                ChargerConfigurations();
            }
        }

        [RelayCommand]
        private void SupprimerConfiguration()
        {
            // Vérifie si aucune configuration n'est sélectionnée
            if (ConfigurationSelectionnee == null)
                return;

            // Vérifie si c'est la seule configuration dans la liste
            if (Configurations.Count == 1)
            {
                MessageBox.Show("Impossible de supprimer la dernière configuration. Le système a besoin d'au moins une configuration.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            // Demande une confirmation avant de supprimer
            if (MessageBox.Show("Confirmer la suppression de cette configuration ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _dbContext.Configurations.Remove(ConfigurationSelectionnee);
                _dbContext.SaveChanges();
                ChargerConfigurations();
            }
        }

        private bool AfficherDialogConfiguration(Configuration configuration)
        {
            var vm = new ConfigurationDialogVM
            {
                CapaciteMax = configuration.CapaciteMax,
                TaxeFederal = configuration.TaxeFederal,
                TaxeProvincial = configuration.TaxeProvincial
            };

            var dialog = new ConfigurationDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                // Mettre à jour la configuration avec les valeurs saisies
                configuration.CapaciteMax = vm.CapaciteMax;
                configuration.TaxeFederal = vm.TaxeFederal;
                configuration.TaxeProvincial = vm.TaxeProvincial;
                return true;
            }
            return false;
        }



        partial void OnUtilisateurSelectionneChanged(Utilisateur? value)
        {
            BoutonsVisible = value != null;
        }


        private async Task<bool> AfficherDialogUtilisateur(Utilisateur utilisateur)
        {
            var dialog = new UtilisateurDialog(new UtilisateurDialogVM(utilisateur));
            return dialog.ShowDialog() == true;
        }


        [RelayCommand]
        private async Task AjouterUtilisateur()
        {
            var nouvelUtilisateur = new Utilisateur { Role = "admin" };  // Par défaut
            await AfficherDialogUtilisateur(nouvelUtilisateur);

            ChargerUtilisateurs();
        }

       

        [RelayCommand]
        private async Task ModifierUtilisateur()
        {
            if (UtilisateurSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un utilisateur.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            await AfficherDialogUtilisateur(UtilisateurSelectionne);

            ChargerUtilisateurs();
        }


        [RelayCommand]
        private void SupprimerUtilisateur()
        {
            if (UtilisateurSelectionne == null)
                return;

            if (MessageBox.Show($"Confirmer la suppression de {UtilisateurSelectionne.NomUtilisateur} ?",
                                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _dbContext.Utilisateurs.Remove(UtilisateurSelectionne);
                _dbContext.SaveChanges();
                ChargerUtilisateurs();
            }
        }

        [RelayCommand]
        private void ModifierTarification()
        {
            if (TarificationSelectionnee != null)
            {
                if (AfficherDialogTarification(TarificationSelectionnee, false))
                {
                    _dbContext.Tarifications.Update(TarificationSelectionnee);
                    _dbContext.SaveChanges();
                    ChargerTarifications();
                }
            }
        }

        private bool AfficherDialogTarification(Tarification tarification, bool estNouvelle)
        {
            var dialog = new TarificationDialog(tarification, estNouvelle);
            return dialog.ShowDialog() == true;
        }
    }
}

