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

namespace BorneSortie.ViewModel
{
    public partial class BorneSortieVM : ObservableObject
    {
        [ObservableProperty] private bool ticketValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool ticketInvalide = false;
        [ObservableProperty] private string ticketInfo;

        [ObservableProperty] private bool abonnmentValide = false;  // ✅ Pour gérer l'affichage dynamique
        [ObservableProperty] private bool abonnmentInvalide = false;
        [ObservableProperty] private string abonnementInfo;

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
                    // ❌ Ticket payé, mise à jour de l'affichage
                    TicketInfo = $"❌ Paiement non validé !\n ce ticket existe bien mais n'a pas été payé";
                    TicketValide = false;
                    TicketInvalide = true;

                    return;
                }

                // ✅ Ticket payé, mise à jour de l'affichage
                TicketInfo = $"✅ Paiement validé !\nHeure d'arrivée : {ticketEstPayeResponse.TempsArrivee}\nHeure de payement : {ticketEstPayeResponse.TempsSortie}";
                TicketValide = true;
                TicketInvalide = false;

                return;
            }
            else
            {
                AbonnementResponse abonnementEstPayeResponse = await AbonnementProcessor.GetAbonnementAsync(ticketId);

                if(abonnementEstPayeResponse.AbonnementId != string.Empty)
                {
                    // ✅ Ticket payé, mise à jour de l'affichage
                    AbonnementInfo = $"✅ Abonnement valide !\n\r vous pouvez sortir";
                    AbonnmentValide = true;
                    AbonnmentInvalide = false;
                }
                else
                {
                    messageAbonnement = abonnementEstPayeResponse.Message;
                    messageTicket = ticketEstPayeResponse.Message;
                    MessageBox.Show("❌" + messageTicket + "\r\n\t\tOU\r\n" + messageAbonnement, "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }

    
}
