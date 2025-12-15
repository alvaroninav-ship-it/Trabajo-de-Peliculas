using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.Repositories;

public class SecurityRepository : BaseRepository<Security>, ISecurityRepository
{
    public SecurityRepository(MoviesContext context) : base(context) { }

    public async Task<Security> GetLoginByCredentials(UserLogin ulogin)
    {
        var user = await _entities.FirstOrDefaultAsync(x => x.Login == ulogin.User);
        if(user == null)
        {
            throw new Exception("No existe el usuario");
        }
        return user;
    }
}


