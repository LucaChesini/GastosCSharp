using Gastos.Api.Dtos;
using Gastos.Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Gastos.Api.Endpoints;

public static class RelatorioEndpoints
{
    public static IEndpointRouteBuilder MapRelatorioEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder grupo = app.MapGroup("/api/v1/relatorios").WithTags("Relatórios");

        grupo.MapGet("/mensal", ObterMensalAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<Results<Ok<IReadOnlyList<RelatorioMensalResponse>>, ProblemHttpResult>> ObterMensalAsync(
        int? ano,
        int? mes,
        RelatorioService service)
    {
        Resultado<IReadOnlyList<RelatorioMensalResponse>> resultado = await service.ObterMensalAsync(ano, mes);

        return resultado.Sucesso ? TypedResults.Ok(resultado.Valor) : resultado.Falha.ParaProblema();
    }
}
