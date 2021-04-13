using System;
using System.Collections.Generic;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class InschrijvingViewModel
    {
        public IList<Lid> AlleLeden { get; set; }
        public IList<Lid> IngeschrevenLeden { get; set; }
        public Activiteit Activiteit { get; set; }

        public Guid Id_Activiteit { get; set; }
    }
}
