using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;

namespace WebAPIUdemy.Repositories;

public class CategoriesRepository : Repository<Category> ,ICategoryRepository
{
    public CategoriesRepository(CatalogoContext? context) : base(context){}

    public bool CategoryExists(int categoryId)
    {
        return _context!.Categories.Any(c => c.CategoryId == categoryId);
    }

    public PagedList<Category> GetCategories(CategoriesParameters categoriesParameters)
    {
        var categories = GetAll().OrderBy(p => p.CategoryId).AsQueryable();
        var orderedCategories = PagedList<Category>.ToPagedList(categories, categoriesParameters.PageNumber, categoriesParameters.PageSize);
        return orderedCategories;
    }
}
