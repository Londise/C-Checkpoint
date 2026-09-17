using Microsoft.Extensions.Configuration;

namespace CP4.Catalogo.Data.Infrastructure;

public sealed class DatabaseProviderResolver
{
    private readonly string _defaultProvider;

    public DatabaseProviderResolver(IConfiguration configuration)
    {
        _defaultProvider = configuration["Database:DefaultProvider"] ?? nameof(DatabaseProvider.SqlServer);
    }

    public bool TryResolve(string? requestedProvider, out DatabaseProvider provider)
    {
        var value = string.IsNullOrWhiteSpace(requestedProvider)
            ? _defaultProvider
            : requestedProvider.Trim();

        return Enum.TryParse(value, ignoreCase: true, out provider)
               && Enum.IsDefined(provider);
    }
}
