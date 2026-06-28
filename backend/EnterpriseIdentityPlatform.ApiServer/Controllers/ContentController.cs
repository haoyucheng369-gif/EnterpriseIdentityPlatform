using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseIdentityPlatform.ApiServer.Controllers;

[ApiController]
[Route("content")]
public class ContentController : ControllerBase
{
    // 鍖垮悕鎺ュ彛锛氫笉闇€瑕?token锛岄€傚悎楠岃瘉 API 鏄惁瀛樻椿銆?
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
        => Ok("Public content");

    // 鍩虹璁よ瘉鎺ュ彛锛氬彧瑕?token 鏈夋晥鍗冲彲锛屼笉妫€鏌?role/scope銆?
    [HttpGet("user")]
    [Authorize]
    public IActionResult UserContent()
        => Ok("User or authenticated service content");

    // 褰撳墠韬唤璋冭瘯鎺ュ彛锛氳繑鍥?ASP.NET Core 璁よ瘉鍚庣殑 Identity 鍜?Claims锛屾柟渚垮姣旀湰鍦?JWT銆丒ntra JWT銆丄PI Key 鐨勫樊寮傘€?
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
        => Ok(new
        {
            authentication = new
            {
                isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                name = User.Identity?.Name,
                authenticationType = User.Identity?.AuthenticationType,
                identities = User.Identities.Select(identity => new
                {
                    identity.AuthenticationType,
                    identity.Name,
                    identity.IsAuthenticated
                })
            },
            claims = User.Claims
                .GroupBy(claim => claim.Type)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => ToClaimValue(group
                        .Select(claim => claim.Value)
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToArray()))
        });

    // 瑙掕壊鎺堟潈锛氳姹?token 閲屾湁 Admin role銆?
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminContent()
        => Ok("Admin content");

    // scope 鎺堟潈锛氭湰鍦?token 浣跨敤 scope锛孍ntra token 浣跨敤 scp銆?
    [HttpGet("read")]
    [Authorize(Policy = "ContentRead")]
    public IActionResult ReadContent()
        => Ok("Content read allowed");

    // 鏇寸粏绮掑害鐨勫啓鏉冮檺锛氭櫘閫?user 娌℃湁 write scope 鏃朵細寰楀埌 403銆?
    [HttpPost("write")]
    [Authorize(Policy = "ContentWrite")]
    public IActionResult WriteContent()
        => Ok("Content write allowed");

    // 鏈嶅姟璋冪敤鎺堟潈锛氳姹?token_type=service锛屾櫘閫氱敤鎴?token 涓嶈兘璁块棶銆?
    [HttpGet("service")]
    [Authorize(Policy = "ServiceOnly")]
    public IActionResult ServiceContent()
        => Ok("Service-only content");

    // API Key 閴存潈锛氫笉浣跨敤 JWT锛屼篃涓嶈蛋 AuthServer锛孉piServer 鐩存帴鏍￠獙 X-Api-Key銆?
    [HttpGet("api-key")]
    [Authorize(Policy = "ApiKeyOnly")]
    public IActionResult ApiKeyContent()
        => Ok("API key content");

    private static object ToClaimValue(string[] values)
        => values.Length == 1 ? values[0] : values;
}
