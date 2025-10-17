using Movies.Core.Entities;
using Movies.Core.Interfaces;

namespace Movies.Core.Services
{

    public class MovieServices : IMovieServices
    {
        public readonly IMovieRepository _movieRepository;
        public readonly IUnitOfWork _unitOfWork;
        public MovieServices(IMovieRepository movieRepository,
                             IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _movieRepository = movieRepository;
        }
        public async Task DeleteMovieAsync(Movie movie)
        {
            await _unitOfWork.MovieRepository.Delete(movie.Id);
        }

        public Task<IEnumerable<Movie>> GetAllMovieAsync()
        {
            return _unitOfWork.MovieRepository.GetAllAsync();
        }

        public Task<Movie> GetMovieAsync(int id)
        {
            return _unitOfWork.MovieRepository.GetByIdAsync(id);
        }

        public Task InsertMovieAsync(Movie movie)
        {
            return _unitOfWork.MovieRepository.Insert(movie);
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            var existingMovie =  await _movieRepository.GetMovieByTittle(movie.Title);
            if (existingMovie != null)
            {
                throw new Exception("Ya existe una pelicula con ese titulo");
            }
            if(!Genres.Contains(movie.Genre))
            {
                throw new Exception("El genero no es valido");
            }
            await _unitOfWork.MovieRepository.Update(movie);
        }

        public readonly List<string> Genres = new List<string>
            {
                "Terror",
                "Accion",
                "Romantica",
                "Suspenso",
                "Musical"
            };


        public async Task InsertMovieJustOne(Movie movie)
        {
            var exist = await _movieRepository.GetMovieByTittle(movie.Title);
            if (exist != null)
            {
                throw new Exception("La pelicula ya existe");
            }
            if (!Genres.Contains(movie.Genre))
            {
                throw new Exception("El genero no es valido");
            }
            await _movieRepository.InsertMovieAsync(movie);
        }
    }
}
