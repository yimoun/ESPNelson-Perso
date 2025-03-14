using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows;
using ESPNelson.Model;
using ESPNelson.Resources;

namespace ESPNelson.ViewModel
{
    /// <summary>
    /// ViewModel pour la gestion des abonnés et de leurs abonnements.
    /// </summary>
    public partial class AbonneVM : ObservableObject
    {
        [ObservableProperty] private string abonnementId;

        [ObservableProperty] private bool abonnmentValide = false;
        [ObservableProperty] private bool abonnmentInvalide = false;

        /// <summary>
        /// Réponse d'abonnement contenant les détails après vérification.
        /// </summary>
        public AbonnementResponse AbonnementResponse { get; set; } = new AbonnementResponse();

        [ObservableProperty] private string abonnementInfo = string.Empty;
        

        public AbonneVM() { }


        /// <summary>
        ///  Vérifie si un abonnement est valide et met à jour l'affichage en conséquence.
        /// </summary>
        /// <param name="abonnementId">ID de l'abonnement scanné</param>
        /// <returns></returns>
        public async Task VerifierTicketabonnment(string abonnementId)
        {
            if (string.IsNullOrWhiteSpace(abonnementId))
                return;
           
             AbonnementResponse = await AbonnementProcessor.GetAbonnementAsync(abonnementId);
            
           

            if (!string.IsNullOrEmpty(AbonnementResponse.Message))
            {
                AbonnementInfo = Resource.InvalidTicket;
                AbonnmentInvalide = true;
                AbonnmentValide = false;

                
            }
            else
            {
                AbonnmentValide = true;
                AbonnmentInvalide = false;

                // Mise à jour des labels avec les données dynamiques et les libellés localisés
                AbonnementInfo = Resource.ValidTicketSubscription;
                

                //On catch l'id de l'abonnment pour la suite 
                abonnementId = AbonnementResponse.AbonnementId;

                
                MessageBox.Show(Resource.OpenBarrierMessage, Resource.Subscriber, MessageBoxButton.OK, MessageBoxImage.Information);
            }

           
        }


        /// <summary>
        /// Met à jour l'affichage des informations d'abonnement après un changement de langue.
        /// </summary>
        public void UpdateTicketInfoLanguage()
        {
            if (AbonnmentValide)
            {
                // Mise à jour des labels avec les données dynamiques et les libellés localisés
                AbonnementInfo = Resource.ValidTicketSubscription;

            }
            else
            {
                AbonnementInfo = string.Empty;
            }
        }

    }
}
