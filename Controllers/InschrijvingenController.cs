using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.Controllers
{
    public class InschrijvingenController : Controller
    {
        private readonly TegoareContext _context;

        public InschrijvingenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Inschrijvingen
        public async Task<IActionResult> Index()
        {
            var tegoareContext = _context.Inschrijving.Include(i => i.Activiteit).Include(i => i.Lid);
            return View(await tegoareContext.ToListAsync());
        }

        // GET: Inschrijvingen/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inschrijving = await _context.Inschrijving
                .Include(i => i.Activiteit)
                .Include(i => i.Lid)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inschrijving == null)
            {
                return NotFound();
            }

            return View(inschrijving);
        }

        // GET: Inschrijvingen/Create
        public IActionResult Create()
        {
            ViewData["Id_Activiteit"] = new SelectList(_context.Activiteiten, "Id", "Naam");
            ViewData["Id_Lid"] = new SelectList(_context.Leden, "Id", "Achternaam");
            return View();
        }

        // POST: Inschrijvingen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_Lid,Id_Activiteit")] Inschrijving inschrijving)
        {
            if (ModelState.IsValid)
            {
                inschrijving.Id = Guid.NewGuid();
                _context.Add(inschrijving);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Activiteit"] = new SelectList(_context.Activiteiten, "Id", "Naam", inschrijving.Id_Activiteit);
            ViewData["Id_Lid"] = new SelectList(_context.Leden, "Id", "Achternaam", inschrijving.Id_Lid);
            return View(inschrijving);
        }

        // GET: Inschrijvingen/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inschrijving = await _context.Inschrijving.FindAsync(id);
            if (inschrijving == null)
            {
                return NotFound();
            }
            ViewData["Id_Activiteit"] = new SelectList(_context.Activiteiten, "Id", "Naam", inschrijving.Id_Activiteit);
            ViewData["Id_Lid"] = new SelectList(_context.Leden, "Id", "Achternaam", inschrijving.Id_Lid);
            return View(inschrijving);
        }

        // POST: Inschrijvingen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Id_Lid,Id_Activiteit")] Inschrijving inschrijving)
        {
            if (id != inschrijving.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inschrijving);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InschrijvingExists(inschrijving.Id))
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
            ViewData["Id_Activiteit"] = new SelectList(_context.Activiteiten, "Id", "Naam", inschrijving.Id_Activiteit);
            ViewData["Id_Lid"] = new SelectList(_context.Leden, "Id", "Achternaam", inschrijving.Id_Lid);
            return View(inschrijving);
        }

        // GET: Inschrijvingen/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inschrijving = await _context.Inschrijving
                .Include(i => i.Activiteit)
                .Include(i => i.Lid)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inschrijving == null)
            {
                return NotFound();
            }

            return View(inschrijving);
        }

        // POST: Inschrijvingen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inschrijving = await _context.Inschrijving.FindAsync(id);
            _context.Inschrijving.Remove(inschrijving);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InschrijvingExists(Guid id)
        {
            return _context.Inschrijving.Any(e => e.Id == id);
        }
    }
}
