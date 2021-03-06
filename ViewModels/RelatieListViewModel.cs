using System.Collections.Generic;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class RelatieListViewModel
    {
        public IList<Lid> Leden { get; set; }
        public IList<Relatie> Relaties { get; set; }
    }
}
