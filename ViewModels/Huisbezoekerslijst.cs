using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class Huisbezoekerslijst
    {
        public SortedDictionary<string, Guid> HuisbezoekersList { get; set; }
        public Guid IdHuisbezoeker { get; set; }
        public IList<Lid> Leden { get; set; }
    }
}
