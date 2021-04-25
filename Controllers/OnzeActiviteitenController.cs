using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.Controllers
{
    public class OnzeActiviteitenController : Controller
    {
        private readonly TegoareContext _context;

        public OnzeActiviteitenController(TegoareContext context)
        {
            _context = context;
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

            foreach(var activiteit in activiteiten)
            {
                activiteit.AantalInschrijvingen = _context.Inschrijvingen
                .Where(i => i.Id_Activiteit == activiteit.Id)
                .Count();
            }

            return View(activiteiten);
        }

    }
}
