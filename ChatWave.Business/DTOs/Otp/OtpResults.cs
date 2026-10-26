namespace ChatWave.Business.DTOs.Otp;

public record OtpIssueResult(bool Issued, string? Code, int? RetryAfterSeconds);

public enum OtpVerifyStatus
{
    Valid,
    Invalid,
    Expired,
    TooManyAttempts
}
