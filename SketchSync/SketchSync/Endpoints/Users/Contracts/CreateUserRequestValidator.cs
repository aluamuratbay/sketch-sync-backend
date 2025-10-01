using FluentValidation;
using SketchSync.Database.Repositories.Users;

namespace SketchSync.Endpoints.Users.Contracts;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator(IServiceProvider serviceProvider) 
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(3).WithMessage("First name must be at least 3 characters long");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(3).WithMessage("Last name must be at least 3 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(async (email, cancellationToken) =>
                (await userRepository.GetByEmailAsync(email, cancellationToken)) is null)
            .WithMessage("Email is already in use");
    
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]).{8,}$")
            .WithMessage("Password must contain at least 8 characters," +
                         " including at least one uppercase letter," +
                         " one lowercase letter, one digit, and one special character");
    }
}