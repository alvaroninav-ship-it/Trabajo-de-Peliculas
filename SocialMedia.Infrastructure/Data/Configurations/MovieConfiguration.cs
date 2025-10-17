using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Infrastructure.Data.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK_Movie");
            builder.ToTable("Movie");

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Title)
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(e => e.ReleaseDate)
                .HasColumnType("date");

            builder.Property(e => e.Length)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.Genre)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasMany(d => d.Actors)
                .WithOne(p => p.Movie)
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Actor_Movie");

            builder.HasMany(d => d.Reviews)
                .WithOne(p => p.Movie)
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Movie");
        }
    }
}
