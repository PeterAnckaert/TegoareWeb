using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Data
{
    public class LoginData
    {
        [MinLength(8, ErrorMessage = "De loginnaam moet uit minstens {1} tekens bestaan")]
        [MaxLength(30)]
        public string Login_Naam { get; set; }

        [MinLength(6, ErrorMessage = "Het wachtwoord moet uit minstens {1} tekens bestaan")]
        [MaxLength(1024)]
        [DataType(DataType.Password)]
        public string Wachtwoord { get; set; }
    }
}
