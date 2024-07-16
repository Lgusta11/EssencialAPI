using WebAPIUdemy.Model;

namespace WebAPIUdemy.Repositories;

public interface IProductRepository : IRepository<Product>
{
    IEnumerable<Product> GetProductsByCategory(int id);
}
