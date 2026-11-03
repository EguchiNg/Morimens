using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Morimens.Infrastructure.Data;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations add …</c>.
/// Run from the Infrastructure project directory:
///   dotnet ef migrations add InitialCreate --startup-project ../Morimens.Api
/// </summary>
public class MorimensDbContextFactory : IDesignTimeDbContextFactory<MorimensDbContext>
{
    public MorimensDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<MorimensDbContext>()
            .UseSqlite("Data Source=morimens_dev.db")
            .Options;
        return new MorimensDbContext(opts);
    }
}
