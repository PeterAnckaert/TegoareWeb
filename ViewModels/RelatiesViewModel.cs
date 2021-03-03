using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class RelatiesViewModel
    {
        public IList<Lid> Leden { get; set; }
        public IList<Relatie> Relaties { get; set; }
    }
}
