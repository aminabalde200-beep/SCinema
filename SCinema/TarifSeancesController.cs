using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SCinema.Data;
using SCinema.Models;

namespace SCinema
{
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
            var applicationDbContext = _context.TarifsSeance.Include(t => t.Seance);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TarifSeances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarifSeance = await _context.TarifsSeance
                .Include(t => t.Seance)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarifSeance == null)
            {
                return NotFound();
            }

            return View(tarifSeance);
        }

        // GET: TarifSeances/Create
        public IActionResult Create()
        {
            ViewData["SeanceId"] = new SelectList(_context.Seances, "Id", "Id");
            return View();
        }

        // POST: TarifSeances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeanceId,Categorie,Prix")] TarifSeance tarifSeance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarifSeance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeanceId"] = new SelectList(_context.Seances, "Id", "Id", tarifSeance.SeanceId);
            return View(tarifSeance);
        }

        // GET: TarifSeances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarifSeance = await _context.TarifsSeance.FindAsync(id);
            if (tarifSeance == null)
            {
                return NotFound();
            }
            ViewData["SeanceId"] = new SelectList(_context.Seances, "Id", "Id", tarifSeance.SeanceId);
            return View(tarifSeance);
        }

        // POST: TarifSeances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeanceId,Categorie,Prix")] TarifSeance tarifSeance)
        {
            if (id != tarifSeance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarifSeance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarifSeanceExists(tarifSeance.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeanceId"] = new SelectList(_context.Seances, "Id", "Id", tarifSeance.SeanceId);
            return View(tarifSeance);
        }

        // GET: TarifSeances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarifSeance = await _context.TarifsSeance
                .Include(t => t.Seance)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarifSeance == null)
            {
                return NotFound();
            }

            return View(tarifSeance);
        }

        // POST: TarifSeances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarifSeance = await _context.TarifsSeance.FindAsync(id);
            if (tarifSeance != null)
            {
                _context.TarifsSeance.Remove(tarifSeance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarifSeanceExists(int id)
        {
            return _context.TarifsSeance.Any(e => e.Id == id);
        }
    }
}
