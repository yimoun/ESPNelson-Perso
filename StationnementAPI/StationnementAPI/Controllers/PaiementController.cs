using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
using StationnementAPI.Models.ModelsDTO;
using System;
using System.Threading.Tasks;

namespace StationnementAPI.Controllers
{
    [Route("api/paiements")]
    [ApiController]
    public class PaiementController : ControllerBase
    {
        private readonly StationnementDbContext _context;

        public PaiementController(StationnementDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calculer le montant du paiement sans l'effectuer
        /// </summary>
        [HttpGet("calculer-montant/{ticketId}")]
        public async Task<ActionResult<object>> CalculerMontantTicket(string ticketId)
        {
            // Rechercher le ticket dans la base de données
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound(new
                {
                    Message = "Ticket non trouvé",
                    TicketId = ticketId
                });
            }


            // Déterminer la durée de stationnement
            var tempsSortie = DateTime.Now; // Simulation du temps de sortie
            var dureeStationnement = (tempsSortie - ticket.TempsArrive).TotalHours;

            // Cas spécial : dépassement des 24h
            if (dureeStationnement > 24)
            {
                return StatusCode(403, new
                {
                    Message = "⛔ La durée de stationnement dépasse les 24h autorisées. Veuillez contacter l'administration.",
                    DureeStationnement = Math.Round(dureeStationnement, 2),
                    TempsArrive = ticket.TempsArrive,
                    TempsSortie = tempsSortie
                });
            }

            // Vérifier la tarification applicable
            var tarification = await _context.Tarifications
                .FirstOrDefaultAsync(t => dureeStationnement >= t.DureeMin && dureeStationnement <= t.DureeMax);

            if (tarification == null)
            {
                return BadRequest(new
                {
                    Message = "Aucune tarification trouvée pour la durée du stationnement.",
                    DureeStationnement = Math.Round(dureeStationnement, 2)
                });
            }


            // Récupérer les taxes les plus récentes
            var configuration = await _context.Configurations
                .OrderByDescending(c => c.DateModification)
                .FirstOrDefaultAsync();

            if (configuration == null)
            {
                return BadRequest(new
                {
                    Message = "Aucune configuration trouvée pour les taxes."
                });
            }

            // Calculer le montant total avec taxes
            decimal montantTotal = tarification.Prix;
            decimal taxes = montantTotal * (configuration.TaxeFederal + configuration.TaxeProvincial) / 100;
            decimal montantAvecTaxes = montantTotal + taxes;

            // Retourner les informations de calcul du montant
            return Ok(new
            {
                Montant = montantTotal,
                Taxes = taxes,
                MontantAvecTaxes = montantAvecTaxes,
                DureeStationnement = Math.Round(dureeStationnement, 2),
                TarificationAppliquee = tarification.Niveau,
                TarificationPrix = tarification.Prix,
                TarificationDureeMin = tarification.DureeMin,
                TarificationDureeMax = tarification.DureeMax,
                TempsArrivee = ticket.TempsArrive,
                TempsSortie = tempsSortie,
                EstPaye = ticket.EstPaye,
                EstConverti = ticket.EstConverti,
                TaxeFederal = configuration.TaxeFederal,
                TaxeProvincial = configuration.TaxeProvincial
            });
        }



        /// <summary>
        /// Effectuer le paiement d'un ticket
        /// </summary>
        [HttpPost("payer-ticket")]
        public async Task<ActionResult> PayerTicket([FromBody] PaiementDto paiementDto)
        {
            if (string.IsNullOrEmpty(paiementDto.TicketId))
                return BadRequest("TicketId est requis pour un paiement de ticket.");

            var ticket = await _context.Tickets.FindAsync(paiementDto.TicketId);
            if (ticket == null)
                return NotFound("Ticket non trouvé");

            if (ticket.EstPaye)
                return BadRequest("Ce ticket a déjà été payé.");

            if (ticket.EstConverti)
                return BadRequest("Ce ticket a déjà été converti en abonnement.");

            // Déterminer le montant du paiement en appelant la méthode
            var montantResponse = await CalculerMontantTicket(ticket.Id);
            if (montantResponse.Result is BadRequestObjectResult || montantResponse.Result is NotFoundObjectResult)
                return montantResponse.Result; // Retourne l'erreur appropriée


            var montantResult = ((OkObjectResult)montantResponse.Result).Value;
            decimal montantTotal = (decimal)montantResult.GetType().GetProperty("Montant")?.GetValue(montantResult);
            decimal taxes = (decimal)montantResult.GetType().GetProperty("Taxes")?.GetValue(montantResult);
            decimal montantAvecTaxes = (decimal)montantResult.GetType().GetProperty("MontantAvecTaxes")?.GetValue(montantResult);

            string tarificationNiveau = (string)montantResult.GetType().GetProperty("TarificationAppliquee")?.GetValue(montantResult);
            decimal tarificationPrix = (decimal)montantResult.GetType().GetProperty("TarificationPrix")?.GetValue(montantResult);
            int tarificationDureeMin = (int)montantResult.GetType().GetProperty("TarificationDureeMin")?.GetValue(montantResult);
            int tarificationDureeMax = (int)montantResult.GetType().GetProperty("TarificationDureeMax")?.GetValue(montantResult);

            // Mettre à jour le statut du ticket et enregistrer le paiement
            ticket.EstPaye = true;
            ticket.TempsSortie = DateTime.UtcNow;

            var paiement = new Paiement
            {
                TicketId = ticket.Id,
                Montant = montantAvecTaxes,
                DatePaiement = ticket.TempsSortie.Value,
                TarificationNiveau = tarificationNiveau,
                TarificationPrix = tarificationPrix,
                TarificationDureeMin = tarificationDureeMin,
                TarificationDureeMax = tarificationDureeMax
            };

            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Paiement du ticket effectué. Montant : {montantAvecTaxes}$",
                MontantTotal = montantTotal,
                Taxes = taxes,
                MontantAvecTaxes = montantAvecTaxes,
                TempsArrivee = ticket.TempsArrive,
                TempsSortie = ticket.TempsSortie,
                TarificationAppliquee = tarificationNiveau
            });
        }
    }
}
