using BorneSortie.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BorneSortie.Model;

namespace BorneSortie.ViewModel
{
    public partial class AbonneVM : ObservableObject
    {
        [ObservableProperty] private string abonnementId;

        [ObservableProperty] private bool abonnmentValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool abonnmentInvalide = false;
        [ObservableProperty] private string abonnementInfo;



        public AbonneVM()
        {

        }



        public async Task VerifierTicketabonnment(string abonnementId)
        {
            if (string.IsNullOrWhiteSpace(abonnementId))
                return;

            // Récupérer l'abonnement depuis l'API
            var abonnementResponse = await AbonnementProcessor.GetAbonnementAsync(abonnementId);

            if (!string.IsNullOrEmpty(abonnementResponse.Message))
            {
                // Cas d'erreur : abonnement inexistant, expiré ou erreur API
                AbonnementInfo = abonnementResponse.Message;
                AbonnmentInvalide = true;
                AbonnmentValide = false;

                // Afficher un MessageBox pour informer l'utilisateur
                MessageBox.Show(abonnementResponse.Message, "Erreur d'Abonnement", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // ✅ Cas normal : abonnement valide
                AbonnementInfo = $"✅ Abonnement valide !\n\n" +
                                 $"ID : {abonnementResponse.AbonnementId}\n" +
                                 $"Type : {abonnementResponse.TypeAbonnement}\n" +
                                 $"Début : {abonnementResponse.DateDebut:dd/MM/yyyy}\n" +
                                 $"Fin : {abonnementResponse.DateFin:dd/MM/yyyy}\n";

                AbonnmentValide = true;
                AbonnmentInvalide = false;

                //On catch l'id de l'abonnment pour la suite 
                abonnementId = abonnementResponse.AbonnementId;

                // ✅ Afficher une confirmation via MessageBox
                //MessageBox.Show("L'abonnement est valide et actif.", "Abonnement Confirmé", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
