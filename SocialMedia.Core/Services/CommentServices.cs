using Movies.Core.Entities;
using Movies.Core.Interfaces;

namespace Movies.Core.Services
{

    public class CommentServices : ICommentServices
    {
        //public readonly ICommentRepository _commentRepository;
        public readonly IUnitOfWork _unitOfWork;
        public CommentServices(//ICommentRepository commentRepository
                               IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteCommentAsync(Comment comment)
        {
            await _unitOfWork.CommentRepository.Delete(comment.Id);
        }

        public Task<IEnumerable<Comment>> GetAllCommentAsync()
        {
            return _unitOfWork.CommentRepository.GetAllAsync();
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
            return _unitOfWork.CommentRepository.GetByIdAsync(id);
        }

        public Task InsertCommentAsync(Comment comment)
        {
            if (ContainsForbiddenWords(comment.Description))
            {
                throw new Exception("El comentario contiene palabras prohibidas");
            }
            return _unitOfWork.CommentRepository.Insert(comment);
        }

       
        public Task UpdateCommentAsync(Comment comment)
        {
            if (ContainsForbiddenWords(comment.Description))
            {
                throw new Exception("El comentario contiene palabras prohibidas");
            }
            return _unitOfWork.CommentRepository.Update(comment);
        }
    }
}
