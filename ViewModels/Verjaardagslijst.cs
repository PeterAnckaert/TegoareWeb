using System.Collections.Generic;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class Verjaardagslijst
    {
        public string Maand { get; set; }
        public IList<Lid> Jarigen { get; set; }
    }
}
