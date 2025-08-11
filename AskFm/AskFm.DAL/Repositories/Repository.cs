using System.Linq.Expressions;
using AskFm.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace AskFm.DAL.Repositories;

public class Repository<T>  : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    
    
    public IQueryable<T> GetAll() => _dbSet.AsQueryable();
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    

    
    
    public T? GetById(int id)  => _dbSet.Find(id);
    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    
    
    
    
    public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate, string[] includes = null)
    {
        IQueryable<T> query = _dbSet;
        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }
        return query.Where(predicate);
    }

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, string[] includes = null)
    {
        IQueryable<T> query = _dbSet;

        if (includes != null)
            foreach (var include in includes)
                query = query.Include(include);

        return await query.Where(predicate).ToListAsync();
    }

    
    
    
    
    public T? Find(Expression<Func<T, bool>> predicate, string[] includes = null)
    {
        IQueryable<T> query = _dbSet;
        
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        
        return  query.FirstOrDefault(predicate);
    }
    
    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, string[] includes = null)
    {
        IQueryable<T> query = _dbSet;
        
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        
        return await query.FirstOrDefaultAsync(predicate);
    }
    
    
    
    
    public void Add(T entity) => _dbSet.Add(entity);

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    
    
    
    
    public void Update(T entity) => _dbSet.Update(entity);
    
    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
    
    
    public void Remove(T entity) => _dbSet.Remove(entity);
    
    public Task RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

}