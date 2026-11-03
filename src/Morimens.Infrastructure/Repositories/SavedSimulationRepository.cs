using Microsoft.EntityFrameworkCore;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;
using Morimens.Infrastructure.Data;

namespace Morimens.Infrastructure.Repositories;

public class SavedSimulationRepository : ISavedSimulationRepository
{
    private readonly MorimensDbContext _db;
    public SavedSimulationRepository(MorimensDbContext db) => _db = db;

    public async Task<IReadOnlyList<SavedSimulation>> GetByUserIdAsync(string userId) =>
        await _db.SavedSimulations
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync();

    public async Task<SavedSimulation?> GetByIdAsync(int id, string userId) =>
        await _db.SavedSimulations
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

    public async Task<SavedSimulation> CreateAsync(SavedSimulation simulation)
    {
        _db.SavedSimulations.Add(simulation);
        await _db.SaveChangesAsync();
        return simulation;
    }

    public async Task<SavedSimulation> UpdateAsync(SavedSimulation simulation)
    {
        simulation.UpdatedAt = DateTime.UtcNow;
        _db.SavedSimulations.Update(simulation);
        await _db.SaveChangesAsync();
        return simulation;
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var sim = await _db.SavedSimulations
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
        if (sim is null) return false;
        _db.SavedSimulations.Remove(sim);
        await _db.SaveChangesAsync();
        return true;
    }
}
