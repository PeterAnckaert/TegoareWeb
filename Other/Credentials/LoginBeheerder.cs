using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Data;

namespace TegoareWeb.Models
{
    public class LoginBeheerder : IMyLoginBeheerder
    {
        private readonly TegoareContext _context;

        public LoginBeheerder(TegoareContext context)
        {
            _context = context;
        }

        public Lid FindUser(string login, string password)
        {
            if(login == null || password == null)
            {
                return null;
            }

            var lid = _context.Leden.FirstOrDefault(l => l.Login_Naam == login);

            if ( lid == null)
            {
                return null; 
            }

            if(Crypto.Verify(password,lid.Wachtwoord))
            {
                return lid;
            }

            return null;
        }


        public bool CheckRole(string login, string password, string role)
        {
            var lid = FindUser(login, password);

            if(lid == null)
            {
                return false;
            }

            var groep = _context.Groepen.FirstOrDefault(g => g.Rol.ToLower() == role);

            if (groep == null)
            {
                return false;
            }

            var rol = _context.Relaties.FirstOrDefault(r => r.Id_Lid1 == lid.Id && r.Groep.Rol.ToLower() == role);

            if(rol == null)
            {
                return false;
            }
            return true;
        }
    }
}
