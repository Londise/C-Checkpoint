using CP4.Catalogo.Data.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CP4.Catalogo.Api.Controllers;

public abstract class CatalogoControllerBase : ControllerBase
{
    protected CatalogoControllerBase(
        CatalogoDbContextFactory contextFactory,
        DatabaseProviderResolver providerResolver)
    {
        ContextFactory = contextFactory;
        ProviderResolver = providerResolver;
    }

    protected CatalogoDbContextFactory ContextFactory { get; }
    protected DatabaseProviderResolver ProviderResolver { get; }

    protected bool TryResolveProvider(string? requestedProvider, out DatabaseProvider provider)
    {
        if (!ProviderResolver.TryResolve(requestedProvider, out provider))
        {
            return false;
        }

        Response.Headers["X-Database-Provider"] = provider.ToString();
        return true;
    }

    protected IActionResult InvalidProvider() => BadRequest(new
    {
        mensagem = "O header X-Database-Provider deve ser SqlServer ou Oracle."
    });
}
