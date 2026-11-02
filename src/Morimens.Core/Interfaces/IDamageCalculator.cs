using Morimens.Core.Pipeline;
using Morimens.Core.Results;

namespace Morimens.Core.Interfaces;

public interface IDamageCalculator
{
    DamageResult Calculate(DamageContext ctx);
}
