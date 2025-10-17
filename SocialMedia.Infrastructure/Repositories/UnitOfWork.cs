using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;

namespace Movies.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MoviesContext _context;
        public readonly IBaseRepository<Review> _reviewRepository;
        public readonly IBaseRepository<User> _userRepository;
        public readonly IBaseRepository<Movie> _movieRepository;
        public readonly IBaseRepository<Comment> _commentRepository;
        public readonly IBaseRepository<Actor> _actorRepository;
        public UnitOfWork(MoviesContext context)
        {
            _context = context;
        }
        public IBaseRepository<Review> ReviewRepository =>
            _reviewRepository ?? new BaseRepository<Review>(_context);

        public IBaseRepository<User> UserRepository => 
            _userRepository ??new BaseRepository<User>(_context);

        public IBaseRepository<Movie> MovieRepository =>
            _movieRepository ?? new BaseRepository<Movie>(_context);

        public IBaseRepository<Comment> CommentRepository => 
             _commentRepository ?? new BaseRepository<Comment>(_context);

        public IBaseRepository<Actor> ActorRepository =>
            _actorRepository ?? new BaseRepository<Actor>(_context);

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            _context.SaveChangesAsync();
        }
    }
}
