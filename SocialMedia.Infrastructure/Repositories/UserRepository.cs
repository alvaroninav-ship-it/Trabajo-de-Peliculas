using Microsoft.EntityFrameworkCore;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.Queries;

namespace Movies.Infrastructure.Repositories
{
    public class UserRepository: BaseRepository<User>, IUserRepository
    {
        private readonly MoviesContext _context;
        private readonly IDapperContext _dapper;
        public UserRepository(MoviesContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
            //_context = context;
        }

        public async Task<IEnumerable<UsersThatReviewLastYearMovies>> GetUsersThatReviewLastYearMovies()
        {
            try
            {
                var sql = UserQueries.UsersThatReviewLastYearMovies;

                return await _dapper.QueryAsync<UsersThatReviewLastYearMovies>(sql);
            }

            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<IEnumerable<Top10UsersMostCommentedInTheirReview>> GetTop10UsersMostCommentedInTheirReview()
        {
            try
            {
                var sql = UserQueries.Top10UsersMostCommentedInTheirReview;

                return await _dapper.QueryAsync<Top10UsersMostCommentedInTheirReview>(sql);
            }

            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<IEnumerable<Top10UsersThatHasDoneMoreComments>> GetTop10UsersThatHasDoneMoreComments()
        {
            try
            {
                var sql = UserQueries.Top10UsersThatHasDoneMoreComments;

                return await _dapper.QueryAsync<Top10UsersThatHasDoneMoreComments>(sql);
            }

            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}
