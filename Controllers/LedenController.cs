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
    public class LedenController : Controller
    {
        private readonly LidContext _context;

        public LedenController(LidContext context)
        {
            _context = context;
        }

        // GET: Leden
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lid.ToListAsync());
        }

        // GET: Leden/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lid = await _context.Lid
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lid == null)
            {
                return NotFound();
            }

            return View(lid);
        }

        // GET: Leden/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leden/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Achternaam,Voornaam,Geboortedatum,Straatnaam,Straatnummer,Postcode,Gemeente,Telefoon_vast,Telefoon_GSM,Email")] Lid lid)
        {
            if (ModelState.IsValid)
            {
                lid.Id = Guid.NewGuid();
                _context.Add(lid);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lid);
        }

        // GET: Leden/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lid = await _context.Lid.FindAsync(id);
            if (lid == null)
            {
                return NotFound();
            }
            return View(lid);
        }

        // POST: Leden/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Achternaam,Voornaam,Geboortedatum,Straatnaam,Straatnummer,Postcode,Gemeente,Telefoon_vast,Telefoon_GSM,Email")] Lid lid)
        {
            if (id != lid.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LidExists(lid.Id))
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
            return View(lid);
        }

        // GET: Leden/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lid = await _context.Lid
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lid == null)
            {
                return NotFound();
            }

            return View(lid);
        }

        // POST: Leden/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lid = await _context.Lid.FindAsync(id);
            _context.Lid.Remove(lid);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LidExists(Guid id)
        {
            return _context.Lid.Any(e => e.Id == id);
        }
    }
}
