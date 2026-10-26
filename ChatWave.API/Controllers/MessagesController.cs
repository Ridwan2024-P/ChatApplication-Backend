using ChatWave.Business.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatWave.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }
}
