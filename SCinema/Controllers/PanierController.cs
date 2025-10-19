using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCinema.Data;
using SCinema.Models;

namespace SCinema
{
    [Authorize(Roles = "Client")]
    public class PanierController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public PanierController(ApplicationDbContext ctx) => _ctx = ctx;

        // Récupère ou crée le panier du client courant
        private async Task<Panier> GetOrCreateCartAsync(string userId)
        {
            var cart = await _ctx.Paniers
                .Include(p => p.Items)
                    .ThenInclude(i => i.Seance)
                .FirstOrDefaultAsync(p => p.ClientId == userId);

            if (cart == null)
            {
                cart = new Panier { ClientId = userId, Total = 0m };
                _ctx.Paniers.Add(cart);
                await _ctx.SaveChangesAsync();
            }
            return cart;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var cart = await _ctx.Paniers
                .Include(p => p.Items)
                    .ThenInclude(i => i.Seance!)
                        .ThenInclude(s => s.Film)
                .Include(p => p.Items)
                    .ThenInclude(i => i.Seance!)
                        .ThenInclude(s => s.Salle)
                .FirstOrDefaultAsync(p => p.ClientId == userId);

            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int seanceId, string categorie)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var seance = await _ctx.Seances.FirstOrDefaultAsync(s => s.Id == seanceId);
            if (seance == null) return NotFound();

            if (!System.Enum.TryParse<AgeCategorie>(categorie, out var cat))
            {
                TempData["CartError"] = "Catégorie invalide.";
                return RedirectToAction("Film", "Catalogue", new { id = seance.FilmId });
            }

            var tarif = await _ctx.TarifsSeance
                .Where(t => t.SeanceId == seanceId && t.Categorie == cat) // colonne DB "Categorie"
                .FirstOrDefaultAsync();

            if (tarif == null)
            {
                TempData["CartError"] = "Tarif indisponible pour cette catégorie.";
                return RedirectToAction("Film", "Catalogue", new { id = seance.FilmId });
            }

            var cart = await GetOrCreateCartAsync(userId);

            cart.Items.Add(new PanierItem
            {
                SeanceId = seanceId,
                CategorieAge = cat,          // ✅ on utilise CategorieAge côté modèle
                PrixUnitaire = tarif.Prix
            });

            cart.Total = cart.Items.Sum(i => i.PrixUnitaire);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Remove/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var cart = await _ctx.Paniers
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.ClientId == userId);
            if (cart == null) return RedirectToAction(nameof(Index));

            var item = cart.Items.FirstOrDefault(i => i.Id == id);
            if (item != null) _ctx.PanierItems.Remove(item);

            await _ctx.SaveChangesAsync();

            cart.Total = await _ctx.PanierItems
                .Where(i => i.PanierId == cart.Id)
                .SumAsync(i => (decimal?)i.PrixUnitaire) ?? 0m;

            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var cart = await _ctx.Paniers
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.ClientId == userId);
            if (cart == null) return RedirectToAction(nameof(Index));

            _ctx.PanierItems.RemoveRange(cart.Items);
            cart.Total = 0m;
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
