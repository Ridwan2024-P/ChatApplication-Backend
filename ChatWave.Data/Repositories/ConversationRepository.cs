using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Data.Data;

namespace ChatWave.Data.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly ChatWaveDbContext _context;

    public ConversationRepository(ChatWaveDbContext context)
    {
        _context = context;
    }
}
