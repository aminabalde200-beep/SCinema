using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCinema.Data;
using SCinema.Models;

namespace SCinema
{
    [AllowAnonymous]
    public class CatalogueController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public CatalogueController(ApplicationDbContext ctx) => _ctx = ctx;

        // GET: /Catalogue?q=...&genre=...
        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? genre)
        {
            var query = _ctx.Films.AsNoTracking().AsQueryable();

            // Recherche simple dans Title + Synopsis
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(f =>
                    f.Title.Contains(q) || f.Synopsis.Contains(q));
            }

            // Filtre par genre exact si fourni
            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(f => f.Genre == genre);
            }

            // Genres distincts pour la vue (liste déroulante)
            var genres = await _ctx.Films
                .Where(f => !string.IsNullOrEmpty(f.Genre))
                .Select(f => f.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            ViewBag.Genres = genres;   // List<string>
            ViewBag.q = q;             // valeur saisie
            ViewBag.genre = genre;     // genre sélectionné

            var films = await query
                .OrderBy(f => f.Title)
                .ToListAsync();

            return View(films);
        }

        // GET: /Catalogue/Film/5  -> détail du film + séances à venir
        [HttpGet]
        public async Task<IActionResult> Film(int id)
        {
            var film = await _ctx.Films.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (film == null) return NotFound();

            // ⚠️ Utilise le bon nom de propriété date/heure de ta classe Seance:
            //   - si tu as Seance.Debut  -> garde .Debut
            //   - si c’est DateHeureDebut -> remplace ci-dessous par .DateHeureDebut
            var nowUtc = DateTime.UtcNow;

            var seances = await _ctx.Seances
                .AsNoTracking()
                .Include(s => s.Salle)
                .Where(s => s.FilmId == id && s.HeureDebut >= nowUtc) // <-- adapte ici si besoin
                .OrderBy(s => s.HeureDebut)                           // <-- idem si DateHeureDebut
                .ToListAsync();

            var seanceIds = seances.Select(s => s.Id).ToList();

            var tarifs = await _ctx.TarifsSeance
                .AsNoTracking()
                .Where(t => seanceIds.Contains(t.SeanceId))
                .ToListAsync();

            // dico: SeanceId -> liste de tarifs
            var tarifsBySeance = tarifs
                .GroupBy(t => t.SeanceId)
                .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.Seances = seances;
            ViewBag.Tarifs = tarifsBySeance; // Dictionary<int, List<TarifSeance>>
            return View(film);
        }
    }
}
