namespace CP4.Catalogo.Api.DTOs;

public class ProdutoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public int CategoriaId { get; set; }
    public string? NomeCategoria { get; set; }
}
