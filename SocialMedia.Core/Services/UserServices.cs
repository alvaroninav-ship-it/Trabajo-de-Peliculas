using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Core.QueryFilters;

namespace Movies.Core.Services
{

    public class UserServices : IUserServices
    {
       
       public readonly IUnitOfWork _unitOfWork;
        public UserServices(IUnitOfWork unitOfWork)
        {
           
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteUserAsync(User user)
        {
            await _unitOfWork.UserRepository.Delete(user.Id);
        }

        public async Task<ResponseData> GetAllUserAsync(
           UserQueryFilter userQueryFilter)
        {
            var users = await _unitOfWork.UserRepository.GetAll();
            if (userQueryFilter.DateOfBirth != null)
            {
                users = users.Where(u => u.DateOfBirth.ToShortDateString() ==
                userQueryFilter.DateOfBirth);
            }
            if (userQueryFilter.IsActive != null)
            {
                users = users.Where(u =>
                u.IsActive == userQueryFilter.IsActive);
            }
            if(userQueryFilter.FirstName!=null)
            {
                users = users.Where(u => u.FirstName == userQueryFilter.FirstName);
            }
            if (userQueryFilter.LastName!=null)
            {
                users = users.Where(u => u.LastName == userQueryFilter.LastName);
            }
            
            var totalCount = users.Count();

           
            var pageUsers = PageList<object>.Create(users, userQueryFilter.PageNumber, userQueryFilter.PageSize);

            
          
            if (pageUsers.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de usuarios recuperados correctamente" } },
                    Pagination = pageUsers,
                    StatusCode = HttpStatusCode.OK,
                    totalcount = totalCount
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pageUsers,
                    StatusCode = HttpStatusCode.OK
                };
            }
        }

        public Task<IEnumerable<User>> GetAllUserAsyncDapper()
        {
            return _unitOfWork.UserRepository.GetAll();
        }

        public Task<User> GetUserAsync(int id)
        {
            return _unitOfWork.UserRepository.GetById(id);
        }

        public Task InsertUserAsync(User user)
        {
           
            return _unitOfWork.UserRepository.Add(user);
        }

        public Task UpdateUserAsync(User user)
        {
            return _unitOfWork.UserRepository.Update(user);
        }

        public async Task<IEnumerable<Top10UsersMostCommentedInTheirReview>> GetTop10UsersMostCommentedInTheirReview()
        {
            var users = await _unitOfWork.UserRepository.GetTop10UsersMostCommentedInTheirReview();
            return users;
        }

        public async Task<IEnumerable<Top10UsersThatHasDoneMoreComments>> GetTop10UsersThatHasDoneMoreComments()
        {
            var users = await _unitOfWork.UserRepository.GetTop10UsersThatHasDoneMoreComments();
            return users;
        }

        async Task<IEnumerable<UsersThatReviewLastYearMovies>> IUserServices.GetUsersThatReviewLastYearMovies()
        {
            var users = await _unitOfWork.UserRepository.GetUsersThatReviewLastYearMovies();
            return users;
        }
    }
}
