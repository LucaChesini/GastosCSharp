namespace Gastos.Api.Domain;

public class LinhaRelatorioMensal
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public int CategoriaId { get; set; }
    public string Categoria { get; set; } = null!;
    public string Cor { get; set; } = null!;
    public decimal Total { get; set; }
    public int Quantidade { get; set; }
    public decimal Percentual { get; set; }
}
