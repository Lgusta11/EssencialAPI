using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;

namespace WebAPIUdemy.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    bool CategoryExists(int categoryId);
    PagedList<Category> GetCategories(CategoriesParameters categoriesParameters);
    PagedList<Category> GetCategoriesFilterName(CategoriesFilterName categoriesParams);
}
