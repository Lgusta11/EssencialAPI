using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.Repositories;

public class CategoriesRepository : Repository<Category> ,ICategoryRepository
{
    public CategoriesRepository(CatalogoContext? context) : base(context){}

    public bool CategoryExists(int categoryId)
    {
        return _context.Categories.Any(c => c.CategoryId == categoryId);
    }
}
