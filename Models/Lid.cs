using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TegoareWeb.Data;

namespace TegoareWeb.Models
{
    public class Lid
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(50)]
        public string Achternaam { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [StringLength(50)]
        public string Voornaam { get; set; }

        [Required(ErrorMessage = "{0} dient ingevuld te worden")]
        [DataType(DataType.Date)]
        public DateTime Geboortedatum { get; set; }

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

        [DataType(DataType.PhoneNumber)]
        public string Telefoon_vast { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Telefoon_GSM { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MinLength(8, ErrorMessage = "De loginnaam moet uit minstens {1} tekens bestaan")]
        [MaxLength(30)]
        public string Login_Naam { get; set; }

        [MinLength(6, ErrorMessage = "Het wachtwoord moet uit minstens {1} tekens bestaan")]
        [MaxLength(1024)]
        [DataType(DataType.Password)]
        public string Wachtwoord { get; set; }

        public IList<Relatie> Relaties { get; set; }

        [NotMapped]
        public string VolledigeNaam => $"{Voornaam} {Achternaam}";

        [NotMapped]
        // het nieuwe wachtwoord mag NIET gelijk aan het oude wachtwoord zijn
        [NotEqual("Wachtwoord")]
        [MinLength(6, ErrorMessage = "Het wachtwoord moet uit minstens {1} tekens bestaan")]
        public string NieuwWachtwoord { get; set; }

        [NotMapped]
        // het nieuwe wachtwoord MOET gelijk zijn aan het confirm wachtwoord
        [Compare("NieuwWachtwoord", ErrorMessage = "Het nieuwe wachtwoord komt niet overeen met het herhaalde wachtwoord")]
        public string ConfirmWachtwoord { get; set; }
    }
}
