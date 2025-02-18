using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
using StationnementAPI.Models.ModelsDTO;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace StationnementAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly StationnementDbContext _context;

        public AdminController(StationnementDbContext context)
        {
            _context = context;
        }

        #region Gestion des utilisateurs

        [HttpPost("connexion")]
        public async Task<ActionResult> ConnexionAdmin([FromBody] UtilisateurDto utilisateurDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.NomUtilisateur == utilisateurDto.NomUtilisateur && u.Role == "admin");

            if (utilisateur == null || utilisateur.MotDePasse != utilisateurDto.MotDePasse)
                return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");

            return Ok(new
            {
                Message = "Connexion réussie.",
                UtilisateurId = utilisateur.Id,
                NomUtilisateur = utilisateur.NomUtilisateur,
                Role = utilisateur.Role
            });
        }

        /// <summary>
        /// Liste de tous les utilisateurs (accessible uniquement par un admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet("utilisateurs")]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            var utilisateurs = await _context.Utilisateurs.ToListAsync();
            if (!utilisateurs.Any())
                return NotFound("Aucun utilisateur trouvé.");

            return Ok(utilisateurs);
        }

        /// <summary>
        /// Récupérer un utilisateur spécifique
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("utilisateurs/{id}")]
        public async Task<ActionResult<Utilisateur>> GetUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
                return NotFound("Utilisateur non trouvé.");

            return Ok(utilisateur);
        }

        //Ajouter un nouvel utilisateur
        [HttpPost("utilisateurs")]
        public async Task<ActionResult> AjouterUtilisateur([FromBody] Utilisateur utilisateur)
        {
            if (string.IsNullOrEmpty(utilisateur.NomUtilisateur) || string.IsNullOrEmpty(utilisateur.MotDePasse))
                return BadRequest("Le nom d'utilisateur et le mot de passe sont obligatoires.");

            if (await _context.Utilisateurs.AnyAsync(u => u.NomUtilisateur == utilisateur.NomUtilisateur))
                return Conflict("Ce nom d'utilisateur est déjà pris.");

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUtilisateur), new { id = utilisateur.Id }, utilisateur);
        }

        /// <summary>
        /// Modifier un utilisateur existant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="utilisateurModifie"></param>
        /// <returns></returns>
        [HttpPut("utilisateurs/{id}")]
        public async Task<ActionResult> ModifierUtilisateur(int id, [FromBody] Utilisateur utilisateurModifie)
        {
            if (id != utilisateurModifie.Id)
                return BadRequest("L'ID de l'utilisateur ne correspond pas à l'ID de la requête.");

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
                return NotFound("Utilisateur non trouvé.");

            utilisateur.NomUtilisateur = utilisateurModifie.NomUtilisateur ?? utilisateur.NomUtilisateur;
            utilisateur.MotDePasse = utilisateurModifie.MotDePasse ?? utilisateur.MotDePasse;
            utilisateur.Role = utilisateurModifie.Role ?? utilisateur.Role;
            utilisateur.Email = utilisateurModifie.Email ?? utilisateur.Email;

            _context.Utilisateurs.Update(utilisateur);
            await _context.SaveChangesAsync();

            return Ok("Utilisateur mis à jour avec succès.");
        }

        /// <summary>
        /// Supprimer un utilisateur
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("utilisateurs/{id}")]
        public async Task<ActionResult> SupprimerUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
                return NotFound("Utilisateur non trouvé.");

            _context.Utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();

            return Ok("Utilisateur supprimé avec succès.");
        }

        #endregion


        #region Gestion des tarifications

        /// <summary>
        /// Liste des tarifications
        /// </summary>
        /// <returns></returns>
        [HttpGet("tarifications")]
        public async Task<ActionResult<IEnumerable<Tarification>>> GetTarifications()
        {
            var tarifications = await _context.Tarifications.ToListAsync();
            if (!tarifications.Any())
                return NotFound("Aucune tarification disponible.");

            return Ok(tarifications);
        }


        /// <summary>
        /// Modifier une tarification existante
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tarificationModifiee"></param>
        /// <returns></returns>
        [HttpPut("tarifications/{id}")]
        public async Task<ActionResult> ModifierTarification([FromBody] Tarification tarification)
        {

            var tarificationExistante = await _context.Tarifications.FindAsync(tarification.Id);
            if (tarification == null)
                return NotFound("Tarification non trouvée.");

            // Mise à jour des champs modifiés
            tarificationExistante.Niveau = tarification.Niveau;
            tarificationExistante.Prix = tarification.Prix;
            tarificationExistante.DureeMin = tarification.DureeMin;
            tarificationExistante.DureeMax = tarification.DureeMax;

            _context.Tarifications.Update(tarification);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Tarification mise à jour avec succès.",
                tarification.Id,
                tarification.Niveau,
                tarification.DureeMin,
                tarification.DureeMax,
                tarification.Prix,
            });
        }


        /// <summary>
        /// Supprimer une tarification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("tarifications/{id}")]
        public async Task<ActionResult> SupprimerTarification(int id)
        {
            var tarification = await _context.Tarifications.FindAsync(id);
            if (tarification == null)
                return NotFound("Tarification non trouvée.");

            _context.Tarifications.Remove(tarification);
            await _context.SaveChangesAsync();

            return Ok("Tarification supprimée avec succès.");
        }

        /// <summary>
        /// Ajouter une nouvelle tarification
        /// </summary>
        /// <param name="nouvelleTarification"></param>
        /// <returns></returns>
        [HttpPost("tarifications")]
        public async Task<ActionResult> AjouterTarification([FromBody] Tarification nouvelleTarification)
        {
            if (nouvelleTarification == null)
                return BadRequest("Données invalides.");

            _context.Tarifications.Add(nouvelleTarification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTarifications), new { id = nouvelleTarification.Id }, nouvelleTarification);
        }

        #endregion


        #region Gestion des rapports

        /// <summary>
        ///Générer un rapport
        /// </summary>
        /// <param name="dateDebut"></param>
        /// <param name="dateFin"></param>
        /// <param name="utilisateurId"></param>
        /// <returns></returns>
        [HttpPost("rapports/generer")]
        public async Task<ActionResult> GenererRapport([FromBody] RapportDto rapportDto)
        {
            if (rapportDto.DateDebut >= rapportDto.DateFin)
                return BadRequest("La date de début doit être inférieure à la date de fin.");

            var utilisateur = await _context.Utilisateurs.FindAsync(rapportDto.UtilisateurId);
            if (utilisateur == null || utilisateur.Role.ToLower() != "admin")
                return Unauthorized("Seuls les administrateurs peuvent générer des rapports.");

            // Calcul des revenus des paiements sur la période
            var revenus = await _context.Paiements
                .Where(p => p.DatePaiement >= rapportDto.DateFin && p.DatePaiement <= rapportDto.DateFin)
                .SumAsync(p => p.Montant);

            // Calcul de l'occupation moyenne du parking sur la période
            var tickets = await _context.Tickets
                .Where(t => t.TempsArrive >= rapportDto.DateDebut && t.TempsSortie <= rapportDto.DateFin)
                .ToListAsync();
            int nombreTickets = tickets.Count;

            //Nombre d'abonnements actifs dans la période
            var abonnementsActifs = await _context.Abonnements
                .Where(a => a.DateDebut <= rapportDto.DateFin && a.DateFin >= rapportDto.DateDebut)
                .CountAsync();

            //Générer le contenu du rapport
            string contenuRapport = $"Rapport généré le {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}\n\n";
            contenuRapport += $"Période : {rapportDto.DateDebut:yyyy-MM-dd} à {rapportDto.DateFin:yyyy-MM-dd}\n\n";
            contenuRapport += $"Revenus générés : {revenus} $\n";
            contenuRapport += $"Nombre total de tickets traités : {nombreTickets}\n";
            contenuRapport += $"Nombre d'abonnements actifs : {abonnementsActifs}\n";

            //Sauvegarde du fichier rapport
            string nomFichier = $"Rapport_{rapportDto.DateDebut:yyyyMMdd}_{rapportDto.DateFin:yyyyMMdd}.txt";
            string cheminFichier = Path.Combine("Rapports", nomFichier);
            Directory.CreateDirectory("Rapports"); // Crée le dossier si inexistant
            await System.IO.File.WriteAllTextAsync(cheminFichier, contenuRapport, Encoding.UTF8);

            //Enregistrement dans la base de données
            var rapport = new Rapport
            {
                DateGeneration = DateTime.UtcNow,
                DateDebut = rapportDto.DateDebut,
                DateFin = rapportDto.DateFin,
                Fichier = cheminFichier,
                UtilisateurId = rapportDto.UtilisateurId
            };

            _context.Rapports.Add(rapport);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Rapport généré avec succès.",
                CheminFichier = cheminFichier,
                DateDebut = rapport.DateDebut,
                DateFin = rapport.DateFin,
                Revenus = revenus,
                NombreTickets = nombreTickets,
                AbonnementsActifs = abonnementsActifs
            });
        }

        /// <summary>
        /// Récupérer tous les rapports
        /// </summary>
        /// <returns></returns>
        [HttpGet("rapports")]
        public async Task<ActionResult<IEnumerable<Rapport>>> GetRapports()
        {
            var rapports = await _context.Rapports.ToListAsync();
            if (!rapports.Any())
                return NotFound("Aucun rapport trouvé.");

            return Ok(rapports);
        }


        /// <summary>
        /// Télécharger un rapport spécifique
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("rapports/{id}")]
        public async Task<IActionResult> TelechargerRapport(int id)
        {
            var rapport = await _context.Rapports.FindAsync(id);
            if (rapport == null)
                return NotFound("Rapport non trouvé.");

            if (!System.IO.File.Exists(rapport.Fichier))
                return NotFound("Le fichier rapport n'existe plus sur le serveur.");

            var bytes = await System.IO.File.ReadAllBytesAsync(rapport.Fichier);
            return File(bytes, "text/plain", Path.GetFileName(rapport.Fichier));
        }

        #endregion


        #region Gestion du tableau de bord
        /// <summary>
        ///Occupation du parking en temps réel selon le moment précis de la journée.
        /// </summary>
        /// <returns></returns>
        [HttpGet("occupation")]
        public async Task<ActionResult> GetOccupationActuelle()
        {
            DateTime heureActuelle = DateTime.UtcNow;

            //Nombre total de tickets générés AVANT maintenant (véhicules entrés)
            int totalEntrees = await _context.Tickets.CountAsync(t => t.TempsArrive <= heureActuelle);

            //Nombre total de véhicules qui ont quitté AVANT maintenant
            int totalSorties = await _context.Tickets.CountAsync(t => t.TempsSortie != null && t.TempsSortie <= heureActuelle);

            //Calcul de l'occupation actuelle
            int occupationActuelle = totalEntrees - totalSorties;

            return Ok(new
            {
                OccupationActuelle = occupationActuelle,
                TotalEntrees = totalEntrees,
                TotalSorties = totalSorties,
                HeureActuelle = heureActuelle,
                Message = "Occupation en temps réel du parking récupérée avec succès."
            });
        }


        /// <summary>
        /// Revenus en temps réel (journalier 
        /// </summary>
        /// <returns></returns>& hebdomadaire)
        [HttpGet("revenus")]
        public async Task<ActionResult> GetRevenus()
        {
            DateTime aujourdHui = DateTime.UtcNow.Date;
            DateTime debutSemaine = aujourdHui.AddDays(-(int)aujourdHui.DayOfWeek); // Lundi de la semaine actuelle

            //Somme des paiements effectués aujourd’hui
            decimal revenusJournaliers = await _context.Paiements
                .Where(p => p.DatePaiement.Date == aujourdHui)
                .SumAsync(p => p.Montant);

            //Somme des paiements effectués cette semaine
            decimal revenusHebdomadaires = await _context.Paiements
                .Where(p => p.DatePaiement.Date >= debutSemaine)
                .SumAsync(p => p.Montant);

            return Ok(new
            {
                RevenusJournaliers = revenusJournaliers,
                RevenusHebdomadaires = revenusHebdomadaires,
                Message = "Revenus en temps réel récupérés avec succès."
            });
        }

        #endregion
    }
}
