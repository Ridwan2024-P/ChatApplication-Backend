using System.Security.Claims;
using ChatWave.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatWave.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue("sub");
        var phone = User.FindFirstValue("phone_number");

        return Ok(new { userId, phoneNumber = phone });
    }
}
