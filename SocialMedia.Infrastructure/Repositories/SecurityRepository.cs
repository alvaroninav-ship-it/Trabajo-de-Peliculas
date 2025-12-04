using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.Repositories;

public class SecurityRepository : BaseRepository<Security>, ISecurityRepository
{
    public SecurityRepository(MoviesContext context) : base(context) { }

    public async Task<Security> GetLoginByCredentials(UserLogin login)
    {
        return await _entities.FirstOrDefaultAsync(x => x.Login == login.User
        && x.Password == login.Password);
    }
}


