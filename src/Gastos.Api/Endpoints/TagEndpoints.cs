using Gastos.Api.Dtos;
using Gastos.Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Gastos.Api.Endpoints;

public static class TagEndpoints
{
    private const string Rota = "/api/v1/tags";

    public static IEndpointRouteBuilder MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder grupo = app.MapGroup(Rota).WithTags("Tags");

        grupo.MapGet("", ListarAsync);
        grupo.MapPost("", CriarAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
        grupo.MapDelete("/{id:int}", RemoverAsync)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<TagResponse>>> ListarAsync(TagService service)
    {
        return TypedResults.Ok(await service.ListarAsync());
    }

    private static async Task<Results<Created<TagResponse>, ProblemHttpResult>> CriarAsync(
        TagRequest request,
        TagService service)
    {
        Resultado<TagResponse> resultado = await service.CriarAsync(request);
        if (!resultado.Sucesso)
        {
            return resultado.Falha.ParaProblema();
        }

        return TypedResults.Created($"{Rota}/{resultado.Valor.Id}", resultado.Valor);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RemoverAsync(int id, TagService service)
    {
        Resultado resultado = await service.RemoverAsync(id);

        return resultado.Sucesso ? TypedResults.NoContent() : resultado.Falha.ParaProblema();
    }
}
