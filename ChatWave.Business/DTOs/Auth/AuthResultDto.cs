namespace ChatWave.Business.DTOs.Auth;

public class AuthResultDto
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int? RetryAfterSeconds { get; set; }

    public string? DebugOtp { get; set; }

    public int? UserId { get; set; }

    public string? PhoneNumber { get; set; }
}