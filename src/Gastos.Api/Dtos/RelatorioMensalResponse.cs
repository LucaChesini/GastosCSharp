namespace Gastos.Api.Dtos;

public record RelatorioMensalResponse(
    int CategoriaId,
    string Categoria,
    string Cor,
    decimal Total,
    int Quantidade,
    decimal Percentual);
