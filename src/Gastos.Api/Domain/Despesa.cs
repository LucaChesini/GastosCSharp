namespace Gastos.Api.Domain;

public class Despesa
{
    public const int TamanhoMaximoDescricao = 200;
    public const decimal ValorMaximo = 9_999_999_999_999_999.99m;

    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public List<Tag> Tags { get; set; } = [];
    public Comprovante? Comprovante { get; set; }
}
