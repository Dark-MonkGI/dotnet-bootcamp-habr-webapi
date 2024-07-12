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

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(Constants.User.EmailMaxLength);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Created)
                .IsRequired();

            builder.Property(u => u.IsEmailConfirmed)
                .IsRequired()
                .HasDefaultValue(Constants.User.DefaultIsEmailConfirmed);

            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
