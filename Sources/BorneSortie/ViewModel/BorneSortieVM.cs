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
    public partial class BorneSortieVM : ObservableObject
    {
        [ObservableProperty] private bool ticketValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool ticketInvalide = false;
        [ObservableProperty] private string ticketInfo = string.Empty;

        [ObservableProperty] private bool abonnmentValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool abonnmentInvalide = false;
        [ObservableProperty] private string abonnementInfo = string.Empty;

        [ObservableProperty] public bool hasScanned = false; 


        [ObservableProperty] private string abonnementId;

        private string ticketScanne = ""; // 🔹 Stocke temporairement le scan



        public BorneSortieVM() { }

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
                }
                else
                {
                    messageAbonnement = abonnementEstPayeResponse.Message;
                    messageTicket = ticketEstPayeResponse.Message;
                    MessageBox.Show(string.Format(Resource.ErrorSubscriptionTicket.Replace("\n", Environment.NewLine), messageTicket, messageAbonnement), "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                AbonnementInfo = string.Format(Resource.ValidSubscription).Replace("\n", Environment.NewLine);
            }
            else
            {
                //à revoir quand l'abonnement n'est pas valid
            }
        }

    }


}
