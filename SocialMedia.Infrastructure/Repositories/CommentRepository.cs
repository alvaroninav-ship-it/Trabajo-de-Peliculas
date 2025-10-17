using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;

namespace Movies.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly MoviesContext _context;
        public CommentRepository(MoviesContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Comment>> GetAllCommentAsync()
        {
            var comments = await _context.Comments.ToListAsync();
            return comments;
        }

        public async Task<Comment> GetCommentAsync(int id)
        { 
            var comment = await _context.Comments.FirstOrDefaultAsync(
                x => x.Id == id);
            return comment;
        }

        public async Task InsertCommentAsync(Comment comment)
        {
            try
            {
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
