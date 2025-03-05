using Administration.Data;
using Administration.Data.Context;
using Administration.Helpers;
using Administration.Model;
using Administration.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Security;
using System.Windows;

namespace Administration.ViewModel
{
    public partial class UtilisateurDialogVM : ObservableObject
    {
        private readonly AdministrationContext _dbContext;

        public Action<bool>? CloseDialogAction { get; set; }  // <- Action injectée depuis la vue


        [ObservableProperty]
        private Utilisateur utilisateur;

        [ObservableProperty]
        private bool peutModifierSonMotDePasse;

        [ObservableProperty]
        private bool afficherChangementMotDePasse;

        public bool EstNouveau { get; }
        public SecureString? OldPassword { get; set; }
        public SecureString? NewPassword { get; set; }
        public SecureString? ConfirmPassword { get; set; }

        public string TitreDialog => EstNouveau ? "Ajouter un administrateur" : "Modifier un administrateur";
        public string TexteBouton => EstNouveau ? "Ajouter" : "Modifier";

        public UtilisateurDialogVM(Utilisateur utilisateur)
        {
            _dbContext = new AdministrationContextFactory().CreateDbContext(new string[0]);
            Utilisateur = utilisateur;
            EstNouveau = utilisateur.Id == 0;
            PeutModifierSonMotDePasse = !EstNouveau && App.Current.User?.Id == utilisateur.Id;
        }

        [RelayCommand]
        private void ToggleMotDePasse() => AfficherChangementMotDePasse = !AfficherChangementMotDePasse;

        [RelayCommand]
        private void Enregistrer()
        {
            // Vérifier l'unicité de l'email (autre que lui-même s'il s'agit d'une modification)
            bool emailDejaPris = _dbContext.Utilisateurs
                .Any(u => u.Email == Utilisateur.Email && u.Id != Utilisateur.Id);

            if (emailDejaPris)
            {
                MessageBox.Show("Cet email est déjà utilisé par un autre administrateur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EstNouveau)
            {
               //On lui donne un mot de passe par défaut, il sera invité à le changer plus tard
                Utilisateur.MotDePasse = CryptographyHelper.HashPassword("admin");
                _dbContext.Utilisateurs.Add(Utilisateur);

                MessageBox.Show("l'utilisateur a été crée avec \"admin\" comme mot de passe temporaire", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (AfficherChangementMotDePasse)
                {
                    var oldPassword = ConvertHelper.SecureStringToString(OldPassword);
                    var newPassword = ConvertHelper.SecureStringToString(NewPassword);
                    var confirmPassword = ConvertHelper.SecureStringToString(ConfirmPassword);

                    if (!CryptographyHelper.ValidateHashedPassword(oldPassword, Utilisateur.MotDePasse))
                    {
                        MessageBox.Show("Ancien mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (newPassword != confirmPassword)
                    {
                        MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    Utilisateur.MotDePasse = CryptographyHelper.HashPassword(newPassword);
                }
                _dbContext.Utilisateurs.Update(Utilisateur);
            }

            _dbContext.SaveChanges();

            // Appeler l'action passée depuis la fenêtre
            CloseDialogAction?.Invoke(true);

            
        }

        [RelayCommand]
        private void Annuler()
        {
            CloseDialogAction?.Invoke(false);
        }


    }

}
