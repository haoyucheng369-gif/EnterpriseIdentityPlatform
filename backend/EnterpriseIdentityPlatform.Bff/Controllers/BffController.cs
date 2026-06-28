using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using EnterpriseIdentityPlatform.Bff.Models;
using EnterpriseIdentityPlatform.Bff.Options;
using EnterpriseIdentityPlatform.Bff.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseIdentityPlatform.Bff.Controllers;

[ApiController]
[Route("bff")]
public sealed class BffController : ControllerBase
{
    private const string CsrfHeaderName = "X-CSRF-TOKEN";
    private readonly BffOptions _options;
    private readonly BffSessionStore _sessionStore;
    private readonly IHttpClientFactory _httpClientFactory;

    public BffController(
        IOptions<BffOptions> options,
        BffSessionStore sessionStore,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _sessionStore = sessionStore;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        // BFF 鑷繁鐢熸垚骞朵繚瀛?PKCE verifier锛屾祻瑙堝櫒鍙礋璐ｈ窡闅忛噸瀹氬悜锛屼笉淇濆瓨 OAuth token銆?
        var verifier = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        var challenge = Base64UrlEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(verifier)));
        var state = _sessionStore.CreateLoginState(verifier);
        var nonce = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(16));

        var authorizeUrl = QueryHelpers.AddQueryString(
            $"{_options.AuthServerPublicUrl.TrimEnd('/')}/connect/authorize",
            new Dictionary<string, string?>
            {
                ["response_type"] = "code",
                ["client_id"] = _options.ClientId,
                ["redirect_uri"] = _options.CallbackUrl,
                ["scope"] = _options.Scope,
                ["state"] = state,
                ["nonce"] = nonce,
                ["code_challenge"] = challenge,
                ["code_challenge_method"] = "S256"
            });

        return Redirect(authorizeUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state)
    {
        // callback 蹇呴』娑堣垂涓€娆℃€?state锛屽啀鐢?BFF 鏈嶅姟绔惡甯?secret 鍜?PKCE verifier 鎹?token銆?
        if (string.IsNullOrWhiteSpace(code) ||
            string.IsNullOrWhiteSpace(state) ||
            !_sessionStore.TryConsumeLoginState(state, out var loginState) ||
            loginState is null)
        {
            return BadRequest(new { error = "invalid_callback" });
        }

        var authServer = _httpClientFactory.CreateClient("AuthServer");
        var response = await authServer.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["code"] = code,
            ["redirect_uri"] = _options.CallbackUrl,
            ["code_verifier"] = loginState.CodeVerifier
        }));

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        if (token is null)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = "invalid_token_response" });
        }

        var sessionId = _sessionStore.CreateSession(token);
        Response.Cookies.Append(
            _options.SessionCookieName,
            sessionId,
            CreateSessionCookieOptions(token.ExpiresIn));

        return Redirect(_options.FrontendUrl);
    }

    [HttpGet("session")]
    public IActionResult Session()
    {
        // 鍙繑鍥為〉闈㈤渶瑕佺殑浼氳瘽鎽樿鍜?CSRF token锛屼笉鎶婃湇鍔＄淇濆瓨鐨?access_token 娉勯湶缁欏墠绔€?
        return TryGetCurrentSession(out var session)
            ? Ok(new { authenticated = true, session.Scope, session.ExpiresAt, session.CsrfToken })
            : Unauthorized(new { authenticated = false });
    }

    [HttpGet("content/read")]
    public Task<IActionResult> ReadContent()
        => ProxyApiRequest(HttpMethod.Get, "/content/read");

    [HttpGet("content/me")]
    public Task<IActionResult> ReadClaims()
        => ProxyApiRequest(HttpMethod.Get, "/content/me");

    [HttpPost("content/write")]
    public Task<IActionResult> WriteContent()
    {
        // Cookie 浼氳娴忚鍣ㄨ嚜鍔ㄦ惡甯︼紝鍥犳鍐欒姹傚繀椤婚澶栨牎楠屽墠绔樉寮忓彂閫佺殑 CSRF token銆?
        return ProxyApiRequest(HttpMethod.Post, "/content/write", requireCsrfToken: true);
    }

    [HttpGet("userinfo")]
    public Task<IActionResult> UserInfo()
        => ProxyRequest("AuthServer", HttpMethod.Get, "/connect/userinfo");

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // 閫€鍑烘椂鍒犻櫎鏈嶅姟绔?token session锛屽苟璁╂祻瑙堝櫒鍒犻櫎鍙繚瀛?session id 鐨?HttpOnly cookie銆?
        Request.Cookies.TryGetValue(_options.SessionCookieName, out var sessionId);
        _sessionStore.RemoveSession(sessionId);
        Response.Cookies.Delete(_options.SessionCookieName);
        return NoContent();
    }

    private Task<IActionResult> ProxyApiRequest(HttpMethod method, string path, bool requireCsrfToken = false)
        => ProxyRequest("ApiServer", method, path, requireCsrfToken);

    private async Task<IActionResult> ProxyRequest(
        string clientName,
        HttpMethod method,
        string path,
        bool requireCsrfToken = false)
    {
        // BFF 鏍规嵁 cookie 鎵惧埌鏈嶅姟绔?token锛屽啀浠ｇ悊璋冪敤 Auth Server 鎴栬祫婧?API銆?
        if (!TryGetCurrentSession(out var session))
        {
            return Unauthorized();
        }

        if (requireCsrfToken && !HasValidCsrfToken(session.CsrfToken))
        {
            return BadRequest(new { error = "invalid_csrf_token" });
        }

        var client = _httpClientFactory.CreateClient(clientName);
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        using var response = await client.SendAsync(request);

        return new ContentResult
        {
            Content = await response.Content.ReadAsStringAsync(),
            ContentType = response.Content.Headers.ContentType?.ToString() ?? "text/plain; charset=utf-8",
            StatusCode = (int)response.StatusCode
        };
    }

    private bool HasValidCsrfToken(string expectedToken)
    {
        var providedToken = Request.Headers[CsrfHeaderName].ToString();
        if (string.IsNullOrWhiteSpace(providedToken))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(providedToken),
            Encoding.UTF8.GetBytes(expectedToken));
    }

    private bool TryGetCurrentSession(out BffSession session)
    {
        Request.Cookies.TryGetValue(_options.SessionCookieName, out var sessionId);
        return _sessionStore.TryGetSession(sessionId, out session!);
    }

    private CookieOptions CreateSessionCookieOptions(int expiresIn)
    {
        // 鏈湴 HTTP 娴嬭瘯鍏佽闈?Secure锛涢儴缃插埌 HTTPS 鍚庤嚜鍔ㄥ啓鍏?Secure cookie銆?
        return new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps,
            Expires = DateTimeOffset.UtcNow.AddSeconds(expiresIn)
        };
    }
}
