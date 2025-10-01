using SketchSync.Entities;

namespace SketchSync.Endpoints.Users.Contracts;

public sealed record UserResponse(string FirstName, string LastName, string Email)
{
    public string FirstName { get; } = FirstName;
    public string LastName { get; } = LastName;
    public string Email { get; } = Email;
    
    public static UserResponse FromUser(User user)
    {
        return new UserResponse(user.FirstName, user.LastName, user.Email); 
    }
}