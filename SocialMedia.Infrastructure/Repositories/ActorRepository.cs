using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.Queries;
using Movies.Infrastructure.Repositories;

namespace Movies.Infrastructure.Repositories
{
    public class ActorRepository : BaseRepository<Actor>, IActorRepository
    {
        private readonly IDapperContext _dapper;
        //private readonly SocialMediaContext _context;
        public ActorRepository(MoviesContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
            //_context = context;
        }

        public async Task<IEnumerable<Top10TheYoungestActors>> GetTop10TheYoungestActors()
        {
            try
            {
                var sql = ActorQueries.Top10TheYoungestActors;

                return await _dapper.QueryAsync<Top10TheYoungestActors>(sql);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}
