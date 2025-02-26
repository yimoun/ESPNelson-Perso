using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;

namespace StationnementAPI.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly StationnementDbContext _context;

        public TicketController(StationnementDbContext context)
        {
            _context = context;
        }

        private string GenerateTicketId()
        {
            string guid = Guid.NewGuid().ToString("N").ToUpper(); // Supprime les tirets et met en majuscule
            return guid.Substring(0, 12); // Prend les 12 premiers caractères
        }


        [HttpPost("generer")]
        public async Task<ActionResult<Ticket>> GenererTicket()
        {
            // Conversion UTC vers fuseau horaire local
            var heureLocale = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            var ticket = new Ticket
            {
                Id = GenerateTicketId(),
                TempsArrive = heureLocale
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }

        [HttpGet]
        public async Task<ActionResult<List<Ticket>>> GetAllTickets()
        {
            try
            {
                //Récupération de tous les tickets avec leurs informations associées
                var lesTickets = await _context.Tickets.ToListAsync();

                if (lesTickets == null || lesTickets.Count == 0)
                {
                    return NotFound("Aucun ticket trouvé.");
                }

                return Ok(lesTickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }


        /// <summary>
        /// Par exemple à la borne de sortie pour valider si un ticket a été payé
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            return ticket;
        }


        /// <summary>
        /// Vérifie si un ticket a été payé et retourne un statut détaillé.
        /// </summary>
        /// <param name="id">L'ID du ticket</param>
        /// <returns>Statut détaillé du ticket</returns>
        [HttpGet("{id}/verifier-paiement")]
        public async Task<ActionResult<TicketEstPayeResponse>> VerifierPaiementTicket(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new TicketEstPayeResponse
                {
                    Message = "⛔ L'ID du ticket est invalide."
                });
            }

            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound(new TicketEstPayeResponse
                {
                    TicketId = id,
                    Message = "❌ Ticket introuvable."
                });
            }

            // Construire la réponse détaillée
            var response = new TicketEstPayeResponse
            {
                TicketId = ticket.Id,
                EstPaye = ticket.EstPaye,
                EstConverti = ticket.EstConverti,
                Message = ticket.EstPaye ? "✅ Le ticket a déjà été payé." : "⚠️ Le ticket existe mais n'a pas encore été payé.",
                TempsArrivee = ticket.TempsArrive,
                TempsSortie = ticket.TempsSortie
            };

            return Ok(response);
        }


    }


}
