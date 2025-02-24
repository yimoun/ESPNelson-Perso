using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows;
using BornePaiement.Model;

namespace BornePaiement.ViewModel
{
    public partial class AbonneVM : ObservableObject
    {
        [ObservableProperty] private string abonnementId;

        public IRelayCommand VerifierAbonnementCommand { get; }

        public AbonneVM()
        {
            VerifierAbonnementCommand = new RelayCommand(async () => await VerifierAbonnement());
        }

        private async Task VerifierAbonnement()
        {
            //var actif = await AbonnementProcessor.VerifierAbonnementAsync(AbonnementId);

            //if (actif)
            //{
            //    MessageBox.Show("✅ Abonnement actif ! Vous pouvez passer.");
            //}
            //else
            //{
            //    MessageBox.Show("❌ Abonnement introuvable ou expiré.");
            //}
        }
    }
}
