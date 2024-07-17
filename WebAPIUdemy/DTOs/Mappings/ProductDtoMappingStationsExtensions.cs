using System.Xml.Linq;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.DTOs.Mappings;

public static class ProductDtoMappingStationsExtensions
{
    public static ProductDTO? ToProductDTO(this Product product)
    {
        if (product is null)
            return null;

        return new ProductDTO
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };
    }

    public static Product? ToProduct(this ProductDTO productDto)
    {
        if (productDto is null) return null;

        return new Product
        {
            ProductId = productDto.ProductId,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            ImageUrl = productDto.ImageUrl,
            CategoryId = productDto.CategoryId
        };
    }


    public static IEnumerable<ProductDTO> ToProductDtoList(this IEnumerable<Product> products)
    {
        if (products is null || !products.Any())
        {
            return new List<ProductDTO>();
        }

        return products.Select(product => new ProductDTO
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        }).ToList();
    }

    public static ProductDTOUpdateRequest? ProductUpdateDtoRequest(this Product product)
    {
        if (product is null)
            return null;

        return new ProductDTOUpdateRequest
        {
            Stock = product.Stock,
            RegistrationDate = product.RegistrationDate ?? DateTime.Now 
        };
    }

    public static Product? ProductUpdateDtoRequestToProduct(this ProductDTOUpdateRequest productDTOUpdateRequest)
    {
        if (productDTOUpdateRequest is null) return null;

        return new Product
        {
            Stock = productDTOUpdateRequest.Stock,
            RegistrationDate = productDTOUpdateRequest.RegistrationDate!
        };
    }

    public static ProductDTOUpdateResponse? ToProductUpdateResponse(this Product product)
    {
        if (product is null)
            return null;

        return new ProductDTOUpdateResponse
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            RegistrationDate = (DateTime)product.RegistrationDate!,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };
    }

}
