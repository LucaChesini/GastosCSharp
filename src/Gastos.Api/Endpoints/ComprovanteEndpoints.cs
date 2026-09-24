using Gastos.Api.Dtos;
using Gastos.Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Gastos.Api.Endpoints;

public static class ComprovanteEndpoints
{
    public static IEndpointRouteBuilder MapComprovanteEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder grupo = app.MapGroup("/api/v1/despesas/{id:int}/comprovante").WithTags("Comprovantes");

        grupo.MapPut("", DefinirAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        grupo.MapDelete("", RemoverAsync)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DefinirAsync(
        int id,
        ComprovanteRequest request,
        ComprovanteService service)
    {
        Resultado resultado = await service.DefinirAsync(id, request);

        return resultado.Sucesso ? TypedResults.NoContent() : resultado.Falha.ParaProblema();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RemoverAsync(int id, ComprovanteService service)
    {
        Resultado resultado = await service.RemoverAsync(id);

        return resultado.Sucesso ? TypedResults.NoContent() : resultado.Falha.ParaProblema();
    }
}
