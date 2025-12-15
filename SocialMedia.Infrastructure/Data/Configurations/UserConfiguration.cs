using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK_User");
            builder.ToTable("User");

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired();


            builder.Property(e => e.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("UQ_User_Email");

            builder.Property(e => e.DateOfBirth)
                .HasColumnType("date");

            builder.Property(e => e.Telephone)
                .HasMaxLength(20);

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasMany(d => d.Reviews)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_User");

            builder.HasMany(d => d.Comments)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        }
    }
}
