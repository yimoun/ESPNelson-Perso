using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
using StationnementAPI.Models.ModelsDTO;
using System;
using System.Threading.Tasks;
using StationnementAPI.Models.ModelsDTO;

namespace StationnementAPI.Controllers
{
    [Route("api/abonnements")]
    [ApiController]
    public class AbonnementController : ControllerBase
    {
        private readonly StationnementDbContext _context;

        public AbonnementController(StationnementDbContext context)
        {
            _context = context;
        }

        [HttpPost("souscrire")]
        public async Task<ActionResult> SouscrireAbonnement([FromBody] PaiementDto paiementDto)
        {
            if (string.IsNullOrEmpty(paiementDto.TicketId))
                return BadRequest("TicketId est requis pour souscrire un abonnement.");

            if (string.IsNullOrEmpty(paiementDto.Email))
                return BadRequest("L'email est requis pour enregistrer un nouvel abonné.");

            if (string.IsNullOrEmpty(paiementDto.TypeAbonnement) ||
                (paiementDto.TypeAbonnement.ToLower() != "mensuel" && paiementDto.TypeAbonnement.ToLower() != "hebdomadaire"))
            {
                return BadRequest("Type d'abonnement invalide. Choisissez 'mensuel' ou 'hebdomadaire'.");
            }

            var ticket = await _context.Tickets.FindAsync(paiementDto.TicketId);
            if (ticket == null)
                return NotFound("Ticket non trouvé.");
            if (ticket.EstConverti)
                return Conflict("Ce ticket a déjà été utilisé pour un abonnement.");
            

            var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == paiementDto.Email);
            if (existingUser != null)
                return Conflict("Cet email est déjà associé à un abonné.");

            if(!ticket.EstConverti)
                ticket.EstConverti = true;  //Afin de ne plus l'utiliser dans le future pour souscrire à un abonnment !

            var utilisateur = new Utilisateur
            {
                NomUtilisateur = paiementDto.Email.Split('@')[0],
                MotDePasse = "MotDePasseTemporaire123!",
                Email = paiementDto.Email,
                BadgeId = Utilisateur.GenerateBadgeId(),
                Role = "abonne"
            };
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            int dureeJours = paiementDto.TypeAbonnement.ToLower() == "hebdomadaire" ? 7 : 30;
            decimal montant = paiementDto.TypeAbonnement.ToLower() == "mensuel" ? 50 : 15;

            var abonnement = new Abonnement
            {
                UtilisateurId = utilisateur.Id,
                DateDebut = DateTime.UtcNow,
                DateFin = DateTime.UtcNow.AddDays(dureeJours),
                Type = paiementDto.TypeAbonnement.ToLower()
            };
            _context.Abonnements.Add(abonnement);
            await _context.SaveChangesAsync();

            ticket.EstConverti = true;

            var paiement = paiementDto.DtoToPaiement(montant, abonnement.Id);
            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();

            return Created($"api/abonnements/{abonnement.Id}", new
            {
                Message = "Abonnement souscrit avec succès.",
                UtilisateurId = utilisateur.Id,
                BadgeId = utilisateur.BadgeId,
                TypeAbonnement = abonnement.Type,
                DateDebut = abonnement.DateDebut,
                DateFin = abonnement.DateFin,
                MontantPaye = paiement.Montant
            });
        }

        [HttpGet("actifs")]
        public async Task<ActionResult> GetAbonnementsActifs()
        {
            var abonnements = await _context.Abonnements
                .Where(a => a.DateFin > DateTime.UtcNow)
                .ToListAsync();

            return Ok(abonnements);
        }

    }
}
