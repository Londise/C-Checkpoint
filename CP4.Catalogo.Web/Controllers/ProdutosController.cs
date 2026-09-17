using CP4.Catalogo.Web.Models;
using CP4.Catalogo.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CP4.Catalogo.Web.Controllers;

public sealed class ProdutosController : Controller
{
    private readonly ApiService _apiService;

    public ProdutosController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(string? provider)
    {
        var selectedProvider = NormalizeProvider(provider);
        ViewBag.SelectedProvider = selectedProvider;

        var produtos = await _apiService.GetProdutosAsync(selectedProvider);
        return View(produtos);
    }

    public async Task<IActionResult> Details(int id, string? provider)
    {
        var selectedProvider = NormalizeProvider(provider);
        ViewBag.SelectedProvider = selectedProvider;

        var produto = await _apiService.GetProdutoByIdAsync(id, selectedProvider);
        return produto is null ? NotFound() : View(produto);
    }

    public async Task<IActionResult> Create(string? provider)
    {
        var selectedProvider = NormalizeProvider(provider);
        ViewBag.SelectedProvider = selectedProvider;

        return View(new ProdutoCreateViewModel
        {
            Categorias = await _apiService.GetCategoriasAsync(selectedProvider)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProdutoCreateViewModel model, string? provider)
    {
        var selectedProvider = NormalizeProvider(provider);
        ViewBag.SelectedProvider = selectedProvider;

        if (!ModelState.IsValid)
        {
            model.Categorias = await _apiService.GetCategoriasAsync(selectedProvider);
            return View(model);
        }

        var created = await _apiService.CreateProdutoAsync(model, selectedProvider);
        if (created)
        {
            return RedirectToAction(nameof(Index), new { provider = selectedProvider });
        }

        ModelState.AddModelError(string.Empty, "Não foi possível cadastrar o produto. Confira os dados e a API.");
        model.Categorias = await _apiService.GetCategoriasAsync(selectedProvider);
        return View(model);
    }

    private static string NormalizeProvider(string? provider) =>
        string.Equals(provider, "Oracle", StringComparison.OrdinalIgnoreCase)
            ? "Oracle"
            : "SqlServer";
}
