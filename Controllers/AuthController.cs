using Microsoft.AspNetCore.Mvc;
using SecureDocumentStorageSystem.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto dto)
    {
        try
        {
            var token = await _authService.Register(dto.Username, dto.Password);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto dto)
    {
        try
        {
            var token = await _authService.Login(dto.Username, dto.Password);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public record UserDto(string Username, string Password);
