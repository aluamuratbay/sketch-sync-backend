using System.Security.Cryptography;
using SketchSync.Primitives;

namespace SketchSync.Services.Hashing;

public class HashingService : IHashingService
{
    public async Task<Password> CreateAsync(string password)
    {
        using var hmac = new HMACSHA512();
        var hash = await Task.Run(() => hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        return new Password(hash, hmac.Key);
    }

    public async Task<bool> VerifyAsync(string password, Password targetPassword)
    {
        using var hmac = new HMACSHA512(targetPassword.Salt);
        var computedHash = await Task.Run(() => hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        return computedHash.SequenceEqual(targetPassword.Hash);
    }
}