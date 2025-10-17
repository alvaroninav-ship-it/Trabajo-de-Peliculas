using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.Entities;
using Movies.Core.Interfaces;

namespace Movies.Core.Services
{

    public class UserServices : IUserServices
    {
        public readonly IUserRepository _userRepository;
        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task DeleteUserAsync(User user)
        {
            await _userRepository.DeleteUserAsync(user);
        }

        public Task<IEnumerable<User>> GetAllUserAsync()
        {
            return _userRepository.GetAllUserAsync();
        }

        public Task<User> GetUserAsync(int id)
        {
            return _userRepository.GetUserAsync(id);
        }

        public Task InsertUserAsync(User user)
        {
            return _userRepository.InsertUserAsync(user);
        }

        public Task UpdateUserAsync(User user)
        {
            return _userRepository.InsertUserAsync(user);
        }
    }
}
