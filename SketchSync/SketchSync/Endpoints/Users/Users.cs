using FluentValidation;
using SketchSync.Database.Repositories.Users;
using SketchSync.Endpoints.Users.Contracts;
using SketchSync.Entities;
using SketchSync.Exceptions;
using SketchSync.Extensions;
using SketchSync.Services.Authorization;
using SketchSync.Services.Hashing;
using SketchSync.Services.JwtToken;
using ValidationException = FluentValidation.ValidationException;

namespace SketchSync.Endpoints.Users;

public class Users() : BaseModule(nameof(Users))
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(string.Empty).RequireAuthorization();

        app.MapPost(string.Empty, async (CreateUserRequest request, IValidator<CreateUserRequest> validator, IUserRepository userRepository, IHashingService hashingService) =>
        {
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);
            
            var passwordHash = await hashingService.CreateAsync(request.Password);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = passwordHash
            };
            await userRepository.CreateAsync(user);
            return Results.Created();
        });

        app.MapPost("/login", async (LoginUserRequest request, IAuthorizationService authorizationService) =>
        {
            var token = await authorizationService.AuthorizeAsync(request.Email, request.Password);
            return Results.Ok(token);
        });

        group.MapPost("/logout", async (IJwtTokenService tokenService, HttpContext context) =>
        {
            var tokenId = context.GetTokenId();
            await tokenService.DeactivateAsync(tokenId);
        });

        group.MapGet("/current", async (IUserRepository userRepository, HttpContext context) =>
        {
            var userId = context.GetUserId();
            var user = await userRepository.GetByIdAsync(userId);
            var userResponse = UserResponse.FromUser(user);
            return Results.Ok(userResponse);
        });
    }
}