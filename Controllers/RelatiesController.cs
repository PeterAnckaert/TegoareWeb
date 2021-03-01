using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.Controllers
{
    public class RelatiesController : Controller
    {
        private readonly TegoareContext _context;

        public RelatiesController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Relaties
        public async Task<IActionResult> Index()
        {
            var tegoareContext = _context.Leden.Include(l => l.Relaties1).Include(l => l.Relaties2);
            var leden = await tegoareContext.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam).ToListAsync(); ;
            return View(leden);
        }

        // GET: Relaties/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatie = await _context.Relaties
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relatie == null)
            {
                return NotFound();
            }

            return View(relatie);
        }

        // GET: Relaties/Create
        public IActionResult Create()
        {
            ViewData["Id_Groep"] = new SelectList(_context.Groepen.OrderBy(g =>g.Rol), "Id", "Rol");
            ViewData["Id_Lid1"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam");
            ViewData["Id_Lid2"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam");
            return View();
        }

        // POST: Relaties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_Lid1,Id_Groep,Id_Lid2")] Relatie relatie)
        {
            if (ModelState.IsValid)
            {
                relatie.Id = Guid.NewGuid();
                _context.Add(relatie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Groep"] = new SelectList(_context.Groepen.OrderBy(g => g.Rol), "Id", "Rol", relatie.Id_Groep);
            ViewData["Id_Lid1"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid1);
            ViewData["Id_Lid2"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid2);
            return View(relatie);
        }

        // GET: Relaties/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatie = await _context.Relaties.FindAsync(id);
            if (relatie == null)
            {
                return NotFound();
            }
            ViewData["Id_Groep"] = new SelectList(_context.Groepen.OrderBy(g => g.Rol), "Id", "Rol", relatie.Id_Groep);
            ViewData["Id_Lid1"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid1);
            ViewData["Id_Lid2"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid2);
            return View(relatie);
        }

        // POST: Relaties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Id_Lid1,Id_Groep,Id_Lid2")] Relatie relatie)
        {
            if (id != relatie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relatie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelatieExists(relatie.Id))
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
            ViewData["Id_Groep"] = new SelectList(_context.Groepen.OrderBy(g => g.Rol), "Id", "Rol", relatie.Id_Groep);
            ViewData["Id_Lid1"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid1);
            ViewData["Id_Lid2"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid2);
            return View(relatie);
        }

        // GET: Relaties/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatie = await _context.Relaties
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relatie == null)
            {
                return NotFound();
            }

            return View(relatie);
        }

        // POST: Relaties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var relatie = await _context.Relaties.FindAsync(id);
            _context.Relaties.Remove(relatie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelatieExists(Guid id)
        {
            return _context.Relaties.Any(e => e.Id == id);
        }
    }
}
