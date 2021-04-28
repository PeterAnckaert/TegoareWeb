using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.ViewModels;

namespace TegoareWeb.Controllers
{
    public class LijstenController : Controller
    {
        private readonly TegoareContext _context;

        public LijstenController(TegoareContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            return View();
        }
        public IActionResult Verjaardagslijst(string maand)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            IEnumerable Maanden = new[]
             {
                new { Value = "1", Text = "Januari" },
                new { Value = "2", Text = "Februari" },
                new { Value = "3", Text = "Maart" },
                new { Value = "4", Text = "April" },
                new { Value = "5", Text = "Mei" },
                new { Value = "6", Text = "Juni" },
                new { Value = "7", Text = "Juli" },
                new { Value = "8", Text = "Augustus" },
                new { Value = "9", Text = "September" },
                new { Value = "10", Text = "Oktober" },
                new { Value = "11", Text = "November" },
                new { Value = "12", Text = "December" }
            };

            // geselecteerde maand is standaard de volgende maand
            string selectedMonth = (DateTime.Today.Month+1).ToString();

            // is er reeds een maand geselecteerd
            // zet die dan als de geselecteerde maand
            if (maand != null) 
            {
                selectedMonth = maand;
            }
            // maak de lijst met de maanden en selecteer een maand
            SelectList maandList = new(Maanden,"Value","Text", selectedMonth);
            ViewData["Maanden"] = maandList;
            Verjaardagslijst lijst = new();
            lijst.Maand = selectedMonth;
            // maak een lijst met alle jarigen van de geselecteerde maand
            // gesorteerd per dag en van jong naar oud
            lijst.Jarigen = _context.Leden
                .Where(l => l.Geboortedatum.Month.ToString() == selectedMonth)
                .OrderBy(l => l.Geboortedatum.Day)
                .ThenByDescending(l => l.Geboortedatum.Year)
                .ToList();
            return View(lijst);
        }

        public async Task<IActionResult> Huisbezoekerslijst()
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var model = new Huisbezoekerslijst
            {
                //  lijst gesorteerd op naam van de huisbezoeker en zijn id
                HuisbezoekersList = new SortedDictionary<string, Guid>()
            };

            // vind de id van de rol huisbezoeker
            var queryHuisbezoeker = await _context.Groepen.FirstOrDefaultAsync(g => g.Rol == "Huisbezoeker");
            Guid huisbezoekerId = queryHuisbezoeker.Id;

            // maak een lijst met alle huisbezoekers
            // AsNoTracking want de db wordt niet aangepast
            // Select want ik ben enkel geïnteresseerd in de id van de huisbezoeker
            // en niet in de ids voor wie hij verantwoordelijk is
            // Distinct want de id van de huisbezoeker is uniek voor hem/haar
            var query = _context.Relaties
                    .AsNoTracking()
                    .Where(r => r.Id_Groep == huisbezoekerId)
                    .Select(r => r.Id_Lid1)
                    .Distinct()
                    .ToList();

            // zoek de naam van de huisbezoeker
            // en voeg het toe aan de gesorteerde lijst
            foreach (var huisbezoeker in query)
            {
                var lid = await _context.Leden
                    .Where(l => l.Id == huisbezoeker)
                    .FirstOrDefaultAsync();
                String naam = lid?.VolledigeNaam;
                model.HuisbezoekersList.Add(naam, huisbezoeker);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LedenHuisbezoekerlijst(Guid IdCurrentHuisbezoeker, String NaamHuisbezoeker)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            // vind de id van de rol huisbezoeker
            var queryHuisbezoeker = await _context.Groepen.FirstOrDefaultAsync(g => g.Rol == "Huisbezoeker");

            // maak een lijst van de namen van leden waarvoor een
            // huisbezoeker verantwoordelijk is
            // Id_Lid1 + Id-Groep = de persoon die huisbezoeker is
            // Id_Lid2 = lid waarvoor hij/zij verantwoordelijk is
            // AsNoTracking want de db wordt niet aangepast
            // Include want ik wil alle gegevens van dat lid
            // gesorteerd op achternaam, dan voornaam
            var leden = _context.Relaties
                    .AsNoTracking()
                    .Where(r => r.Id_Groep == queryHuisbezoeker.Id && r.Id_Lid1 == IdCurrentHuisbezoeker)
                    .Include(r => r.Lid2)
                    .OrderBy(r => r.Lid2.Achternaam)
                    .ThenBy(r => r.Lid2.Voornaam)
                    .Select(r => r.Lid2.VolledigeNaam)
                    .ToList();

            ViewData["NaamHuisbezoeker"] = NaamHuisbezoeker;
            return View(leden);
        }

        public async Task<IActionResult> Stuurgroep()
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            return View(await FillList("Stuurgroep"));
        }
        public async Task<IActionResult> Vrijwilligers()
        {

            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            return View(await FillList ("Vrijwilliger"));
        }

        public async Task<IActionResult> Beheerders()
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var model = new Beheerderslijst
            {
                Activiteitenmanagerlijst = new List<string>(),
                Ledenmanagerlijst = new List<string>()
            };
            model.Activiteitenmanagerlijst = await FillList("Activiteitenmanager");
            model.Ledenmanagerlijst = await FillList("Ledenmanager");
            return View(model);
        }

        private async Task<List<string>> FillList(string Rol)
        {
            // vind de id van de gevraagde rol
            var queryRol =  await _context.Groepen.FirstOrDefaultAsync(g => g.Rol == Rol);

            // maak een lijst van de namen van de leden
            // die de gevraagde rol hebben
            // AsNoTracking want de db wordt niet aangepast
            // Include want ik wil alle gegevens van dat lid
            // gesorteerd op achternaam, dan voornaam
            var lijst = _context.Relaties
                    .AsNoTracking()
                    .Where(r => r.Id_Groep == queryRol.Id)
                    .Include(r => r.Lid1)
                    .OrderBy(r => r.Lid1.Achternaam)
                    .ThenBy(r => r.Lid1.Voornaam)
                    .Select(r => r.Lid1.VolledigeNaam)
                    .ToList();

            return lijst;
        }

        private IActionResult CheckIfNotAllowed()
        {
            // indien gebruiker niet gekend, ga naar login pagina
            if (!CredentialBeheerder.Check(null, TempData, _context))
            {
                return RedirectToAction("LogIn", "Account");
            }

            // indien de gekende gebruiker niet de juiste authorisatie heeft
            // (in dit geval ledenmanager)
            // mag hij de gegevens niet zien
            string[] roles = { "ledenmanager" };
            if (!CredentialBeheerder.Check(roles, TempData, _context))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            // gebruiker is gekend en heeft de juiste authorisatie
            return null;
        }
    }
}
