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
            var activiteiten = await _context.Activiteiten
                .AsNoTracking()
                .Where(a => a.Activiteitendatum > DateTime.Now.AddDays(-1) &&
                                (a.Publicatiedatum == null || a.Publicatiedatum < DateTime.Now) &&
                                (a.Uiterste_inschrijfdatum == null || a.Uiterste_inschrijfdatum > DateTime.Now))
                .Include(o => o.Ontmoetingsplaats)
                .OrderByDescending(a => a.Activiteitendatum)
                .ToListAsync();

            foreach (var activiteit in activiteiten)
            {
                activiteit.AantalInschrijvingen = _context.Inschrijvingen
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

            var lid = _credentials.FindUser((String)TempData.Peek("LoginNaam"),(String)TempData.Peek("LoginWachtwoord"));

            if(lid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var activiteit = await _context.Activiteiten.Include(o => o.Ontmoetingsplaats).FirstOrDefaultAsync(a => a.Id == id);

            if (activiteit == null)
            {
                return NotFound();
            }

            activiteit.AantalInschrijvingen = _context.Inschrijvingen
                                                .Where(i => i.Id_Activiteit == activiteit.Id)
                                                .Count();

            var relatiesLid = await _context.Relaties
                .AsNoTracking()
                .Include(r => r.Groep)
                .Include(r => r.Lid2)
                .Where(r => r.Id_Lid1 == lid.Id && (r.Groep.Rol == "Gezinshoofd" || r.Groep.Rol == "Huisbezoeker"))
                .Select(r => r.Lid2)
                .ToListAsync();

            relatiesLid.Add(lid);

            var ingeschrevenLeden = await _context.Inschrijvingen
                .Where(i => i.Id_Activiteit == id && relatiesLid.Contains(i.Lid))
                .Select(l => l.Lid)
                .ToListAsync();

            DetailsOnzeActiviteitenViewModel viewModel = new() {
                Activiteit = activiteit,
                HuidigLid = lid,
                Leden = relatiesLid.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam).ToList(),
                IngeschrevenLeden = ingeschrevenLeden };

            return View(viewModel);
        }

        // POST: OnzeActiviteiten/Details/5
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

            var lid = _credentials.FindUser((String)TempData.Peek("LoginNaam"), (String)TempData.Peek("LoginWachtwoord"));

            if (lid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var relatiesLid = await _context.Relaties
                    .AsNoTracking()
                    .Include(r => r.Groep)
                    .Include(r => r.Lid2)
                    .Where(r => r.Id_Lid1 == lid.Id && (r.Groep.Rol == "Gezinshoofd" || r.Groep.Rol == "Huisbezoeker"))
                    .Select(r => r.Lid2)
                    .ToListAsync();

                relatiesLid.Add(lid);

                var ingeschrevenLeden = await _context.Inschrijvingen
                    .Where(i => i.Id_Activiteit == id && relatiesLid.Contains(i.Lid))
                    .ToListAsync();

                    _context.RemoveRange(ingeschrevenLeden);

                foreach (Guid id_lid in ledenLijst)
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
    }
}
