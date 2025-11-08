using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.QueryFilters;

namespace Movies.Core.Interfaces
{
    public interface IActorServices
    {
        Task<ResponseData> GetAllActorAsync(ActorQueryFilter actorQueryFilter);
        Task<IEnumerable<Actor>> GetAllActorDapperAsync();
        Task<IEnumerable<Top10TheYoungestActors>> GetTop10TheYoungestActors();

        Task<Actor> GetActorAsync(int id);
        Task InsertActorAsync(Actor actor);
        Task UpdateActorAsync(Actor actor);
        Task DeleteActorAsync(Actor actor);
    }
}