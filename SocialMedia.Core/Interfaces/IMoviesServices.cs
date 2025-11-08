using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.QueryFilters;

namespace Movies.Core.Interfaces
{
    public interface IMovieServices
    {
        Task<ResponseData> GetAllMovieAsync(MovieQueryFilter movieQueryFilter);
        Task<IEnumerable<Movie>> GetAllMovieAsyncDapper();
        Task<MostFamousMovieForYear> GetMostFamousMovieForYear(int year);
        Task<IEnumerable<Top10MoviesThatHasMostActors>> Gettop10MoviesThatHasMostActors();
        Task<Movie> GetMovieAsync(int id);
        Task InsertMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(Movie movie);

        Task InsertMovieJustOne(Movie movie);

    }
}
