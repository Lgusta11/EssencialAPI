using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.Repositories;

public class CategoriesRepository : Repository<Category> ,ICategoryRepository
{
    public CategoriesRepository(CatalogoContext? context) : base(context)
    {
    }

  
}
