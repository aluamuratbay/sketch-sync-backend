using SketchSync.Enums;

namespace SketchSync.Endpoints.Boards.Contracts;

public class UpdateMemberRequest
{
    public required Guid BoardId { get; init; }
    public required Guid UserId { get; init; }
    public required RoleEnum Role { get; init; }
}