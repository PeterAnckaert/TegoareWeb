using Microsoft.AspNetCore.Mvc;
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
    public class OnzeActiviteitenController : Controller
    {
        private readonly TegoareContext _context;
        private readonly IMyLoginBeheerder _credentials;

        public OnzeActiviteitenController(TegoareContext context, IMyLoginBeheerder credentials)
        {
            _context = context;
            _credentials = credentials;
        }

        // GET: OnzeActiviteiten
        public async Task<IActionResult> Index()
        {
            // geen check op gebruiker, iedereen mag de activiteiten zien

            // toon alle activiteiten die nog niet gebeurd zijn
            // en waarvan de publicatiedatum in het verleden ligt
            // AsNoTracking omdat de db niet aangepast wordt
            // Include omdat ik gegevens van de ontmoetingsplaats nodig heb
            // sorteren van toekomst naar verleden
            // AddDays omdat de activiteiten van vandaag ook nog getoond moeten worden
            var activiteiten = await _context.Activiteiten
                .AsNoTracking()
                .Where(a => a.Activiteitendatum > DateTime.Now.AddDays(-1) &&
                                (a.Publicatiedatum == null || a.Publicatiedatum < DateTime.Now))
                .Include(o => o.Ontmoetingsplaats)
                .OrderByDescending(a => a.Activiteitendatum)
                .ToListAsync();

            // per activiteit bijhouden hoeveel inschrijvingen er zijn
            // AsNoTracking omdat de db niet aangepast wordt
            foreach (var activiteit in activiteiten)
            {
                activiteit.AantalInschrijvingen = _context.Inschrijvingen
                    .AsNoTracking()
                    .Where(i => i.Id_Activiteit == activiteit.Id)
                    .Count();
            }

            return View(activiteiten);
        }

        // GET: OnzeActiviteiten/Details
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // bestaat de huidige gebruiker
            // indien niet, laat hem inloggen
            var lid = _credentials.FindUser((String)TempData.Peek("LoginNaam"),(String)TempData.Peek("LoginWachtwoord"));

            if(lid == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            // vind de activiteit
            // AsNoTracking omdat de db niet aangepast wordt
            var activiteit = await _context.Activiteiten
                                        .AsNoTracking()
                                        .Include(o => o.Ontmoetingsplaats)
                                        .FirstOrDefaultAsync(a => a.Id == id);

            if (activiteit == null)
            {
                return NotFound();
            }

            // bijhouden hoeveel inschrijvingen er zijn voor de activiteit
            // AsNoTracking omdat de db niet aangepast wordt
            activiteit.AantalInschrijvingen = _context.Inschrijvingen
                                                .AsNoTracking()
                                                .Where(i => i.Id_Activiteit == activiteit.Id)
                                                .Count();

            // maak een lijst met alle relaties voor het lid
            // AsNoTracking want de db wordt niet aangepast
            // Include want ik wil gegevens van twee groepen en de leden
            // waarmee het huidig lid een relatie heeft
            // Select want ik heb verder enkel de gegevens nodig van de leden
            // waarmee het huidig lid een relatie heeft
            var relatiesLid = await _context.Relaties
                .AsNoTracking()
                .Include(r => r.Groep)
                .Include(r => r.Lid2)
                .Where(r => r.Id_Lid1 == lid.Id && (r.Groep.Rol == "Gezinshoofd" || r.Groep.Rol == "Huisbezoeker"))
                .Select(r => r.Lid2)
                .ToListAsync();

            // het huidig lid is ook een relatie van zichzelf
            // moet dus toegevoegd worden aan de lijst
            relatiesLid.Add(lid);

            // hebben één of meer leden in de lijst met relaties
            // zich ingeschreven voor de activiteit
            // indien ja: voeg dan het lid toe bij die activiteit
            var ingeschrevenLeden = await _context.Inschrijvingen
                .Where(i => i.Id_Activiteit == id && relatiesLid.Contains(i.Lid))
                .AsNoTracking()
                .Select(l => l.Lid)
                .ToListAsync();

            DetailsOnzeActiviteitenViewModel viewModel = new() {
                Activiteit = activiteit,
                HuidigLid = lid,
                Leden = relatiesLid.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam).ToList(),
                IngeschrevenLeden = ingeschrevenLeden };

            return View(viewModel);
        }

        // PUT: OnzeActiviteiten/Details/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(Guid id, [Bind("Id")] Activiteit activiteit, List<Guid> ledenLijst)
        {
            if (id != activiteit.Id)
            {
                return NotFound();
            }

            // bestaat de huidige gebruiker
            // indien niet, laat hem inloggen
            var lid = _credentials.FindUser((String)TempData.Peek("LoginNaam"), (String)TempData.Peek("LoginWachtwoord"));

            if (lid == null)
            {
                return RedirectToAction("LogIn","Account");
            }

            try
            {
                // maak een lijst met alle relaties voor het lid
                // AsNoTracking want de db wordt niet aangepast
                // Include want ik wil gegevens van twee groepen en de leden
                // waarmee het huidig lid een relatie heeft
                // Select want ik heb verder enkel de gegevens nodig van de leden
                // waarmee het huidig lid een relatie heeft
                var relatiesLid = await _context.Relaties
                    .AsNoTracking()
                    .Include(r => r.Groep)
                    .Include(r => r.Lid2)
                    .Where(r => r.Id_Lid1 == lid.Id && (r.Groep.Rol == "Gezinshoofd" || r.Groep.Rol == "Huisbezoeker"))
                    .Select(r => r.Lid2)
                    .ToListAsync();

                // het huidig lid is ook een relatie van zichzelf
                // moet dus toegevoegd worden aan de lijst
                relatiesLid.Add(lid);

                // maak een lijst met alle inschrijvingen van leden die een
                // relatie hebben met het huidig lid en
                // ook ingeschreven zijn voor de activiteit
                var ingeschrevenLeden = await _context.Inschrijvingen
                    .Where(i => i.Id_Activiteit == id && relatiesLid.Contains(i.Lid))
                    .ToListAsync();
                // verwijder al die inschrijvingen
                _context.RemoveRange(ingeschrevenLeden);

                foreach (Guid id_lid in ledenLijst)
                {
                    Inschrijving inschrijving = new()
                    {
                        Id = new Guid(),
                        Id_Activiteit = id,
                        Id_Lid = id_lid
                    };
                    // voeg de relevante inschrijvingen toe in de database
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
    }
}
