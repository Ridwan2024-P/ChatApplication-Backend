using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Data.Data;

namespace ChatWave.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatWaveDbContext _context;

    public MessageRepository(ChatWaveDbContext context)
    {
        _context = context;
    }
}
