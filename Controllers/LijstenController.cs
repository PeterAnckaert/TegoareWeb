using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;

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
            lijst.Jarigen = _context.Leden.Where(l => l.Geboortedatum.Month.ToString() == selectedMonth).OrderBy(l => l.Geboortedatum.Day).ThenByDescending(l => l.Geboortedatum.Year).ToList();
            return View(lijst);
        }
        public IActionResult NotImpl()
        {
            return View();
        }
    }
}
