namespace Gastos.Api.Dtos;

public record DespesaRequest(
    string Descricao,
    decimal Valor,
    DateTime Data,
    int CategoriaId,
    IReadOnlyList<int>? TagIds = null);
