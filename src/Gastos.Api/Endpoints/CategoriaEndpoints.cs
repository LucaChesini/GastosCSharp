using Gastos.Api.Dtos;
using Gastos.Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Gastos.Api.Endpoints;

public static class CategoriaEndpoints
{
    private const string Rota = "/api/v1/categorias";

    public static IEndpointRouteBuilder MapCategoriaEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder grupo = app.MapGroup(Rota).WithTags("Categorias");

        grupo.MapGet("", ListarAsync);
        grupo.MapPost("", CriarAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
        grupo.MapPut("/{id:int}", AtualizarAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        grupo.MapDelete("/{id:int}", RemoverAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<CategoriaResponse>>> ListarAsync(CategoriaService service)
    {
        return TypedResults.Ok(await service.ListarAsync());
    }

    private static async Task<Results<Created<CategoriaResponse>, ProblemHttpResult>> CriarAsync(
        CategoriaRequest request,
        CategoriaService service)
    {
        Resultado<CategoriaResponse> resultado = await service.CriarAsync(request);
        if (!resultado.Sucesso)
        {
            return resultado.Falha.ParaProblema();
        }

        return TypedResults.Created($"{Rota}/{resultado.Valor.Id}", resultado.Valor);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> AtualizarAsync(
        int id,
        CategoriaRequest request,
        CategoriaService service)
    {
        Resultado resultado = await service.AtualizarAsync(id, request);

        return resultado.Sucesso ? TypedResults.NoContent() : resultado.Falha.ParaProblema();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RemoverAsync(int id, CategoriaService service)
    {
        Resultado resultado = await service.RemoverAsync(id);

        return resultado.Sucesso ? TypedResults.NoContent() : resultado.Falha.ParaProblema();
    }
}
