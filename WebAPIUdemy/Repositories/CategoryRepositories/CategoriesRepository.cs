using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;

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

    public PagedList<Category> GetCategoriesFilterName(CategoriesFilterName categoriesParams)
    {
        var categories = GetAll().AsQueryable();

        if (!string.IsNullOrEmpty(categoriesParams.Name))
        {
            categories = categories.Where(c => c.Name.Contains(categoriesParams.Name));
        }
        var filteredCategories = PagedList<Category>.ToPagedList(categories, categoriesParams.PageNumber, categoriesParams.PageSize);

        return filteredCategories;
    }
}
