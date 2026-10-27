namespace ChatWave.Business.DTOs.Auth;

public class AccessTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    
}