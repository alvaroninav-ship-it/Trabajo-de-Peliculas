using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.QueryFilters;

namespace Movies.Core.Interfaces
{
    public interface ICommentServices
    {
        Task<ResponseData> GetAllCommentAsync(CommentQueryFilter commentQueryFilter);

        Task<IEnumerable<Comment>> GetAllCommentAsyncDapper();
        Task<Comment> GetCommentAsync(int id);
        Task InsertCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Comment comment);
    }
}
