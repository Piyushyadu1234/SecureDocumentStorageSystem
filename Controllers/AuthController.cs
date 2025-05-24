using Microsoft.AspNetCore.Mvc;
using SecureDocumentStorageSystem.Services;
using SecureDocumentStorageSystem.DTOs;


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
    public async Task<IActionResult> Register([FromBody] LoginDto dto)
        => Ok(await _authService.Register(dto.Username, dto.Password));

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
        => Ok(await _authService.Login(dto.Username, dto.Password));
}
