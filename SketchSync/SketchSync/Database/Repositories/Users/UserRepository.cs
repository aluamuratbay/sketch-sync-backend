using Marten;
using SketchSync.Entities;
using SketchSync.Exceptions;

namespace SketchSync.Database.Repositories.Users;

public class UserRepository(IDocumentStore store) : IUserRepository
{
    private readonly IDocumentSession _lightweightSession = store.LightweightSession();
    private readonly IDocumentSession _dirtySession = store.DirtyTrackedSession();

    public async Task<User[]> GetAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var users = await _lightweightSession.Query<User>()
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
        
        return users.ToArray();
    }

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
       _dirtySession.Store(user);
       await _dirtySession.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _lightweightSession.Query<User>()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return user;
    }

    public async Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
       var user = await _lightweightSession.LoadAsync<User>(userId, cancellationToken);
       return user ?? throw new NotFoundException($"User with ID {userId} not found");
    }
}