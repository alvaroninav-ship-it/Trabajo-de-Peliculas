using Movies.Core.CustomEntities;
using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IReviewRepository:IBaseRepository<Review>
    {
        Task<IEnumerable<Top10MostCommentedReviews>> GetTop10MostCommentedReviews();
        Task<IEnumerable<ReviewsThatWereDoneByUsers20YearsOldOrYounger>> GetReviewsThatWereDoneByUsers20YearsOldOrYounger();
        Task<IEnumerable<ReviewsThatRefersAnSpecificGenre>> GetReviewsThatRefersAnSpecificGenre(string genre);

    }
}
