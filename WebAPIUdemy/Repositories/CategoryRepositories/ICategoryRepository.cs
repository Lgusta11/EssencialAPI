using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;

namespace WebAPIUdemy.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> CategoryExistsAsync(int categoryId);
    Task<PagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParameters);
    Task<PagedList<Category>> GetCategoriesFilterNameAsync(CategoriesFilterName categoriesParams);
}
