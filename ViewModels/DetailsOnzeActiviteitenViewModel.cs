using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class DetailsOnzeActiviteitenViewModel
    {
        public IList<Lid> Leden { get; set; }
        public IList<Lid> IngeschrevenLeden { get; set; }

        public Lid HuidigLid { get; set; }
        public Activiteit Activiteit { get; set; }
    }
}
