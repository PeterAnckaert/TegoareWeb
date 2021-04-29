using System.Collections.Generic;

namespace TegoareWeb.ViewModels
{
    public class BeheerderslijstViewModel
    {
        // lijst met namen van de managers, opgesplitst op groep
        public IList<string> Ledenmanagerlijst { get; set; }
        public IList<string> Activiteitenmanagerlijst { get; set; }
    }
}
