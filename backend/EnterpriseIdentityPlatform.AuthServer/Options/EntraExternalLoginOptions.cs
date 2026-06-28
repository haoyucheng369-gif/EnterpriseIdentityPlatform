namespace EnterpriseIdentityPlatform.AuthServer.Options;

// 杩欓噷闆嗕腑绠＄悊 Auth Server 浣滀负瀹㈡埛绔幓杩炴帴 Entra ID 鏃堕渶瑕佺殑澶栭儴鐧诲綍閰嶇疆銆?
public sealed class EntraExternalLoginOptions
{
    // Enabled=false 鏃朵笉浼氭樉绀?Microsoft 鐧诲綍鍏ュ彛锛屼篃涓嶄細娉ㄥ唽 Entra OIDC handler銆?
    public bool Enabled { get; init; }

    // Entra tenant 鐨?OIDC authority锛屼緥濡?https://login.microsoftonline.com/{tenant-id}/v2.0銆?
    public string Authority { get; init; } = string.Empty;

    // Auth Server 鍦?Entra 涓敞鍐岀殑 Web 搴旂敤 client id銆?
    public string ClientId { get; init; } = string.Empty;

    // Auth Server 鍚庣瀹夊叏淇濆瓨鐨?client secret锛屼笉鑳芥斁鍒板墠绔€?
    public string ClientSecret { get; init; } = string.Empty;

    // Entra 鐧诲綍瀹屾垚鍚庡洖璋?Auth Server 鐨勫湴鍧€锛屽繀椤诲拰 Azure Portal 涓厤缃竴鑷淬€?
    public string CallbackPath { get; init; } = "/signin-entra";

    // 鐢ㄥ摢涓?Entra claim 鍘诲尮閰嶆湰鍦扮敤鎴凤紝鍖归厤鎴愬姛鍚庡啀浣跨敤鏈湴 scopes 鍜?roles銆?
    public string UserNameClaim { get; init; } = "preferred_username";

    // 杩欓噷鍙姹傜櫥褰曡璇侀渶瑕佺殑 OIDC scopes锛屼笉鐩存帴璇锋眰涓氬姟 API 鏉冮檺銆?
    public List<string> Scopes { get; init; } = ["openid", "profile", "email"];
}
