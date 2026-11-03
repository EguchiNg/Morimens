using Morimens.Core.Models;

namespace Morimens.Core.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetActiveAsync(string token);
    Task<RefreshToken>  CreateAsync(string userId, TimeSpan lifetime);
    Task                RevokeAsync(string token);
    Task                RevokeAllForUserAsync(string userId);
}
