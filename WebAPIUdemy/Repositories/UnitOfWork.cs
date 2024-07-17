using WebAPIUdemy.Context;

namespace WebAPIUdemy.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IProductRepository? _productRepository;
        private ICategoryRepository? _categoryRepository;
        public CatalogoContext? _context;

        public UnitOfWork(CatalogoContext? context)
        {
            _context = context;
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

        public void Commit()
        {
            _context!.SaveChanges();
        }

        public void Dispose() 
        {
            _context!.Dispose();        
        }
    }
}
