using System;
using System.Collections.Generic;

namespace TegoareWeb.ViewModels
{
    public class HuisbezoekerslijstViewModel
    {
        // lijst met key = naam en value = id
        public SortedDictionary<string, Guid> HuisbezoekersList { get; set; }
        public Guid IdCurrentHuisbezoeker { get; set; }
        public string NaamHuisbezoeker { get; set; }
    }
}
