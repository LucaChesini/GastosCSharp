using Gastos.Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Gastos.Api.Endpoints;

public static class FalhaExtensions
{
    public static ProblemHttpResult ParaProblema(this Falha falha)
    {
        if (falha.Tipo == TipoFalha.Validacao)
        {
            return TypedResults.Problem(new HttpValidationProblemDetails(falha.Erros ?? new Dictionary<string, string[]>())
            {
                Status = StatusCodes.Status400BadRequest,
                Detail = falha.Detalhe,
            });
        }

        int status = falha.Tipo switch
        {
            TipoFalha.NaoEncontrado => StatusCodes.Status404NotFound,
            TipoFalha.Conflito => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

        return TypedResults.Problem(detail: falha.Detalhe, statusCode: status, extensions: falha.Extensoes);
    }
}
