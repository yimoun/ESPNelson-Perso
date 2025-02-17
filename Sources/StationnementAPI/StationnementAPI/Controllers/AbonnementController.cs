using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
using System;
using System.Threading.Tasks;

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
        public async Task<ActionResult> SouscrireAbonnement([FromBody] Abonnement abonnement)
        {
            _context.Abonnements.Add(abonnement);
            await _context.SaveChangesAsync();
            return Ok("Abonnement souscrit avec succès.");
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
