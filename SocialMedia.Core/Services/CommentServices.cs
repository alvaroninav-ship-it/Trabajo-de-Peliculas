using System.Net;
using Microsoft.Extensions.Hosting;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Exceptions;
using Movies.Core.Interfaces;
using Movies.Core.QueryFilters;

namespace Movies.Core.Services
{

    public class CommentServices : ICommentServices
    {
        public readonly IUnitOfWork _unitOfWork;
        public CommentServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteCommentAsync(Comment comment)
        {
            await _unitOfWork.CommentRepository.Delete(comment.Id);
        }

        public async Task<ResponseData> GetAllCommentAsync(
          CommentQueryFilter commentQueryFilter)
        {
            var comments = await _unitOfWork.CommentRepository.GetAll();
            if (commentQueryFilter.UserId != null)
            {
                comments = comments.Where(c => c.UserId == commentQueryFilter.UserId);
            }
            if (commentQueryFilter.Date != null)
            {
                comments = comments.Where(c => c.Date.ToShortDateString() ==
                commentQueryFilter.Date);
            }
            if (commentQueryFilter.Description != null)
            {
                comments = comments.Where(x => x.Description.ToLower().Contains(commentQueryFilter.Description.ToLower()));
            }
            if (commentQueryFilter.ReviewId != null)
            {
                comments = comments.Where(c => c.ReviewId == commentQueryFilter.ReviewId);
            }

           
            var totalCount = comments.Count();

            var pageComments = PageList<object>.Create(comments, commentQueryFilter.PageNumber, commentQueryFilter.PageSize);

            

            if (pageComments.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de comentarios recuperados correctamente" } },
                    Pagination = pageComments,
                    StatusCode = HttpStatusCode.OK,
                    totalcount = totalCount
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pageComments,
                    StatusCode = HttpStatusCode.OK
                };
            }
        }
        public Task<IEnumerable<Comment>> GetAllCommentAsyncDapper()
        {
            return _unitOfWork.CommentRepository.GetAll();
        }

        public readonly List<string> ForbiddenWords = new List<string>
            {
                "mierda",
                "puta",
                "cabron",
                "joder",
                "coño"
            };
        public bool ContainsForbiddenWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            foreach (var word in ForbiddenWords)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public Task<Comment> GetCommentAsync(int id)
        {
            return _unitOfWork.CommentRepository.GetById(id);
        }

        public Task InsertCommentAsync(Comment comment)
        {
            if (ContainsForbiddenWords(comment.Description))
            {
                throw new BusinessException("El comentario contiene palabras prohibidas");
            }
            return _unitOfWork.CommentRepository.Add(comment);
        }

       
        public Task UpdateCommentAsync(Comment comment)
        {
            if (ContainsForbiddenWords(comment.Description))
            {
                throw new BusinessException("El comentario contiene palabras prohibidas");
            }
            return _unitOfWork.CommentRepository.Update(comment);
        }

        
    }
}
