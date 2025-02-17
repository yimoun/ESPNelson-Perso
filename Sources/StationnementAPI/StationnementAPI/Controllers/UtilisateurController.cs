using Microsoft.AspNetCore.Mvc;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace StationnementAPI.Controllers
{
    [Route("api/utilisateurs")]
    [ApiController]
    public class UtilisateurController : ControllerBase
    {
        private readonly StationnementDbContext _context;

        public UtilisateurController(StationnementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            return await _context.Utilisateurs.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Utilisateur>> GetUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
                return NotFound();

            return utilisateur;
        }

        [HttpPost]
        public async Task<ActionResult<Utilisateur>> PostUtilisateur(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUtilisateur), new { id = utilisateur.Id }, utilisateur);
        }
    }

}
