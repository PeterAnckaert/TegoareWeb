using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public class Activiteit
    {
        public Guid Id { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        [DataType(DataType.Date)]
        public DateTime Publicatiedatum { get; set; }
        [DataType(DataType.Date)]
        public DateTime Uiterste_inschrijfdatum { get; set; }
        public short Prijs { get; set; }
        public short Max_inschrijvingen { get; set; }
    }
}
