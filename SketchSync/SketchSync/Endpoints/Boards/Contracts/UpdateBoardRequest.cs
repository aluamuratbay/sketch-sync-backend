using SketchSync.Entities;

namespace SketchSync.Endpoints.Boards.Contracts;

public class UpdateBoardRequest
{
    public string? Title { get; set; }
    public required string Content { get; set; }
    public required string Preview { get; init; }

    public Board MapToBoard(Board board)
    {
        board.Title = Title ?? board.Title;
        board.Content = Content;
    
        var base64 = Preview;
        var commaIndex = base64.IndexOf(',');
        if (commaIndex >= 0)
            base64 = base64.Substring(commaIndex + 1);
        var bytes = Convert.FromBase64String(base64);
        
        board.Preview = bytes;
        board.UpdatedAt = DateTime.Now;
        
        return board;
    }
}