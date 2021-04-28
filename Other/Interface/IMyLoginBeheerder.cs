using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public interface IMyLoginBeheerder
    {
        public Lid FindUser(string login, string password);
        public bool CheckRole(string login, string password, string role);
    }
}
