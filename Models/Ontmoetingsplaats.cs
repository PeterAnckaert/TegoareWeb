using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public class Ontmoetingsplaats
    {
        public Guid Id { get; set; }
        public string Plaatsnaam { get; set; }
        public string Straatnaam { get; set; }
        public string Straatnummer { get; set; }
        public short Postcode { get; set; }
        public string Gemeente { get; set; }
        public virtual ICollection<Activiteit> Activiteiten { get; set; }
    }
}
