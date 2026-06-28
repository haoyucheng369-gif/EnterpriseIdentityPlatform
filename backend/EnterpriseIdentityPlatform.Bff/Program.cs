using EnterpriseIdentityPlatform.Bff.Options;
using EnterpriseIdentityPlatform.Bff.Services;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.Configure<BffOptions>(builder.Configuration.GetSection("Bff"));
builder.Services.AddSingleton<BffSessionStore>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        var origins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:5173"];

        // 娴忚鍣ㄥ彧鎼哄甫 BFF 鐨?HttpOnly cookie锛屼笉鐩存帴鎺ヨЕ access_token銆?
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddHttpClient("AuthServer", (services, client) =>
{
    var options = services.GetRequiredService<Microsoft.Extensions.Options.IOptions<BffOptions>>().Value;
    // BFF 鍦ㄦ湇鍔＄璋冪敤 Auth Server token endpoint锛岀敤鎺堟潈鐮佸厬鎹?access_token銆?
    client.BaseAddress = new Uri(options.AuthServerBackchannelUrl);
});

builder.Services.AddHttpClient("ApiServer", (services, client) =>
{
    var options = services.GetRequiredService<Microsoft.Extensions.Options.IOptions<BffOptions>>().Value;
    // BFF 浠ｇ悊璋冪敤 API 鏃舵墠闄勫姞 bearer token锛屾祻瑙堝櫒鏈韩涓嶄細鎷垮埌杩欎釜 token銆?
    client.BaseAddress = new Uri(options.ApiServerBackchannelUrl);
});

var app = builder.Build();

app.UseCors(FrontendCorsPolicy);

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program;
