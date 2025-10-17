using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;

namespace Movies.Infrastructure.Repositories
{
    public class BaseRepository<T> :IBaseRepository<T> where T : BaseEntity
    { 
        private readonly MoviesContext _context;
        private readonly DbSet<T> _entities;
        public BaseRepository(MoviesContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var entities = await _entities.ToListAsync();
                return entities;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                return Enumerable.Empty<T>();
            }
        }
        public async Task<T> GetByIdAsync(int? id)
        {
            var entity = await _entities.FindAsync(id);
            return entity;
        }
        public async Task Insert(T entity)
        {
            _entities.Add(entity);
            await _context.SaveChangesAsync();
        }
        public async Task Update(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            try
            {
                T entity = await GetByIdAsync(id);
                _entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
            }
        }
    }
}
