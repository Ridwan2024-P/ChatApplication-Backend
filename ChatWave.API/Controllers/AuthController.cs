using ChatWave.Business.DTOs.Auth;
using ChatWave.Business.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatWave.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IWebHostEnvironment _environment;

    public AuthController(
        IAuthService auth,
        IWebHostEnvironment environment)
    {
        _auth = auth;
        _environment = environment;
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp(RequestOtpDto dto)
    {
        var result = await _auth.RequestOtpAsync(
            dto.PhoneNumber,
            _environment.IsDevelopment());

        if (result.RetryAfterSeconds is not null)
        {
            Response.Headers["Retry-After"] =
                result.RetryAfterSeconds.Value.ToString();

            return StatusCode(429, result);
        }

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
    {
        var result = await _auth.VerifyOtpAsync(
            dto.PhoneNumber, dto.Otp);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
}