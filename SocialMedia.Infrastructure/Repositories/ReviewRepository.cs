using Microsoft.EntityFrameworkCore;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.Queries;

namespace Movies.Infrastructure.Repositories
{
    public class ReviewRepository: BaseRepository<Review>, IReviewRepository
    {
        private readonly MoviesContext _context;
        private readonly IDapperContext _dapper;
        public ReviewRepository(MoviesContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
            //_context = context;
        }

        public async Task<IEnumerable<ReviewsThatRefersAnSpecificGenre>> GetReviewsThatRefersAnSpecificGenre(string genre=" ")
        {
            try
            {
                var sql = ReviewQueries.ReviewsThatRefersAnSpecificGenre;

                return await _dapper.QueryAsync<ReviewsThatRefersAnSpecificGenre>(sql, new { genre = genre });
            }

            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<IEnumerable<ReviewsThatWereDoneByUsers20YearsOldOrYounger>> GetReviewsThatWereDoneByUsers20YearsOldOrYounger()
        {
            try
            {
                var sql = ReviewQueries.ReviewsThatWereDoneByUsers20YearsOldOrYounger;

                return await _dapper.QueryAsync<ReviewsThatWereDoneByUsers20YearsOldOrYounger>(sql);
            }

            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<IEnumerable<Top10MostCommentedReviews>> GetTop10MostCommentedReviews()
        {
            try
            {
                var sql = ReviewQueries.Top10MostCommentedReviews;

                return await _dapper.QueryAsync<Top10MostCommentedReviews>(sql);
            }

            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}
