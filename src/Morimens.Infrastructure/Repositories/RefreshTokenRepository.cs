using Microsoft.EntityFrameworkCore;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;
using Morimens.Infrastructure.Data;

namespace Morimens.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MorimensDbContext _db;
    public RefreshTokenRepository(MorimensDbContext db) => _db = db;

    public async Task<RefreshToken?> GetActiveAsync(string token) =>
        await _db.RefreshTokens.FirstOrDefaultAsync(r =>
            r.Token == token &&
            !r.IsRevoked &&
            r.ExpiresAt > DateTime.UtcNow);

    public async Task<RefreshToken> CreateAsync(string userId, TimeSpan lifetime)
    {
        var rt = new RefreshToken
        {
            UserId    = userId,
            Token     = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.Add(lifetime),
        };
        _db.RefreshTokens.Add(rt);
        await _db.SaveChangesAsync();
        return rt;
    }

    public async Task RevokeAsync(string token)
    {
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
        if (rt is null) return;
        rt.IsRevoked = true;
        await _db.SaveChangesAsync();
    }

    public async Task RevokeAllForUserAsync(string userId)
    {
        await _db.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsRevoked, true));
    }
}
