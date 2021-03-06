using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class CreateRelatieViewModel
    {
        public SelectList LedenList { get; set; }
        public SelectList GroepenRolList { get; set; }
        public SelectList GroepenDubbeleRelatieList { get; set; }
        public IList<Lid> AlleLeden { get; set; }

        public Guid Id_Lid1 { get; set; }
        public Guid Id_Groep { get; set; }
        public Guid Id_Lid2 { get; set; }
    }
}
