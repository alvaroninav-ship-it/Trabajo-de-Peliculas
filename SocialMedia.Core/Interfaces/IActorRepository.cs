using Microsoft.Extensions.Hosting;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IActorRepository:IBaseRepository<Actor>
    {
        Task<IEnumerable<Top10TheYoungestActors>> GetTop10TheYoungestActors();

    }
}
