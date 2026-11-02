using ChatWave.Business.DTOs.Auth;

namespace ChatWave.Business.Interfaces.Services;

public interface ITokenService
{
    AccessTokenDto CreateAccessToken(
        int userId,
        string phoneNumber);
    RefreshTokenDto CreateRefreshToken();
    string HashRefreshToken(string rawToken);
    
    
}