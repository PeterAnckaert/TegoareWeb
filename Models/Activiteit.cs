using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateTime? Publicatiedatum { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Uiterste_inschrijfdatum { get; set; }
        public decimal? Prijs { get; set; }
        public short? Max_inschrijvingen { get; set; }
        public Guid? Id_ontmoetingsplaats { get; set; }
        public Ontmoetingsplaats Ontmoetingsplaats { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Activiteitendatum { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan? Beginuur { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan? Einduur { get; set; }
        [Column("deel_van_reeks")]
        public bool? DeelVanReeks { get; set; }
    }
}
