using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;
using TegoareWeb.ViewModels;

namespace TegoareWeb.Controllers
{
    public class MijnGegevensController : Controller
    {
        private readonly TegoareContext _context;

        private readonly IMyLoginBeheerder _credentials;

        public MijnGegevensController(TegoareContext context, IMyLoginBeheerder credentials)
        {
            _context = context;
            _credentials = credentials;
        }

        //GET MijnGegevens
        public async Task<IActionResult> Index()
        {
            // bestaat de huidige gebruiker
            // indien niet, laat hem inloggen
            var lid = _credentials.FindUser((String)TempData.Peek("LoginNaam"), (String)TempData.Peek("LoginWachtwoord"));

            if (lid == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            // maak een lijst met alle relaties voor het lid
            // AsNoTracking want de db wordt niet aangepast
            // Select want ik heb verder enkel de volledige naam nodig van de leden
            // waarmee het huidig lid een relatie heeft
            var relatiesLid = await _context.Relaties
                .AsNoTracking()
                .Where(r => r.Id_Lid1 == lid.Id && (r.Groep.Rol == "Gezinshoofd" || r.Groep.Rol == "Huisbezoeker"))
                .OrderBy(r => r.Lid2.Achternaam)
                .ThenBy(r => r.Lid2.Voornaam)
                .Select(r => r.Lid2)
                .ToListAsync();

            // als er geen andere relaties zijn, kun je onmiddellijk naar
            // de gegevens van het lid gaan en moet je geen lijst met leden
            // meer tonen
            if(relatiesLid.Count == 0)
            {
                return RedirectToAction(nameof(Edit), new { id = lid.Id });
            }

            MijnGegevensViewModel model = new()
            {
                Relaties = relatiesLid,
                HuidigLid = lid
            };

            return View(model);
        }

        //GET MijnGegevens/Edit
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // bestaat de huidige gebruiker
            // indien niet, laat hem inloggen
            var lid = _credentials.FindUser((String)TempData.Peek("LoginNaam"), (String)TempData.Peek("LoginWachtwoord"));

            if (lid == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            // zoek het lid in de db
            lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            return View(lid);
        }

        // POST: Leden/MijnInschrijvingen/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Achternaam,Voornaam,Geboortedatum,Straatnaam,Straatnummer,Postcode,Gemeente,Telefoon_vast,Telefoon_GSM,Email,Login_Naam,Wachtwoord")] Lid lid)
        {
            if (id != lid.Id)
            {
                return NotFound();
            }

            // bestaat de huidige gebruiker
            // indien niet, laat hem inloggen
            var user = _credentials.FindUser((String)TempData.Peek("LoginNaam"), (String)TempData.Peek("LoginWachtwoord"));

            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            // zijn er geen validatie fouten
            // pas dan de gegevens van het nieuwe lid aan in de db
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

                // tel het aantal relaties voor het lid
                // AsNoTracking want de db wordt niet aangepast
                // waarmee het huidig lid een relatie heeft
                int aantalRelaties = await _context.Relaties
                    .AsNoTracking()
                    .Where(r => r.Id_Lid1 == lid.Id && (r.Groep.Rol == "Gezinshoofd" || r.Groep.Rol == "Huisbezoeker"))
                    .CountAsync();
                // als er geen relaties zijn, keer dan terug naar de homepage
                if (aantalRelaties == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                // er waren relaties, dus keer terug naar de lijst
                return RedirectToAction(nameof(Index));
            }
            return View(lid);
        }

        // bestaat het lid
        private bool LidExists(Guid id)
        {
            return _context.Leden.Any(e => e.Id == id);
        }
    }
}
