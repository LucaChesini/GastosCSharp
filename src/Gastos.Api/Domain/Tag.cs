namespace Gastos.Api.Domain;

public class Tag
{
    public const int TamanhoMaximoNome = 50;

    public int Id { get; set; }
    public string Nome { get; set; } = null!;

    public List<Despesa> Despesas { get; set; } = [];
}
