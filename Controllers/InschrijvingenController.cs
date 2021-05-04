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

        private readonly string[] _role = { "ledenmanager" };

        public InschrijvingenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Inschrijvingen
        public IActionResult Index()
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // haal het aantal inschrijvingen uit de db, gegroepeerd per activiteit
            var query = _context.Inschrijvingen
                .AsNoTracking()
                .GroupBy(i => i.Id_Activiteit)
                .Select(g => new { Id_Activiteit = g.Key, Count = g.Count() }).ToList();

            // maak een lijst met activiteiten en het aantal inschrijvingen per activiteit
            List<Activiteit> lijstActiviteiten = new();
            foreach (var item in query)
            {
                var activiteit = _context.Activiteiten.FirstOrDefault(a => a.Id == item.Id_Activiteit);
                activiteit.AantalInschrijvingen = item.Count;
                lijstActiviteiten.Add(activiteit);
            }

            //sorteer de lijst van toekomst naar verleden en dan op naam
            lijstActiviteiten = lijstActiviteiten.OrderByDescending(a => a.Activiteitendatum)
                .ThenBy(a => a.Naam).ToList();

            return View(lijstActiviteiten);
        }

        // GET: Inschrijvingen/Details/5
        public async Task<IActionResult> Details(Guid? id)
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

            // haal de inschrijvingen voor de activiteit uit de db
            var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                .AsNoTracking()
                .Where(i => i.Id_Activiteit == id)
                .Include(i => i.Lid)
                .ToListAsync();
            //geen inschrijvingen, dus geen details
            if (inschrijvingenVoorActiviteit == null)
            {
                return NotFound();
            }

            // haal de activiteit (+ontmoetingsplaats) ui de db
            // AsNoTracking omdat de db niet gewijzigd wordt
            var activiteit = _context.Activiteiten
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Include(a => a.Ontmoetingsplaats)
                .FirstOrDefault();
            // activiteit niet gevonden
            if (activiteit == null)
            {
                return NotFound();
            }
            // onthoud het aantal inschrijvingen
            activiteit.AantalInschrijvingen = inschrijvingenVoorActiviteit.Count;
            // haal een lijst met alle leden uit de db
            var leden = await _context.Leden
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam)
                .ToListAsync();

            var model = new InschrijvingViewModel()
            {
                AlleLeden = leden,
                //van de inschrijvingen, enkel de leden nodig
                IngeschrevenLeden = inschrijvingenVoorActiviteit.Select(i => i.Lid).ToList(),
                Activiteit = activiteit,
                Id_Activiteit = activiteit.Id
            };

            return View(model);
        }

        // GET: Inschrijvingen/Edit/5
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

            // haal de inschrijvingen voor de activiteit uit de db
            var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                .AsNoTracking()
                .Where(i => i.Id_Activiteit == id)
                .Include(i => i.Lid)
                .ToListAsync();
            // geen inschrijvingen
            if (inschrijvingenVoorActiviteit == null)
            {
                return NotFound();
            }

            // haal de activiteit uit de db
            var activiteit = _context.Activiteiten
                .AsNoTracking()
                .Where(a => a.Id == id)
                .FirstOrDefault();
            // niet gevonden
            if (activiteit == null)
            {
                return NotFound();
            }
            // onthoud het aantal inschrijvingen
            activiteit.AantalInschrijvingen = inschrijvingenVoorActiviteit.Count;
            // haal een lijst met alle leden uit de db
            var leden = await _context.Leden
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam)
                .ToListAsync();

            var model = new InschrijvingViewModel()
            {
                AlleLeden = leden,
                // enkel de leden van de inschrijvingen nodig
                IngeschrevenLeden = inschrijvingenVoorActiviteit.Select(l => l.Lid).ToList(),
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
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }
            if (id != id_Activiteit)
            {
                return NotFound();
            }

            // als er geen validatiefouten zijn
            // verwijderen alle inschrijvingen voor de activiteit
            // en schrijf alle leden die aangevinkt zijn terug in voor de activiteit
            if (ModelState.IsValid)
            {
                try
                {
                    var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                        .Where(i => i.Id_Activiteit == id)
                        .ToListAsync();
                    // verwijder alle inschrijvingen voor de activiteit
                    _context.RemoveRange(inschrijvingenVoorActiviteit);

                    foreach(Guid id_lid in ledenLijst)
                    {
                        Inschrijving inschrijving = new()
                        {
                            Id = new Guid(),
                            Id_Activiteit = id,
                            Id_Lid = id_lid
                        };
                        // voeg de geselecteerde leden terug toe aan de inschrijving
                        _context.Add(inschrijving);
                    }
                    // bewaar de aanpassingen
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                // aanpassingen bewaard, terug naar lijst
                return RedirectToAction(nameof(Index));
            }
            // validatiefouten, terug naar lijst
            return RedirectToAction(nameof(Index));
        }

        // GET: Inschrijvingen/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // haal alle inschrijvingen voor de activiteit uit de db
            var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                .AsNoTracking()
                .Where(i => i.Id_Activiteit == id)
                .Include(i => i.Lid)
                .ToListAsync();

            // geen inschrijvingen, terug naar lijst
            if (inschrijvingenVoorActiviteit == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // haal de activiteit uit de db
            var activiteit = _context.Activiteiten
                .AsNoTracking()
                .Where(a => a.Id == id)
                .FirstOrDefault();
            // activiteit niet gevonden
            if (activiteit == null)
            {
                return NotFound();
            }
            // onthoud het aantal inschrijvingen voor de activiteit
            activiteit.AantalInschrijvingen = inschrijvingenVoorActiviteit.Count;

            var model = new InschrijvingViewModel()
            {
                // lijst met leden niet nodig
                AlleLeden = null,
                // enkel de leden nodig van de inschrijvingen
                IngeschrevenLeden = inschrijvingenVoorActiviteit.Select(i => i.Lid).ToList(),
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
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // haal alle inschrijvingen voor de activiteit uit de db
            var inschrijvingenVoorActiviteit = await _context.Inschrijvingen
                .Where(i => i.Id_Activiteit == id)
                .ToListAsync();
            // verwijder alle inschrijvingen voor de activiteit
            _context.RemoveRange(inschrijvingenVoorActiviteit);
            // bewaar de db
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // bestaat de inschrijving
        private bool InschrijvingExists(Guid id)
        {
            return _context.Inschrijvingen.Any(e => e.Id == id);
        }
    }
}
