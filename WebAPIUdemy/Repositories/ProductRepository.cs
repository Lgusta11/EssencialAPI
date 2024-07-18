using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;

namespace WebAPIUdemy.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository 
{


    public ProductRepository(CatalogoContext? context) : base(context)
    {
    }

    //public IEnumerable<Product> GetProducts(ProductsParameters productsParams)
    //{
    //   return GetAll()
    //        .OrderBy(p => p.Name)
    //        .Skip((productsParams.PageNumber - 1) * productsParams.PageSize )
    //        .Take(productsParams.PageSize).ToList();
    //}


    public PagedList<Product> GetProducts(ProductsParameters productsParams)
    {
        var products = GetAll().OrderBy(p => p.ProductId).AsQueryable();
        var orderedProducts = PagedList<Product>.ToPagedList(products, productsParams.PageNumber, productsParams.PageSize);
        return orderedProducts;
    }

    public IEnumerable<Product> GetProductsByCategory(int id)
    {
        return GetAll().Where(c => c.CategoryId == id);
    }
}
