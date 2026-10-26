using ChatWave.Business.DTOs.Auth;

namespace ChatWave.Business.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResultDto> RequestOtpAsync(
        string phoneNumber,
        bool includeDebugOtp);

    Task<AuthResultDto> VerifyOtpAsync(
        string phoneNumber,
        string otp);
}