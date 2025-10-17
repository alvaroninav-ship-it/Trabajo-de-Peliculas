using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;

namespace Movies.Infrastructure.Repositories
{
    public class ReviewRepository:IReviewRepository
    {
        private readonly MoviesContext _context;
        public ReviewRepository(MoviesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllReviewAsync()
        {
            var reviews = await _context.Reviews.ToListAsync();
            return reviews;
        }

        public async Task<Review> GetReviewAsync(int id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(
                x => x.Id == id);
            return review;
        }

        public async Task InsertReviewAsync(Review review)
        {
            try
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        public async Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

    }
}
