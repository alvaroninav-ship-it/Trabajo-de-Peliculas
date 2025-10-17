using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IReviewServices
    {
        Task<IEnumerable<Review>> GetAllReviewAsync();
        Task<Review> GetReviewAsync(int id);
        Task InsertReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
        Task InsertReviewByUserAsync(Review review);
    }
}
