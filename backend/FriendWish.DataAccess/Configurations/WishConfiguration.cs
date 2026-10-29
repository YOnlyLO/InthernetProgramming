using FriendWish.Core.Models;
using FriendWish.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendWish.DataAccess.Configurations;

internal class WishConfiguration : IEntityTypeConfiguration<WishEntity>
{
    public void Configure(EntityTypeBuilder<WishEntity> builder)
    {
        builder.ToTable("wishes", table =>
            table.HasCheckConstraint("ck_wishes_price_not_negative", "price >= 0"));

        builder.HasKey(wish => wish.Id);
        builder.Property(wish => wish.Id).ValueGeneratedNever();

        builder.Property(wish => wish.Title).HasMaxLength(Wish.MaxTitleLength);
        builder.Property(wish => wish.Description).HasMaxLength(Wish.MaxDescriptionLength);
        builder.Property(wish => wish.Link).HasMaxLength(Wish.MaxUrlLength);
        builder.Property(wish => wish.ImageUrl).HasMaxLength(Wish.MaxUrlLength);

        // Модель допускает не больше двух знаков после запятой — столько же хранит колонка.
        builder.Property(wish => wish.Price).HasPrecision(18, 2);

        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(wish => wish.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
