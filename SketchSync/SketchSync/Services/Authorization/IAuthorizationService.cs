namespace SketchSync.Services.Authorization;

public interface IAuthorizationService
{
    Task<string> AuthorizeAsync(string email, string password, CancellationToken cancellationToken = default);
}