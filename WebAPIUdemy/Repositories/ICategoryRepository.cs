using WebAPIUdemy.Model;

namespace WebAPIUdemy.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    bool CategoryExists(int categoryId);
}
