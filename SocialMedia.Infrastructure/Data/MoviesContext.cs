using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;

namespace Movies.Infrastructure.Data;
public partial class MoviesContext:DbContext
{
    public MoviesContext()
    {

    }
    public MoviesContext(DbContextOptions<MoviesContext> options)
       : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<Actor> Actors { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<Security> Securities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

