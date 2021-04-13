using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;
using TegoareWeb.ViewModels;

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
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            int? pageSize)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NaamSortParm"] = sortOrder == "naam_asc" ? "naam_desc" : "naam_asc";
            ViewData["OmschrijvingSortParm"] = sortOrder == "omschrijving_asc" ? "omschrijving_desc" : "omschrijving_asc";
            ViewData["OntmoetingsplaatsSortParm"] = sortOrder == "plaats_asc" ? "plaats_desc" : "plaats_asc";
            ViewData["ActiviteitendatumSortParm"] = sortOrder == "datum_asc" ? "datum_desc" : "datum_asc";
            ViewData["PublicatiedatumSortParm"] = sortOrder == "publicatie_asc" ? "publicatie_desc" : "publicatie_asc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var tegoareContext = _context.Activiteiten.Include(a => a.Ontmoetingsplaats);
            var activiteiten = from a in _context.Activiteiten.Include(a => a.Ontmoetingsplaats)
                               select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                activiteiten = activiteiten.Where(a => a.Naam.Contains(searchString)
                                       || a.Omschrijving.Contains(searchString)
                                       || a.Ontmoetingsplaats.Plaatsnaam.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "naam_asc":
                    activiteiten = activiteiten.OrderBy(a => a.Naam)
                        .ThenByDescending(a => a.Activiteitendatum);
                    break;
                case "naam_desc":
                    activiteiten = activiteiten.OrderByDescending(a => a.Naam)
                        .ThenByDescending(a => a.Activiteitendatum);
                    break;
                case "omschrijving_asc":
                    activiteiten = activiteiten.OrderBy(a => a.Omschrijving)
                        .ThenByDescending(a => a.Activiteitendatum);
                    break;
                case "omschrijving_desc":
                    activiteiten = activiteiten.OrderByDescending(a => a.Omschrijving)
                        .ThenByDescending(a => a.Activiteitendatum);
                    break;
                case "plaats_asc":
                    activiteiten = activiteiten.OrderBy(a => a.Ontmoetingsplaats)
                        .ThenByDescending(a => a.Activiteitendatum);
                    break;
                case "plaats_desc":
                    activiteiten = activiteiten.OrderByDescending(a => a.Ontmoetingsplaats)
                        .ThenByDescending(a => a.Activiteitendatum);
                    break;
                case "datum_asc":
                    activiteiten = activiteiten.OrderBy(a => a.Activiteitendatum)
                        .ThenBy(a => a.Naam);
                    break;
                case "datum_desc":
                    activiteiten = activiteiten.OrderByDescending(a => a.Activiteitendatum)
                        .ThenBy(a => a.Naam);
                    break;
                case "publicatie_asc":
                    activiteiten = activiteiten.OrderBy(a => a.Publicatiedatum)
                        .ThenByDescending(a => a.Activiteitendatum);
                    break;
                case "publicatie_desc":
                    activiteiten = activiteiten.OrderByDescending(a => a.Publicatiedatum)
                        .ThenByDescending(a => a.Activiteitendatum);
                    break;
                default:
                    activiteiten = activiteiten.OrderByDescending(a => a.Activiteitendatum)
                        .ThenBy(a => a.Naam);
                    break;
            }

            return View(await PaginatedList<Activiteit>.CreateAsync(activiteiten, pageNumber ?? 1, pageSize ?? 10));
        }

        // GET: Activiteiten/Create
        public IActionResult Create()
        {
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam");
            return View();
        }
        // POST: Activiteiten/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naam,Omschrijving,Publicatiedatum,Uiterste_inschrijfdatum,Prijs,Max_inschrijvingen,Id_ontmoetingsplaats,Activiteitendatum,Beginuur,Einduur")] Activiteit activiteit)
        {
            if (ModelState.IsValid)
            {
                activiteit.Id = Guid.NewGuid();
                NormalizePrijs(activiteit);
                _context.Add(activiteit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam", activiteit.Id_ontmoetingsplaats);
            return View(activiteit);
        }

        // GET: Activiteiten/Copy/5
        public async Task<IActionResult> Copy(Guid? id)
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
            return View(activiteit);
        }

        // POST: Activiteiten/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Naam,Omschrijving,Publicatiedatum,Uiterste_inschrijfdatum,Prijs,Max_inschrijvingen,Id_ontmoetingsplaats,Activiteitendatum,Beginuur,Einduur")] Activiteit activiteit)
        {
            if (id != activiteit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    NormalizePrijs(activiteit);
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
                .AsNoTracking()
                .Include(a => a.Ontmoetingsplaats)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activiteit == null)
            {
                return NotFound();
            }

           activiteit.AantalInschrijvingen = _context.Inschrijvingen
                .Where(i => i.Id_Activiteit == id)
                .Count();

            return View(activiteit);
        }

        // POST: Activiteiten/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var activiteit = await _context.Activiteiten.FindAsync(id);
            if (activiteit == null)
            {
                return NotFound();
            }

            var inschrijvingen = _context.Inschrijvingen.Where(i => i.Id_Activiteit == id);

            _context.Inschrijvingen.RemoveRange(inschrijvingen);
            _context.Activiteiten.Remove(activiteit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActiviteitExists(Guid id)
        {
            return _context.Activiteiten.Any(e => e.Id == id);
        }

        private void NormalizePrijs(Activiteit activiteit)
        {
            if (activiteit.Prijs != null)
            {
                activiteit.Prijs = activiteit.Prijs.Replace('.', ',');
                if (!activiteit.Prijs.Contains(','))
                {
                    activiteit.Prijs += ",00";
                }
            }
        }
    }
}
