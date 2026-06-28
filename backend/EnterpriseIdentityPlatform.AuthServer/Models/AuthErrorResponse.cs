using System.Text.Json.Serialization;

namespace EnterpriseIdentityPlatform.AuthServer.Models;

// OAuth2 椋庢牸閿欒鍝嶅簲锛屼究浜庡墠绔拰娴嬭瘯鏍规嵁 error code 鍋氱ǔ瀹氬垽鏂€?
public sealed record AuthErrorResponse(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("error_description")] string ErrorDescription);
