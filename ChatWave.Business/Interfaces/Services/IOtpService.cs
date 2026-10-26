using ChatWave.Business.DTOs.Otp;

namespace ChatWave.Business.Interfaces.Services;

public interface IOtpService
{
    Task<OtpIssueResult> IssueAsync(string phoneNumber);

    Task<OtpVerifyStatus> VerifyAsync(string phoneNumber, string otp);
}
