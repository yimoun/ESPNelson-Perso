using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Administration.Data;
using Administration.Data.Context;
using Administration.Model;

namespace Administration.Helpers
{
    public class PasswordHelper
    {
        /// <summary>
        /// Génère un mot de passe aléatoire de la longueur spécifiée. Contiens les 26 lettres de l'alphabet en majuscule et minuscule ainsi que les chiffres de 0 à 9.
        /// </summary>
        /// <param name="length">La longueur voulu du mot de passe</param>
        /// <returns>Un mot de passe aléatoire de la longueur spécifié</returns>
        public static async Task<string> GeneratePassword(int length)
        {
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                sb.Append(validChars[rnd.Next(validChars.Length)]);
            }
            return sb.ToString();
        }


        public static async Task<bool> ResetPassword(string email, Utilisateur? utilisateur = null, AdministrationContext? context = null)
        {
            // Verify that the email is valid
            if (email is null || !EmailHelper.IsValidEmail(email))
            {
                MessageBox.Show("Adresse courriel invalide", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Generate a new password
            string newPassword = await GeneratePassword(8);

            // Send the new password to the user
            string subject = "Réinitialisation de mot de passe";
            string body = "Votre nouveau mot de passe est: " + newPassword + ".\n\nVeuillez changer votre mot de passe une fois connecter!";
            await EmailHelper.SendEmail(string.Empty, email, subject, body);

            // Update the password in the database
            if (utilisateur is null || context is null)
            {
                AdministrationContextFactory factory = new AdministrationContextFactory();
                context = factory.CreateDbContext(new string[0]);
                utilisateur = context.Utilisateurs.FirstOrDefault(u => u.Email == email);
            }
            utilisateur.MotDePasse = CryptographyHelper.HashPassword(newPassword);
            utilisateur.MotDePasseDoitEtreChange = true; //Pour qu'il change son mot de passe lors de la prochaine connexion par lui meme dans la console de gestion
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {

                MessageBox.Show("Erreur lors de la réinitialisation du mot de passe: " + e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vérifie si le mot de passe, un SecureString, est vide
        /// </summary>
        /// <param name="password">Le SecureString</param>
        /// <returns>True s'il est vide. Sinon false</returns>
        public static bool IsPasswordEmpty(SecureString password)
        {
            try
            {
                return password.Length == 0;
            }
            catch (Exception e)
            {
                return true;
            }
        }
    }
}
