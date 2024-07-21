using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;

namespace WebAPIUdemy.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository 
{


    public ProductRepository(CatalogoContext? context) : base(context)
    {
    }


    public async Task<PagedList<Product>> GetProductsAsync(ProductsParameters productsParams)
    {
        var products = await GetAllAsync();
        var orderedProducts = products.OrderBy(p => p.ProductId).AsQueryable();
        var result = PagedList<Product>.ToPagedList(orderedProducts, productsParams.PageNumber, productsParams.PageSize);
        return result;
    }


    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
    {
        var products = await GetAllAsync();
        var productsCategory = products.Where(c => c.CategoryId == id);
        return productsCategory;
    }

    public async Task<PagedList<Product>> GetProductsFilterPriceAsync(ProductsFilterPrice productsFilterParams)
    {
        var products = await GetAllAsync();

        if (productsFilterParams.Price.HasValue && !string.IsNullOrEmpty(productsFilterParams.PriceCriterion))
        {
            if (productsFilterParams.PriceCriterion.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                products = products.Where(p => p.Price > productsFilterParams.Price.Value).OrderBy(p => p.Price);
            }
            else if (productsFilterParams.PriceCriterion.Equals("menor", StringComparison.OrdinalIgnoreCase))
            {
                products = products.Where(p => p.Price < productsFilterParams.Price.Value).OrderBy(p => p.Price);
            }
            else if (productsFilterParams.PriceCriterion.Equals("igual", StringComparison.OrdinalIgnoreCase))
            {
                products = products.Where(p => p.Price == productsFilterParams.Price.Value).OrderBy(p => p.Price);
            }
        }
        var filteredProducts = PagedList<Product>.ToPagedList(products.AsQueryable(), productsFilterParams.PageNumber, productsFilterParams.PageSize);

        return filteredProducts;
    }
}
