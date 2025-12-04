
using Movies.Core.Entities;

public interface ISecurityService
{
    Task<Security> GetLoginByCredentials(UserLogin userLogin);
    Task RegisterUser(Security security);
}
