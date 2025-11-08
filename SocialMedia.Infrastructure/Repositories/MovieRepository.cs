using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.Queries;

namespace Movies.Infrastructure.Repositories
{
    public class MovieRepository : BaseRepository<Movie>, IMovieRepository
    {
        private readonly IDapperContext _dapper;
  
        public MovieRepository(MoviesContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        public async Task<Movie> GetMovieByTittle(string title)
        {
            var movie = await _entities.FirstOrDefaultAsync(x => x.Title == title);
            return movie;
        }

        public async Task<MostFamousMovieForYear> GetMostFamousMovieForYear(int year = 2025)
        {
            try
            {
                var sql = MovieQueries.MostFamousMovieForYear;

                return await _dapper.QueryFirstOrDefaultAsync<MostFamousMovieForYear>(sql, new { year = year });
            }

            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<IEnumerable<Top10MoviesThatHasMostActors>> Gettop10MoviesThatHasMostActors()
        {
            try
            {
                var sql = MovieQueries.Top10MoviesThatHasMostActors;

                return await _dapper.QueryAsync<Top10MoviesThatHasMostActors>(sql);
            }

            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}
