namespace Gastos.Api.Dtos;

public record DespesaResponse(
    int Id,
    string Descricao,
    decimal Valor,
    DateTime Data,
    int CategoriaId,
    string CategoriaNome,
    IReadOnlyList<TagResponse> Tags,
    ComprovanteResponse? Comprovante);
