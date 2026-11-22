using App.BLL.Interfaces;
using App.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public virtual void Update(T entity) => _dbSet.Update(entity);

    public virtual void Delete(T entity) => _dbSet.Remove(entity);

    public virtual async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
