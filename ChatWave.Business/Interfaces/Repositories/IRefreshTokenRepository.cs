namespace ChatWave.Business.Interfaces.Repositories;

public record RefreshTokenRecord(
    int Id,
    int UserId,
    string TokenHash,
    Guid FamilyId,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc,
    DateTime? RevokedAtUtc,
    string? ReplacedByTokenHash);

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshTokenRecord token);
    Task<RefreshTokenRecord?> GetByHashAsync(string tokenHash);
    Task<bool> TryRevokeAsync(int id, string? replacedByTokenHash, DateTime nowUtc);
    Task RevokeFamilyAsync(Guid familyId, DateTime nowUtc);
    Task RevokeAllForUserAsync(int userId, DateTime nowUtc);
}