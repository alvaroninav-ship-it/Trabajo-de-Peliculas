using Movies.Core.Entities;
using Movies.Core.Interfaces;

namespace Movies.Core.Services
{

    public class ActorServices : IActorServices
    {
        //public readonly IActorRepository _actorRepository;
        //public readonly IMovieRepository _movieRepository;
        //public readonly IBaseRepository<Actor> _actorRepository;
        //public readonly IBaseRepository<Movie> _movieRepository;

        public readonly IUnitOfWork _unitOfWork;
        public ActorServices(//IActorRepository actorRepository,IMovieRepository movieRepository
                             //IBaseRepository<Actor> actorRepository, IBaseRepository<Movie> movieRepository,
                             IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteActorAsync(Actor actor)
        {
            await _unitOfWork.ActorRepository.Delete(actor.Id);
        }

        public async Task<IEnumerable<Actor>> GetAllActorAsync()
        {
            return await _unitOfWork.ActorRepository.GetAllAsync();
        }

        public Task<Actor> GetActorAsync(int id)
        {
            return _unitOfWork.ActorRepository.GetByIdAsync(id);
        }

        public Task InsertActorAsync(Actor actor)
        {
            return _unitOfWork.ActorRepository.Insert(actor);
        }

        public async Task UpdateActorAsync(Actor actor)
        {
            var movie = await _unitOfWork.MovieRepository.GetByIdAsync(actor.MovieId);
            if (movie == null)
            {
                throw new Exception("La pelicula no existe");
            }
            await  _unitOfWork.ActorRepository.Update(actor);
        }
        public async Task InsertActorByMovieExist(Actor actor)
        {
            var movie = await _unitOfWork.MovieRepository.GetByIdAsync(actor.MovieId);
            if (movie == null)
            {
                throw new Exception("La pelicula no existe");
            }
            await _unitOfWork.ActorRepository.Insert(actor);
        }   
    }
}
