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
    public class ActiviteitenController : Controller
    {
        private readonly TegoareContext _context;

        public ActiviteitenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Activiteiten
        public async Task<IActionResult> Index()
        {
            var tegoareContext = _context.Activiteiten.Include(a => a.Ontmoetingsplaats).Include(a => a.Publicatiedatum).Include(a => a.Uiterste_inschrijfdatum);
            return View(await tegoareContext.ToListAsync());
        }

        // GET: Activiteiten/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activiteit = await _context.Activiteiten
                .Include(a => a.Ontmoetingsplaats)
                .Include(a => a.Publicatiedatum)
                .Include(a => a.Uiterste_inschrijfdatum)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activiteit == null)
            {
                return NotFound();
            }

            return View(activiteit);
        }

        // GET: Activiteiten/Create
        public IActionResult Create()
        {
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam");
            ViewData["Id_publicatiedatum"] = new SelectList(_context.Tijdstippen, "Id", "Datum");
            ViewData["Id_uiterste_inschrijfdatum"] = new SelectList(_context.Tijdstippen, "Id", "Datum");
            return View();
        }

        // POST: Activiteiten/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naam,Omschrijving,Id_publicatiedatum,Id_uiterste_inschrijfdatum,Prijs,Max_inschrijvingen,Id_ontmoetingsplaats")] Activiteit activiteit)
        {
            if (ModelState.IsValid)
            {
                activiteit.Id = Guid.NewGuid();
                _context.Add(activiteit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam", activiteit.Id_ontmoetingsplaats);
            ViewData["Id_publicatiedatum"] = new SelectList(_context.Tijdstippen, "Id", "Datum", activiteit.Id_publicatiedatum);
            ViewData["Id_uiterste_inschrijfdatum"] = new SelectList(_context.Tijdstippen, "Id", "Datum", activiteit.Id_uiterste_inschrijfdatum);
            return View(activiteit);
        }

        // GET: Activiteiten/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activiteit = await _context.Activiteiten.FindAsync(id);
            if (activiteit == null)
            {
                return NotFound();
            }
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam", activiteit.Id_ontmoetingsplaats);
            ViewData["Id_publicatiedatum"] = new SelectList(_context.Tijdstippen, "Id", "Datum", activiteit.Id_publicatiedatum);
            ViewData["Id_uiterste_inschrijfdatum"] = new SelectList(_context.Tijdstippen, "Id", "Datum", activiteit.Id_uiterste_inschrijfdatum);
            return View(activiteit);
        }

        // POST: Activiteiten/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Naam,Omschrijving,Id_publicatiedatum,Id_uiterste_inschrijfdatum,Prijs,Max_inschrijvingen,Id_ontmoetingsplaats")] Activiteit activiteit)
        {
            if (id != activiteit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activiteit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActiviteitExists(activiteit.Id))
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
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam", activiteit.Id_ontmoetingsplaats);
            ViewData["Id_publicatiedatum"] = new SelectList(_context.Tijdstippen, "Id", "Datum", activiteit.Id_publicatiedatum);
            ViewData["Id_uiterste_inschrijfdatum"] = new SelectList(_context.Tijdstippen, "Id", "Datum", activiteit.Id_uiterste_inschrijfdatum);
            return View(activiteit);
        }

        // GET: Activiteiten/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activiteit = await _context.Activiteiten
                .Include(a => a.Ontmoetingsplaats)
                .Include(a => a.Publicatiedatum)
                .Include(a => a.Uiterste_inschrijfdatum)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activiteit == null)
            {
                return NotFound();
            }

            return View(activiteit);
        }

        // POST: Activiteiten/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var activiteit = await _context.Activiteiten.FindAsync(id);
            _context.Activiteiten.Remove(activiteit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActiviteitExists(Guid id)
        {
            return _context.Activiteiten.Any(e => e.Id == id);
        }
    }
}
