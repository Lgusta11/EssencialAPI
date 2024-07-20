using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;

namespace WebAPIUdemy.Repositories;

public interface IProductRepository : IRepository<Product>
{
    PagedList<Product> GetProducts(ProductsParameters productsParams);
    PagedList<Product> GetProductsFilterPrice(ProductsFilterPrice productsFilterParams);
    IEnumerable<Product> GetProductsByCategory(int id);
}
