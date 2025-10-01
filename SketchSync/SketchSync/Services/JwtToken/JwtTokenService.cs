using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SketchSync.Database.Repositories.JwtToken;
using static SketchSync.Constants.Constants;

namespace SketchSync.Services.JwtToken;

public sealed class JwtTokenService(IOptions<JwtOptions> options, IJwtTokenRepository jwtTokenRepository) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public async Task DeactivateAsync(Guid tokenId, CancellationToken cancellation = default)
    {
        var userJwtToken = await jwtTokenRepository.GetUserJwtToken(tokenId, true, cancellation);
        userJwtToken.DeactivatedAt = DateTimeOffset.UtcNow;
        userJwtToken.IsActive = false;
        await jwtTokenRepository.UpdateAsync(userJwtToken, cancellation);
    }
    
    public async Task<string> GenerateAsync(GenerateJwtTokenRequest request, CancellationToken cancellation = default)
    {
        var (userId, email) = request;
    
        var tokenId = Guid.NewGuid();
    
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()), 
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userId.ToString()), 
            new(ClaimTypes.Email, email),
            new("JwtTokenId", tokenId .ToString())
        };
    
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),SecurityAlgorithms.HmacSha256);
        
        var jwtToken = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddDays(Token.DaysCount),
            signingCredentials);
    
        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
    
        await jwtTokenRepository.CreateAsync(userId, tokenId, token, cancellation);
    
        return await Task.FromResult(token);
    }

    public async Task ValidateAsync(Guid tokenId, CancellationToken cancellation = default)
    {
        var userJwtToken = await jwtTokenRepository.GetUserJwtToken(tokenId, false, cancellation);
        if (userJwtToken is { IsActive: false }) throw new Exception(tokenId.ToString());
        await Task.CompletedTask;
    }
}
