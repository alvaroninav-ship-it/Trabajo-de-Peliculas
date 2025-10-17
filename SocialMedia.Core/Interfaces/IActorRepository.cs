using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IActorRepository
    {
        Task<IEnumerable<Actor>> GetAllActorAsync();
        Task<Actor> GetActorAsync(int id);
        Task InsertActorAsync(Actor actor);
        Task UpdateActorAsync(Actor actor);
        Task DeleteActorAsync(Actor actor);
    }
}
