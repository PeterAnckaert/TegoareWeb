using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;
using TegoareWeb.ViewModels;

namespace TegoareWeb.Controllers
{
    public class LedenController : Controller
    {
        private readonly TegoareContext _context;

        public LedenController(TegoareContext context)
        {
            _context = context;
        }

        // GET: Leden
        public async Task<IActionResult> Index(
            int? pageNumber,
            int? pageSize,
            string sortOrder = null,
            string currentFilter = null,
            string searchString = null)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            ViewData["CurrentSort"] = sortOrder;
            ViewData["LidnaamSortParm"] = sortOrder == "lidnaam_asc" ? "lidnaam_desc" : "lidnaam_asc";
            ViewData["StraatnaamSortParm"] = sortOrder == "straatnaam_asc" ? "straatnaam_desc" : "straatnaam_asc";
            ViewData["GemeenteSortParm"] = sortOrder == "gemeente_asc" ? "gemeente_desc" : "gemeente_asc";

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var leden = _context.Leden
                .AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                leden = leden.Where(l => l.Achternaam.ToLower().Contains(searchString)
                                       || l.Voornaam.ToLower().Contains(searchString)
                                       || l.Straatnaam.ToLower().Contains(searchString)
                                       || l.Straatnummer.ToLower().Contains(searchString)
                                       || l.Gemeente.ToLower().Contains(searchString)
                                       || l.Email.ToLower().Contains(searchString)
                                       || l.Telefoon_vast.ToLower().Contains(searchString)
                                       || l.Telefoon_GSM.ToLower().Contains(searchString));
            }

            leden = sortOrder switch
            {
                "lidnaam_asc" => leden.OrderBy(l => l.Achternaam)
                                       .ThenBy(l => l.Voornaam),
                "lidnaam_desc" => leden.OrderByDescending(l => l.Achternaam)
                                        .ThenByDescending(l => l.Voornaam),
                "straatnaam_asc" => leden.OrderBy(l => l.Straatnaam)
                                        .ThenBy(l => l.Straatnummer)
                                        .ThenBy(l => l.Postcode)
                                        .ThenBy(l => l.Gemeente)
                                        .ThenBy(l => l.Achternaam)
                                        .ThenBy(l => l.Voornaam),
                "straatnaam_desc" => leden.OrderByDescending(l => l.Straatnaam)
                                        .ThenByDescending(l => l.Straatnummer)
                                        .ThenByDescending(l => l.Postcode)
                                        .ThenByDescending(l => l.Gemeente)
                                        .ThenByDescending(l => l.Achternaam)
                                        .ThenByDescending(l => l.Voornaam),
                "gemeente_asc" => leden.OrderBy(l => l.Postcode)
                                        .ThenBy(l => l.Gemeente)
                                        .ThenBy(l => l.Straatnaam)
                                        .ThenBy(l => l.Straatnummer)
                                        .ThenBy(l => l.Achternaam)
                                        .ThenBy(l => l.Voornaam),
                "gemeente_desc" => leden.OrderByDescending(l => l.Postcode)
                                        .ThenByDescending(l => l.Gemeente)
                                        .ThenByDescending(l => l.Straatnaam)
                                        .ThenByDescending(l => l.Straatnummer)
                                        .ThenByDescending(l => l.Achternaam)
                                        .ThenByDescending(l => l.Voornaam),
                _ => leden.OrderBy(l => l.Achternaam)
                            .ThenBy(l => l.Voornaam),
            };
            return View(await PaginatedList<Lid>.CreateAsync(leden, pageNumber ?? 1, pageSize ?? 10));
        }

        // GET: Leden/Create
        public IActionResult Create()
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            return View();
        }

        // POST: Leden/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Achternaam,Voornaam,Geboortedatum,Straatnaam,Straatnummer,Postcode,Gemeente,Telefoon_vast,Telefoon_GSM,Email")] Lid lid)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (ModelState.IsValid)
            {
                lid.Id = Guid.NewGuid();
                lid.Login_Naam = CreateDefaultLoginNaam(lid);
                lid.Wachtwoord = Crypto.Hash(CreateDefaultLoginWachtwoord(lid));
                _context.Add(lid);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lid);
        }

        // GET: Leden/Edit/5
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

            var lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            var relaties = await _context.Relaties
                .AsNoTracking()
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .Where(r => r.Id_Lid1 == lid.Id || r.Id_Lid2 == lid.Id)
                .OrderBy(r => r.Groep.Rol)
                .ToListAsync();

            lid.Relaties = relaties;

            return View(lid);
        }

        // POST: Leden/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Achternaam,Voornaam,Geboortedatum,Straatnaam,Straatnummer,Postcode,Gemeente,Telefoon_vast,Telefoon_GSM,Email,Login_Naam,Wachtwoord")] Lid lid)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (id != lid.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LidExists(lid.Id))
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
            return View(lid);
        }

        // GET: Leden/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

            var lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            var relaties = await _context.Relaties
                .AsNoTracking()
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .Where(r => r.Id_Lid1 == lid.Id || r.Id_Lid2 == lid.Id)
                .OrderBy(r => r.Groep.Rol)
                .ToListAsync();

            lid.Relaties = relaties;

            return View(lid);
        }

        // POST: Leden/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            var lid = _context.Leden.Find(id);

            if (lid == null)
            {
                return NotFound();
            }

            var relaties = _context.Relaties
                .Include(r => r.Groep)
                .Include(r => r.Lid1)
                .Include(r => r.Lid2)
                .Where(r => r.Id_Lid1 == lid.Id || r.Id_Lid2 == lid.Id)
                .ToList();
            _context.Relaties.RemoveRange(relaties);

            var inschrijvingen = _context.Inschrijvingen.Where(i => i.Id_Lid == id);
            _context.Inschrijvingen.RemoveRange(inschrijvingen);

            _context.Leden.Remove(lid);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteRelatie(Guid? RelId, Guid? LidId)
        {
            IActionResult actionResult = CheckIfNotAllowed(); ;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (RelId == null || LidId == null)
            {
                return NotFound();
            }

            var relatie = _context.Relaties.Find(RelId);

            if (relatie == null)
            {
                return NotFound();
            }

            _context.Relaties.Remove(relatie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = LidId }) ;
        }

        public async Task<IActionResult> EditLoginData(Guid? id)
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

            var lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            return View(lid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLoginDataPost(Guid? id, String Login_Naam, String Wachtwoord)
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

            var lid = await _context.Leden.FindAsync(id);

            if (lid == null)
            {
                return NotFound();
            }

            lid.Login_Naam = Login_Naam;
            if(Wachtwoord != null)
            {
                lid.Wachtwoord = Crypto.Hash(Wachtwoord);
            }

            try
            {
                _context.Update(lid);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LidExists(lid.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Edit), new { id });
        }

        private bool LidExists(Guid id)
        {
            return _context.Leden.Any(e => e.Id == id);
        }

        private static String CreateDefaultLoginNaam(Lid lid)
        {
            String voor, achter;

            if (lid != null)
            {
                string cleanString;
                cleanString = new string(lid.Voornaam.Where(Char.IsLetter).ToArray());
                voor = cleanString.Length > 3 ? cleanString.ToLower().Substring(0, 3) : cleanString.ToLower();
                cleanString = new string(lid.Achternaam.Where(Char.IsLetter).ToArray());
                achter = cleanString.Length > 5 ? cleanString.ToLower().Substring(0, 5) : cleanString.ToLower();
                return voor + achter;
            }
            return null;
        }

        private static String CreateDefaultLoginWachtwoord(Lid lid)
        {
            String dag, maand, jaar;

            if (lid != null)
            {
                dag = lid.Geboortedatum.Day.ToString("00");
                maand = lid.Geboortedatum.Month.ToString("00");
                jaar = lid.Geboortedatum.Year.ToString("0000");

                return dag + maand + jaar;
            }
            return null;
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
