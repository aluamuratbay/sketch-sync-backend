using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SketchSync.Services.JwtToken;

namespace SketchSync.OptionsSetup.Jwt;

public class JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions) : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _options = jwtOptions.Value;
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters.ValidIssuer = _options.Issuer;
        options.TokenValidationParameters.ValidAudience = _options.Audience;
        options.TokenValidationParameters.IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
    }
}
