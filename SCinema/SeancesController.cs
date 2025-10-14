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
    public class SeancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Seances
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var list = await _context.Seances
                .Include(s => s.Film)
                .Include(s => s.Salle)
                .Where(s => s.FournisseurId == userId)
                .OrderByDescending(s => s.HeureDebut)
                .ToListAsync();

            return View(list);
        }

        // GET: Seances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var seance = await _context.Seances
                .Include(s => s.Film)
                .Include(s => s.Salle)
                .Where(s => s.FournisseurId == userId)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (seance == null) return NotFound();
            return View(seance);
        }

        // GET: Seances/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Films (tu peux mettre Title si tu veux, garde Id si tes vues attendent ça)
            ViewData["FilmId"] = new SelectList(_context.Films.OrderBy(f => f.Title), "Id", "Title");

            // Salles du fournisseur courant uniquement
            var salles = _context.Salles
                .Where(s => s.FournisseurId == userId)
                .OrderBy(s => s.NumeroSalle)
                .ToList();
            ViewData["SalleId"] = new SelectList(salles, "Id", "NumeroSalle");

            // FournisseurId : on met l’utilisateur courant (pour ne pas casser la vue scaffold)
            ViewData["FournisseurId"] = new SelectList(new[] { new { Id = userId } }, "Id", "Id", userId);

            return View();
        }

        // POST: Seances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmId,SalleId,FournisseurId,DateHeureDebut,DateHeureFin,Prix,FormatFilm,Langue")] Seance seance)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // sécurité : la salle doit appartenir au fournisseur
            var salleOk = await _context.Salles.AnyAsync(s => s.Id == seance.SalleId && s.FournisseurId == userId);
            if (!salleOk) ModelState.AddModelError("SalleId", "Salle invalide.");

            if (!ModelState.IsValid)
            {
                ViewData["FilmId"] = new SelectList(_context.Films.OrderBy(f => f.Title), "Id", "Title", seance.FilmId);
                ViewData["SalleId"] = new SelectList(_context.Salles.Where(s => s.FournisseurId == userId), "Id", "NumeroSalle", seance.SalleId);
                ViewData["FournisseurId"] = new SelectList(new[] { new { Id = userId } }, "Id", "Id", userId);
                return View(seance);
            }

            // on force le propriétaire
            seance.FournisseurId = userId;

            _context.Add(seance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Seances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var seance = await _context.Seances
                .Where(s => s.FournisseurId == userId)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (seance == null) return NotFound();

            ViewData["FilmId"] = new SelectList(_context.Films.OrderBy(f => f.Title), "Id", "Title", seance.FilmId);
            ViewData["SalleId"] = new SelectList(_context.Salles.Where(s => s.FournisseurId == userId), "Id", "NumeroSalle", seance.SalleId);
            ViewData["FournisseurId"] = new SelectList(new[] { new { Id = userId } }, "Id", "Id", userId);

            return View(seance);
        }

        // POST: Seances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FilmId,SalleId,FournisseurId,DateHeureDebut,DateHeureFin,Prix,FormatFilm,Langue")] Seance seance)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id != seance.Id) return NotFound();

            var salleOk = await _context.Salles.AnyAsync(s => s.Id == seance.SalleId && s.FournisseurId == userId);
            if (!salleOk) ModelState.AddModelError("SalleId", "Salle invalide.");

            if (!ModelState.IsValid)
            {
                ViewData["FilmId"] = new SelectList(_context.Films.OrderBy(f => f.Title), "Id", "Title", seance.FilmId);
                ViewData["SalleId"] = new SelectList(_context.Salles.Where(s => s.FournisseurId == userId), "Id", "NumeroSalle", seance.SalleId);
                ViewData["FournisseurId"] = new SelectList(new[] { new { Id = userId } }, "Id", "Id", userId);
                return View(seance);
            }

            try
            {
                seance.FournisseurId = userId; // on garde le proprio
                _context.Update(seance);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Seances.Any(e => e.Id == seance.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Seances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var seance = await _context.Seances
                .Include(s => s.Film)
                .Include(s => s.Salle)
                .Where(s => s.FournisseurId == userId)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (seance == null) return NotFound();
            return View(seance);
        }

        // POST: Seances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var seance = await _context.Seances
                .Where(s => s.FournisseurId == userId)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seance != null) _context.Seances.Remove(seance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
