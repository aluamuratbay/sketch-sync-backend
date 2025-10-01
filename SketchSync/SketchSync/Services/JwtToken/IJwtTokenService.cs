namespace SketchSync.Services.JwtToken;

public interface IJwtTokenService
{
   Task DeactivateAsync(Guid tokenId, CancellationToken cancellation = default);
   Task<string> GenerateAsync(GenerateJwtTokenRequest request, CancellationToken cancellation = default);
   Task ValidateAsync(Guid tokenId, CancellationToken cancellation = default);
}

public record GenerateJwtTokenRequest(Guid UserId, string Email);