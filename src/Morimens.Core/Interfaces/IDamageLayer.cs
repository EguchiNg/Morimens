using Morimens.Core.Pipeline;

namespace Morimens.Core.Interfaces;

public interface IDamageLayer
{
    string Name  { get; }
    int    Order { get; }
    void   Apply(DamageContext ctx);
}
