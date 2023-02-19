using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Microservices.SeedWork;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    DbContext _context;
    DbSet<TEntity> _dbSet;
 
    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
 
    public IEnumerable<TEntity> Get()
    {
        return _dbSet.AsNoTracking().ToList();
    }
         
    public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
    {
        return _dbSet.AsNoTracking().Where(predicate).ToList();
    }
    public TEntity FindById(int id)
    {
        return _dbSet.Find(id);
    }
 
    public void Create(TEntity item)
    {
        _dbSet.Add(item);
        _context.SaveChanges();
    }
    public void Update(TEntity item)
    {
        _context.Entry(item).State = EntityState.Modified;
        _context.SaveChanges();
    }
    public void Remove(TEntity item)
    {
        _dbSet.Remove(item);
        _context.SaveChanges();
    }
    
    public IEnumerable<TEntity> GetWithInclude(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return Include(includeProperties).ToList();
    }
 
    public IEnumerable<TEntity> GetWithInclude(Func<TEntity,bool> predicate, 
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query =  Include(includeProperties);
        return query.Where(predicate).ToList();
    }
 
    public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();
        return includeProperties
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }
}