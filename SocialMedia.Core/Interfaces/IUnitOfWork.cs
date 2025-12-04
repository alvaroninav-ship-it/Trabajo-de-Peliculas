using System.Data;
using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IReviewRepository ReviewRepository { get; }
        IUserRepository UserRepository { get; }   
        IMovieRepository MovieRepository { get; }
        IBaseRepository<Comment> CommentRepository { get; }
        IActorRepository ActorRepository { get; }

        ISecurityRepository SecurityRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();


        Task BeginTransactionAsync();

        Task CommintTrasaction();

        Task RollBackTransaction();

        // nuevos miembros 

        IDbConnection? GetdbConnection();

        IDbTransaction? GetDbTransaction();
    }
}
