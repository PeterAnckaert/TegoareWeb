using System.ComponentModel.DataAnnotations;

namespace TegoareWeb.Data
{
    public class NotEqualAttribute : ValidationAttribute
    {
        private string Other { get; set; }

        public NotEqualAttribute(string other)
        {
            Other = other;
        }

        //NotEquel is valid (correct) als de stringwaarden niet gelijk zijn
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // zoek de property waarmee vergeleken moet worden (in ons geval Wachtwoord in Lid)
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(Other);
            // haal de waarde van de property
            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            // zijn beide waarden ingevuld en zijn ze gelijk (wat niet goed is)
            // genereer een validatie fout
            if (otherValue != null && value != null && value.ToString().Equals(otherValue.ToString()))
                return new ValidationResult(string.Format("Het nieuwe wachtwoord mag niet gelijk zijn aan het huidige wachtwoord"));
            else
                // er is niets om te vergelijken (kan niet valideren) of
                // de waarden zijn verschillend (dit is een succes)
                return ValidationResult.Success;
        }
    }
}
