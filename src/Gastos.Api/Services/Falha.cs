namespace Gastos.Api.Services;

public enum TipoFalha
{
    NaoEncontrado,
    Validacao,
    Conflito,
}

public sealed record Falha(
    TipoFalha Tipo,
    string Detalhe,
    IDictionary<string, string[]>? Erros = null,
    IDictionary<string, object?>? Extensoes = null)
{
    public static Falha NaoEncontrado(string detalhe)
    {
        return new Falha(TipoFalha.NaoEncontrado, detalhe);
    }

    public static Falha Validacao(IDictionary<string, string[]> erros)
    {
        return new Falha(TipoFalha.Validacao, "Um ou mais campos são inválidos.", erros);
    }

    public static Falha Conflito(string detalhe, IDictionary<string, object?>? extensoes = null)
    {
        return new Falha(TipoFalha.Conflito, detalhe, Extensoes: extensoes);
    }
}
