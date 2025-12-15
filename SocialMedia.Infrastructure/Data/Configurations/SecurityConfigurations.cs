using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;
using Movies.Core.Enum;
using static Dapper.SqlMapper;

namespace Movies.Infrastructure.Data.Configurations
{
    public class SecurityConfiguration : IEntityTypeConfiguration<Security>
    {
        public void Configure(EntityTypeBuilder<Security> builder)
        {
            builder.ToTable("Security");

            builder.Property(e => e.Login)
               .HasMaxLength(50)
               .IsUnicode(false);
            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            builder.Property(e => e.Password)
                .HasMaxLength(200)
                .IsUnicode(false);
            builder.Property(e => e.Role)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasConversion(
                    x => x.ToString(),
                    x => (RoleType)Enum.Parse(typeof(RoleType), x)
                );

            builder.Property(e => e.UserId)
                .IsRequired();
        }
    }
}
