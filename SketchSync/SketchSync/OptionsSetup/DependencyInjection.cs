using SketchSync.OptionsSetup.Database;
using SketchSync.OptionsSetup.Jwt;

namespace SketchSync.OptionsSetup;

public static class DependencyInjection
{
    public static void AddOptionsSetup(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<DatabaseOptionsSetup>()
            .ConfigureOptions<JwtOptionsSetup>()
            .ConfigureOptions<JwtBearerOptionsSetup>();
    }
}