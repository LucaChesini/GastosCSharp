using Gastos.Api.Dtos;
using Gastos.Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Gastos.Api.Endpoints;

public static class DespesaEndpoints
{
    private const string RotaObterDespesa = "ObterDespesa";

    public static IEndpointRouteBuilder MapDespesaEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder grupo = app.MapGroup("/api/v1/despesas").WithTags("Despesas");

        grupo.MapGet("", ListarAsync);
        grupo.MapGet("/{id:int}", ObterAsync)
            .WithName(RotaObterDespesa)
            .ProducesProblem(StatusCodes.Status404NotFound);
        grupo.MapPost("", CriarAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        grupo.MapPut("/{id:int}", AtualizarAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        grupo.MapDelete("/{id:int}", RemoverAsync)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<DespesaResponse>>> ListarAsync(DespesaService service)
    {
        return TypedResults.Ok(await service.ListarAsync());
    }

    private static async Task<Results<Ok<DespesaResponse>, ProblemHttpResult>> ObterAsync(int id, DespesaService service)
    {
        Resultado<DespesaResponse> resultado = await service.ObterAsync(id);

        return resultado.Sucesso ? TypedResults.Ok(resultado.Valor) : resultado.Falha.ParaProblema();
    }

    private static async Task<Results<CreatedAtRoute<DespesaResponse>, ProblemHttpResult>> CriarAsync(
        DespesaRequest request,
        DespesaService service)
    {
        Resultado<DespesaResponse> resultado = await service.CriarAsync(request);
        if (!resultado.Sucesso)
        {
            return resultado.Falha.ParaProblema();
        }

        return TypedResults.CreatedAtRoute(resultado.Valor, RotaObterDespesa, new { id = resultado.Valor.Id });
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> AtualizarAsync(
        int id,
        DespesaRequest request,
        DespesaService service)
    {
        Resultado resultado = await service.AtualizarAsync(id, request);

        return resultado.Sucesso ? TypedResults.NoContent() : resultado.Falha.ParaProblema();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RemoverAsync(int id, DespesaService service)
    {
        Resultado resultado = await service.RemoverAsync(id);

        return resultado.Sucesso ? TypedResults.NoContent() : resultado.Falha.ParaProblema();
    }
}
