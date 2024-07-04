using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.Context;

public class CatalogoContext: DbContext
{
    public CatalogoContext(DbContextOptions<CatalogoContext> options)  : base(options)
    {
        
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }


}
