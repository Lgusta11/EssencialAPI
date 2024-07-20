using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPIUdemy.Context;

namespace WebAPIUdemy.Repositories;

public class Repository<T> : IRepository<T> where T : class
{

    protected readonly CatalogoContext? _context;

    public Repository(CatalogoContext? context)
    {
        _context = context;
    }

    public IEnumerable<T> GetAll()
    {
       return _context!.Set<T>().AsNoTracking().ToList();
    }
    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return _context!.Set<T>().FirstOrDefault(predicate);
    }

    public T? Create(T entity)
    {
        _context!.Set<T>().Add(entity);    
        //_context!.SaveChanges();
        return entity;
    }
    public T? Update(T entity)
    {
        _context!.Entry(entity).State = EntityState.Modified;
        //_context!.SaveChanges();
        return entity;
    }
    public T? Delete(T entity)
    {
        _context!.Set<T>().Remove(entity);
        //_context!.SaveChanges();
        return entity;
    }
}
