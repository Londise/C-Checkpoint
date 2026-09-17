namespace CP4.Catalogo.Web.Models;

public class ProdutoViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public int CategoriaId { get; set; }
    public string? NomeCategoria { get; set; }
}
