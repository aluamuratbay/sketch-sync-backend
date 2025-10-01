using SketchSync.Entities;

namespace SketchSync.Endpoints.Boards.Contracts;

public sealed record OwnedBoardResponse(Guid Id, string Title, DateTime CreatedAt, DateTime UpdatedAt, string Content, string? Preview, BoardMemberResponse[] Members)
{
    public Guid Id { get; set; } = Id;
    public string Title { get; set; } = Title;
    public DateTime CreatedAt { get; set; }  = CreatedAt;
    public DateTime UpdatedAt { get; set; } = UpdatedAt;
    public string Content { get; set; } = Content;
    public string? Preview { get; set; } = Preview;
    public BoardMemberResponse[] Members { get; set; } = Members;
    
    public static OwnedBoardResponse FromBoard(Board board, BoardMemberResponse[] members)
    {
        return new OwnedBoardResponse(
            board.Id, 
            board.Title, 
            board.CreatedAt, 
            board.UpdatedAt, 
            board.Content, 
            board.GetPreviewResponse(),
            members); 
    }
}