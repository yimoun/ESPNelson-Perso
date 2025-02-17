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

        [HttpPost("generer")]
        public async Task<ActionResult<Ticket>> GenererTicket()
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid().ToString(),  //Génération automatique de l'ID unique
                TempsArrive = DateTime.UtcNow
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


        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            return ticket;
        }
    }


}
