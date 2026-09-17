using CP4.Catalogo.Api.DTOs;
using CP4.Catalogo.Data.Entities;
using CP4.Catalogo.Data.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CP4.Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProdutosController : CatalogoControllerBase
{
    public ProdutosController(
        CatalogoDbContextFactory contextFactory,
        DatabaseProviderResolver providerResolver)
        : base(contextFactory, providerResolver)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromHeader(Name = "X-Database-Provider")] string? dbProvider)
    {
        if (!TryResolveProvider(dbProvider, out var provider))
        {
            return InvalidProvider();
        }

        await using var context = ContextFactory.Create(provider);
        var produtos = await context.Produtos
            .AsNoTracking()
            .Include(produto => produto.Categoria)
            .OrderBy(produto => produto.Id)
            .ToListAsync();

        return Ok(produtos.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        [FromHeader(Name = "X-Database-Provider")] string? dbProvider)
    {
        if (!TryResolveProvider(dbProvider, out var provider))
        {
            return InvalidProvider();
        }

        await using var context = ContextFactory.Create(provider);
        var produto = await context.Produtos
            .AsNoTracking()
            .Include(item => item.Categoria)
            .FirstOrDefaultAsync(item => item.Id == id);

        return produto is null ? NotFound() : Ok(ToResponse(produto));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] ProdutoCreateDto dto,
        [FromHeader(Name = "X-Database-Provider")] string? dbProvider)
    {
        if (!TryResolveProvider(dbProvider, out var provider))
        {
            return InvalidProvider();
        }

        await using var context = ContextFactory.Create(provider);
        var categoria = await context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
        {
            return BadRequest(new { mensagem = "A categoria informada não existe." });
        }

        var produto = new Produto
        {
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao?.Trim(),
            Preco = dto.Preco,
            QuantidadeEstoque = dto.QuantidadeEstoque,
            CategoriaId = dto.CategoriaId,
            Categoria = categoria
        };

        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, ToResponse(produto));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] ProdutoUpdateDto dto,
        [FromHeader(Name = "X-Database-Provider")] string? dbProvider)
    {
        if (!TryResolveProvider(dbProvider, out var provider))
        {
            return InvalidProvider();
        }

        await using var context = ContextFactory.Create(provider);
        var produto = await context.Produtos.FindAsync(id);
        if (produto is null)
        {
            return NotFound();
        }

        var categoriaExiste = await context.Categorias.AnyAsync(categoria => categoria.Id == dto.CategoriaId);
        if (!categoriaExiste)
        {
            return BadRequest(new { mensagem = "A categoria informada não existe." });
        }

        produto.Nome = dto.Nome.Trim();
        produto.Descricao = dto.Descricao?.Trim();
        produto.Preco = dto.Preco;
        produto.QuantidadeEstoque = dto.QuantidadeEstoque;
        produto.CategoriaId = dto.CategoriaId;

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        [FromHeader(Name = "X-Database-Provider")] string? dbProvider)
    {
        if (!TryResolveProvider(dbProvider, out var provider))
        {
            return InvalidProvider();
        }

        await using var context = ContextFactory.Create(provider);
        var produto = await context.Produtos.FindAsync(id);
        if (produto is null)
        {
            return NotFound();
        }

        context.Produtos.Remove(produto);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static ProdutoResponseDto ToResponse(Produto produto) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        Descricao = produto.Descricao,
        Preco = produto.Preco,
        QuantidadeEstoque = produto.QuantidadeEstoque,
        CategoriaId = produto.CategoriaId,
        NomeCategoria = produto.Categoria?.Nome
    };
}
