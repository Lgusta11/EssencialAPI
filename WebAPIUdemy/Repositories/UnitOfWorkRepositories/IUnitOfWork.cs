using WebAPIUdemy.Repositories.RoleRepositories;
using WebAPIUdemy.Repositories.UserRepositories;

namespace WebAPIUdemy.Repositories;

public interface IUnitOfWork
{
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    Task CommitAsync();
}
