﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TegoareWeb.Models
{
    public class Activiteit
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Activiteit")]
        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(50)]
        public string Naam { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(250)]
        public string Omschrijving { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Publicatiedatum { get; set; }

        [Display(Name = "Max. inschrijfdatum")]
        [DataType(DataType.Date)]
        public DateTime? Uiterste_inschrijfdatum { get; set; }

        [StringLength(50)]
        [RegularExpression("^(\\d+(?:[\\.\\,]\\d{2})?)$", ErrorMessage = "Gelieve een correct geldbedrag in te vullen")]
        // enkel cijfers, één punt of komma en twee cijfers na de komma
        // zijn toegelaten als mogelijke prijs
        public string Prijs { get; set; }

        [Display(Name = "Max. inschrijvingen")]
        [RegularExpression("^\\d*$", ErrorMessage = "Gelieve een geheel getal in te vullen")]
        public short? Max_inschrijvingen { get; set; }

        [Display(Name = "Ontmoetingsplaats")]
        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        public Guid Id_ontmoetingsplaats { get; set; }
        public Ontmoetingsplaats Ontmoetingsplaats { get; set; }

        [Display(Name = "Datum")]
        [DisplayFormat(DataFormatString = "{0:dddd dd MMMM yyyy}")]
        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [DataType(DataType.Date)]
        public DateTime Activiteitendatum { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [DataType(DataType.Time)]
        public TimeSpan Beginuur { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        [DataType(DataType.Time)]
        public TimeSpan? Einduur { get; set; }

        [NotMapped]
        public int AantalInschrijvingen;

        [NotMapped]
        public string ActiviteitendatumEnNaam => $"{Activiteitendatum.ToShortDateString()} -- {Naam}";
    }
}
