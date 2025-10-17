using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;

namespace Movies.Infrastructure.Repositories
{
    public class ActorRepository : IActorRepository
    {
        private readonly MoviesContext _context;
        public ActorRepository(MoviesContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Actor>> GetAllActorAsync()
        {
            var posts = await _context.Actors.ToListAsync();
            return posts;
        }

        public async Task<Actor> GetActorAsync(int id)
        {
            var post = await _context.Actors.FirstOrDefaultAsync(
                x => x.Id == id);
            return post;
        }

        public async Task InsertActorAsync(Actor actor)
        {
            try
            {
                _context.Actors.Add(actor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
            }

        }

        public async Task UpdateActorAsync(Actor actor)
        {
            _context.Actors.Update(actor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteActorAsync(Actor actor)
        {
            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();
        }
    }
}
