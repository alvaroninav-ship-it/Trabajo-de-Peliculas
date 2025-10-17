using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Review> ReviewRepository { get; }
        IBaseRepository<User> UserRepository { get; }   
        IBaseRepository<Movie> MovieRepository { get; }
        IBaseRepository<Comment> CommentRepository { get; }
        IBaseRepository<Actor> ActorRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();



    }
}
