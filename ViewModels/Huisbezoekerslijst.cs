using System;
using System.Collections.Generic;

namespace TegoareWeb.ViewModels
{
    public class Huisbezoekerslijst
    {
        public SortedDictionary<string, Guid> HuisbezoekersList { get; set; }
        public Guid IdCurrentHuisbezoeker { get; set; }
        public string NaamHuisbezoeker { get; set; }
    }
}
