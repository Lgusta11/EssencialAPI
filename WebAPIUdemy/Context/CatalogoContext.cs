using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Model;
using WebAPIUdemy.Models;

namespace WebAPIUdemy.Context;

public class CatalogoContext: IdentityDbContext<ApplicationUser>
{
    public CatalogoContext(DbContextOptions<CatalogoContext> options)  : base(options)
    {
        
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }


}
