using SketchSync.Entities;
using SketchSync.Enums;

namespace SketchSync.Endpoints.Boards.Contracts;

public sealed record BoardMemberResponse(Guid UserId, string FirstName, string LastName, string Email, RoleEnum Role)
{
    public Guid UserId { get; set; } = UserId;
    public string FirstName { get; } = FirstName;
    public string LastName { get; } = LastName;
    public string Email { get; } = Email;
    public RoleEnum Role { get; set; } = Role;
    
    public static BoardMemberResponse FromUser(User user, RoleEnum role)
    {
        return new BoardMemberResponse(user.Id, user.FirstName, user.LastName, user.Email, role); 
    }
}