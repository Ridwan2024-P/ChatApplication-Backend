
using System.Text.Json;
using ChatWave.Business.Interfaces.Services;
using Microsoft.Extensions.Configuration;
namespace ChatWave.Data.Services;
public sealed class MoceanSmsSender : ISmsSender
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public MoceanSmsSender(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendOtpAsync(
        string phoneNumber,
        string otp,
        CancellationToken cancellationToken = default)
    {
        var apiToken = _configuration["Mocean:ApiToken"];
        var senderId = _configuration["Mocean:SenderId"];

        if (string.IsNullOrWhiteSpace(apiToken) ||
            string.IsNullOrWhiteSpace(senderId))
        {
            throw new InvalidOperationException(
                "Mocean API token or Sender ID is missing.");
        }

        var recipient = phoneNumber.Trim().TrimStart('+');

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "rest/2/sms");

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", apiToken);

        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["mocean-from"] = senderId,
                ["mocean-to"] = recipient,
                ["mocean-text"] =
                    $"Your ChatWave OTP is {otp}. It expires in 60 seconds.",
                ["mocean-resp-format"] = "json"
            });

        using var response = await _httpClient.SendAsync(
            request, cancellationToken);

        var body = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Mocean HTTP error: {(int)response.StatusCode}");
        }
        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;
        if (root.TryGetProperty("messages", out var messages) &&
            messages.ValueKind == JsonValueKind.Array &&
            messages.GetArrayLength() > 0)
        {
            var first = messages[0];

            if (first.TryGetProperty("status", out var status) &&
                status.ValueKind == JsonValueKind.Number &&
                status.GetInt32() == 0)
            {
                return;
            }
        }
        throw new HttpRequestException(
            "Mocean did not accept the SMS. Check your Mocean dashboard.");
    }
}
