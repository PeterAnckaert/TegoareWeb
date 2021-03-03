using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public class Verjaardagslijst
    {
        public string Maand { get; set; }
        public IList<Lid> Jarigen { get; set; }
    }
}
