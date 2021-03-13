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
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(50)]
        public string Naam { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(250)]
        public string Omschrijving { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [DataType(DataType.Date)]
        public DateTime? Publicatiedatum { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [DataType(DataType.Date)]
        public DateTime? Uiterste_inschrijfdatum { get; set; }

        [Range(0.01, 9999.99, ErrorMessage = "Prijs moet leeg zijn of tussen 0.01 en 9999.99 liggen")]
        [DataType(DataType.Currency)]
        public decimal? Prijs { get; set; }

        [RegularExpression("^\\d*$", ErrorMessage = "Gelieve een geheel getal in te vullen")]
        public short? Max_inschrijvingen { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        public Guid? Id_ontmoetingsplaats { get; set; }
        public Ontmoetingsplaats Ontmoetingsplaats { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [DataType(DataType.Date)]
        public DateTime? Activiteitendatum { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [DataType(DataType.Time)]
        public TimeSpan? Beginuur { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? Einduur { get; set; }
    }
}
