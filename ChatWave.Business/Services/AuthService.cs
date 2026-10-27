using System.Text.RegularExpressions;
using ChatWave.Business.DTOs.Auth;
using ChatWave.Business.DTOs.Otp;
using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Business.Interfaces.Services;

namespace ChatWave.Business.Services;

public class AuthService : IAuthService
{
    private static readonly Regex PhoneRegex = new(@"^\+?[0-9]{7,15}$", RegexOptions.Compiled);

    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly ISmsSender _smsSender;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IOtpService otpService,
        ISmsSender smsSender,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _smsSender = smsSender;
        _tokenService = tokenService;
    }

    public async Task<AuthResultDto> RequestOtpAsync(string phoneNumber, bool includeDebugOtp)
    {
        var phone = Normalize(phoneNumber);
        if (!PhoneRegex.IsMatch(phone))
            return new AuthResultDto { Success = false, Message = "Invalid phone number." };

        var issued = await _otpService.IssueAsync(phone);

        if (!issued.Issued)
        {
            return new AuthResultDto
            {
                Success = false,
                Message = "Please wait before requesting another code.",
                RetryAfterSeconds = issued.RetryAfterSeconds,
                PhoneNumber = phone
            };
        }

        // NEW: asholei SMS pathano
        try
        {
            await _smsSender.SendOtpAsync(phone, issued.Code!);
        }
        catch (Exception ex)
        {
            return new AuthResultDto
            {
                Success = false,
                Message = $"SMS send failed: {ex.Message}",
                PhoneNumber = phone
            };
        }

        return new AuthResultDto
        {
            Success = true,
            Message = "OTP sent.",
            PhoneNumber = phone,
            DebugOtp = includeDebugOtp ? issued.Code : null
        };
    }

    public async Task<AuthResultDto> VerifyOtpAsync(string phoneNumber, string otp)
    {
        var phone = Normalize(phoneNumber);

        var status = await _otpService.VerifyAsync(phone, otp?.Trim() ?? string.Empty);
        if (status != OtpVerifyStatus.Valid)
        {
            return new AuthResultDto
            {
                Success = false,
                Message = status switch
                {
                    OtpVerifyStatus.Expired => "OTP expired or not found. Request a new one.",
                    OtpVerifyStatus.TooManyAttempts => "Too many attempts. Request a new OTP.",
                    _ => "Invalid OTP."
                }
            };
        }

        var userId = await _userRepository.GetIdByPhoneAsync(phone);
        if (userId is null)
        {
            await _userRepository.AddAsync(phone);
            userId = await _userRepository.GetIdByPhoneAsync(phone);
        }

        var token = _tokenService.CreateAccessToken(userId!.Value, phone);

        return new AuthResultDto
        {
            Success = true,
            Message = "Verified.",
            UserId = userId,
            PhoneNumber = phone,
            AccessToken = token.AccessToken,
            AccessTokenExpiresUtc = token.Expires
        };
    }

    private static string Normalize(string? phone) =>
        (phone ?? string.Empty).Replace(" ", "").Replace("-", "").Trim();
}
