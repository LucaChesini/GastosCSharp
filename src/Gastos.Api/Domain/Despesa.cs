namespace Gastos.Api.Domain;

public class Despesa
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }

    public int CategoriaId { get; set; }
}
