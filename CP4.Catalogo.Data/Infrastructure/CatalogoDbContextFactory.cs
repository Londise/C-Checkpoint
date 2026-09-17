using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CP4.Catalogo.Data.Infrastructure;

public sealed class CatalogoDbContextFactory
{
    private readonly IConfiguration _configuration;

    public CatalogoDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CatalogoDbContext Create(DatabaseProvider provider)
    {
        var connectionString = _configuration.GetConnectionString(provider.ToString());
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"A connection string para {provider} não foi configurada.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<CatalogoDbContext>();

        if (provider == DatabaseProvider.Oracle)
        {
            optionsBuilder.UseOracle(connectionString);
        }
        else
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        return new CatalogoDbContext(optionsBuilder.Options);
    }
}
