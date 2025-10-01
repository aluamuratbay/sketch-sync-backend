using SketchSync.Entities;

namespace SketchSync.Database.Repositories.Boards;

public interface IBoardRepository
{
    public Task<Board[]> GetSharedAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<Board[]> GetOwnedAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<Board> GetAsync(Guid boardId, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Guid boardId, CancellationToken cancellationToken = default);
    public Task CreateAsync(Board board, CancellationToken cancellationToken = default);
    public Task UpdateAsync(Board board, CancellationToken cancellationToken = default);
}