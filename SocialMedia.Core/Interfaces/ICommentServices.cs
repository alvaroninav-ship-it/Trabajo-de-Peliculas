using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface ICommentServices
    {
        Task<IEnumerable<Comment>> GetAllCommentAsync();
        Task<Comment> GetCommentAsync(int id);
        Task InsertCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Comment comment);
    }
}
