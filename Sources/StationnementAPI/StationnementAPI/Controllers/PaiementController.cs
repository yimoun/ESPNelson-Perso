using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
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

        [HttpPost("payer-ticket")]
        public async Task<ActionResult> PayerTicket([FromBody] Paiement paiement)
        {
            var ticket = await _context.Tickets.FindAsync(paiement.TicketId);
            if (ticket == null)
                return NotFound("Ticket non trouvé");

            ticket.EstPaye = true;
            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();

            return Ok("Paiement du ticket effectué.");
        }

        [HttpPost("payer-abonnement")]
        public async Task<ActionResult> PayerAbonnement([FromBody] Paiement paiement)
        {
            var abonnement = await _context.Abonnements.FindAsync(paiement.AbonnementId);
            if (abonnement == null)
                return NotFound("Abonnement non trouvé");

            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();

            return Ok("Paiement de l'abonnement effectué.");
        }
    }
}
