using FriendWish.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendWish.DataAccess.Configurations;

/// <summary>
/// Уникальность пары пользователей без учёта направления заявки — в <see cref="UniqueConstraints.FriendshipPair"/>.
/// </summary>
internal class FriendshipConfiguration : IEntityTypeConfiguration<FriendshipEntity>
{
    public void Configure(EntityTypeBuilder<FriendshipEntity> builder)
    {
        builder.ToTable("friendships", table =>
            table.HasCheckConstraint("ck_friendships_not_self", "requester_id <> addressee_id"));

        builder.HasKey(friendship => friendship.Id);
        builder.Property(friendship => friendship.Id).ValueGeneratedNever();

        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(friendship => friendship.RequesterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(friendship => friendship.AddresseeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
