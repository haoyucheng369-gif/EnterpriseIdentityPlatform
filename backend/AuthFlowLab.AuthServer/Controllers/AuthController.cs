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
            var token = _jwtService.GenerateAccessToken("admin", "Admin");

            return Ok(new
            {
                access_token = token,
                token_type = "Bearer"
            });
        }

        if (request.Username == "user" && request.Password == "user123")
        {
            var token = _jwtService.GenerateAccessToken("user", "User");

            return Ok(new
            {
                access_token = token,
                token_type = "Bearer"
            });
        }

        return Unauthorized("Invalid username or password");
    }
}