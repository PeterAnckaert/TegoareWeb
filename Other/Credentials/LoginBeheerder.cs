using Microsoft.EntityFrameworkCore;
using System.Linq;
using TegoareWeb.Data;

namespace TegoareWeb.Models
{
    // gebruikt het contract gesteld door IMyLoginBeheerder
    // implementeert dus de afspraken gesteld in IMyLoginBeheerder
    public class LoginBeheerder : IMyLoginBeheerder
    {
        private readonly TegoareContext _context;

        public LoginBeheerder(TegoareContext context)
        {
            _context = context;
        }

        // implementatie van FindUser
        public Lid FindUser(string login, string password)
        {
            // is minstens één van beide null dan kan het lid niet gevonden worden
            if(login == null || password == null)
            {
                return null;
            }

            // vind het lid met de login naam
            // AsNoTracking want we veranderen niets in de db
            var lid = _context.Leden
                            .AsNoTracking()
                            .FirstOrDefault(l => l.Login_Naam == login);

            if ( lid == null)
            {
                return null; 
            }

            // controleer of de hash van wachtwoord identiek is
            // aan de hash die verbonden is aan het lid
            if(Crypto.Verify(password,lid.Wachtwoord))
            {
                return lid;
            }

            // indien hier wel lid, maar hashen zijn niet identiek
            return null;
        }

        // heeft het lid de juiste authorizatie
        public bool CheckRole(string login, string password, string role)
        {
            // kijk of de lid bestaat en de correcte wachtwoord is gegeven
            var lid = FindUser(login, password);

            if(lid == null)
            {
                return false;
            }

            // vind de groep (rol) waarop gecontroleerd zal worden
            // AsNoTracking want we veranderen niets in de db
            var groep = _context.Groepen
                            .AsNoTracking()
                            .FirstOrDefault(g => g.Rol.ToLower() == role);

            if (groep == null)
            {
                return false;
            }

            // zit het lid in die groep (heeft het die rol)
            // AsNoTracking want we veranderen niets in de db
            var rol = _context.Relaties
                            .FirstOrDefault(r => r.Id_Lid1 == lid.Id && r.Groep.Rol.ToLower() == role);

            if(rol == null)
            {
                return false;
            }

            return true;
        }
    }
}
