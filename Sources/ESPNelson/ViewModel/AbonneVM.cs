using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows;
using ESPNelson.Model;
using ESPNelson.Resources;

namespace ESPNelson.ViewModel
{
    public partial class AbonneVM : ObservableObject
    {
        [ObservableProperty] private string abonnementId;

        [ObservableProperty] private bool abonnmentValide = false;
        [ObservableProperty] private bool abonnmentInvalide = false;

        public AbonnementResponse AbonnementResponse { get; set; } = new AbonnementResponse();

        [ObservableProperty] private string abonnementInfo = string.Empty;
        



        public AbonneVM() { }


        /// <summary>
        ///  Vérifie si un abonnement est valide et met à jour l'affichage sous forme de labels distincts.
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

                //MessageBox.Show(abonnementResponse.Message, "Erreur d'Abonnement", MessageBoxButton.OK, MessageBoxImage.Error);
                //MessageBox.Show(Resource.InvalidTicket, Resource.Subscriber, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                AbonnmentValide = true;
                AbonnmentInvalide = false;

                // Mise à jour des labels avec les données dynamiques et les libellés localisés
                AbonnementInfo = Resource.ValidTicket;
                

                //On catch l'id de l'abonnment pour la suite 
                abonnementId = AbonnementResponse.AbonnementId;

                //MessageBox.Show("L'abonnement est valide et actif donc la barrière va s'ouvrir.", "Ouverture de Barrière", MessageBoxButton.OK, MessageBoxImage.Information);
                // Message d'ouverture de la barrière traduit
                MessageBox.Show(Resource.OpenBarrierMessage, Resource.Subscriber, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void UpdateTicketInfoLanguage()
        {
            if (AbonnmentValide)
            {
                // Mise à jour des labels avec les données dynamiques et les libellés localisés
                AbonnementInfo = Resource.ValidTicket;

            }
            else
            {
                AbonnementInfo = Resource.ValidTicket;
            }
        }

    }
}
