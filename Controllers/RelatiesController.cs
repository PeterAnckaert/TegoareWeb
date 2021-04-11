using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using TegoareWeb.Data;
using TegoareWeb.Models;
using TegoareWeb.ViewModels;

namespace TegoareWeb.Controllers
{
    public class RelatiesController : Controller
    {
        private readonly TegoareContext _context;
        private static readonly RelatieListViewModel _listModel = new RelatieListViewModel();

        public RelatiesController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Relaties
        public async Task<IActionResult> Index(string searchString = null)
        {
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

        // GET: Relaties/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatie = await _context.Relaties
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relatie == null)
            {
                return NotFound();
            }

            return View(relatie);
        }

        // GET: Relaties/Create
        public async Task<IActionResult> Create()
        {
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
            Relatie relatie = new Relatie
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

        // GET: Relaties/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatie = await _context.Relaties.FindAsync(id);
            if (relatie == null)
            {
                return NotFound();
            }
            ViewData["Id_Groep"] = new SelectList(_context.Groepen.OrderBy(g => g.Rol), "Id", "Rol", relatie.Id_Groep);
            ViewData["Id_Lid1"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid1);
            ViewData["Id_Lid2"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid2);
            return View(relatie);
        }

        // POST: Relaties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Id_Lid1,Id_Groep,Id_Lid2")] Relatie relatie)
        {
            if (id != relatie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relatie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelatieExists(relatie.Id))
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
            ViewData["Id_Groep"] = new SelectList(_context.Groepen.OrderBy(g => g.Rol), "Id", "Rol", relatie.Id_Groep);
            ViewData["Id_Lid1"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid1);
            ViewData["Id_Lid2"] = new SelectList(_context.Leden.OrderBy(l => l.Achternaam).ThenBy(l => l.Voornaam), "Id", "VolledigeNaam", relatie.Id_Lid2);
            return View(relatie);
        }

        // GET: Relaties/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatie = await _context.Relaties
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relatie == null)
            {
                return NotFound();
            }

            return View(relatie);
        }

        // POST: Relaties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var relatie = await _context.Relaties.FindAsync(id);
            _context.Relaties.Remove(relatie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelatieExists(Guid id)
        {
            return _context.Relaties.Any(e => e.Id == id);
        }
    }
}
