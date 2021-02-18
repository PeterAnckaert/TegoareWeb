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
        private readonly TegoareContext _context;

        public LedenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Leden
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AchternaamSortParm"] = sortOrder == "achternaam_asc" ? "achternaam_desc" : "achternaam_asc";
            ViewData["VoornaamSortParm"] = sortOrder == "voornaam_asc" ? "voornaam_desc" : "voornaam_asc";
            ViewData["GeboorteDatumSortParm"] = sortOrder == "geboorte_asc" ? "geboorte_desc" : "geboorte_asc";
            ViewData["StraatnaamSortParm"] = sortOrder == "straatnaam_asc" ? "straatnaam_desc" : "straatnaam_asc";
            ViewData["StraatnummerSortParm"] = sortOrder == "straatnummer_asc" ? "straatnummer_desc" : "straatnummer_asc";
            ViewData["PostcodeSortParm"] = sortOrder == "postcode_asc" ? "postcode_desc" : "postcode_asc";
            ViewData["GemeenteSortParm"] = sortOrder == "gemeente_asc" ? "gemeente_desc" : "gemeente_asc";
            ViewData["TelVastSortParm"] = sortOrder == "telvast_asc" ? "telvast_desc" : "telvast_asc";
            ViewData["TelGSMSortParm"] = sortOrder == "telgsm_asc" ? "telgsm_desc" : "telgsm_asc";
            ViewData["EmailSortParm"] = sortOrder == "email_asc" ? "email_desc" : "email_asc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var leden = from l in _context.Leden
                           select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                leden = leden.Where(l => l.Achternaam.Contains(searchString)
                                       || l.Voornaam.Contains(searchString)
                                       || l.Straatnaam.Contains(searchString)
                                       || l.Straatnummer.Contains(searchString)
                                       || l.Gemeente.Contains(searchString)
                                       || l.Email.Contains(searchString)
                                       || l.Telefoon_vast.Contains(searchString)
                                       || l.Telefoon_GSM.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "achternaam_asc":
                    leden = leden.OrderBy(l => l.Achternaam)
                        .ThenBy(l => l.Voornaam);
                    break;
                case "achternaam_desc":
                    leden = leden.OrderByDescending(l => l.Achternaam)
                        .ThenByDescending(l => l.Voornaam);
                    break;
                case "voornaam_asc":
                    leden = leden.OrderBy(l => l.Voornaam)
                        .ThenBy(l => l.Achternaam);
                    break;
                case "voornaam_desc":
                    leden = leden.OrderByDescending(l => l.Voornaam)
                        .ThenByDescending(l => l.Achternaam);
                    break;
                case "geboorte_asc":
                    leden = leden.OrderBy(l => l.Geboortedatum);
                    break;
                case "geboorte_desc":
                    leden = leden.OrderByDescending(l => l.Geboortedatum);
                    break;
                case "straatnaam_asc":
                    leden = leden.OrderBy(l => l.Straatnaam)
                        .ThenBy(l => l.Straatnummer);
                    break;
                case "straatnaam_desc":
                    leden = leden.OrderByDescending(l => l.Straatnaam)
                        .ThenByDescending(l => l.Straatnummer);
                    break;
                case "straatnummer_asc":
                    leden = leden.OrderBy(l => l.Straatnummer);
                    break;
                case "straatnummer_desc":
                    leden = leden.OrderByDescending(l => l.Straatnummer);
                    break;
                case "postcode_asc":
                    leden = leden.OrderBy(l => l.Postcode)
                        .ThenBy(l => l.Gemeente)
                        .ThenBy(l => l.Straatnummer);
                    break;
                case "postcode_desc":
                    leden = leden.OrderByDescending(l => l.Postcode)
                        .ThenByDescending(l => l.Gemeente)
                        .ThenByDescending(l => l.Straatnummer);
                    break;
                case "gemeente_asc":
                    leden = leden.OrderBy(l => l.Gemeente)
                        .ThenBy(l => l.Straatnaam)
                        .ThenBy(l => l.Straatnummer);
                    break;
                case "gemeente_desc":
                    leden = leden.OrderByDescending(l => l.Gemeente)
                        .ThenByDescending(l => l.Straatnaam)
                        .ThenByDescending(l => l.Straatnummer);
                    break;
                case "telvast_asc":
                    leden = leden.OrderBy(l => l.Telefoon_vast);
                    break;
                case "telvast_desc":
                    leden = leden.OrderByDescending(l => l.Telefoon_vast);
                    break;
                case "telgsm_asc":
                    leden = leden.OrderBy(l => l.Telefoon_GSM);
                    break;
                case "telgsm_desc":
                    leden = leden.OrderByDescending(l => l.Telefoon_GSM);
                    break;
                case "email_asc":
                    leden = leden.OrderBy(l => l.Email);
                    break;
                case "email_desc":
                    leden = leden.OrderByDescending(l => l.Email);
                    break;
                default:
                    leden = leden.OrderBy(l => l.Achternaam)
                        .ThenBy(l=> l.Voornaam);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Lid>.CreateAsync(leden, pageNumber ?? 1, pageSize));
        }

        // GET: Leden/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lid = await _context.Leden
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

            var lid = await _context.Leden.FindAsync(id);
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

            var lid = await _context.Leden
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
            var lid = await _context.Leden.FindAsync(id);
            _context.Leden.Remove(lid);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LidExists(Guid id)
        {
            return _context.Leden.Any(e => e.Id == id);
        }
    }
}
