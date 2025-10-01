using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using SketchSync.Exceptions;
using SketchSync.Services.JwtToken;

namespace SketchSync.Extensions;

public static class JwtBearerExtensions
{
    private const string AuthorizationHeader = "Authorization";

    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthorization()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context => { await HandleTokenValidationAsync(context); },
                    OnAuthenticationFailed = async context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            await HandleTokenExpirationAsync(context);
                        }
                    }
                };
            });
        
        return services;
    }
    
    private static async Task HandleTokenValidationAsync(TokenValidatedContext context,
        CancellationToken cancellation = default)
    {
        var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenService>();
        var jwtToken = context.Request.Headers[AuthorizationHeader].FirstOrDefault()?.Split(" ").Last() ??
                       throw new BadRequestException("Bad request while retrieving a token");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        var tokenIdClaim = token.Claims.FirstOrDefault(c => c.Type is "JwtTokenId") ??
                           throw new BadRequestException("Bad request while retrieving a token");
        var tokenId = Guid.Parse(tokenIdClaim.Value);
        await jwtService.ValidateAsync(tokenId, cancellation);
    }

    private static async Task HandleTokenExpirationAsync(AuthenticationFailedContext context,
        CancellationToken cancellation = default)
    {
        var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenService>();
        var jwtToken = context.Request.Headers[AuthorizationHeader].FirstOrDefault()?.Split(" ").Last() ??
                       throw new BadRequestException("Bad request while retrieving a token");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        var tokenIdClaim = token.Claims.FirstOrDefault(c => c.Type is "JwtTokenId") ??
                           throw new BadRequestException("Bad request while retrieving a token");
        var tokenId = Guid.Parse(tokenIdClaim.Value);
        await jwtService.DeactivateAsync(tokenId, cancellation);
    }
}