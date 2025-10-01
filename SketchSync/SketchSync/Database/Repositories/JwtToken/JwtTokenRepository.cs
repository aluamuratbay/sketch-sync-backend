using Marten;
using SketchSync.Entities;
using SketchSync.Exceptions;
using static SketchSync.Constants.Constants;

namespace SketchSync.Database.Repositories.JwtToken;

public class JwtTokenRepository(IDocumentStore store) : IJwtTokenRepository
{
    private readonly IDocumentSession _lightweightSession = store.LightweightSession();
    private readonly IDocumentSession _dirtySession = store.DirtyTrackedSession();
    
    public async Task CreateAsync(Guid userId, Guid tokenId, string token,
        CancellationToken cancellationToken = default)
    {
        var jwtToken = new Entities.JwtToken
        {
            Id = tokenId,
            Token = token,
            ExpirationDate = DateTime.UtcNow.AddDays(Token.DaysCount)
        };

        var tokenJournal = new UserJwtToken
        {
            JwtTokenId = jwtToken.Id,
            IsActive = true,
            ActivatedAt = DateTime.UtcNow,
            UserId = userId
        };

        _dirtySession.Store(jwtToken);
        _dirtySession.Store(tokenJournal);
        
        await _dirtySession.SaveChangesAsync(cancellationToken);
    }

    public async Task<GetCurrentTokenResponse> GetCurrentAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Entities.JwtToken? jwtToken = null;
        var userJwtToken = await _lightweightSession.Query<UserJwtToken>()
            .Include<Entities.JwtToken>(u => u.JwtTokenId, x => jwtToken = x)
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive, cancellationToken);

        return new GetCurrentTokenResponse(userJwtToken, jwtToken);
    }

    public async Task<UserJwtToken> GetUserJwtToken(Guid tokenId, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var userJwtToken = await _dirtySession.Query<UserJwtToken>()
                               .FirstOrDefaultAsync(u => u.JwtTokenId == tokenId, cancellationToken) ??
                           throw new NotFoundException("Jwt token was not found");

        return userJwtToken;
    }

    public async Task UpdateAsync(UserJwtToken token, CancellationToken cancellationToken = default)
    {
       _dirtySession.Update(token);
       await _dirtySession.SaveChangesAsync(cancellationToken);
    }
}