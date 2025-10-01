using Marten;
using SketchSync.Entities;
using SketchSync.Exceptions;

namespace SketchSync.Database.Repositories.Boards;

public class BoardRepository(IDocumentStore store)  : IBoardRepository
{
    private readonly IDocumentSession _lightweightSession = store.LightweightSession();
    private readonly IDocumentSession _dirtySession = store.DirtyTrackedSession();
    
    public async Task<Board[]> GetSharedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var boards = await _lightweightSession
            .Query<Board>()
            .Where(w => w.Members.Any(m => m.UserId == userId))
            .ToListAsync(cancellationToken);
      
        return boards.ToArray();
    }

    public async Task<Board[]> GetOwnedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        List<User> users = []; 
        var boards = await _lightweightSession
            .Query<Board>()
            .Where(w => w.OwnerId == userId)
            .ToListAsync(cancellationToken);
        
        return boards.ToArray();
    }

    public async Task<Board> GetAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        var board = await _lightweightSession.LoadAsync<Board>(boardId, cancellationToken);
        return board ?? throw new NotFoundException($"Board with ID {boardId} not found");
    }

    public async Task DeleteAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        _dirtySession.Delete<Board>(boardId);
        await _dirtySession.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateAsync(Board board, CancellationToken cancellationToken = default)
    {
        _dirtySession.Store(board);
        await _dirtySession.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(Board board, CancellationToken cancellationToken = default)
    {
        _dirtySession.Update(board);
        await _dirtySession.SaveChangesAsync(cancellationToken);
    }
}