using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public class KalenderItem
    {
        public Guid Id_activiteit;
        public Activiteit Activiteit;

        public Guid Id_tijdstip;
        public Tijdstip Tijdstip;
    }
}
