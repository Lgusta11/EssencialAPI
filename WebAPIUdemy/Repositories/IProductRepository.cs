using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;

namespace WebAPIUdemy.Repositories;

public interface IProductRepository : IRepository<Product>
{
    PagedList<Product> GetProducts(ProductsParameters productsParams);
    IEnumerable<Product> GetProductsByCategory(int id);
}
