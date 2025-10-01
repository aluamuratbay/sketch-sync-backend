using Microsoft.Extensions.Options;
using SketchSync.Services.JwtToken;

namespace SketchSync.OptionsSetup.Jwt;

public sealed class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    private const string SectionName = "Jwt";

    public void Configure(JwtOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}