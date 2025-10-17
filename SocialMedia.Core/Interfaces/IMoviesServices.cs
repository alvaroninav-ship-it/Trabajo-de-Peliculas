using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IMovieServices
    {
        Task<IEnumerable<Movie>> GetAllMovieAsync();
        Task<Movie> GetMovieAsync(int id);
        Task InsertMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(Movie movie);

        Task InsertMovieJustOne(Movie movie);

    }
}
