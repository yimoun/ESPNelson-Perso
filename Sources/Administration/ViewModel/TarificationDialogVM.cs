using Administration.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace Administration.ViewModel
{
    public partial class TarificationDialogVM : ObservableObject
    {
        [ObservableProperty]
        private Tarification tarification;

        public Action<bool> CloseDialogAction { get; }

        public TarificationDialogVM(Tarification tarification, Action<bool> closeDialogAction)
        {
            Tarification = tarification;
           
            CloseDialogAction = closeDialogAction;
        }

        [RelayCommand]
        private void Enregistrer()
        {
            if (Tarification.Prix < 0 || Tarification.DureeMin < 0 || Tarification.DureeMax <= 0)
            {
                MessageBox.Show("Veuillez remplir tous les champs correctement.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Tarification.DureeMin >= Tarification.DureeMax)
            {
                MessageBox.Show("La durée minimale doit être inférieure à la durée maximale.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CloseDialogAction?.Invoke(true); // Clôture avec succès
        }

        [RelayCommand]
        private void Annuler()
        {
            CloseDialogAction?.Invoke(false); // Clôture sans succès
        }
    }
}
