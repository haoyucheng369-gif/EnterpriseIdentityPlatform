using System.Collections.Concurrent;
using System.Security.Cryptography;
using EnterpriseIdentityPlatform.Bff.Models;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseIdentityPlatform.Bff.Services;

public sealed class BffSessionStore
{
    private readonly ConcurrentDictionary<string, BffLoginState> _loginStates = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, BffSession> _sessions = new(StringComparer.Ordinal);

    public string CreateLoginState(string codeVerifier)
    {
        // state 鎶?callback 缁戝畾鍒?BFF 鍙戣捣鐨勭櫥褰曡姹傦紝鍚屾椂鍦ㄦ湇鍔＄鍏宠仈 PKCE verifier銆?
        var state = CreateRandomValue();
        _loginStates[state] = new BffLoginState(codeVerifier, DateTimeOffset.UtcNow.AddMinutes(5));
        return state;
    }

    public bool TryConsumeLoginState(string state, out BffLoginState? loginState)
    {
        if (!_loginStates.TryRemove(state, out loginState))
        {
            return false;
        }

        return loginState.ExpiresAt > DateTimeOffset.UtcNow;
    }

    public string CreateSession(TokenResponse token)
    {
        // cookie 鍙繚瀛橀殢鏈?session id锛涚湡姝ｇ殑 access_token 鍜?CSRF token 淇濈暀鍦?BFF 鏈嶅姟绔唴瀛樹腑銆?
        var sessionId = CreateRandomValue();
        _sessions[sessionId] = new BffSession(
            token.AccessToken,
            token.Scope,
            DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn),
            CreateRandomValue());
        return sessionId;
    }

    public bool TryGetSession(string? sessionId, out BffSession? session)
    {
        session = null;
        if (string.IsNullOrWhiteSpace(sessionId) || !_sessions.TryGetValue(sessionId, out var storedSession))
        {
            return false;
        }

        if (storedSession.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            _sessions.TryRemove(sessionId, out _);
            return false;
        }

        session = storedSession;
        return true;
    }

    public void RemoveSession(string? sessionId)
    {
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            _sessions.TryRemove(sessionId, out _);
        }
    }

    private static string CreateRandomValue()
        => Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
}
