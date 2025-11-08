using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;

namespace Movies.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        private readonly IDapperContext _dapper;
        //private readonly SocialMediaContext _context;
        public CommentRepository(MoviesContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
            //_context = context;
        }
    }

}
