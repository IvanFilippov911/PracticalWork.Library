using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PracticalWork.Library.Data.Reports.PostgreSql;

/// <summary>
/// Фабрика контекста reports для design-time операций EF Core.
/// </summary>
public sealed class ReportsDbContextFactory : IDesignTimeDbContextFactory<ReportsDbContext>
{
    public ReportsDbContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionString(args);

        var optionsBuilder = new DbContextOptionsBuilder<ReportsDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ReportsDbContext(optionsBuilder.Options, TimeProvider.System);
    }

    private static string GetConnectionString(string[] args)
    {
        var fromArgs = args
            .FirstOrDefault(arg => arg.StartsWith("ConnectionString=", StringComparison.OrdinalIgnoreCase))
            ?.Split('=', 2)[1];

        return fromArgs
               ?? Environment.GetEnvironmentVariable("App__ConnectionStrings__ReportsDbContext")
               ?? Environment.GetEnvironmentVariable("ConnectionStrings__ReportsDbContext")
               ?? "Host=localhost;Port=5439;Database=reports;Username=postgres;Password=postgres";
    }
}
