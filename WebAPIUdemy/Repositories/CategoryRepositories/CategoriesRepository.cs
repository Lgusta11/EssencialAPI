using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;


namespace WebAPIUdemy.Repositories;

public class CategoriesRepository : Repository<Category> ,ICategoryRepository
{
    public CategoriesRepository(CatalogoContext? context) : base(context){}

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _context!.Categories.AnyAsync(c => c.CategoryId == categoryId);
    }

    public async Task<PagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParameters)
    {
        var categories = await GetAllAsync();
        var orderedCategories = categories.OrderBy(p => p.CategoryId).AsQueryable();
        var result = PagedList<Category>.ToPagedList(orderedCategories, categoriesParameters.PageNumber, categoriesParameters.PageSize);
        return result;
    }

    public async Task<PagedList<Category>> GetCategoriesFilterNameAsync(CategoriesFilterName categoriesParams)
    {
        var categories = await GetAllAsync();

        if (!string.IsNullOrEmpty(categoriesParams.Name))
        {
            categories = categories.Where(c => c.Name!.Contains(categoriesParams.Name));
        }
        var filteredCategories = PagedList<Category>.ToPagedList(categories.AsQueryable(), categoriesParams.PageNumber, categoriesParams.PageSize);

        return filteredCategories;
    }
}
