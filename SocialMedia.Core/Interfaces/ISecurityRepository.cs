
using Movies.Core.Entities;
using Movies.Core.Interfaces;

public interface ISecurityRepository : IBaseRepository<Security>
{
    Task<Security> GetLoginByCredentials(UserLogin login);
}