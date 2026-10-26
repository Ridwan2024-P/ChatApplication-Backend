namespace ChatWave.Business.Interfaces.Services;

public interface ISmsSender
{
    Task SendOtpAsync(
        string phoneNumber,
        string otp,
        CancellationToken cancellationToken = default);
}