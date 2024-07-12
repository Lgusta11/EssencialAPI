using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.Repositories;

public class CategoriesRepository : ICategoryRepository
{
    private readonly CatalogoContext? _context;

    public CategoriesRepository(CatalogoContext? context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetCategories()
    {
       return _context!.Categories.ToList();
    }

    public Category GetCategory(int id)
    {
        return _context!.Categories.FirstOrDefault(c => c.CategoryId == id);
    }

    public Category Create(Category category)
    {
        if (category is null)
        throw new ArgumentNullException(nameof(category));

        _context!.Categories.Add(category);
        _context!.SaveChanges();

        return category;
    }

    public Category Update(Category category)
    {
        if (category is null)
            throw new ArgumentNullException(nameof(category));

        _context!.Entry(category).State = EntityState.Modified;
        _context!.SaveChanges();
        return category;
    }

    public Category Delete(int id)
    {
        var category = _context!.Categories.Find(id);

        if (category is null)
            throw new ArgumentException(nameof(category));

        _context!.Categories.Remove(category);
        _context!.SaveChanges();
        return category;

    }
}
