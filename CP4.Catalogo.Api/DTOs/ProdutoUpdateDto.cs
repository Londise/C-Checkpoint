using System.ComponentModel.DataAnnotations;

namespace CP4.Catalogo.Api.DTOs;

public class ProdutoUpdateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99", ErrorMessage = "O preço deve ser maior ou igual a zero.")]
    public decimal Preco { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque deve ser maior ou igual a zero.")]
    public int QuantidadeEstoque { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe uma categoria válida.")]
    public int CategoriaId { get; set; }
}
