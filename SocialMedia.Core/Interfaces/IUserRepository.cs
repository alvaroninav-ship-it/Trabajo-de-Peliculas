using Movies.Core.CustomEntities;
using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IUserRepository:IBaseRepository<User>
    {
        Task<IEnumerable<Top10UsersMostCommentedInTheirReview>> GetTop10UsersMostCommentedInTheirReview();
        Task<IEnumerable<UsersThatReviewLastYearMovies>> GetUsersThatReviewLastYearMovies();
        Task<IEnumerable<Top10UsersThatHasDoneMoreComments>> GetTop10UsersThatHasDoneMoreComments();

    }
}
