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

        private readonly string[] _role = { "activiteitenmanager" };

        public OntmoetingsplaatsenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Ontmoetingsplaatsen
        public async Task<IActionResult> Index(string searchString = null)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // haal alle ontmoetingsplaatsen
            // AsNoTracking omdat de db niet aangepast wordt
            var query = _context.Ontmoetingsplaatsen.AsNoTracking();

            // moet er gezocht worden
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(o => o.Plaatsnaam.ToLower().Contains(searchString)
                                        || o.Gemeente.ToLower().Contains(searchString)
                                        || o.Straatnaam.ToLower().Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            // sorteer de lijst met ontmoetingsplaatsen op plaatsnaam
            // AsNoTracking omdat de db niet aangepast wordt
            var ontmoetingsplaatsen = await query.AsNoTracking()
                .OrderBy(o => o.Plaatsnaam).ToListAsync();

            return View(ontmoetingsplaatsen);
        }

        // GET: Ontmoetingsplaatsen/Create
        public IActionResult Create()
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            return View();
        }

        // POST: Ontmoetingsplaatsen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Plaatsnaam,Straatnaam,Straatnummer,Postcode,Gemeente")] Ontmoetingsplaats ontmoetingsplaats)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // als er geen validatie fouten zijn
            // bewaar de nieuwe ontmoetingsplaats in de db
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
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (id == null)
            {
                return NotFound();
            }

            //zoek de ontmoetingsplaats
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
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // zoek de ontmoetingsplaats
            if (id != ontmoetingsplaats.Id)
            {
                return NotFound();
            }

            // als er geen validatie fouten zijn
            // pas de gegevens aan van de ontmoetingsplaats in de db
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


        //bestaat de ontmoetingsplaats?
        private bool OntmoetingsplaatsExists(Guid id)
        {
            return _context.Ontmoetingsplaatsen.Any(e => e.Id == id);
        }
    }
}
