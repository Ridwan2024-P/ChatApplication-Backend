using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Business.Interfaces.Services;

namespace ChatWave.Business.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;

    public MessageService(IMessageRepository messageRepository, IConversationRepository conversationRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
    }
}
