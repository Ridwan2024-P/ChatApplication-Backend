namespace ChatWave.Data.Entities;

public class PhoneOtp
{
    public int Id { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string OtpSalt { get; set; } = string.Empty;

    public string OtpHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime LastSentAtUtc { get; set; }

    public int Attempts { get; set; }

    public bool IsUsed { get; set; }
}
