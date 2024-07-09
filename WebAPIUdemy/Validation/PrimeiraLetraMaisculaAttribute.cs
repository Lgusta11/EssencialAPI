using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace WebAPIUdemy.Validation;

public class PrimeiraLetraMaisculaAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object value,
        ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var primeiraLetra = value.ToString()[0].ToString();
        if (primeiraLetra != primeiraLetra.ToUpper())
        {
            return new ValidationResult("A primeira letra do nome deve ser Maiuscula");
        }

        return ValidationResult.Success;
    }

   

}
