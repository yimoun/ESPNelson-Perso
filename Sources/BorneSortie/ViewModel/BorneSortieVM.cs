using BorneSortie.Model;
using BorneSortie.Models;
using BorneSortie.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BorneSortie.Resources;

namespace BorneSortie.ViewModel
{
    /// <summary>
    ///ViewModel pour gérer l'affichage et la vérification des tickets et abonnements.
    /// Utilise l'architecture MVVM et CommunityToolkit.MVVM.
    /// </summary>
    public partial class BorneSortieVM : ObservableObject
    {
        /// <summary>
        /// Indique si le ticket est valide (affichage dynamique).
        /// </summary>
        [ObservableProperty] private bool ticketValide = false;

        /// <summary>
        /// Indique si le ticket est invalide (affichage dynamique).
        /// </summary>
        [ObservableProperty] private bool ticketInvalide = false;


        /// <summary>
        /// Contient les informations du ticket scanné.
        /// </summary>
        [ObservableProperty] private string ticketInfo = string.Empty;

        /// <summary>
        /// Indique si l'abonnement est valide (affichage dynamique).
        /// </summary>
        [ObservableProperty] private bool abonnmentValide = false;

        /// <summary>
        /// Indique si l'abonnement est invalide (affichage dynamique).
        /// </summary>
        [ObservableProperty] private bool abonnmentInvalide = false;

        /// <summary>
        /// Contient les informations de l'abonnement scanné.
        /// </summary>
        [ObservableProperty] private string abonnementInfo = string.Empty;


        /// <summary>
        /// Indique si un ticket ou un abonnement a été scanné.
        /// </summary>
        [ObservableProperty] public bool hasScanned = false;


        /// <summary>
        /// Contient l'ID de l'abonnement en cours de vérification.
        /// </summary>
        [ObservableProperty] private string abonnementId;

        /// <summary>
        /// Stocke temporairement l'Id du ticket scanné.
        /// </summary>
        private string ticketScanne = "";



        public BorneSortieVM() { }

        /// <summary>
        /// Vérifie si un ticket a été payé ou s'il s'agit d'un abonnement valide.
        /// Met à jour les informations d'affichage en fonction du résultat.
        /// </summary>
        /// <param name="ticketId">Identifiant du ticket scanné.</param>
        /// <returns></returns>
        public async Task VerifierTicketPaye(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                return;

            string messageTicket = "";
            string messageAbonnement = "";

            //Double appel pour vérifier s'il s'agit d'un abonnement ou d'un ticket de stationnement valide
            TicketEstPayeResponse ticketEstPayeResponse = await TicketProcessor.GetTicketPayeAsync(ticketId);

            if(ticketEstPayeResponse.TicketId != string.Empty)
            {
                if (ticketEstPayeResponse.EstPaye == false)
                {
                    // ❌ Ticket non payé, mise à jour de l'affichage
                    TicketInfo = string.Format(Resource.UnpaidTicket).Replace("\n", Environment.NewLine);
                    TicketInvalide = true;
                    TicketValide = false;   
                    HasScanned = true;

                    return;
                }

                else
                {
                    // ✅ Ticket payé, mise à jour de l'affichage
                    TicketInfo = string.Format(Resource.ValidPaiment).Replace("\n", Environment.NewLine);
                    TicketValide = true;
                    TicketInvalide = false;

                    HasScanned = true;
                }

               

                return;
            }
            else
            {
                AbonnementResponse abonnementEstPayeResponse = await AbonnementProcessor.GetAbonnementAsync(ticketId);

                if(abonnementEstPayeResponse.AbonnementId != string.Empty)
                {
                    // ✅ Ticket payé, mise à jour de l'affichage
                    AbonnementInfo = string.Format(Resource.ValidSubscription).Replace("\n", Environment.NewLine);
                    AbonnmentValide = true;
                    AbonnmentInvalide = false;

                    TicketInfo = string.Empty;  //POur éviter des encombrement d'affichage
                }
                else
                {
                    MessageBox.Show(string.Format(Resource.ErrorSubscriptionTicket.Replace("\n", Environment.NewLine)), "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    
                    //HasScanned = true; //Juste pour pouvoir afficher ce message via cette variable
                    //TicketInfo = string.Format(Resource.ErrorSubscriptionTicket.Replace("\n", Environment.NewLine));
                   
                }
            }
        }

        /// <summary>
        /// Méthode pour rafraîchir TicketInfo et AbonnementInfo après le changement de langue
        /// </summary>
        public void UpdateTicketInfoLanguage()
        {
            if (TicketValide)
            {
                TicketInfo = string.Format(Resource.ValidPaiment).Replace("\n", Environment.NewLine);
            }
            else if (TicketInvalide)
            {
                TicketInfo = string.Format(Resource.UnpaidTicket).Replace("\n", Environment.NewLine);
            }
            else
            {
                TicketInfo = string.Empty;
            }

            if(AbonnmentValide)
            {
                TicketInfo = string.Empty;  //POur éviter des encombrement d'affichage
                AbonnementInfo = string.Format(Resource.ValidSubscription).Replace("\n", Environment.NewLine);
            }
            else
            {
                //à revoir quand l'abonnement n'est pas valid
            }
        }

    }


}
