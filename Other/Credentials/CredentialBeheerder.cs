using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using TegoareWeb.Controllers;
using TegoareWeb.Models;

namespace TegoareWeb.Data
{
    public class CredentialBeheerder : ActionResult
    {
        // roles: welke roles MOET de gebruiker hebben
        // tempData: waar is de login en wachtooord
        // ctx: in welke database moet ik zoeken
        //
        // RETURN:  - geeft null terug als alles lid gekend is en de gezochte
        //          authorizatierollen heeft
        //          null == succes is niet logisch
        //          - geeft een link naar het login scherm als de gebruiker niet gekend is
        //          - geeft een link naar niet geauthoriseerd als de gebruiker gekend is, maar
        //          niet de juiste authorizatie heeft
        public static IActionResult CheckIfAllowed(string[] roles, ITempDataDictionary tempData, TegoareContext ctx)
        {
            // indien gebruiker niet gekend, ga naar login pagina
            if (!Check(null, tempData, ctx))
            {
                return new RedirectToActionResult("LogIn", "Account", null);
            }

            // indien de gekende gebruiker niet de juiste authorisatie (rollen) heeft
            // mag hij de gegevens niet zien
            // deze check wordt enkel uitgevoerd als het lid gekend is,
            // maar geen rolescheck nodig heeft
            if (roles != null && !Check(roles, tempData, ctx))
            {
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            // gebruiker is gekend en heeft de juiste authorisatie
            return null;
        }

        // deze methode geeft enkel true of false terug, niet wat er niet juist is
        // true  = lid gekend en geauthoriseerd
        // false = lid niet gekend of een authorisatierol is niet toegekend aan lid
        public static bool Check(string[] roles, ITempDataDictionary tempData, TegoareContext ctx)
        {
            LoginBeheerder lb = new(ctx);
            // enkel zoeken op gebruiker, niet op rollen
            if (roles == null) 
            {
                Lid lid = lb.FindUser((String)tempData.Peek("LoginNaam"), (String)tempData.Peek("LoginWachtwoord"));
                if (lid == null)
                {
                    //gebruiker niet gevonden
                    return false;
                }
                else
                {
                    //gebruiker gevonden
                    return true;
                }
            }

            // indien hier ook zoeken op rollen
            foreach(string role in roles)
            {
                // heeft deze rol niet
                if(!lb.CheckRole((String)tempData.Peek("LoginNaam"), (String)tempData.Peek("LoginWachtwoord"), role))
                {
                    return false;
                }
            }
            // is gekend en heeft alle nodige rollen
            return true;
        } 
    }
}
