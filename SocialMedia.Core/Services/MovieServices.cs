using System.Net;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Exceptions;
using Movies.Core.Interfaces;
using Movies.Core.QueryFilters;

namespace Movies.Core.Services
{

    public class MovieServices : IMovieServices
    {
        public readonly IUnitOfWork _unitOfWork;
        public MovieServices(
                             IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllMovieAsync(
          MovieQueryFilter movieQueryFilter)
        {
            var movies = await _unitOfWork.MovieRepository.GetAll();
            if (movieQueryFilter.ReleaseDate != null)
            {
                movies = movies.Where(c => c.ReleaseDate.ToShortDateString() ==
                movieQueryFilter.ReleaseDate);
            }
            if (movieQueryFilter.Description != null)
            {
                movies = movies.Where(x => x.Description.ToLower().Contains(movieQueryFilter.Description.ToLower()));
            }
            if (movieQueryFilter.Genre != null)
            {
                movies = movies.Where(c => c.Genre == movieQueryFilter.Genre);
            }
            if (movieQueryFilter.Length != null)
            {
                movies = movies.Where(c => c.Length == movieQueryFilter.Length);
            }
            if (movieQueryFilter.Title != null)
            {
                movies = movies.Where(c => c.Title == movieQueryFilter.Title);
            }

        
            var totalCount = movies.Count();

          
            var pageMovies = PageList<object>.Create(movies, movieQueryFilter.PageNumber, movieQueryFilter.PageSize);

            
            if (pageMovies.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de peliculas recuperados correctamente" } },
                    Pagination = pageMovies,
                    StatusCode = HttpStatusCode.OK,
                    totalcount = totalCount
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pageMovies,
                    StatusCode = HttpStatusCode.OK
                };
            }
        }

        public async Task DeleteMovieAsync(Movie movie)
        {
            await _unitOfWork.MovieRepository.Delete(movie.Id);
        }

        public Task<IEnumerable<Movie>> GetAllMovieAsyncDapper()
        {
            return _unitOfWork.MovieRepository.GetAll();
        }

        public Task<Movie> GetMovieAsync(int id)
        {
            return _unitOfWork.MovieRepository.GetById(id);
        }

        public Task InsertMovieAsync(Movie movie)
        {
            return _unitOfWork.MovieRepository.Add(movie);
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            var existingMovie =  await _unitOfWork.MovieRepository.GetMovieByTittle(movie.Title);
            if (existingMovie != null)
            {
                throw new BusinessException("Ya existe una pelicula con ese titulo");
            }
            if(!Genres.Contains(movie.Genre))
            {
                throw new BusinessException("El genero no es valido");
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
            var exist = await _unitOfWork.MovieRepository.GetMovieByTittle(movie.Title);
            if (exist != null)
            {
                throw new BusinessException("La pelicula ya existe");
            }
            if (!Genres.Contains(movie.Genre))
            {
                throw new BusinessException("El genero no es valido");
            }
            await _unitOfWork.MovieRepository.Add(movie);
        }

        public async Task<MostFamousMovieForYear> GetMostFamousMovieForYear(int year)
        {
            var movie = await _unitOfWork.MovieRepository.GetMostFamousMovieForYear(year);
            return movie;
        }

        public async Task<IEnumerable<Top10MoviesThatHasMostActors>> Gettop10MoviesThatHasMostActors()
        {
            var movies = await _unitOfWork.MovieRepository.Gettop10MoviesThatHasMostActors();
            return movies;
        }
    }
}
