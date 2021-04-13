using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.Controllers
{
    public class OntmoetingsplaatsenController : Controller
    {
        private readonly TegoareContext _context;

        public OntmoetingsplaatsenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Ontmoetingsplaatsen
        public async Task<IActionResult> Index(string searchString = null)
        {
            var query = _context.Ontmoetingsplaatsen
            .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(o => o.Plaatsnaam.ToLower().Contains(searchString)
                                        || o.Gemeente.ToLower().Contains(searchString)
                                        || o.Straatnaam.ToLower().Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            var ontmoetingsplaatsen = await query
                .OrderBy(o => o.Plaatsnaam).ToListAsync();

            return View(ontmoetingsplaatsen);
        }

        // GET: Ontmoetingsplaatsen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ontmoetingsplaatsen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Plaatsnaam,Straatnaam,Straatnummer,Postcode,Gemeente")] Ontmoetingsplaats ontmoetingsplaats)
        {
            if (ModelState.IsValid)
            {
                ontmoetingsplaats.Id = Guid.NewGuid();
                _context.Add(ontmoetingsplaats);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ontmoetingsplaats);
        }

        // GET: Ontmoetingsplaatsen/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ontmoetingsplaats = await _context.Ontmoetingsplaatsen.FindAsync(id);
            if (ontmoetingsplaats == null)
            {
                return NotFound();
            }
            return View(ontmoetingsplaats);
        }

        // POST: Ontmoetingsplaatsen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Plaatsnaam,Straatnaam,Straatnummer,Postcode,Gemeente")] Ontmoetingsplaats ontmoetingsplaats)
        {
            if (id != ontmoetingsplaats.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ontmoetingsplaats);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OntmoetingsplaatsExists(ontmoetingsplaats.Id))
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
            return View(ontmoetingsplaats);
        }

        private bool OntmoetingsplaatsExists(Guid id)
        {
            return _context.Ontmoetingsplaatsen.Any(e => e.Id == id);
        }
    }
}
