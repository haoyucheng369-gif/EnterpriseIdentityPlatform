namespace EnterpriseIdentityPlatform.Bff.Options;

public sealed class BffOptions
{
    // 娴忚鍣ㄨ闂?Auth Server 鏃朵娇鐢ㄥ叕寮€鍦板潃锛汥ocker 瀹瑰櫒鍐呴儴鍦板潃涓嶈兘鏆撮湶缁欐祻瑙堝櫒銆?
    public string AuthServerPublicUrl { get; init; } = "http://localhost:5001";

    // BFF 鏈嶅姟绔厬鎹?token 鏃朵娇鐢ㄥ悗绔湴鍧€锛屾湰鍦板拰 Docker 鐜鍙互鍒嗗埆瑕嗙洊銆?
    public string AuthServerBackchannelUrl { get; init; } = "http://localhost:5001";

    // BFF 浠ｇ悊 API 璇锋眰鏃朵娇鐢ㄥ悗绔湴鍧€锛屾湰鍦板拰 Docker 鐜鍙互鍒嗗埆瑕嗙洊銆?
    public string ApiServerBackchannelUrl { get; init; } = "http://localhost:5002";

    // callback 蹇呴』鍜?Auth Server 涓?demo-bff client 娉ㄥ唽鐨?redirect_uri 瀹屽叏涓€鑷淬€?
    public string CallbackUrl { get; init; } = "http://localhost:5003/bff/callback";

    // 鐧诲綍瀹屾垚鍚庡洖鍒板墠绔〉闈紝鐢卞墠绔鍙?BFF 浼氳瘽鐘舵€併€?
    public string FrontendUrl { get; init; } = "http://localhost:5173";

    // BFF 鏄?confidential client锛宑lient_secret 鍙兘淇濆瓨鍦ㄦ湇鍔＄銆?
    public string ClientId { get; init; } = "demo-bff";
    public string ClientSecret { get; init; } = "bff-secret";

    // BFF 璇锋眰鏈湴 Auth Server 鍙戞斁鐨?API scopes銆?
    public string Scope { get; init; } = "openid profile content.read content.write";

    public string SessionCookieName { get; init; } = "EnterpriseIdentityPlatform.Bff.Session";
}
