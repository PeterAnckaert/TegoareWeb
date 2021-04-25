using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly TegoareContext _context;

        private readonly IMyLoginBeheerder _credentials;

        public AccountController(TegoareContext context, IMyLoginBeheerder credentials)
        {
            _context = context;
            _credentials = credentials;
        }

        // GET: LogIn
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn([Bind("Login_Naam,Wachtwoord")] Lid user)
        {
            EraseTempData();
            if (String.IsNullOrEmpty(user.Login_Naam) || String.IsNullOrEmpty(user.Wachtwoord))
            {
                return View();
            }

            var lid = _credentials.FindUser(user.Login_Naam, user.Wachtwoord);

            if (lid == null)
            {
                return RedirectToAction("FouteLogin");
            }

            if (_credentials.CheckRole(user.Login_Naam, user.Wachtwoord, "ledenmanager"))
            {
                TempData["IsLedenManager"] = true;
            }
            else
            {
                TempData["IsLedenManager"] = false;
            }

            if (_credentials.CheckRole(user.Login_Naam, user.Wachtwoord, "activiteitenmanager"))
            {
                TempData["IsActiviteitenManager"] = true;
            }
            else
            {
                TempData["IsActiviteitenManager"] = false;
            }

            TempData["LoginNaam"] = lid.Login_Naam;
            TempData["LoginWachtwoord"] = user.Wachtwoord;
            TempData["LoginVoornaam"] = lid.Voornaam;

            if (user.Wachtwoord.Equals($"{lid.Geboortedatum.Day:00}{lid.Geboortedatum.Month:00}{lid.Geboortedatum.Year:0000}"))
            {
                return RedirectToAction("VeranderWachtwoord");
            }
            return RedirectToAction("SuccesLogin");
        }
        // GET: LogUit
        public IActionResult LogUit()
        {
            EraseTempData();
            return View();
        }

        // GET: FouteLogin
        public IActionResult FouteLogin()
        {
            return View();
        }

        // GET: SuccesLogin
        public IActionResult SuccesLogin()
        {
            return View();
        }

        // GET: VeranderWachtwoord
        public IActionResult VeranderWachtwoord()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VeranderWachtwoord([Bind("Login_Naam,Wachtwoord,NieuwWachtwoord,ConfirmWachtwoord")] Lid user)
        {
            EraseTempData();

            var lid = _credentials.FindUser(user.Login_Naam, user.Wachtwoord);

            if (lid == null)
            {
                return RedirectToAction("FouteLogin");
            }
            if (user.NieuwWachtwoord.Equals($"{lid.Geboortedatum.Day:00}{lid.Geboortedatum.Month:00}{lid.Geboortedatum.Year:0000}"))
            {
                ModelState.AddModelError("NieuwWachtwoord", "Het nieuwe wachtwoord mag niet je geboortedatum zijn");

            }

            if (ModelState["NieuwWachtwoord"].ValidationState == ModelValidationState.Invalid ||
                ModelState["ConfirmWachtwoord"].ValidationState == ModelValidationState.Invalid)
            {
                return View();
            }

            TempData["LoginNaam"] = lid.Login_Naam;
            TempData["LoginWachtwoord"] = user.Wachtwoord;
            TempData["LoginVoornaam"] = lid.Voornaam;

            lid.Wachtwoord = Crypto.Hash(user.NieuwWachtwoord);
            try
            {
                _context.Leden.Update(lid);
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToAction("SuccesLogin");
        }

        private void EraseTempData()
        {
            TempData.Remove("LoginNaam");
            TempData.Remove("LoginWachtwoord");
            TempData.Remove("LoginVoornaam");
            TempData.Remove("IsActiviteitenManager");
            TempData.Remove("IsLedenManager");
        }
    }
}
