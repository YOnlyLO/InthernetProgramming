using FriendWish.Core.Common;
using FriendWish.Core.Enums;

namespace FriendWish.Core.Models;

/// <summary>
/// Заявка в друзья между двумя пользователями. Принятая заявка означает дружбу,
/// удаление из друзей — удаление записи.
/// </summary>
public class Friendship
{
    private Friendship(
        Guid id,
        Guid requesterId,
        Guid addresseeId,
        FriendshipStatus status,
        DateTime requestedAt,
        DateTime? respondedAt)
    {
        Id = id;
        RequesterId = requesterId;
        AddresseeId = addresseeId;
        Status = status;
        RequestedAt = requestedAt;
        RespondedAt = respondedAt;
    }

    public Guid Id { get; private set; }

    public Guid RequesterId { get; private set; }

    public Guid AddresseeId { get; private set; }

    public FriendshipStatus Status { get; private set; }

    public DateTime RequestedAt { get; private set; }

    public DateTime? RespondedAt { get; private set; }

    public bool IsAccepted => Status == FriendshipStatus.Accepted;

    public bool IsPending => Status == FriendshipStatus.Pending;

    public static Friendship Create(Guid requesterId, Guid addresseeId)
    {
        Guard.NotEmpty(requesterId, "Отправитель заявки");
        Guard.NotEmpty(addresseeId, "Получатель заявки");

        if (requesterId == addresseeId)
            throw new DomainException("Нельзя отправить заявку в друзья самому себе.");

        return new Friendship(
            Guid.CreateVersion7(),
            requesterId,
            addresseeId,
            FriendshipStatus.Pending,
            DateTime.UtcNow,
            respondedAt: null);
    }

    /// <summary>
    /// Восстанавливает сохранённую заявку без проверок: данные проверены при создании и изменении.
    /// Только для слоя доступа к данным — новую заявку создаёт <see cref="Create"/>.
    /// </summary>
    public static Friendship Restore(
        Guid id,
        Guid requesterId,
        Guid addresseeId,
        FriendshipStatus status,
        DateTime requestedAt,
        DateTime? respondedAt)
    {
        return new Friendship(id, requesterId, addresseeId, status, requestedAt, respondedAt);
    }

    public void Accept(Guid userId)
    {
        EnsureCanRespond(userId);

        Status = FriendshipStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
    }

    public void Decline(Guid userId)
    {
        EnsureCanRespond(userId);

        Status = FriendshipStatus.Declined;
        RespondedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Повторно отправляет отклонённую заявку. Отправить её может любой из двух участников,
    /// поэтому отправитель и получатель могут поменяться местами.
    /// </summary>
    public void Resend(Guid requesterId)
    {
        if (Status != FriendshipStatus.Declined)
            throw new DomainException("Повторно отправить можно только отклонённую заявку.");

        var addresseeId = GetOtherUserId(requesterId);

        RequesterId = requesterId;
        AddresseeId = addresseeId;
        Status = FriendshipStatus.Pending;
        RequestedAt = DateTime.UtcNow;
        RespondedAt = null;
    }

    public bool Involves(Guid userId) => RequesterId == userId || AddresseeId == userId;

    public Guid GetOtherUserId(Guid userId)
    {
        if (userId == RequesterId)
            return AddresseeId;

        if (userId == AddresseeId)
            return RequesterId;

        throw new DomainException("Пользователь не является участником заявки.");
    }

    private void EnsureCanRespond(Guid userId)
    {
        if (userId != AddresseeId)
            throw new DomainException("Ответить на заявку может только её получатель.");

        if (Status != FriendshipStatus.Pending)
            throw new DomainException("Заявка уже рассмотрена.");
    }
}
