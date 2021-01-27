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
            return View(await _context.Activiteit.ToListAsync());
        }

        // GET: Activiteiten/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activiteit = await _context.Activiteit
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
            return View();
        }

        // POST: Activiteiten/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naam,Omschrijving,Publicatiedatum,Uiterste_inschrijfdatum,Prijs,Max_inschrijvingen")] Activiteit activiteit)
        {
            if (ModelState.IsValid)
            {
                activiteit.Id = Guid.NewGuid();
                _context.Add(activiteit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activiteit);
        }

        // GET: Activiteiten/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activiteit = await _context.Activiteit.FindAsync(id);
            if (activiteit == null)
            {
                return NotFound();
            }
            return View(activiteit);
        }

        // POST: Activiteiten/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Naam,Omschrijving,Publicatiedatum,Uiterste_inschrijfdatum,Prijs,Max_inschrijvingen")] Activiteit activiteit)
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
            return View(activiteit);
        }

        // GET: Activiteiten/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activiteit = await _context.Activiteit
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
            var activiteit = await _context.Activiteit.FindAsync(id);
            _context.Activiteit.Remove(activiteit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActiviteitExists(Guid id)
        {
            return _context.Activiteit.Any(e => e.Id == id);
        }
    }
}
