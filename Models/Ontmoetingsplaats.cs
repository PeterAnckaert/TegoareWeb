using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public class Ontmoetingsplaats
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(50)]
        public string Plaatsnaam { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(50)]
        public string Straatnaam { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(20)]
        public string Straatnummer { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [Range(1000, 9999, ErrorMessage = "Gelieve een geldige postcode in te vullen")]
        public short Postcode { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(50)]
        public string Gemeente { get; set; }

        public virtual ICollection<Activiteit> Activiteiten { get; set; }
    }
}
