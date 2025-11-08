using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.QueryFilters;

namespace Movies.Core.Interfaces
{
    public interface IReviewServices
    {
        Task<ResponseData> GetAllReviewAsync(ReviewQueryFilter reviewQueryFilter);
        Task<IEnumerable<Review>> GetAllReviewAsyncDapper();
        Task<IEnumerable<Top10MostCommentedReviews>> GetTop10MostCommentedReviews();
        Task<IEnumerable<ReviewsThatWereDoneByUsers20YearsOldOrYounger>> GetReviewsThatWereDoneByUsers20YearsOldOrYounger();
        Task<IEnumerable<ReviewsThatRefersAnSpecificGenre>> GetReviewsThatRefersAnSpecificGenre(string genre);

        Task<Review> GetReviewAsync(int id);
        Task InsertReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
        Task InsertReviewByUserAsync(Review review);
    }
}
