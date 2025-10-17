using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;

namespace Movies.Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MoviesContext _context;
        public MovieRepository(MoviesContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Movie>> GetAllMovieAsync()
        {
            var movies = await _context.Movies.ToListAsync();
            return movies;
        }

        public async Task<Movie> GetMovieAsync(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(
                x => x.Id == id);
            return movie;
        }

        public async Task InsertMovieAsync(Movie movie)
        {
            try { 
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
            }

        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMovieAsync(Movie movie)
        {

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

        }

        public async Task<Movie> GetMovieByTittle(string tittle)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(
                x => x.Title == tittle);
            return movie;
        }
    }
}
