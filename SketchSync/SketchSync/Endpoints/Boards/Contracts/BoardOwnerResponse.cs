using SketchSync.Entities;

namespace SketchSync.Endpoints.Boards.Contracts;

public sealed record BoardOwnerResponse(Guid UserId, string FirstName, string LastName, string Email)
{
    public Guid UserId { get; set; } = UserId;
    public string FirstName { get; } = FirstName;
    public string LastName { get; } = LastName;
    public string Email { get; } = Email;
    
    public static BoardOwnerResponse FromUser(User user)
    {
        return new BoardOwnerResponse(user.Id, user.FirstName, user.LastName, user.Email); 
    }
}