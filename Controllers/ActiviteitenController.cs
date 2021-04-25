using Microsoft.AspNetCore.Http;
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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if(actionResult != null)
            {
                return actionResult;
            }

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

            var activiteiten = _context.Activiteiten
                .AsNoTracking()
                .Include(a => a.Ontmoetingsplaats)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                activiteiten = activiteiten.Where(a => a.Naam.Contains(searchString)
                                       || a.Omschrijving.Contains(searchString)
                                       || a.Ontmoetingsplaats.Plaatsnaam.Contains(searchString));
            }

            activiteiten = sortOrder switch
            {
                "naam_asc" => activiteiten.OrderBy(a => a.Naam)
                                       .ThenByDescending(a => a.Activiteitendatum),
                "naam_desc" => activiteiten.OrderByDescending(a => a.Naam)
                                        .ThenByDescending(a => a.Activiteitendatum),
                "omschrijving_asc" => activiteiten.OrderBy(a => a.Omschrijving)
                                        .ThenByDescending(a => a.Activiteitendatum),
                "omschrijving_desc" => activiteiten.OrderByDescending(a => a.Omschrijving)
                                        .ThenByDescending(a => a.Activiteitendatum),
                "plaats_asc" => activiteiten.OrderBy(a => a.Ontmoetingsplaats)
                                        .ThenByDescending(a => a.Activiteitendatum),
                "plaats_desc" => activiteiten.OrderByDescending(a => a.Ontmoetingsplaats)
                                        .ThenByDescending(a => a.Activiteitendatum),
                "datum_asc" => activiteiten.OrderBy(a => a.Activiteitendatum)
                                        .ThenBy(a => a.Naam),
                "datum_desc" => activiteiten.OrderByDescending(a => a.Activiteitendatum)
                                        .ThenBy(a => a.Naam),
                "publicatie_asc" => activiteiten.OrderBy(a => a.Publicatiedatum)
                                        .ThenByDescending(a => a.Activiteitendatum),
                "publicatie_desc" => activiteiten.OrderByDescending(a => a.Publicatiedatum)
                                        .ThenByDescending(a => a.Activiteitendatum),
                _ => activiteiten.OrderByDescending(a => a.Activiteitendatum)
                                        .ThenBy(a => a.Naam),
            };
            return View(await PaginatedList<Activiteit>.CreateAsync(activiteiten, pageNumber ?? 1, pageSize ?? 10));
        }

        // GET: Activiteiten/Create
        public IActionResult Create()
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

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

        private static void NormalizePrijs(Activiteit activiteit)
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

        private IActionResult CheckIfNotAllowed()
        {
            if (!CredentialBeheerder.Check(null, TempData, _context))
            {
                return RedirectToAction("LogIn", "Account");
            }

            string[] roles = { "activiteitenmanager" };
            if (!CredentialBeheerder.Check(roles, TempData, _context))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            return null;
        }
    }
}
