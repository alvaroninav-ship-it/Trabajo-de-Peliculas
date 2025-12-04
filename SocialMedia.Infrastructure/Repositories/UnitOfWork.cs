using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Infrastructure.Data;

namespace Movies.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MoviesContext _context;
        public readonly IReviewRepository _reviewRepository;
        public readonly IUserRepository _userRepository;
        public readonly IMovieRepository _movieRepository;
        public readonly IBaseRepository<Comment>? _commentRepository;
        public readonly IActorRepository _actorRepository;
        public readonly IDapperContext _dapper;
        private readonly ISecurityRepository _securityRepository;


        private IDbContextTransaction? _efTransaction;
        public UnitOfWork(MoviesContext context, IDapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }
        public IReviewRepository ReviewRepository =>
            _reviewRepository ?? new ReviewRepository(_context, _dapper);

        public IUserRepository UserRepository =>
            _userRepository ?? new UserRepository(_context, _dapper);

        public IMovieRepository MovieRepository =>
           _movieRepository ?? new MovieRepository(_context, _dapper);

        public IBaseRepository<Comment> CommentRepository => 
             _commentRepository ?? new BaseRepository<Comment>(_context);

        public IActorRepository ActorRepository =>
            _actorRepository ?? new ActorRepository(_context, _dapper);

        public ISecurityRepository SecurityRepository =>
           _securityRepository ?? new SecurityRepository(_context);



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

        public async Task BeginTransactionAsync() 
        {
            if (_efTransaction == null) {
                _efTransaction = await _context.Database.BeginTransactionAsync();

                var conn = _context.Database.GetDbConnection();

                var tx = _efTransaction.GetDbTransaction();

                _dapper.SetAmbientConnection(conn, tx);
            }
        }

        public async Task CommintTrasaction()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_efTransaction != null)
                {
                    await _efTransaction.CommitAsync();
                    _efTransaction.Dispose();
                    _efTransaction = null;
                }
            }
            catch(Exception)
            {

            }
            finally
            {
                _dapper.ClearAmbientConnection();
            }
        }

        public async Task RollBackTransaction()
        {

            if (_efTransaction != null)
            {
                await _efTransaction.RollbackAsync();
                _efTransaction.Dispose();
                _efTransaction = null;
            }
            _dapper.ClearAmbientConnection();


        }

        public IDbConnection? GetdbConnection()
        {
            return _context.Database.GetDbConnection(); 
        }

        public IDbTransaction? GetDbTransaction()
        {
            return _efTransaction?.GetDbTransaction();
        }
    }
}
