using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TegoareWeb.Models
{
    public class Lid
    {
        public Guid Id { get; set; }
        public string Achternaam { get; set; }
        public string Voornaam { get; set; }

        [DataType(DataType.Date)]
        public DateTime Geboortedatum { get; set; }
        public string Straatnaam { get; set; }
        public string Straatnummer { get; set; }
        public short Postcode { get; set; }
        public string Gemeente { get; set; }
        public string Telefoon_vast { get; set; }
        public string Telefoon_GSM { get; set; }
        public string Email { get; set; }
        public IList<Relatie> Relaties1 { get; set; }
        public IList<Relatie> Relaties2 { get; set; }
    }
}
