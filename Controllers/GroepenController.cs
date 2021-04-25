using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public GroepenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Groepen
        public async Task<IActionResult> Index(string searchString = null)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
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
            IActionResult actionResult = CheckIfNotAllowed(); ;
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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

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
            IActionResult actionResult = CheckIfNotAllowed(); ;
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
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (id != groep.Id)
            {
                return NotFound();
            }

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

        private bool GroepExists(Guid id)
        {
            return _context.Groepen.Any(e => e.Id == id);
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
