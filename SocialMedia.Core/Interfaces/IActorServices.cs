using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IActorServices
    {
        Task<IEnumerable<Actor>> GetAllActorAsync();
        Task<Actor> GetActorAsync(int id);
        Task InsertActorAsync(Actor actor);
        Task UpdateActorAsync(Actor actor);
        Task DeleteActorAsync(Actor actor);
        Task InsertActorByMovieExist(Actor actor);
    }
}