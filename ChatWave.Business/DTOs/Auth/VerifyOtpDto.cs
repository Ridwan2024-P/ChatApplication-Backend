namespace ChatWave.Business.DTOs.Auth;

public class VerifyOtpDto
{
    public string PhoneNumber { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;
}