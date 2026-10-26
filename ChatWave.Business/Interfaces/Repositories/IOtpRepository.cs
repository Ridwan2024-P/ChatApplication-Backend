namespace ChatWave.Business.Interfaces.Repositories;

public record OtpRecord(
    int Id,
    string PhoneNumber,
    string OtpSalt,
    string OtpHash,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc,
    DateTime LastSentAtUtc,
    int Attempts,
    bool IsUsed);

public interface IOtpRepository
{
    Task<OtpRecord?> GetLatestAsync(string phoneNumber);

    Task AddAsync(OtpRecord otp);

    Task UpdateAsync(OtpRecord otp);
}
