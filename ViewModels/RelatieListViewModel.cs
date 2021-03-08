using System.Collections.Generic;
using TegoareWeb.Data;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class RelatieListViewModel
    {
        public IList<Lid> Leden { get; set; }
        public IList<Relatie> Relaties { get; set; }
        public IList<ErrorMessage> ErrorMessages { get; set; }
    }
}
