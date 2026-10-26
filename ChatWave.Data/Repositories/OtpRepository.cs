using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Data.Data;
using ChatWave.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatWave.Data.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly ChatWaveDbContext _db;

    public OtpRepository(ChatWaveDbContext db)
    {
        _db = db;
    }

    private static OtpRecord ToRecord(PhoneOtp x) =>
        new(
            x.Id,
            x.PhoneNumber,
            x.OtpSalt,
            x.OtpHash,
            x.CreatedAtUtc,
            x.ExpiresAtUtc,
            x.LastSentAtUtc,
            x.Attempts,
            x.IsUsed);

    public async Task<OtpRecord?> GetLatestAsync(string phoneNumber)
    {
        var x = await _db.PhoneOtps
            .Where(o => o.PhoneNumber == phoneNumber)
            .OrderByDescending(o => o.CreatedAtUtc)
            .FirstOrDefaultAsync();

        return x is null ? null : ToRecord(x);
    }

    public async Task AddAsync(OtpRecord otp)
    {
        _db.PhoneOtps.Add(new PhoneOtp
        {
            PhoneNumber = otp.PhoneNumber,
            OtpSalt = otp.OtpSalt,
            OtpHash = otp.OtpHash,
            CreatedAtUtc = otp.CreatedAtUtc,
            ExpiresAtUtc = otp.ExpiresAtUtc,
            LastSentAtUtc = otp.LastSentAtUtc,
            Attempts = otp.Attempts,
            IsUsed = otp.IsUsed
        });

        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(OtpRecord otp)
    {
        var entity = await _db.PhoneOtps.FirstOrDefaultAsync(x => x.Id == otp.Id);
        if (entity is null)
            return;

        entity.OtpSalt = otp.OtpSalt;
        entity.OtpHash = otp.OtpHash;
        entity.CreatedAtUtc = otp.CreatedAtUtc;
        entity.ExpiresAtUtc = otp.ExpiresAtUtc;
        entity.LastSentAtUtc = otp.LastSentAtUtc;
        entity.Attempts = otp.Attempts;
        entity.IsUsed = otp.IsUsed;

        await _db.SaveChangesAsync();
    }
}
