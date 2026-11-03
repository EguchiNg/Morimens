using Microsoft.EntityFrameworkCore;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;
using Morimens.Infrastructure.Data;

namespace Morimens.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly MorimensDbContext _db;
    public UserProfileRepository(MorimensDbContext db) => _db = db;

    public async Task<UserProfile?> GetByUserIdAsync(string userId) =>
        await _db.UserProfiles
            .Include(p => p.Settings)
            .Include(p => p.Simulations)
            .FirstOrDefaultAsync(p => p.UserId == userId);

    public async Task<UserProfile> CreateAsync(UserProfile profile)
    {
        _db.UserProfiles.Add(profile);
        await _db.SaveChangesAsync();
        return profile;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile profile)
    {
        _db.UserProfiles.Update(profile);
        await _db.SaveChangesAsync();
        return profile;
    }

    public async Task<UserSettings?> GetSettingsAsync(string userId) =>
        await _db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);

    public async Task<UserSettings> UpsertSettingsAsync(UserSettings settings)
    {
        var existing = await _db.UserSettings.FindAsync(settings.UserId);
        if (existing is null)
        {
            _db.UserSettings.Add(settings);
        }
        else
        {
            existing.Theme                 = settings.Theme;
            existing.DefaultAwakenerId     = settings.DefaultAwakenerId;
            existing.DefaultCritMultiplier = settings.DefaultCritMultiplier;
            existing.DefaultIsCritical     = settings.DefaultIsCritical;
            _db.UserSettings.Update(existing);
        }
        await _db.SaveChangesAsync();
        return existing ?? settings;
    }
}
