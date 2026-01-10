using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Linq.Expressions;

namespace Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected NewsManagermentDbContext _context;

        public GenericRepository(NewsManagermentDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<bool> CreateAsync(T entity)
        {
            try
            {
                var result = await _context.Set<T>().AddAsync(entity);
                return result.Entity != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _context.ChangeTracker.Clear();
                var tracker = _context.Attach(entity);
                tracker.State = EntityState.Modified;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            try
            {
                _context.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetAllAsQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes) 
                query = query.Include(include);

            if (predicate != null) 
                query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
