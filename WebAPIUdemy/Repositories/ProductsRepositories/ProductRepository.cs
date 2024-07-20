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


    public PagedList<Product> GetProducts(ProductsParameters productsParams)
    {
        var products = GetAll().OrderBy(p => p.ProductId).AsQueryable();
        var orderedProducts = PagedList<Product>.ToPagedList(products, productsParams.PageNumber, productsParams.PageSize);
        return orderedProducts;
    }


    public IEnumerable<Product> GetProductsByCategory(int id)
    {
        return GetAll().Where(c => c.CategoryId == id);
    }

    public PagedList<Product> GetProductsFilterPrice(ProductsFilterPrice productsFilterParams)
    {
        var products = GetAll().AsQueryable();

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
        var filteredProducts = PagedList<Product>.ToPagedList(products, productsFilterParams.PageNumber, productsFilterParams.PageSize);

        return filteredProducts;
    }
}
