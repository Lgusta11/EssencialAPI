using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository 
{


    public ProductRepository(CatalogoContext? context) : base(context)
    {
    }

   

    public IEnumerable<Product> GetProductsByCategory(int id)
    {
        return GetAll().Where(c => c.CategoryId == id);
    }
}
