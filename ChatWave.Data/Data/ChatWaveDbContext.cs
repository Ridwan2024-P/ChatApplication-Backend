using ChatWave.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatWave.Data.Data;

public class ChatWaveDbContext : DbContext
{
    public ChatWaveDbContext(
        DbContextOptions<ChatWaveDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<PhoneOtp> PhoneOtps => Set<PhoneOtp>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(x => x.PhoneNumber)
                .IsUnique();
        });

        modelBuilder.Entity<PhoneOtp>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.OtpSalt).IsRequired();
            entity.Property(x => x.OtpHash).IsRequired();

            entity.HasIndex(x => new
            {
                x.PhoneNumber,
                x.CreatedAtUtc
            });
        });
    }
}