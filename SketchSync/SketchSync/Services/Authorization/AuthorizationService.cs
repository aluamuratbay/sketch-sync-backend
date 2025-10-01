using SketchSync.Database.Repositories.JwtToken;
using SketchSync.Database.Repositories.Users;
using SketchSync.Exceptions;
using SketchSync.Services.Hashing;
using SketchSync.Services.JwtToken;
using static SketchSync.Constants.Constants;

namespace SketchSync.Services.Authorization;

public sealed class AuthorizationService(
    IJwtTokenService tokenService,
    IUserRepository userRepository,
    IJwtTokenRepository jwtTokenRepository,
    IHashingService hashingService
   )
    : IAuthorizationService
{
    public async Task<string> AuthorizeAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null) 
            throw new NotFoundException($"User with email {email} not found");
        
        var isPasswordValid = await hashingService.VerifyAsync(password, user.Password);
        if (!isPasswordValid) 
            throw new BadRequestException("Invalid password");
    
        var response = await jwtTokenRepository.GetCurrentAsync(user.Id, cancellationToken);
        var (userJwtToken, jwtToken) = response;
    
        var currentTime = GetExpirationDate();
        
        if (userJwtToken is { IsActive: true } && userJwtToken.ActivatedAt > currentTime)
            return jwtToken?.Token is not null ? jwtToken.Token : throw new BadRequestException("User authorization failed");
        
        if (userJwtToken?.JwtTokenId is not null)
            await tokenService.DeactivateAsync(userJwtToken.JwtTokenId, cancellationToken);
        
        var token = await tokenService.GenerateAsync(new GenerateJwtTokenRequest(user.Id, email), cancellationToken);
        return token;
    }
    
    private DateTimeOffset GetExpirationDate()
    {
        return DateTimeOffset.UtcNow.AddDays(-Token.DaysCount);
    }
}
