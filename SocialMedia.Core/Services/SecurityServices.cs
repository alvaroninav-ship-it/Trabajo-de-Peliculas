using Movies.Core.Entities;
using Movies.Core.Interfaces;

public class SecurityService : ISecurityService
{
    private readonly IUnitOfWork _unitOfWork;

    public SecurityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Security> GetLoginByCredentials(UserLogin userLogin)
    {
        return await _unitOfWork.SecurityRepository.GetLoginByCredentials(userLogin);
    }

    public async Task RegisterUser(Security security)
    {
        var User=await _unitOfWork.UserRepository.GetById(security.UserId);
        if (User == null)
        {
            throw new Exception("No existe ese usuario para registrar");
        }
        await _unitOfWork.SecurityRepository.Add(security);
        await _unitOfWork.SaveChangesAsync();
    }
}

