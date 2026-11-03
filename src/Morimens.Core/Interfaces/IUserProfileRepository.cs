using Morimens.Core.Models;

namespace Morimens.Core.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(string userId);
    Task<UserProfile>  CreateAsync(UserProfile profile);
    Task<UserProfile>  UpdateAsync(UserProfile profile);

    Task<UserSettings?> GetSettingsAsync(string userId);
    Task<UserSettings>  UpsertSettingsAsync(UserSettings settings);
}
