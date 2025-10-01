using SketchSync.Entities;

namespace SketchSync.Database.Repositories.Users;

public interface IUserRepository
{
    Task<User[]> GetAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}