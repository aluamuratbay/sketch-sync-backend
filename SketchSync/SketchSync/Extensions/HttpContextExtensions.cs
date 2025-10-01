using System.Security.Claims;
using SketchSync.Exceptions;

namespace SketchSync.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
        return Guid.TryParse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : throw new UnauthorizedException();
    }

    public static Guid GetTokenId(this HttpContext httpContext)
    {
        _ = Guid.TryParse(httpContext.User.FindFirstValue("JwtTokenId"), out var tokenId);
        if (tokenId == Guid.Empty) throw new UnauthorizedException();
        return tokenId;
    }
}