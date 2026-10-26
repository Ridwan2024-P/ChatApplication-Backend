namespace ChatWave.Data.Entities;

public class User
{
    public int Id { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
