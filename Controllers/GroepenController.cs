﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.Controllers
{
    public class GroepenController : Controller
    {
        private readonly TegoareContext _context;

        private readonly string[] _role = { "ledenmanager" };


        public GroepenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Groepen
        public async Task<IActionResult> Index(string searchString = null)
        {
            // mag de huidige gebruiker (indien gekend) deze gegevens zien
            // als het resultaat null is, mag hij de gegevens zien
            // als het resultaat niet null is, toon dan de gepaste pagina (login of unauthorized)
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var query = _context.Groepen
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(g => g.Rol.ToLower().Contains(searchString)
                                        || g.Omschrijving.ToLower().Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            var groepen = await query
                .OrderBy(g => g.Rol).ToListAsync();

            return View(groepen);
        }

        // GET: Groepen/Create
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

        // POST: Groepen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Rol,Omschrijving,Dubbele_Relatie")] Groep groep)
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
            // voeg dan de nieuwe groep toe aan de db
            if (ModelState.IsValid)
            {
                groep.Id = Guid.NewGuid();
                _context.Add(groep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groep);
        }

        // GET: Groepen/Edit/5
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

            var groep = await _context.Groepen.FindAsync(id);
            if (groep == null)
            {
                return NotFound();
            }
            return View(groep);
        }

        // POST: Groepen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Rol,Omschrijving,Dubbele_Relatie")] Groep groep)
        {
            IActionResult actionResult = CredentialBeheerder.CheckIfAllowed(_role, TempData, _context); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (id != groep.Id)
            {
                return NotFound();
            }

            // zijn er geen validatie fouten
            // pas dan de groep aan in de db
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroepExists(groep.Id))
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
            return View(groep);
        }

        // bestaat de groep?
        private bool GroepExists(Guid id)
        {
            return _context.Groepen.Any(e => e.Id == id);
        }
    }
}
