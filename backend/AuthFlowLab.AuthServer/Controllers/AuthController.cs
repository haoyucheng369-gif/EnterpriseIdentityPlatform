using AuthFlowLab.AuthServer.Models;
using AuthFlowLab.AuthServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthFlowLab.AuthServer.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        if (request.Username == "admin" && request.Password == "admin123")
        {
            return Ok(new
            {
                access_token = _jwtService.GenerateUserToken("admin", "Admin"),
                token_type = "Bearer"
            });
        }

        if (request.Username == "user" && request.Password == "user123")
        {
            return Ok(new
            {
                access_token = _jwtService.GenerateUserToken("user", "User"),
                token_type = "Bearer"
            });
        }

        return Unauthorized("Invalid username or password");
    }

    [HttpPost("client-token")]
    public IActionResult ClientToken(ClientTokenRequest request)
    {
        if (request.ClientId == "worker-service" && request.ClientSecret == "worker-secret")
        {
            return Ok(new
            {
                access_token = _jwtService.GenerateServiceToken("worker-service", "content.read"),
                token_type = "Bearer"
            });
        }

        return Unauthorized();
    }
}