using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
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

        // GET: Account/LogIn
        public IActionResult LogIn()
        {
            return View();
        }

        //POST: Account/LogIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn([Bind("Login_Naam,Wachtwoord")] Lid user)
        {
            // verwijder alle gegevens van de huidige gebruiker (is normaal niet nodig,
            // want je logt in), maar voor de veiligheid wordt alle huidige gegevens
            // leeggemaakt (je weet nooit dat iemand rechtstreeks naar Account/LogIn surft)
            EraseTempData();

            //  is één (of beide) leeg dan kan nog geen gebruiker gezocht worden,
            //  daarom: blijf op huidige scherm
            if (String.IsNullOrEmpty(user.Login_Naam) || String.IsNullOrEmpty(user.Wachtwoord))
            {
                return View();
            }

            // controlleer of het paar Login_Naam <==> Wachtwoord gekend is
            var lid = _credentials.FindUser(user.Login_Naam, user.Wachtwoord);
            // indien null, geen gepaste gebruiker gevonden, daarom ga naar view FouteLogin 
            if (lid == null)
            {
                return RedirectToAction(nameof(FouteLogin));
            }

            // indien hier, dan is gebruiker gekend en geverifieerd,
            // zijn gegevens worden in de TempData dictionary gestopt
            // zodat andere controllers en view dit verder kunnen controlleren 
            // (deze data in een cookie stoppen??)
            // (is TempData globaal voor de site (BAD) of per connectie een andere TempData (GOOD)
            TempData["LoginNaam"] = lid.Login_Naam;
            TempData["LoginWachtwoord"] = user.Wachtwoord;
            TempData["LoginVoornaam"] = lid.Voornaam;

            // checken indien de gebruiker een ledenmanager is
            // (voor de veiligheid wordt gebruiker met Login_Naam en
            // Wachtwoord nog eens gechecked, moet eigenlijk niet)
            if (_credentials.CheckRole(user.Login_Naam, user.Wachtwoord, "ledenmanager"))
            {
                TempData["IsLedenManager"] = true;
            }
            else
            {
                TempData["IsLedenManager"] = false;
            }

            // idem voor activiteitenmanager
            if (_credentials.CheckRole(user.Login_Naam, user.Wachtwoord, "activiteitenmanager"))
            {
                TempData["IsActiviteitenManager"] = true;
            }
            else
            {
                TempData["IsActiviteitenManager"] = false;
            }

            //is het wachtwoord de geboortedatum van de gebruiker dan is dit zijn eerste
            // aanmelding, laat hem daarom zijn wachtwoord veranderen
            if (user.Wachtwoord.Equals($"{lid.Geboortedatum.Day:00}{lid.Geboortedatum.Month:00}{lid.Geboortedatum.Year:0000}"))
            {
                return RedirectToAction(nameof(VeranderWachtwoord));
            }

            //gebruiker gekend + wachtwoord niet geboortedatum ==> login geslaagd
            return RedirectToAction(nameof(SuccesLogin));
        }
        // GET: LogUit
        public IActionResult LogUit()
        {
            // gebruiker logt uit, dus alle gebruikersgegevens wissen
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
            // controlleer of het paar Login_Naam <==> Wachtwoord gekend is
            var lid = _credentials.FindUser(user.Login_Naam, user.Wachtwoord);

            // gebruiker niet gekend
            if (lid == null)
            {
                return RedirectToAction(nameof(FouteLogin));
            }

            // indien hier, gebruiker gekend
            // nieuw wachtwoord mag NIET gelijk zijn aan de geboortedatum
            if (user.NieuwWachtwoord.Equals($"{lid.Geboortedatum.Day:00}{lid.Geboortedatum.Month:00}{lid.Geboortedatum.Year:0000}"))
            {
                ModelState.AddModelError("NieuwWachtwoord", "Het nieuwe wachtwoord mag niet je geboortedatum zijn");

            }

            // enkel nodig om te kijken naar de validatie voor nieuw wachtwoord:
            // nieuw mag NIET gelijk zijn aan huidig    +
            // nieuw MOET gelijk zijn aan confirm
            // indien fout bij validatie, blijf op huidige view
            if (ModelState["NieuwWachtwoord"].ValidationState == ModelValidationState.Invalid ||
                ModelState["ConfirmWachtwoord"].ValidationState == ModelValidationState.Invalid)
            {
                return View();
            }

            // verwijder alle gegevens van de huidige gebruiker (is normaal niet nodig,
            // want je logt in + verandert wachtwoord), maar voor de veiligheid wordt alle huidige
            // gegevens leeggemaakt (je weet nooit dat iemand rechtstreeks naar
            // Account/VeranderWachtwoord surft)
            EraseTempData();

            // voeg de gegevens van de huidige gebruiker in bij dictionary TempData
            TempData["LoginNaam"] = lid.Login_Naam;
            TempData["LoginWachtwoord"] = user.Wachtwoord; //nog oud wachtwoord
            TempData["LoginVoornaam"] = lid.Voornaam;

            // checken indien de gebruiker een ledenmanager is
            // (voor de veiligheid wordt gebruiker met Login_Naam en
            // Wachtwoord nog eens gechecked, moet eigenlijk niet)
            if (_credentials.CheckRole(user.Login_Naam, user.Wachtwoord, "ledenmanager"))
            {
                TempData["IsLedenManager"] = true;
            }
            else
            {
                TempData["IsLedenManager"] = false;
            }

            // idem voor activiteitenmanager
            if (_credentials.CheckRole(user.Login_Naam, user.Wachtwoord, "activiteitenmanager"))
            {
                TempData["IsActiviteitenManager"] = true;
            }
            else
            {
                TempData["IsActiviteitenManager"] = false;
            }

            // het nieuwe wachtwoord wordt gehasht en opgeslagen in de db
            lid.Wachtwoord = Crypto.Hash(user.NieuwWachtwoord);
            try
            {
                _context.Leden.Update(lid); //gegevens van lid opslaan in db
                _context.SaveChangesAsync();
                TempData["LoginWachtwoord"] = user.NieuwWachtwoord; //vanaf nu nieuw wachtwoord
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToAction(nameof(SuccesLogin));
        }

        //alle relevante gegevens van de huidige gebruiker verwijderen
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
