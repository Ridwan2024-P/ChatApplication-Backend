using System.Security.Cryptography;
using System.Text;
using ChatWave.Business.DTOs.Otp;
using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Business.Interfaces.Services;

namespace ChatWave.Business.Services;

public class OtpService : IOtpService
{
    private const int ExpiryMinutes = 5;
    private const int ResendCooldownSeconds = 60;
    private const int MaxAttempts = 5;

    private readonly IOtpRepository _otpRepository;

    public OtpService(IOtpRepository otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public async Task<OtpIssueResult> IssueAsync(string phoneNumber)
    {
        var now = DateTime.UtcNow;
        var latest = await _otpRepository.GetLatestAsync(phoneNumber);

        if (latest is not null)
        {
            var elapsed = (now - latest.LastSentAtUtc).TotalSeconds;
            if (elapsed < ResendCooldownSeconds)
            {
                var wait = (int)Math.Ceiling(ResendCooldownSeconds - elapsed);
                return new OtpIssueResult(false, null, wait);
            }
        }

        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        await _otpRepository.AddAsync(new OtpRecord(
            Id: 0,
            PhoneNumber: phoneNumber,
            OtpSalt: salt,
            OtpHash: Hash(salt, code),
            CreatedAtUtc: now,
            ExpiresAtUtc: now.AddMinutes(ExpiryMinutes),
            LastSentAtUtc: now,
            Attempts: 0,
            IsUsed: false));

        // TODO: send `code` through an SMS provider here.
        return new OtpIssueResult(true, code, null);
    }

    public async Task<OtpVerifyStatus> VerifyAsync(string phoneNumber, string otp)
    {
        var record = await _otpRepository.GetLatestAsync(phoneNumber);

        if (record is null || record.IsUsed || record.ExpiresAtUtc < DateTime.UtcNow)
            return OtpVerifyStatus.Expired;

        if (record.Attempts >= MaxAttempts)
            return OtpVerifyStatus.TooManyAttempts;

        var expected = Convert.FromBase64String(record.OtpHash);
        var actual = Convert.FromBase64String(Hash(record.OtpSalt, otp));

        if (!CryptographicOperations.FixedTimeEquals(expected, actual))
        {
            await _otpRepository.UpdateAsync(record with { Attempts = record.Attempts + 1 });
            return OtpVerifyStatus.Invalid;
        }

        await _otpRepository.UpdateAsync(record with { IsUsed = true });
        return OtpVerifyStatus.Valid;
    }

    private static string Hash(string salt, string code) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(salt + code)));
}
