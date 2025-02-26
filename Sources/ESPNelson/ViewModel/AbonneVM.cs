using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows;
using ESPNelson.Model;

namespace ESPNelson.ViewModel
{
    public partial class AbonneVM : ObservableObject
    {
        [ObservableProperty] private string abonnementId;

        [ObservableProperty] private bool abonnmentValide = false;
        [ObservableProperty] private bool abonnmentInvalide = false;
        [ObservableProperty] private string abonnementInfo;



        public AbonneVM() { }
        

        public async Task VerifierTicketabonnment(string abonnementId)
        {
            if (string.IsNullOrWhiteSpace(abonnementId))
                return;
           
            var abonnementResponse = await AbonnementProcessor.GetAbonnementAsync(abonnementId);

            if (!string.IsNullOrEmpty(abonnementResponse.Message))
            {
                AbonnementInfo = abonnementResponse.Message;
                AbonnmentInvalide = true;
                AbonnmentValide = false;

                MessageBox.Show(abonnementResponse.Message, "Erreur d'Abonnement", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                AbonnementInfo = $"✅ Abonnement valide !\n\n" +
                                 $"ID : {abonnementResponse.AbonnementId}\n" +
                                 $"Type : {abonnementResponse.TypeAbonnement}\n" +
                                 $"Début : {abonnementResponse.DateDebut:dd/MM/yyyy}\n" +
                                 $"Fin : {abonnementResponse.DateFin:dd/MM/yyyy}\n";

                AbonnmentValide = true;
                AbonnmentInvalide = false;

                //On catch l'id de l'abonnment pour la suite 
                abonnementId = abonnementResponse.AbonnementId;

                MessageBox.Show("L'abonnement est valide et actif donc la barrière va s'ouvrir.", "Ouverture de Barrière", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
