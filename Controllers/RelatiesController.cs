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
    public class RelatiesController : Controller
    {
        private readonly TegoareContext _context;

        private static readonly RelatieListViewModel _listModel = new();

        private readonly string[] _role = { "ledenmanager" };

    public RelatiesController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Relaties
        public async Task<IActionResult> Index(string searchString = null)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context);
            if (actionResult != null)
            {
                return actionResult;
            }

            // haal alle leden uit de database
            // AsNoTracking omdat de db niet aangepast wordt
            var query = _context.Leden.AsNoTracking();

            // moet er gezocht worden
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(l => l.Voornaam.ToLower().Contains(searchString)
                                        || l.Achternaam.ToLower().Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            // lijst met alle leden gesorteerd op achternaam en voornaam
            var leden = await query
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam).ToListAsync();
            // lijst met de Ids van de leden
            var ledenIds = leden.Select(l => l.Id).ToList();

            // lijst van alle relaties
            // met alle gegevens van die relatie
            var relaties = await _context.Relaties
                .AsNoTracking()
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .Where(r => ledenIds.Contains(r.Id_Lid1))
                .OrderBy(r => r.Groep.Rol)
                .ToListAsync();

            _listModel.Leden = leden;
            _listModel.Relaties = relaties;

            return View(_listModel);
        }

        // GET: Relaties/Create
        public async Task<IActionResult> Create()
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context);
            if (actionResult != null)
            {
                return actionResult;
            }

            // geordende lijst van alle leden
            // AsNoTracking want de db wordt niet aangepast
            var leden = await _context.Leden
                .AsNoTracking()
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam)
                .ToListAsync();

            // geordende lijst van alle groepen
            // AsNoTracking want de db wordt niet aangepast
            var groepen = await _context.Groepen
                .AsNoTracking()
                .OrderBy(g => g.Rol)
                .ToListAsync();

            // opvullen van de select lijsten
            var ledenList = new SelectList(leden, "Id", "VolledigeNaam").ToList();
            var groepenList = new SelectList(groepen, "Id", "Rol").ToList();
            
            // bovenaan elke lijst een item toevoegen
            ledenList.Insert(0, new SelectListItem("-- Kies een lid --", "0"));
            groepenList.Insert(0, new SelectListItem("-- Kies een relatie --", "0"));

            var model = new CreateRelatieViewModel
            {
                LedenList = new SelectList(ledenList, "Value", "Text"),
                GroepenRolList = new SelectList(groepenList, "Value", "Text"),
                GroepenDubbeleRelatieList = new SelectList(groepen, "Id", "Dubbele_Relatie"),
                AlleLeden = leden
            };

            return View(model);
        }

        // POST: Relaties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid Id_Lid1, Guid Id_Groep, List<Guid> ledenlijst)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            Relatie relatie = new()
            {
                Id_Lid1 = Id_Lid1,
                Id_Groep = Id_Groep
            };

            var lid1 = await _context.Leden.FirstAsync(l => l.Id == Id_Lid1);
            var groep = await _context.Groepen.FirstAsync(r => r.Id == Id_Groep);

            // als er geen validatie fouten zijn
            // voeg dan de relatie toe in de db (indien nodig)
            if (ModelState.IsValid)
            {
                relatie.Id = Guid.NewGuid();
                _listModel.ErrorMessages = new List<ErrorMessage>();
                // als er geen tweede lid betrokken is bij de relatie
                if (ledenlijst.Count == 0)
                {
                    //controleer of de relatie nog niet bestaat
                    var duplicate = await _context.Relaties
                        .FirstOrDefaultAsync(r => r.Id_Lid1 == relatie.Id_Lid1 &&
                        r.Id_Groep == relatie.Id_Groep);
                    if (duplicate == null)
                    {
                        // geen duplikaat gevonden, dus toevoegen in db
                        _context.Add(relatie);
                        await _context.SaveChangesAsync();
                        _listModel.ErrorMessages.Add(new ErrorMessage { 
                            Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()}: SUCCES",
                            Value = true });
                    }
                    else
                    {
                        // duplikaat gevonden,dus foutbericht
                        _listModel.ErrorMessages.Add(new ErrorMessage {
                            Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()}: MISLUKT (bestaat reeds)",
                            Value = false });
                    }
                }
                // er is een tweede lid betrokken bij de relatie
                else
                {
                    foreach(Guid Id_Lid2 in ledenlijst)
                    {
                        // zoek het tweede lid van de relatie 
                        var lid2 = await _context.Leden.FirstAsync(l => l.Id == Id_Lid2);
                        // is het eerste lid gelijk aan het tweede lid
                        if (Id_Lid1 == Id_Lid2)
                        {
                            //dit mag niet, dus foutbericht
                            _listModel.ErrorMessages.Add(new ErrorMessage {
                                Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()} van {lid2.VolledigeNaam}: MISLUKT (beide leden zijn identiek)",
                                Value = false });
                            continue;
                        }
                        // indien hier, dan lid1 != lid2
                        relatie.Id_Lid2 = Id_Lid2;
                        var duplicate = await _context.Relaties
                            .FirstOrDefaultAsync(r => r.Id_Lid1 == relatie.Id_Lid1 &&
                            r.Id_Groep == relatie.Id_Groep &&
                            r.Id_Lid2 == relatie.Id_Lid2);
                        //controleer of de relatie nog niet bestaat
                        if (duplicate == null)
                        {
                            // geen duplikaat gevonden, dus toevoegen in db
                            _context.Add(relatie);
                            await _context.SaveChangesAsync();
                            _listModel.ErrorMessages.Add(new ErrorMessage
                            {
                                Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()} van {lid2.VolledigeNaam}: SUCCES",
                                Value = true
                            });
                        }
                        else
                        {
                            // duplikaat gevonden,dus foutbericht
                            _listModel.ErrorMessages.Add(new ErrorMessage
                            {
                                Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()} van {lid2.VolledigeNaam}: MISLUKT (bestaat reeds)",
                                Value = false
                            });
                        }
                    }
                }
                // indien hier, terug naar lijst met relaties en tonen van (fout)berichten
                return RedirectToAction(nameof(Index));
            }
            //indien hier, validatiefout, dus blijven op huidige pagina
            return RedirectToAction(nameof(Create));
        }

        // bestaat de relatie
        private bool RelatieExists(Guid id)
        {
            return _context.Relaties.Any(e => e.Id == id);
        }
    }
}
