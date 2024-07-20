using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;

namespace WebAPIUdemy.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    bool CategoryExists(int categoryId);
    PagedList<Category> GetCategories(CategoriesParameters categoriesParameters);
}
