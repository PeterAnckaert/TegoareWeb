using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Models;

namespace TegoareWeb.Data
{
    public class CredentialBeheerder
    {
        public static bool Check(string[] roles, ITempDataDictionary tempData, TegoareContext ctx)
        {
            LoginBeheerder lb = new(ctx);

            if (roles == null) 
            {
                Lid lid = lb.FindUser((String)tempData.Peek("LoginNaam"), (String)tempData.Peek("LoginWachtwoord"));

                if (lid == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            foreach(string role in roles)
            {
                if(!lb.CheckRole((String)tempData.Peek("LoginNaam"), (String)tempData.Peek("LoginWachtwoord"), role))
                {
                    return false;
                }
            }
            return true;
        } 
    }
}
