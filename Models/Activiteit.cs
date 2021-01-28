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
        public Guid? Id_publicatiedatum { get; set; }
        public Tijdstip Publicatiedatum { get; set; }
        public Guid? Id_uiterste_inschrijfdatum { get; set; }
        public Tijdstip Uiterste_inschrijfdatum { get; set; }
        public short Prijs { get; set; }
        public short Max_inschrijvingen { get; set; }
        public Guid? Id_ontmoetingsplaats { get; set; }
        public Ontmoetingsplaats Ontmoetingsplaats { get; set; }
    }
}
