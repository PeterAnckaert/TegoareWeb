using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.Controllers
{
    public class MijnGegevensController : Controller
    {
        private readonly TegoareContext _context;

        private readonly IMyLoginBeheerder _credentials;

        public MijnGegevensController(TegoareContext context, IMyLoginBeheerder credentials)
        {
            _context = context;
            _credentials = credentials;
        }

        //GET MijnGegevens
        public async Task<IActionResult> Index()
        {
            // bestaat de huidige gebruiker
            // indien niet, laat hem inloggen
            var lid = _credentials.FindUser((String)TempData.Peek("LoginNaam"), (String)TempData.Peek("LoginWachtwoord"));

            if (lid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
