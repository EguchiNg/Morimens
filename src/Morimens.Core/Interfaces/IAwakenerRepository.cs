using Morimens.Core.Models;

namespace Morimens.Core.Interfaces;

public interface IAwakenerRepository
{
    IReadOnlyList<Awakener> GetAll();
    Awakener? GetById(int id);
    IReadOnlyList<Awakener> Search(string name);
}
