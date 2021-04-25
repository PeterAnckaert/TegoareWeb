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
    public class RelatiesController : Controller
    {
        private readonly TegoareContext _context;

        private static readonly RelatieListViewModel _listModel = new();

        public RelatiesController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Relaties
        public async Task<IActionResult> Index(string searchString = null)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var query = _context.Leden
                .AsNoTracking();  
               
            if(!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(l => l.Voornaam.ToLower().Contains(searchString)
                                        || l.Achternaam.ToLower().Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            var leden = await query
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam).ToListAsync();
            var ledenIds = leden.Select(l => l.Id).ToList();

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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var leden = await _context.Leden
                .OrderBy(l => l.Achternaam)
                .ThenBy(l => l.Voornaam)
                .ToListAsync();

            var groepen = await _context.Groepen
                .OrderBy(g => g.Rol)
                .ToListAsync();

            var ledenList = new SelectList(leden, "Id", "VolledigeNaam").ToList();
            var groepenList = new SelectList(groepen, "Id", "Rol").ToList();
            
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
            IActionResult actionResult = CheckIfNotAllowed(); ;
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

            if (ModelState.IsValid)
            {
                relatie.Id = Guid.NewGuid();
                _listModel.ErrorMessages = new List<ErrorMessage>();
                if (ledenlijst.Count == 0)
                {
                    var duplicate = await _context.Relaties
                        .FirstOrDefaultAsync(r => r.Id_Lid1 == relatie.Id_Lid1 &&
                        r.Id_Groep == relatie.Id_Groep);
                    if (duplicate == null)
                    {
                        _context.Add(relatie);
                        await _context.SaveChangesAsync();
                        _listModel.ErrorMessages.Add(new ErrorMessage { 
                            Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()}: SUCCES",
                            Value = true });
                    }
                    else
                    {
                        _listModel.ErrorMessages.Add(new ErrorMessage {
                            Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()}: MISLUKT (bestaat reeds)",
                            Value = false });
                    }
                }
                else
                {
                    foreach(Guid Id_Lid2 in ledenlijst)
                    {
                        var lid2 = await _context.Leden.FirstAsync(l => l.Id == Id_Lid2);
                        if (Id_Lid1 == Id_Lid2)
                        {
                            _listModel.ErrorMessages.Add(new ErrorMessage {
                                Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()} van {lid2.VolledigeNaam}: MISLUKT (beide leden zijn identiek)",
                                Value = false });
                            continue;
                        }
                        relatie.Id_Lid2 = Id_Lid2;
                        var duplicate = await _context.Relaties
                            .FirstOrDefaultAsync(r => r.Id_Lid1 == relatie.Id_Lid1 &&
                            r.Id_Groep == relatie.Id_Groep &&
                            r.Id_Lid2 == relatie.Id_Lid2);
                        if (duplicate == null)
                        {
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
                            _listModel.ErrorMessages.Add(new ErrorMessage
                            {
                                Message = $"{lid1.VolledigeNaam} als {groep.Rol.ToLower()} van {lid2.VolledigeNaam}: MISLUKT (bestaat reeds)",
                                Value = false
                            });
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Create));
        }

        private bool RelatieExists(Guid id)
        {
            return _context.Relaties.Any(e => e.Id == id);
        }

        private IActionResult CheckIfNotAllowed()
        {
            if (!CredentialBeheerder.Check(null, TempData, _context))
            {
                return RedirectToAction("LogIn", "Account");
            }

            string[] roles = { "ledenmanager" };
            if (!CredentialBeheerder.Check(roles, TempData, _context))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            return null;
        }
    }
}
