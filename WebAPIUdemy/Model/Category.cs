﻿using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace WebAPIUdemy.Model;

public class Category
{

    public Category() 
    {
        Products = new Collection<Product>();
    }
    [Key]
    public int CategoryId { get; set; }
    [Required]
    [StringLength(80)]
    public string? Name { get; set; }
    [Required]
    [StringLength(100)]
    public string? ImageUrl { get; set; } 

    public ICollection<Product>? Products { get; set; }

}
