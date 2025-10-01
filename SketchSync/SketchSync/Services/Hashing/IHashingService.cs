using SketchSync.Primitives;

namespace SketchSync.Services.Hashing;

public interface IHashingService
{
    Task<Password> CreateAsync(string password);
    Task<bool> VerifyAsync(string password, Password targetPassword);
}