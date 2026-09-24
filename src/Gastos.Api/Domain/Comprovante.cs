namespace Gastos.Api.Domain;

public class Comprovante
{
    public const int TamanhoMaximoUrl = 2048;
    public const int TamanhoMaximoObservacao = 500;

    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public string? Observacao { get; set; }

    public int DespesaId { get; set; }
    public Despesa Despesa { get; set; } = null!;
}
