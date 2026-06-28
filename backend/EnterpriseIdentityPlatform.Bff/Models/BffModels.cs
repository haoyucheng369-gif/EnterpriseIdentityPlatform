using System.Text.Json.Serialization;

namespace EnterpriseIdentityPlatform.Bff.Models;

// 鐧诲綍璺宠浆鍓嶆殏瀛樹竴娆℃€?state 鍜?PKCE verifier锛屽洖璋冩垚鍔熷悗绔嬪嵆娑堣垂锛岄伩鍏嶆巿鏉冪爜鍥炶皟琚噸澶嶄娇鐢ㄣ€?
public sealed record BffLoginState(string CodeVerifier, DateTimeOffset ExpiresAt);

// 娴忚鍣ㄥ彧鎸佹湁闅忔満 session id锛沘ccess token 淇濆瓨鍦?BFF 鍐呭瓨涓紝涓嶆毚闇茬粰鍓嶇 JavaScript銆?
public sealed record BffSession(
    string AccessToken,
    string Scope,
    DateTimeOffset ExpiresAt,
    string CsrfToken);

// BFF 浣跨敤璇ユā鍨嬭В鏋?Auth Server 鐨?token endpoint 杩斿洖鍊笺€?
public sealed record TokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("scope")] string Scope,
    [property: JsonPropertyName("expires_in")] int ExpiresIn);
