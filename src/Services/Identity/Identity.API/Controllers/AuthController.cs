using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Mock implementation for demo
        if (request.Username == "admin" && request.Password == "password")
        {
            return Ok(new { Token = "mock-jwt-token-xyz", Expiration = DateTime.UtcNow.AddHours(1) });
        }
        return Unauthorized("Invalid credentials");
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        return Ok(new { Message = "User registered successfully", UserId = Guid.NewGuid() });
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Email, string Password);
