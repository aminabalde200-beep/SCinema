using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SCinema.Data;
using SCinema.Models;

namespace SCinema
{
    [Authorize(Roles = "Fournisseur")]
    public class TarifSeancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarifSeancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TarifSeances
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var list = await _context.TarifsSeance
                .Include(t => t.Seance)!.ThenInclude(s => s.Film)
                .Where(t => t.Seance!.FournisseurId == userId)
                .OrderByDescending(t => t.Seance!.HeureDebut)
                .ToListAsync();

            return View(list);
        }

        // GET: TarifSeances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var tarifSeance = await _context.TarifsSeance
                .Include(t => t.Seance)!.ThenInclude(s => s.Film)
                .Where(t => t.Seance!.FournisseurId == userId)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tarifSeance == null) return NotFound();
            return View(tarifSeance);
        }

        // GET: TarifSeances/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var seances = _context.Seances
                .Where(s => s.FournisseurId == userId)
                .Include(s => s.Film)
                .OrderByDescending(s => s.HeureDebut)
                .Select(s => new { s.Id, Titre = s.Film!.Title + " - " + s.HeureDebut.ToString("g") })
                .ToList();

            ViewData["SeanceId"] = new SelectList(seances, "Id", "Titre");
            return View();
        }

        // POST: TarifSeances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeanceId,Categorie,Prix")] TarifSeance tarifSeance)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var ok = await _context.Seances.AnyAsync(s => s.Id == tarifSeance.SeanceId && s.FournisseurId == userId);
            if (!ok) ModelState.AddModelError("SeanceId", "Séance invalide.");

            if (!ModelState.IsValid)
            {
                var seances = _context.Seances
                    .Where(s => s.FournisseurId == userId)
                    .Include(s => s.Film)
                    .Select(s => new { s.Id, Titre = s.Film!.Title + " - " + s.HeureDebut.ToString("g") })
                    .ToList();
                ViewData["SeanceId"] = new SelectList(seances, "Id", "Titre", tarifSeance.SeanceId);
                return View(tarifSeance);
            }

            _context.Add(tarifSeance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: TarifSeances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var tarifSeance = await _context.TarifsSeance
                .Include(t => t.Seance)
                .Where(t => t.Seance!.FournisseurId == userId)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarifSeance == null) return NotFound();

            var seances = _context.Seances
                .Where(s => s.FournisseurId == userId)
                .Include(s => s.Film)
                .Select(s => new { s.Id, Titre = s.Film!.Title + " - " + s.HeureDebut.ToString("g") })
                .ToList();
            ViewData["SeanceId"] = new SelectList(seances, "Id", "Titre", tarifSeance.SeanceId);

            return View(tarifSeance);
        }

        // POST: TarifSeances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeanceId,Categorie,Prix")] TarifSeance tarifSeance)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id != tarifSeance.Id) return NotFound();

            var ok = await _context.Seances.AnyAsync(s => s.Id == tarifSeance.SeanceId && s.FournisseurId == userId);
            if (!ok) ModelState.AddModelError("SeanceId", "Séance invalide.");

            if (!ModelState.IsValid)
            {
                var seances = _context.Seances
                    .Where(s => s.FournisseurId == userId)
                    .Include(s => s.Film)
                    .Select(s => new { s.Id, Titre = s.Film!.Title + " - " + s.HeureDebut.ToString("g") })
                    .ToList();
                ViewData["SeanceId"] = new SelectList(seances, "Id", "Titre", tarifSeance.SeanceId);
                return View(tarifSeance);
            }

            try
            {
                _context.Update(tarifSeance);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TarifsSeance.Any(e => e.Id == tarifSeance.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TarifSeances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var tarifSeance = await _context.TarifsSeance
                .Include(t => t.Seance)!.ThenInclude(s => s.Film)
                .Where(t => t.Seance!.FournisseurId == userId)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tarifSeance == null) return NotFound();
            return View(tarifSeance);
        }

        // POST: TarifSeances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var tarifSeance = await _context.TarifsSeance
                .Include(t => t.Seance)
                .Where(t => t.Seance!.FournisseurId == userId)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarifSeance != null) _context.TarifsSeance.Remove(tarifSeance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
