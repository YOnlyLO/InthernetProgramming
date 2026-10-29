using FriendWish.Core.Models;
using FriendWish.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendWish.DataAccess.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        // Id задаёт доменная модель (Guid v7), а не база данных.
        builder.Property(user => user.Id).ValueGeneratedNever();

        builder.Property(user => user.Name).HasMaxLength(User.MaxNameLength);
        builder.Property(user => user.UserName).HasMaxLength(User.MaxUserNameLength);
        builder.Property(user => user.Email).HasMaxLength(User.MaxEmailLength);
        builder.Property(user => user.AvatarUrl).HasMaxLength(User.MaxAvatarUrlLength);

        // Модель хранит email и юзернейм в нижнем регистре, поэтому хватает обычных уникальных индексов.
        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasDatabaseName(UniqueConstraints.UserEmail);

        builder.HasIndex(user => user.UserName)
            .IsUnique()
            .HasDatabaseName(UniqueConstraints.UserName);
    }
}
