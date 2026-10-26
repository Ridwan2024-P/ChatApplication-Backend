using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Business.Interfaces.Services;

namespace ChatWave.Business.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;

    public ConversationService(IConversationRepository conversationRepository, IUserRepository userRepository)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
    }
}
