using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
using StationnementAPI.Models.ModelsDTO;
using System;
using System.Threading.Tasks;
using StationnementAPI.Models.ModelsDTO;
using System.Net.Sockets;

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

            var ticket = await _context.Tickets.FindAsync(paiementDto.TicketId);
            if (ticket == null)
                return NotFound("Ticket non trouvé.");
            if (ticket.EstConverti)
                return Conflict("Ce ticket a déjà été utilisé pour un abonnement.");

            var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == paiementDto.Email);
            if (existingUser != null)
                return Conflict("Cet email est déjà associé à un abonné.");

            

            var utilisateur = new Utilisateur
            {
                NomUtilisateur = paiementDto.Email.Split('@')[0],
                MotDePasse = "MotDePasseTemporaire123!",
                Email = paiementDto.Email,
                Role = "abonne"
            };
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync(); // 🔥 Sauvegarder pour générer l'Id !

            // Maintenant l'Id est généré, on peut créer l'abonnement

                // Déterminer la durée et le montant en fonction du type d'abonnement
            int dureeJours = paiementDto.TypeAbonnement.ToLower() == "hebdomadaire" ? 7 : 30;
            
            var abonnement = new Abonnement
            {
                Id = GenerateAbonnmentId(),
                UtilisateurId = utilisateur.Id,
                DateDebut = DateTime.UtcNow,
                DateFin = DateTime.UtcNow.AddDays(dureeJours),
                Type = paiementDto.TypeAbonnement.ToLower()
            };
            _context.Abonnements.Add(abonnement);
            await _context.SaveChangesAsync(); // 🔥 Sauvegarder pour générer l'Id !

            // Maintenant l'Id est généré, on peut créer l'abonnement
           
            decimal montant = paiementDto.TypeAbonnement.ToLower() == "mensuel" ? 50 : 15;

            var paiement = paiementDto.DtoToPaiement(montant, abonnement.Id);
            _context.Paiements.Add(paiement);
            ticket.EstConverti = true;  //le ticket cosidéré comme converti en abonnement


            try
            {
                await _context.SaveChangesAsync();  //Dernier pour enregistrer le paiement 
            }
            catch (DbUpdateException ex)
            {
                // Log l'exception interne pour plus de détails
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
                return StatusCode(500, "Une erreur s'est produite lors de l'enregistrement en base de données.");
            }

            return Created($"api/abonnements/{abonnement.Id}", new
            {
                Message = "Abonnement souscrit avec succès.",
                AbonnementId = abonnement.Id,
                TypeAbonnement = abonnement.Type,
                DateDebut = abonnement.DateDebut,
                DateFin = abonnement.DateFin,
                MontantPaye = paiement.Montant
            });
        }


       
        [HttpGet("actifs/{id}")]
        public async Task<ActionResult<object>> GetAbonnement(string id)
        {
            var abonnement = await _context.Abonnements.FindAsync(id);
            if (abonnement == null)
                return NotFound("Aucun abonnement existant pour ce ticket !");

            if (abonnement.DateFin < DateTime.Now)
                return BadRequest("Cet abonnement n'est plus actif rendu à cette date");

            return Ok(new
            {
                Message = "",
                AbonnementId = abonnement.Id,
                TypeAbonnement = abonnement.Type,
                DateDebut = abonnement.DateDebut,
                DateFin = abonnement.DateFin,
                MontantPaye = 0
            });
        }

        private string GenerateAbonnmentId()
        {
            string guid = Guid.NewGuid().ToString("N").ToUpper(); // Supprime les tirets et met en majuscule
            return guid.Substring(0, 6); // Prend les 10 premiers caractères
        }

    }
}
