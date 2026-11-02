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
    
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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
        
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TokenHash)
                .HasMaxLength(64)
                .IsRequired();

            entity.Property<string>(x =>x.ReplacedByToken)
                .HasMaxLength(64);

            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => x.FamilyId);
            entity.HasIndex(x => x.UserId);

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}