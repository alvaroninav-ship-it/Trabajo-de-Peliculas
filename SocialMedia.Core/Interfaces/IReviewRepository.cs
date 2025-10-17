using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllReviewAsync();
        Task<Review> GetReviewAsync(int id);
        Task InsertReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
    }
}
