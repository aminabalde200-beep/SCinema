using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCinema.Data;
using SCinema.Models;
using System.Diagnostics;

namespace SCinema.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Page d'accueil : on montre 8 films récents (ou ceux du seeder)
        public async Task<IActionResult> Index()
        {
            var films = await _db.Films
                .AsNoTracking()
                .OrderByDescending(f => f.Id)   // ou DateSortie si tu préfères
                .Take(8)
                .ToListAsync();

            return View(films);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
