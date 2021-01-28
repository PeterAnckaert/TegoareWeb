using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public class Tijdstip
    {
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan Begin_uur { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan Eind_uur { get; set; }
        public virtual ICollection<Activiteit> Activiteiten_Publicatie { get; set; }
        public virtual ICollection<Activiteit> Activiteiten_Inschrijf { get; set; }
        public IList<KalenderItem> KalenderItems { get; set; }
    }
}
