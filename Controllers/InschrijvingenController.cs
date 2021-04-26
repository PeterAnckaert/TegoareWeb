using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;
using TegoareWeb.ViewModels;

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
        public IActionResult Index()
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var tegoareContext = _context.Inschrijvingen
                .AsNoTracking()
                .GroupBy(i => i.Id_Activiteit)
                .Select(g => new { Id_Activiteit = g.Key, Count = g.Count() }).ToList();

            List<Activiteit> lijstActiviteiten = new();

            foreach (var item in tegoareContext)
            {
                var activiteit = _context.Activiteiten.FirstOrDefault(a => a.Id == item.Id_Activiteit);
                activiteit.AantalInschrijvingen = item.Count;
                lijstActiviteiten.Add(activiteit);
            }

            lijstActiviteiten = lijstActiviteiten.OrderByDescending(a => a.Activiteitendatum)
                .ThenBy(a => a.Naam).ToList();

            return View(lijstActiviteiten);
        }

        // GET: Inschrijvingen/Details/5
        public async Task<IActionResult> Details(Guid? id)
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

            var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                .AsNoTracking()
                .Where(i => i.Id_Activiteit == id)
                .Include(i => i.Lid)
                .ToListAsync();

            if (inschrijvingenVoorActiviteit == null)
            {
                return NotFound();
            }

            var activiteit = _context.Activiteiten
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Include(a => a.Ontmoetingsplaats)
                .FirstOrDefault();

            if (activiteit == null)
            {
                return NotFound();
            }

            var ingeschrevenLeden = new List<Lid>();

            foreach (var inschrijving in inschrijvingenVoorActiviteit)
            {
                ingeschrevenLeden.Add(inschrijving.Lid);
            }

            activiteit.AantalInschrijvingen = inschrijvingenVoorActiviteit.Count;

            var leden = await _context.Leden
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam)
                .ToListAsync();

            var model = new InschrijvingViewModel()
            {
                AlleLeden = leden,
                IngeschrevenLeden = ingeschrevenLeden,
                Activiteit = activiteit,
                Id_Activiteit = activiteit.Id
            };

            return View(model);
        }

        // GET: Inschrijvingen/Create
        public IActionResult Create()
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            ViewData["Id_Activiteit"] = new SelectList(_context.Activiteiten
                .OrderByDescending(a => a.Activiteitendatum).ThenBy(a => a.Naam),
                "Id", "ActiviteitendatumEnNaam");
            ViewData["Id_Lid"] = new SelectList(_context.Leden
                .OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam),
                "Id", "VolledigeNaam");
            return View();
        }

        // POST: Inschrijvingen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_Lid,Id_Activiteit")] Inschrijving inschrijving)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (ModelState.IsValid)
            {
                inschrijving.Id = Guid.NewGuid();
                _context.Add(inschrijving);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Activiteit"] = new SelectList(_context.Activiteiten
                .OrderByDescending(a => a.Activiteitendatum).ThenBy(a=>a.Naam),
                "Id", "Naam", inschrijving.Id_Activiteit);
            ViewData["Id_Lid"] = new SelectList(_context.Leden
                .OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam),
                "Id", "VolledigeNaam", inschrijving.Id_Lid);
            return View(inschrijving);
        }

        // GET: Inschrijvingen/Edit/5
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

            var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                .AsNoTracking()
                .Where(i => i.Id_Activiteit == id)
                .Include(i => i.Lid)
                .ToListAsync();

            if (inschrijvingenVoorActiviteit == null)
            {
                return NotFound();
            }

            var activiteit = _context.Activiteiten
                .AsNoTracking()
                .Where(a => a.Id == id)
                .FirstOrDefault();

            if (activiteit == null)
            {
                return NotFound();
            }

            var ingeschrevenLeden = inschrijvingenVoorActiviteit.Select(l => l.Lid).ToList();

            activiteit.AantalInschrijvingen = inschrijvingenVoorActiviteit.Count;

            var leden = await _context.Leden
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam)
                .ToListAsync();

            var model = new InschrijvingViewModel()
            {
                AlleLeden = leden,
                IngeschrevenLeden = ingeschrevenLeden,
                Activiteit = activiteit,
                Id_Activiteit = activiteit.Id
            };

            return View(model);
        }

        // POST: Inschrijvingen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,Guid id_Activiteit, List<Guid> ledenLijst)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (id != id_Activiteit)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                        .Where(i => i.Id_Activiteit == id)
                        .ToListAsync();

                    _context.RemoveRange(inschrijvingenVoorActiviteit);

                    foreach(Guid id_lid in ledenLijst)
                    {
                        Inschrijving inschrijving = new()
                        {
                            Id = new Guid(),
                            Id_Activiteit = id,
                            Id_Lid = id_lid
                        };
                        _context.Add(inschrijving);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Inschrijvingen/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                .AsNoTracking()
                .Where(i => i.Id_Activiteit == id)
                .Include(i => i.Lid)
                .ToListAsync();

            if (inschrijvingenVoorActiviteit == null)
            {
                return NotFound();
            }

            var activiteit = _context.Activiteiten
                .AsNoTracking()
                .Where(a => a.Id == id)
                .FirstOrDefault();

            if (activiteit == null)
            {
                return NotFound();
            }

            var ingeschrevenLeden = new List<Lid>();

            foreach (var inschrijving in inschrijvingenVoorActiviteit)
            {
                ingeschrevenLeden.Add(inschrijving.Lid);
            }

            activiteit.AantalInschrijvingen = inschrijvingenVoorActiviteit.Count;

            var leden = await _context.Leden
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam)
                .ToListAsync();

            var model = new InschrijvingViewModel()
            {
                AlleLeden = leden,
                IngeschrevenLeden = ingeschrevenLeden,
                Activiteit = activiteit,
                Id_Activiteit = activiteit.Id
            };

            return View(model);
        }

        // POST: Inschrijvingen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                .Where(i => i.Id_Activiteit == id)
                .ToListAsync();

            _context.RemoveRange(inschrijvingenVoorActiviteit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InschrijvingExists(Guid id)
        {
            return _context.Inschrijvingen.Any(e => e.Id == id);
        }

        private IActionResult CheckIfNotAllowed()
        {
            if (!CredentialBeheerder.Check(null, TempData, _context))
            {
                return RedirectToAction("LogIn", "Account");
            }

            string[] roles = { "ledenmanager" };
            if (!CredentialBeheerder.Check(roles, TempData, _context))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            return null;
        }
    }
}
