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
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound("Ticket non trouvé");

            if (ticket.EstPaye)
                return BadRequest("Ce ticket a déjà été payé.");

            if (ticket.EstConverti)
                return BadRequest("Ce ticket a déjà été converti en abonnement.");

            //Déterminer la durée de stationnement
            var tempsSortie = DateTime.UtcNow; // Simulation du temps de sortie
            var dureeStationnement = (tempsSortie - ticket.TempsArrive).TotalHours;

            //Cas spécial : dépassement des 24h
            if (dureeStationnement > 24)
            {
                return StatusCode(403, "⛔ La durée de stationnement dépasse les 24h autorisées. Veuillez contacter l'administration.");
            }

            //Vérifier la tarification applicable
            var tarification = await _context.Tarifications
                .FirstOrDefaultAsync(t => dureeStationnement >= t.DureeMin && dureeStationnement <= t.DureeMax);

            if (tarification == null)
                return BadRequest("Aucune tarification trouvée pour la durée du stationnement.");

            return Ok(new
            {
                Montant = tarification.Prix,
                DureeStationnement = Math.Round(dureeStationnement, 2),
                TarificationAppliquee = tarification.Niveau
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

            decimal montant = ((OkObjectResult)montantResponse.Result).Value is { } value ? (decimal)value.GetType().GetProperty("Montant")?.GetValue(value) : 0;

            // Mettre à jour le statut du ticket et enregistrer le paiement
            ticket.EstPaye = true;
            ticket.TempsSortie = DateTime.UtcNow;

            var paiement = new Paiement
            {
                TicketId = ticket.Id,
                Montant = montant,
                DatePaiement = ticket.TempsSortie.Value
            };

            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();

            return Ok($"Paiement du ticket effectué. Montant : {montant}$");
        }
    }
}
