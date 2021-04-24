using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Data
{
    public class NotEqualAttribute : ValidationAttribute
    {
        private string Other { get; set; }

        public NotEqualAttribute(string other)
        {
            Other = other;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(Other);
            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            if (otherValue != null && value != null && value.ToString().Equals(otherValue.ToString()))
                return new ValidationResult(string.Format("Het nieuwe wachtwoord mag niet gelijk zijn aan het huidige wachtwoord"));
            else
                return ValidationResult.Success;
        }
    }
}
