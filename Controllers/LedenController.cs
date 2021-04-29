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
    public class LedenController : Controller
    {
        private readonly TegoareContext _context;

        private readonly string[] _role = { "ledenmanager" };

        public LedenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Leden
        public async Task<IActionResult> Index(
            int? pageNumber,                // <= op welke pagina zijn we nu
            int? pageSize,                  // <= hoeveel activiteiten per pagina
            string sortOrder = null,        // <= hoe moet de activiteiten gesorteerd worden
            string currentFilter = null,    // <= waarop werd er gefilterd
            string searchString = null)     // <= waarop moet er nu gefilterd worden
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // XXX_asc  : a -> z
            // XXX_desc : z -> a
            ViewData["CurrentSort"] = sortOrder;
            ViewData["LidnaamSortParm"] = sortOrder == "lidnaam_asc" ? "lidnaam_desc" : "lidnaam_asc";
            ViewData["StraatnaamSortParm"] = sortOrder == "straatnaam_asc" ? "straatnaam_desc" : "straatnaam_asc";
            ViewData["GemeenteSortParm"] = sortOrder == "gemeente_asc" ? "gemeente_desc" : "gemeente_asc";

            // als er een nieuwe searchstring is, dan terug naar pagina 1
            // anders bijhouden waarop vroeger gezocht werd en daarop opnieuw zoeken
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            // AsNoTracking want database zal niet veranderd worden
            var leden = _context.Leden.AsNoTracking();

            // moet er ergens op gezocht worden?
            // indien JA zoek dan in de naam (voor- en achternaam, adres, email en telefoon
            if (!String.IsNullOrEmpty(searchString))
            {
                leden = leden.Where(l => l.Achternaam.ToLower().Contains(searchString)
                                       || l.Voornaam.ToLower().Contains(searchString)
                                       || l.Straatnaam.ToLower().Contains(searchString)
                                       || l.Straatnummer.ToLower().Contains(searchString)
                                       || l.Gemeente.ToLower().Contains(searchString)
                                       || l.Email.ToLower().Contains(searchString)
                                       || l.Telefoon_vast.ToLower().Contains(searchString)
                                       || l.Telefoon_GSM.ToLower().Contains(searchString));
            }

            // sorteer de leden
            leden = sortOrder switch
            {
                "lidnaam_asc" => leden.OrderBy(l => l.Achternaam)
                                       .ThenBy(l => l.Voornaam),
                "lidnaam_desc" => leden.OrderByDescending(l => l.Achternaam)
                                        .ThenByDescending(l => l.Voornaam),
                "straatnaam_asc" => leden.OrderBy(l => l.Straatnaam)
                                        .ThenBy(l => l.Straatnummer)
                                        .ThenBy(l => l.Postcode)
                                        .ThenBy(l => l.Gemeente)
                                        .ThenBy(l => l.Achternaam)
                                        .ThenBy(l => l.Voornaam),
                "straatnaam_desc" => leden.OrderByDescending(l => l.Straatnaam)
                                        .ThenByDescending(l => l.Straatnummer)
                                        .ThenByDescending(l => l.Postcode)
                                        .ThenByDescending(l => l.Gemeente)
                                        .ThenByDescending(l => l.Achternaam)
                                        .ThenByDescending(l => l.Voornaam),
                "gemeente_asc" => leden.OrderBy(l => l.Postcode)
                                        .ThenBy(l => l.Gemeente)
                                        .ThenBy(l => l.Straatnaam)
                                        .ThenBy(l => l.Straatnummer)
                                        .ThenBy(l => l.Achternaam)
                                        .ThenBy(l => l.Voornaam),
                "gemeente_desc" => leden.OrderByDescending(l => l.Postcode)
                                        .ThenByDescending(l => l.Gemeente)
                                        .ThenByDescending(l => l.Straatnaam)
                                        .ThenByDescending(l => l.Straatnummer)
                                        .ThenByDescending(l => l.Achternaam)
                                        .ThenByDescending(l => l.Voornaam),
                _ => leden.OrderBy(l => l.Achternaam)
                            .ThenBy(l => l.Voornaam),
            };
            // toon de juiste pagina van leden
            return View(await PaginatedList<Lid>.CreateAsync(leden, pageNumber ?? 1, pageSize ?? 10));
        }

        // GET: Leden/Create
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

        // POST: Leden/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Achternaam,Voornaam,Geboortedatum,Straatnaam,Straatnummer,Postcode,Gemeente,Telefoon_vast,Telefoon_GSM,Email")] Lid lid)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // zijn er geen validatie fouten
            // voeg dan het nieuwe lid toe aan de db
            if (ModelState.IsValid)
            {
                lid.Id = Guid.NewGuid();
                // creëer een standaard loginnaam (eerste 3 letters van de voornaam +
                // 5 eerste letters van de achternaam
                lid.Login_Naam = CreateDefaultLoginNaam(lid);
                // creëer een standaard wachtwoord (geboortedatum
                // DDMMJJJJ)
                lid.Wachtwoord = Crypto.Hash(CreateDefaultLoginWachtwoord(lid));
                _context.Add(lid);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lid);
        }

        // GET: Leden/Edit/5
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

            // zoek het lid in de db
            var lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            // maak een lijst met alle relaties van dat lid
            // zowel primaire (lid1) als secundaire (lid2) relaties
            // asnotracking want db wordt niet aangepast
            // zowel alle gegevens van lid1, groep als lid2 moeten geweten zijn
            var relaties = await _context.Relaties
                .AsNoTracking()
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .Where(r => r.Id_Lid1 == lid.Id || r.Id_Lid2 == lid.Id)
                .OrderBy(r => r.Groep.Rol)
                .ToListAsync();

            // bewaar de relaties bij het lid
            lid.Relaties = relaties;

            return View(lid);
        }

        // POST: Leden/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Achternaam,Voornaam,Geboortedatum,Straatnaam,Straatnummer,Postcode,Gemeente,Telefoon_vast,Telefoon_GSM,Email,Login_Naam,Wachtwoord")] Lid lid)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (id != lid.Id)
            {
                return NotFound();
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
                return RedirectToAction(nameof(Index));
            }
            return View(lid);
        }

        // GET: Leden/Delete/5
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

            if (id == null)
            {
                return NotFound();
            }

            // zoek het lid
            var lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            // maak een lijst met alle relaties van dat lid
            // zowel primaire (lid1) als secundaire (lid2) relaties
            // asnotracking want db wordt niet aangepast
            // zowel alle gegevens van lid1, groep als lid2 moeten geweten zijn
            var relaties = await _context.Relaties
                .AsNoTracking()
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .Where(r => r.Id_Lid1 == lid.Id || r.Id_Lid2 == lid.Id)
                .OrderBy(r => r.Groep.Rol)
                .ToListAsync();

            // bewaar de relaties bij het lid
            lid.Relaties = relaties;

            return View(lid);
        }

        // POST: Leden/Delete/5
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

            // zoek het lid in de db
            var lid = _context.Leden.Find(id);

            if (lid == null)
            {
                return NotFound();
            }

            // verwijder alle relaties waar het lid bij betrokken is
            var relaties = _context.Relaties.Where(r => r.Id_Lid1 == lid.Id || r.Id_Lid2 == lid.Id);
            _context.Relaties.RemoveRange(relaties);

            // verwijder alle inschrijvingen van het lid
            var inschrijvingen = _context.Inschrijvingen.Where(i => i.Id_Lid == id);
            _context.Inschrijvingen.RemoveRange(inschrijvingen);

            //verwijder het lid zelf
            _context.Leden.Remove(lid);
            // bewaar de aanpassingen in de db
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteRelatie(Guid? RelId, Guid? LidId)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (RelId == null || LidId == null)
            {
                return NotFound();
            }

            // zoek de relatie
            var relatie = _context.Relaties.Find(RelId);

            if (relatie == null)
            {
                return NotFound();
            }

            // verwijder de relatie
            _context.Relaties.Remove(relatie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = LidId }) ;
        }

        public async Task<IActionResult> EditLoginData(Guid? id)
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

            // zoek het lid
            var lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            return View(lid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLoginDataPost(Guid? id, String Login_Naam, String Wachtwoord)
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

            // zoek het lid
            var lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            // verander de loginnaam
            lid.Login_Naam = Login_Naam;
            // als het wachtwoord ook moet gewijzigd worden
            // verander dan ook het gehashte wachtwoord
            if(Wachtwoord != null)
            {
                lid.Wachtwoord = Crypto.Hash(Wachtwoord);
            }

            // bewaar de aangepaste gegevens in de db
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

            return RedirectToAction(nameof(Edit), new { id });
        }

        // bestaat het lid
        private bool LidExists(Guid id)
        {
            return _context.Leden.Any(e => e.Id == id);
        }

        // maak een standaard loginnaam
        // eerste drie letters van de voornaam +
        // eerste vijf letters van de achternaam
        // alles in kleine letters
        private static String CreateDefaultLoginNaam(Lid lid)
        {
            String voor, achter;

            if (lid != null)
            {
                string cleanString;
                // verwijder alle niet-letters uit de voornaam
                cleanString = new string(lid.Voornaam.Where(Char.IsLetter).ToArray());
                // haal de eerste drie letters uit de voornaam,
                // indien voornaam te kort, de volledige voornaam
                voor = cleanString.Length > 3 ? cleanString.ToLower().Substring(0, 3) : cleanString.ToLower();
                // verwijder alle niet-letters uit de achternaam
                cleanString = new string(lid.Achternaam.Where(Char.IsLetter).ToArray());
                // haal de eerste vijf letters uit de achternaam,
                // indien achternaam te kort, de volledige achternaam
                achter = cleanString.Length > 5 ? cleanString.ToLower().Substring(0, 5) : cleanString.ToLower();
                return voor + achter;
            }
            return null;
        }

        // maak een standaard wachtwoord
        private static String CreateDefaultLoginWachtwoord(Lid lid)
        {
            String dag, maand, jaar;

            // formaat van wachtwoord
            // dag als 2 cijfers    +
            // maand als 2 cijfers  +
            // jaar als 4 cijfers
            if (lid != null)
            {
                dag = lid.Geboortedatum.Day.ToString("00");
                maand = lid.Geboortedatum.Month.ToString("00");
                jaar = lid.Geboortedatum.Year.ToString("0000");

                return dag + maand + jaar;
            }
            return null;
        }
    }
}
