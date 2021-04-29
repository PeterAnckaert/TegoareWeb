using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    // contract dat elke een object van een LoginBeheerder volgende methods zal hebben
    // FindUser:    zoekt indien het paar login <==> password bestaat en
    //              geeft een Lid terug
    // CheckRole:   controleert indien het paar login <==> password bestaat en
    //              indien die user ook de authorizatie voor de rol heeft
    public interface IMyLoginBeheerder
    {
        public Lid FindUser(string login, string password);

        public bool CheckRole(string login, string password, string role);
    }
}
