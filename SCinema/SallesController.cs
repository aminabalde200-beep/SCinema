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
    public class SallesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SallesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Salles
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var list = await _context.Salles
                .Where(s => s.FournisseurId == userId)
                .OrderBy(s => s.NumeroSalle)
                .ToListAsync();
            return View(list);
        }

        // GET: Salles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var salle = await _context.Salles
                .Where(s => s.FournisseurId == userId)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salle == null) return NotFound();

            return View(salle);
        }

        // GET: Salles/Create
        public IActionResult Create()
        {
            // pour ne pas casser la vue scaffold : une liste contenant uniquement l'utilisateur courant
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ViewData["FournisseurId"] = new SelectList(new[] { new { Id = userId } }, "Id", "Id", userId);
            return View();
        }

        // POST: Salles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumeroSalle,Capacite,FournisseurId")] Salle salle)
        {
            if (!ModelState.IsValid)
            {
                var userIdForm = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                ViewData["FournisseurId"] = new SelectList(new[] { new { Id = userIdForm } }, "Id", "Id", userIdForm);
                return View(salle);
            }

            // on force le propriétaire
            salle.FournisseurId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            _context.Add(salle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Salles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var salle = await _context.Salles
                .Where(s => s.FournisseurId == userId)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (salle == null) return NotFound();

            ViewData["FournisseurId"] = new SelectList(new[] { new { Id = userId } }, "Id", "Id", userId);
            return View(salle);
        }

        // POST: Salles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroSalle,Capacite,FournisseurId")] Salle salle)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id != salle.Id) return NotFound();

            // vérifier la propriété
            var exists = await _context.Salles.AnyAsync(s => s.Id == id && s.FournisseurId == userId);
            if (!exists) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["FournisseurId"] = new SelectList(new[] { new { Id = userId } }, "Id", "Id", userId);
                return View(salle);
            }

            salle.FournisseurId = userId; // on garde le proprio
            _context.Update(salle);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Salles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (id == null) return NotFound();

            var salle = await _context.Salles
                .Where(s => s.FournisseurId == userId)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salle == null) return NotFound();

            return View(salle);
        }

        // POST: Salles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var salle = await _context.Salles
                .Where(s => s.FournisseurId == userId)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salle != null) _context.Salles.Remove(salle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
