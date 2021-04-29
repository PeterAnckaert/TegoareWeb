using System.Collections.Generic;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class MijnGegevensViewModel
    {
        // volledige naam van de leden
        public IList<Lid> Relaties { get; set; }

        public Lid HuidigLid { get; set; }
    }
}
