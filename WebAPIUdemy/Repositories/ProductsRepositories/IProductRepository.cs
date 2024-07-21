using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;

namespace WebAPIUdemy.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<PagedList<Product>> GetProductsAsync(ProductsParameters productsParams);
    Task<PagedList<Product>> GetProductsFilterPriceAsync(ProductsFilterPrice productsFilterParams);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id);
}
