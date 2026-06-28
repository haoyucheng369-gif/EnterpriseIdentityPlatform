using EnterpriseIdentityPlatform.AuthServer.Models;
using EnterpriseIdentityPlatform.AuthServer.Options;
using EnterpriseIdentityPlatform.AuthServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EnterpriseIdentityPlatform.AuthServer.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly AuthOptions _authOptions;

    public AuthController(JwtService jwtService, IOptions<AuthOptions> authOptions)
    {
        _jwtService = jwtService;
        _authOptions = authOptions.Value;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // 璁よ瘉闃舵锛氭牎楠岀敤鎴锋彁浜ょ殑 username/password锛岀‘璁よ皟鐢ㄦ柟鏄皝銆?
        var user = _authOptions.Users.FirstOrDefault(user =>
            user.Username == request.Username && user.Password == request.Password);

        if (user is null)
        {
            // 鐢ㄦ埛鍚嶆垨瀵嗙爜閿欒灞炰簬璁よ瘉澶辫触锛岃繑鍥?401 + OAuth 椋庢牸閿欒鐮併€?
            return Unauthorized(new AuthErrorResponse(
                "invalid_grant",
                "The username or password is invalid."));
        }

        // 鐧诲綍鎴愬姛鍚庯紝AuthServer 鏍规嵁閰嶇疆涓殑 role/scope 鐢熸垚鐢ㄦ埛 access token銆?
        return Ok(_jwtService.GenerateUserToken(user.Username, user.Role, user.Scopes));
    }

}
