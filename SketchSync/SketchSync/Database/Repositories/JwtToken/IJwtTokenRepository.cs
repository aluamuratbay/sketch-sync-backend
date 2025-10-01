using SketchSync.Entities;

namespace SketchSync.Database.Repositories.JwtToken;

public interface IJwtTokenRepository
{
    Task CreateAsync(Guid userId, Guid tokenId, string token,
        CancellationToken cancellationToken = default);

    Task<GetCurrentTokenResponse> GetCurrentAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserJwtToken> GetUserJwtToken(Guid tokenId, bool enableTracking = true,
        CancellationToken cancellationToken = default);
    
    Task UpdateAsync(UserJwtToken token, CancellationToken cancellationToken = default);
}

public record GetCurrentTokenResponse(UserJwtToken? UserJwtToken, Entities.JwtToken? JwtToken);