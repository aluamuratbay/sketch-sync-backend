using SketchSync.Services.Authorization;
using SketchSync.Services.Hashing;
using SketchSync.Services.JwtToken;

namespace SketchSync.Services;

public static class DependencyInjection
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IHashingService, HashingService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
    }
}