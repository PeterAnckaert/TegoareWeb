using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TegoareWeb.Models
{
    public class Groep
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(50)]
        public string Rol { get; set; }

        [StringLength(250)]
        public string Omschrijving { get; set; }

        [Display(Name = "Tweepersoonsrelatie")]
        public bool Dubbele_Relatie { get; set; }

        public IList<Relatie> Relaties { get; set; }
    }
}
