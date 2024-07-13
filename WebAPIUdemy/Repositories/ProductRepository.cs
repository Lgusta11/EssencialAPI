using WebAPIUdemy.Context;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogoContext? _context;

    public ProductRepository(CatalogoContext? context)
    {
        _context = context;
    }

    public IQueryable<Product> GetProducts()
    {
        return _context!.Products;
    }
    public Product GetProduct(int id)
    {
        return _context.Products.FirstOrDefault(p => p.ProductId == id);
    }
    public Product Create(Product product)
    {
        if (product == null) 
            throw new ArgumentNullException(nameof(product));

        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }
    public bool Update(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        if (_context.Products.Any(p=> p.ProductId == product.ProductId))
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            return true;
        }
        return false;

    }
    public bool Delete(int id)
    {
        var product = _context.Products.Find(id);
        
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
            return true;
        }
        return false;
    }
}
