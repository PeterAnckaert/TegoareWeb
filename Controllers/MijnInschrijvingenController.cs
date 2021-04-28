using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.Controllers
{
    public class MijnInschrijvingenController : Controller
    {
        private readonly TegoareContext _context;
        private readonly IMyLoginBeheerder _credentials;

        public MijnInschrijvingenController(TegoareContext context, IMyLoginBeheerder credentials)
        {
            _context = context;
            _credentials = credentials;
        }

        //GET MijnInschrijvingen
        public async Task<IActionResult> Index()
        {
            // bestaat de huidige gebruiker
            // indien niet, laat hem inloggen
            var lid = _credentials.FindUser((String)TempData.Peek("LoginNaam"), (String)TempData.Peek("LoginWachtwoord"));

            if (lid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lijstInschrijvingen = new Dictionary<Activiteit, List<string>>();

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

            // lijst met alle activiteiten
            // AsNoTracking want de db wordt niet aangepast
            var activiteiten = await _context.Activiteiten
                .AsNoTracking()
                .ToListAsync();

            foreach(Activiteit activiteit in activiteiten)
            {
                // hebben één of meer leden in de lijst met relaties
                // zich ingeschreven voor de activiteit
                // indien ja: voeg de naam dan toe bij die activiteit
                var ingeschrevenLeden = await _context.Inschrijvingen
                    .Where(i => i.Id_Activiteit == activiteit.Id && relatiesLid.Contains(i.Lid))
                    .AsNoTracking()
                    .OrderBy(i => i.Lid.Achternaam)
                    .ThenBy(i => i.Lid.Voornaam)
                    .Select(l => l.Lid.VolledigeNaam)
                    .ToListAsync();

                lijstInschrijvingen.Add(activiteit, ingeschrevenLeden);
            }

            //sorteer de lijst met activiteiten van toekomst naar verleden
            return View(lijstInschrijvingen.OrderByDescending(item => item.Key.Activiteitendatum)
                                            .ThenBy(item => item.Key.Naam)
                                            .ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
