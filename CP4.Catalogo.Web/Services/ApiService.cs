using System.Net.Http.Json;
using CP4.Catalogo.Web.Models;

namespace CP4.Catalogo.Web.Services;

public sealed class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ProdutoViewModel>> GetProdutosAsync(string provider)
    {
        using var request = CreateRequest(HttpMethod.Get, "api/produtos", provider);
        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<ProdutoViewModel>();
        }

        return await response.Content.ReadFromJsonAsync<IEnumerable<ProdutoViewModel>>()
               ?? Enumerable.Empty<ProdutoViewModel>();
    }

    public async Task<ProdutoViewModel?> GetProdutoByIdAsync(int id, string provider)
    {
        using var request = CreateRequest(HttpMethod.Get, $"api/produtos/{id}", provider);
        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ProdutoViewModel>();
    }

    public async Task<List<CategoriaViewModel>> GetCategoriasAsync(string provider)
    {
        using var request = CreateRequest(HttpMethod.Get, "api/categorias", provider);
        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return new List<CategoriaViewModel>();
        }

        return await response.Content.ReadFromJsonAsync<List<CategoriaViewModel>>()
               ?? new List<CategoriaViewModel>();
    }

    public async Task<bool> CreateProdutoAsync(ProdutoCreateViewModel model, string provider)
    {
        using var request = CreateRequest(HttpMethod.Post, "api/produtos", provider);
        request.Content = JsonContent.Create(new
        {
            model.Nome,
            model.Descricao,
            model.Preco,
            model.QuantidadeEstoque,
            model.CategoriaId
        });

        using var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string uri, string provider)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Add("X-Database-Provider", provider);
        return request;
    }
}
