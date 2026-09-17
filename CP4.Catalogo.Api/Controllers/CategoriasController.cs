using CP4.Catalogo.Api.DTOs;
using CP4.Catalogo.Data.Entities;
using CP4.Catalogo.Data.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CP4.Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriasController : CatalogoControllerBase
{
    public CategoriasController(
        CatalogoDbContextFactory contextFactory,
        DatabaseProviderResolver providerResolver)
        : base(contextFactory, providerResolver)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromHeader(Name = "X-Database-Provider")] string? dbProvider)
    {
        if (!TryResolveProvider(dbProvider, out var provider))
        {
            return InvalidProvider();
        }

        await using var context = ContextFactory.Create(provider);
        var categorias = await context.Categorias
            .AsNoTracking()
            .OrderBy(categoria => categoria.Nome)
            .ToListAsync();

        return Ok(categorias.Select(ToResponse));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CategoriaCreateDto dto,
        [FromHeader(Name = "X-Database-Provider")] string? dbProvider)
    {
        if (!TryResolveProvider(dbProvider, out var provider))
        {
            return InvalidProvider();
        }

        await using var context = ContextFactory.Create(provider);
        var nome = dto.Nome.Trim();
        var jaExiste = await context.Categorias.AnyAsync(categoria => categoria.Nome == nome);
        if (jaExiste)
        {
            return BadRequest(new { mensagem = "Já existe uma categoria com esse nome." });
        }

        var categoria = new Categoria
        {
            Nome = nome,
            Descricao = dto.Descricao?.Trim()
        };

        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, ToResponse(categoria));
    }

    private static CategoriaResponseDto ToResponse(Categoria categoria) => new()
    {
        Id = categoria.Id,
        Nome = categoria.Nome,
        Descricao = categoria.Descricao
    };
}
