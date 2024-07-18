using System.ComponentModel.DataAnnotations;

namespace WebAPIUdemy.DTOs;

public class ProductDTOUpdateRequest : IValidatableObject
{
    [Range(1,9999, ErrorMessage = "Estoque deve estar entre 1 e 9999")]
    public float? Stock { get; set; }
    public DateTime RegistrationDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RegistrationDate.Date < DateTime.Now.Date)
        {
            yield return new ValidationResult("A data deve ser maior ou igual que a data atual",
                new[] { nameof(this.RegistrationDate) });
        }
    }
}
