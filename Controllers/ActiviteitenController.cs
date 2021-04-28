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
            string sortOrder,       // <= hoe moet de activiteiten gesorteerd worden
            string currentFilter,   // <= waarop werd er gefilterd
            string searchString,    // <= waarop moet er nu gefilterd worden
            int? pageNumber,        // <= op welke pagina zijn we nu
            int? pageSize)          // <= hoeveel activiteiten per pagina
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if(actionResult != null)
            {
                return actionResult;
            }
            // XXX_asc  : a -> z of verleden -> toekomst
            // XXX_desc : z -> a of toekomst -> verleden
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NaamSortParm"] = sortOrder == "naam_asc" ? "naam_desc" : "naam_asc";
            ViewData["OmschrijvingSortParm"] = sortOrder == "omschrijving_asc" ? "omschrijving_desc" : "omschrijving_asc";
            ViewData["OntmoetingsplaatsSortParm"] = sortOrder == "plaats_asc" ? "plaats_desc" : "plaats_asc";
            ViewData["ActiviteitendatumSortParm"] = sortOrder == "datum_asc" ? "datum_desc" : "datum_asc";
            ViewData["PublicatiedatumSortParm"] = sortOrder == "publicatie_asc" ? "publicatie_desc" : "publicatie_asc";

            // als er een nieuwe searchstring is, dan terug naar pagina 1
            // anders bijhouden waarop vroeger gezocht werd en daarop opnieuw zoeken
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            // AsNoTracking want database zal niet veranderd worden
            // ook de ontmoetingsplaats (en niet enkel de ontmoetingsplaats id) moet gekend zijn
            var activiteiten = _context.Activiteiten
                .AsNoTracking()
                .Include(a => a.Ontmoetingsplaats)
                .AsQueryable();

            // moet er ergens op gezocht worden?
            // indien JA zoek dan in de naam, omschrijving en plaatsnaam (van de ontmoetingsplaats)
            if (!String.IsNullOrEmpty(searchString))
            {
                activiteiten = activiteiten.Where(a => a.Naam.Contains(searchString)
                                       || a.Omschrijving.Contains(searchString)
                                       || a.Ontmoetingsplaats.Plaatsnaam.Contains(searchString));
            }

            // sorteer de activiteiten (datum steeds van toekomst naar verleden,
            // tenzij anders gevraagd)
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

            // toon de juiste pagina van activiteiten
            return View(await PaginatedList<Activiteit>.CreateAsync(activiteiten, pageNumber ?? 1, pageSize ?? 10));
        }

        // GET: Activiteiten/Create
        public IActionResult Create()
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            //vul de select lijst op met de naam van de ontmoetingsplaatsen
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
            // mag de huidige gebruiker (indien gekend) deze gegevens opslaan
            // als het resultaat null is, mag hij de gegevens opslaan
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // zijn er geen validatie fouten
            // voeg dan de nieuwe activiteit toe aan de db
            if (ModelState.IsValid)
            {
                activiteit.Id = Guid.NewGuid();
                //verander de punt (Engelse getalnotatie) naar een komma (onze getalnotatie)
                NormalizePrijs(activiteit);
                _context.Add(activiteit);
                await _context.SaveChangesAsync();
                // keer terug naar de lijst met activiteiten
                return RedirectToAction(nameof(Index));
            }
            //indien hier was er een fout met de validatie
            //vul de select lijst op met de naam van de ontmoetingsplaatsen
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam", activiteit.Id_ontmoetingsplaats);
            //blijf op de huidige pagina
            return View(activiteit);
        }

        // toon de basisactiviteit waarvan een kopie zal gecreëerd worden
        // geen POST, want de kopie wordt GECREËERD (via Create action)
        // GET: Activiteiten/Copy/5
        public async Task<IActionResult> Copy(Guid? id)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // id == null ==> geen activiteit
            if (id == null)
            {
                return NotFound();
            }

            // zoek de activiteit die dient als basis voor de kopie
            var activiteit = await _context.Activiteiten.FindAsync(id);

            // activiteit niet gevonden
            if (activiteit == null)
            {
                return NotFound();
            }

            // vul de select lijst op met de naam van de ontmoetingsplaatsen
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam", activiteit.Id_ontmoetingsplaats);

            // toon de activiteit
            return View(activiteit);
        }


        // GET: Activiteiten/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // id == null ==> geen activiteit
            if (id == null)
            {
                return NotFound();
            }

            // zoek de activiteit in de db
            var activiteit = await _context.Activiteiten.FindAsync(id);
            // indien null ==> niet gevonden
            if (activiteit == null)
            {
                return NotFound();
            }
            // vul de select lijst op met ontmoetingsplaatsen
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
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // is de doorgegeven id gelijk aan de id van de activiteit
            if (id != activiteit.Id)
            {
                return NotFound();
            }

            // zijn er geen validatie fouten
            // pas dan de gewijzigde activiteit aan in de db
            if (ModelState.IsValid)
            {
                try
                {
                    // punt in getal (Engels getal) vervangen door komma
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
                // toon de lijst met activiteiten
                return RedirectToAction(nameof(Index));
            }
            // indien hier was er een fout met de validatie
            // vul de select lijst op met de naam van de ontmoetingsplaatsen
            ViewData["Id_ontmoetingsplaats"] = new SelectList(_context.Ontmoetingsplaatsen, "Id", "Plaatsnaam", activiteit.Id_ontmoetingsplaats);
            // blijf op de huidige pagina
            return View(activiteit);
        }

        // GET: Activiteiten/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // id == null ==> geen activiteit
            if (id == null)
            {
                return NotFound();
            }

            // zoek de activiteit
            // AsNoTracking want database zal niet veranderd worden
            // ook de ontmoetingsplaats (en niet enkel de ontmoetingsplaats id) moet gekend zijn
            var activiteit = await _context.Activiteiten
                .AsNoTracking()
                .Include(a => a.Ontmoetingsplaats)
                .FirstOrDefaultAsync(m => m.Id == id);

            // activiteit == null ==> geen activiteit gevonden
            if (activiteit == null)
            {
                return NotFound();
            }

            // zoek het aantal inschrijvingen voor deze activiteit en onthoud het aantal
            // AsNoTracking want database zal niet veranderd worden
            activiteit.AantalInschrijvingen = _context.Inschrijvingen
                .AsNoTracking()
                .Where(i => i.Id_Activiteit == id)
                .Count();

            return View(activiteit);
        }

        // POST: Activiteiten/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // zoek de activiteit in de db die verwijderd zal worden
            // indien null ==> niet gevonden
            var activiteit = await _context.Activiteiten.FindAsync(id);
            if (activiteit == null)
            {
                return NotFound();
            }

            // zoek de inschrijvingen voor deze activiteit
            var inschrijvingen = _context.Inschrijvingen.Where(i => i.Id_Activiteit == id);

            // verwijder de inschrijvingen uit de db
            _context.Inschrijvingen.RemoveRange(inschrijvingen);
            // verwijder de activiteit uit de db
            _context.Activiteiten.Remove(activiteit);
            // bewaar de db
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // bestaat de activiteit?
        private bool ActiviteitExists(Guid id)
        {
            return _context.Activiteiten.Any(e => e.Id == id);
        }

        // pas de punt in het getal aan naar een komma
        // indien er geen komma is (geheel getal), plaats dan ',00' na het gehele getal
        // is nodig omdat er anders validatiefouten optreden (8,99 is geen engels getal)
        // Prijs is een varchar in de tabel en geen getal
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
            // indien gebruiker niet gekend, ga naar login pagina
            if (!CredentialBeheerder.Check(null, TempData, _context))
            {
                return RedirectToAction("LogIn", "Account");
            }
            // indien de gekende gebruiker niet de juiste authorisatie heeft
            // (in dit geval activiteitenmanager)
            // mag hij de gegevens niet zien
            string[] roles = { "activiteitenmanager" };
            if (!CredentialBeheerder.Check(roles, TempData, _context))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            // gebruiker is gekend en heeft de juiste authorisatie
            return null;
        }
    }
}
