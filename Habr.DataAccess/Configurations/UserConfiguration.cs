using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Habr.DataAccess.Entities;
using Habr.Common;

namespace Habr.DataAccess.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(Constants.User.NameMaxLength);

            builder.Property(u => u.Created)
                .IsRequired();

            builder.Property(u => u.IsEmailConfirmed)
                .IsRequired()
                .HasDefaultValue(Constants.User.DefaultIsEmailConfirmed);

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(Constants.User.RefreshTokenMaxLength);

            builder.Property(u => u.RefreshTokenExpiryTime)
                .IsRequired();

            builder.Property(u => u.SecurityStamp)
                .IsRequired()
                .HasMaxLength(Constants.User.SecurityStampMaxLength);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasIndex(u => u.RefreshToken)
                .HasDatabaseName("IX_Users_RefreshToken")
                .IsUnique(false);
        }
    }
}
