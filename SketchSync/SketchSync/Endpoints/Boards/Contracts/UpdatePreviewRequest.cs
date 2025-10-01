namespace SketchSync.Endpoints.Boards.Contracts;

public class UpdatePreviewRequest
{
    public required Guid BoardId { get; init; }
    public required IFormFile File { get; init; }
}