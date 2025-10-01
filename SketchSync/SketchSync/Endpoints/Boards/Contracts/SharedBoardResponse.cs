using SketchSync.Entities;
using SketchSync.Enums;

namespace SketchSync.Endpoints.Boards.Contracts;

public sealed record SharedBoardResponse(Guid Id, string Title, DateTime CreatedAt, DateTime UpdatedAt, string Content, string? Preview, RoleEnum Role, BoardOwnerResponse Owner, BoardMemberResponse[] Members)
{
    public Guid Id { get; set; } = Id;
    public string Title { get; set; } = Title;
    public DateTime CreatedAt { get; set; }  = CreatedAt;
    public DateTime UpdatedAt { get; set; } = UpdatedAt;
    public string Content { get; set; } = Content;
    public string? Preview { get; set; } = Preview;
    public RoleEnum Role { get; set; } = Role;
    public BoardMemberResponse[] Members { get; set; } = Members;
    public BoardOwnerResponse Owner { get; set; } = Owner;
    
    public static SharedBoardResponse FromBoard(Board board, RoleEnum role, BoardOwnerResponse owner, BoardMemberResponse[] members)
    {
        return new SharedBoardResponse(
            board.Id, 
            board.Title, 
            board.CreatedAt, 
            board.UpdatedAt, 
            board.Content, 
            board.GetPreviewResponse(),
            role, 
            owner, 
            members); 
    }
}