using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.QueryFilters;

namespace Movies.Core.Interfaces
{
    public interface IUserServices
    {
        Task<ResponseData> GetAllUserAsync(UserQueryFilter userQueryFilter);
        Task<IEnumerable<User>> GetAllUserAsyncDapper();
        Task<IEnumerable<Top10UsersMostCommentedInTheirReview>> GetTop10UsersMostCommentedInTheirReview();
        Task<IEnumerable<UsersThatReviewLastYearMovies>> GetUsersThatReviewLastYearMovies();
        Task<IEnumerable<Top10UsersThatHasDoneMoreComments>> GetTop10UsersThatHasDoneMoreComments();
        Task<User> GetUserAsync(int id);
        Task InsertUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
    }
}
