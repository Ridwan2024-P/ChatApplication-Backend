using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Data.Data;
using ChatWave.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatWave.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ChatWaveDbContext _context;

    public UserRepository(ChatWaveDbContext context)
    {
        _context = context;
    }

    public async Task<int?> GetIdByPhoneAsync(string phoneNumber)
    {
        return await _context.Users
            .Where(u => u.PhoneNumber == phoneNumber)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(string phoneNumber)
    {
        _context.Users.Add(new User { PhoneNumber = phoneNumber });
        await _context.SaveChangesAsync();
    }
}
