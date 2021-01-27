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
    public class TijdstippenController : Controller
    {
        private readonly TegoareContext _context;

        public TijdstippenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Tijdstippen
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tijdstippen.ToListAsync());
        }

        // GET: Tijdstippen/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tijdstip = await _context.Tijdstippen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tijdstip == null)
            {
                return NotFound();
            }

            return View(tijdstip);
        }

        // GET: Tijdstippen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tijdstippen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Datum,Begin_uur,Eind_uur")] Tijdstip tijdstip)
        {
            if (ModelState.IsValid)
            {
                tijdstip.Id = Guid.NewGuid();
                _context.Add(tijdstip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tijdstip);
        }

        // GET: Tijdstippen/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tijdstip = await _context.Tijdstippen.FindAsync(id);
            if (tijdstip == null)
            {
                return NotFound();
            }
            return View(tijdstip);
        }

        // POST: Tijdstippen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Datum,Begin_uur,Eind_uur")] Tijdstip tijdstip)
        {
            if (id != tijdstip.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tijdstip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TijdstipExists(tijdstip.Id))
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
            return View(tijdstip);
        }

        // GET: Tijdstippen/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tijdstip = await _context.Tijdstippen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tijdstip == null)
            {
                return NotFound();
            }

            return View(tijdstip);
        }

        // POST: Tijdstippen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tijdstip = await _context.Tijdstippen.FindAsync(id);
            _context.Tijdstippen.Remove(tijdstip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TijdstipExists(Guid id)
        {
            return _context.Tijdstippen.Any(e => e.Id == id);
        }
    }
}
