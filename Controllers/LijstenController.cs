using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;
using TegoareWeb.ViewModels;

namespace TegoareWeb.Controllers
{
    public class LijstenController : Controller
    {
        private readonly TegoareContext _context;

        private readonly IMyLoginBeheerder _credentials;

        public LijstenController(TegoareContext context, IMyLoginBeheerder credentials)
        {
            _context = context;
            _credentials = credentials;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Verjaardagslijst(string maand)
        {
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

            string selectedMonth = (DateTime.Today.Month+1).ToString();

            if (maand != null) 
            {
                selectedMonth = maand;
            }

            SelectList maandList = new SelectList(Maanden,"Value","Text", selectedMonth);
            ViewData["Maanden"] = maandList;
            Verjaardagslijst lijst = new Verjaardagslijst();
            lijst.Maand = selectedMonth;
            lijst.Jarigen = _context.Leden
                .Where(l => l.Geboortedatum.Month.ToString() == selectedMonth)
                .OrderBy(l => l.Geboortedatum.Day)
                .ThenByDescending(l => l.Geboortedatum.Year)
                .ToList();
            return View(lijst);
        }

        public async Task<IActionResult> Huisbezoekerslijst()
        {
            var model = new Huisbezoekerslijst
            {
                HuisbezoekersList = new SortedDictionary<string, Guid>()
            };

            var queryHuisbezoeker = await _context.Groepen.FirstOrDefaultAsync(g => g.Rol == "Huisbezoeker");
            Guid huisbezoekerId = queryHuisbezoeker.Id;

            var query = _context.Relaties
                    .AsNoTracking()
                    .Where(r => r.Id_Groep == huisbezoekerId)
                    .Select(r => r.Id_Lid1)
                    .Distinct()
                    .ToList();

            foreach (var huisbezoeker in query)
            {
                var lid = await _context.Leden
                    .Where(l => l.Id == huisbezoeker)
                    .FirstOrDefaultAsync();
                String naam = lid.VolledigeNaam;
                model.HuisbezoekersList.Add(naam, huisbezoeker);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LedenHuisbezoekerlijst(Guid IdCurrentHuisbezoeker, String NaamHuisbezoeker)
        {
            var queryHuisbezoeker = await _context.Groepen.FirstOrDefaultAsync(g => g.Rol == "Huisbezoeker");

            var query = _context.Relaties
                    .AsNoTracking()
                    .Where(r => r.Id_Groep == queryHuisbezoeker.Id && r.Id_Lid1 == IdCurrentHuisbezoeker)
                    .Include(r => r.Lid2)
                    .ToList()
                    .OrderBy(r => r.Lid2.Achternaam)
                    .ThenBy(r => r.Lid2.VolledigeNaam);

            var model = new List<string>();

            foreach (var rel in query)
            {
                model.Add(rel.Lid2.VolledigeNaam);
            }
            ViewData["NaamHuisbezoeker"] = NaamHuisbezoeker;
            return View(model);
        }

        public async Task<IActionResult> Stuurgroep()
        {
            return View(await FillList("Stuurgroep"));
        }
        public async Task<IActionResult> Vrijwilligers()
        {

            return View(await FillList ("Vrijwilliger"));
        }

        public async Task<IActionResult> Beheerders()
        {
            var model = new Beheerderslijst
            {
                Activiteitenmanagerlijst = new List<string>(),
                Ledenmanagerlijst = new List<string>()
            };
            model.Activiteitenmanagerlijst = await FillList("Activiteitenmanager");
            model.Ledenmanagerlijst = await FillList("Ledenmanager");
            return View(model);
        }
        public IActionResult NotImpl()
        {
            return View();
        }

        private async Task<List<string>> FillList(string Rol)
        {
            List<string> lijst = new();

            var queryRol =  await _context.Groepen.FirstOrDefaultAsync(g => g.Rol == Rol);

            var query = _context.Relaties
                    .AsNoTracking()
                    .Where(r => r.Id_Groep == queryRol.Id)
                    .Include(r => r.Lid1)
                    .ToList()
                    .OrderBy(r => r.Lid1.Achternaam)
                    .ThenBy(r => r.Lid1.VolledigeNaam);

            foreach (var rel in query)
            {
                lijst.Add(rel.Lid1.VolledigeNaam);
            }
            return lijst;
        }
    }
}
