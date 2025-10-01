using SketchSync.Enums;

namespace SketchSync.Entities;

public class Board
{
    public required Guid Id { get; set; }
    public required string Title { get; set; } 
    public required Guid OwnerId { get; set; } 
    public required DateTime CreatedAt { get; set; } 
    public required DateTime UpdatedAt { get; set; }
    public required string Content { get; set; }
    public BoardMember[] Members { get; set; } = [];
    public byte[]? Preview { get; set; }

    public string GetPreviewResponse()
    {
        return  Preview is null ? string.Empty : $"data:image/png;base64,{Convert.ToBase64String(Preview)}";
    }
}

public record BoardMember
{
    public required Guid UserId { get; set; }
    public required RoleEnum Role { get; set; }
}