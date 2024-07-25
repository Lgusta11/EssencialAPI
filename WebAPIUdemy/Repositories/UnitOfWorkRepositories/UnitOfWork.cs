using Microsoft.AspNetCore.Identity;
using WebAPIUdemy.Context;
using WebAPIUdemy.Models;
using WebAPIUdemy.Repositories.RoleRepositories;
using WebAPIUdemy.Repositories.UserRepositories;

namespace WebAPIUdemy.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IProductRepository? _productRepository;
        private ICategoryRepository? _categoryRepository;
        private IUserRepository? _userRepository;
        private IRoleRepository? _roleRepository;
        private readonly CatalogoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UnitOfWork(CatalogoContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IProductRepository ProductRepository
        {
            get 
            {
                return _productRepository = _productRepository ?? new ProductRepository(_context);
            }
        }
        public ICategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepository = _categoryRepository ?? new CategoriesRepository(_context);
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository = _userRepository ?? new UserRepository(_userManager, _context);
            }
        }

        public IRoleRepository RoleRepository
        {
            get
            {
                return _roleRepository = _roleRepository ?? new RoleRepository(_roleManager);
            }
        }

        public async Task CommitAsync()
        {
            await _context!.SaveChangesAsync();
        }

        public void Dispose() 
        {
            _context!.Dispose();        
        }
    }
}
