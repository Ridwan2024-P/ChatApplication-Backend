namespace ChatWave.Business.DTOs.Auth;

public class RefreshTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}