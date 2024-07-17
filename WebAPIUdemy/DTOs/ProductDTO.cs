using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebAPIUdemy.Model;
using WebAPIUdemy.Validation;

namespace WebAPIUdemy.DTOs;

public class ProductDTO
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
    public decimal? Price { get; set; }
    [Required]
    [StringLength(300, MinimumLength = 10)]
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
 
}
