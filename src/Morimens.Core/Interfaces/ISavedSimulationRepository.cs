using Morimens.Core.Models;

namespace Morimens.Core.Interfaces;

public interface ISavedSimulationRepository
{
    Task<IReadOnlyList<SavedSimulation>> GetByUserIdAsync(string userId);
    Task<SavedSimulation?>               GetByIdAsync(int id, string userId);
    Task<SavedSimulation>                CreateAsync(SavedSimulation simulation);
    Task<SavedSimulation>                UpdateAsync(SavedSimulation simulation);
    Task<bool>                           DeleteAsync(int id, string userId);
}
