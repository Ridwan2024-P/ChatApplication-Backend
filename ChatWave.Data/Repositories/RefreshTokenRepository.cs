using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Data.Data;
using ChatWave.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatWave.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ChatWaveDbContext _db;
    public RefreshTokenRepository(ChatWaveDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(RefreshTokenRecord token)
    {
        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = token.UserId,
            TokenHash = token.TokenHash,
            FamilyId = token.FamilyId,
            CreatedAtUtc = token.CreatedAtUtc,
            ExpiresAtUtc = token.ExpiresAtUtc,
            RevokedAtUtc = token.RevokedAtUtc,
            ReplacedByToken = token.ReplacedByTokenHash
        });

        await _db.SaveChangesAsync();
    }

    public async Task<RefreshTokenRecord?> GetByHashAsync(string tokenHash)
    {
        return await _db.RefreshTokens
            .AsNoTracking()
            .Where(x => x.TokenHash == tokenHash)
            .Select(x => new RefreshTokenRecord(
                x.Id,
                x.UserId,
                x.TokenHash,
                x.FamilyId,
                x.CreatedAtUtc,
                x.ExpiresAtUtc,
                x.RevokedAtUtc,
                x. ReplacedByToken))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> TryRevokeAsync(int id, string? replacedByTokenHash, DateTime nowUtc)
    {
        var affected = await _db.RefreshTokens
            .Where(x => x.Id == id && x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.RevokedAtUtc, nowUtc)
                .SetProperty(x => x. ReplacedByToken, replacedByTokenHash));

        return affected == 1;
    }

    public async Task RevokeFamilyAsync(Guid familyId, DateTime nowUtc)
    {
        await _db.RefreshTokens
            .Where(x => x.FamilyId == familyId && x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAtUtc, nowUtc));
    }

    public async Task RevokeAllForUserAsync(int userId, DateTime nowUtc)
    {
        await _db.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAtUtc, nowUtc));
    }
}
