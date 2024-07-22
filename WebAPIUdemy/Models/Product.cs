using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebAPIUdemy.Validation;

namespace WebAPIUdemy.Model;

public class Product : IValidatableObject
{ 
    public int ProductId { get; set; }
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(20, ErrorMessage = "O nome deve ter entre 5 e 20 caracteres", MinimumLength = 5)]
    [PrimeiraLetraMaisculaAttribute] 
    public string? Name { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
    public string? Description { get; set; }
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Range(1, 10000, ErrorMessage = "O preço deve estar entre {1} e {2}")]
    public decimal? Price { get; set; }
    [Required]
    [StringLength(300, MinimumLength = 10)]
    public string? ImageUrl { get; set; }
    public float? Stock { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public int CategoryId { get; set; }
    [JsonIgnore]
    public Category? Category { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (this.Stock <= 0)
        {
            yield return new ValidationResult("O a qauntidade no estoque tem que ser maior que 0", [nameof(Stock)]);
        }
    }
}
