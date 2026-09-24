using Gastos.Api.Data;
using Gastos.Api.Domain;
using Gastos.Api.Dtos;

using Microsoft.EntityFrameworkCore;

namespace Gastos.Api.Services;

public sealed class ComprovanteService(AppDbContext db)
{
    public async Task<Resultado> DefinirAsync(int despesaId, ComprovanteRequest request)
    {
        Despesa? despesa = await db.Despesas
            .Include(d => d.Comprovante)
            .FirstOrDefaultAsync(d => d.Id == despesaId);

        if (despesa is null)
        {
            return DespesaNaoEncontrada(despesaId);
        }

        Dictionary<string, string[]> erros = Validar(request);
        if (erros.Count > 0)
        {
            return Falha.Validacao(erros);
        }

        string url = request.Url.Trim();
        string? observacao = string.IsNullOrWhiteSpace(request.Observacao) ? null : request.Observacao.Trim();

        if (despesa.Comprovante is null)
        {
            despesa.Comprovante = new Comprovante { Url = url, Observacao = observacao };
        }
        else
        {
            despesa.Comprovante.Url = url;
            despesa.Comprovante.Observacao = observacao;
        }

        await db.SaveChangesAsync();

        return Resultado.Ok;
    }

    public async Task<Resultado> RemoverAsync(int despesaId)
    {
        int removidos = await db.Comprovantes.Where(c => c.DespesaId == despesaId).ExecuteDeleteAsync();
        if (removidos > 0)
        {
            return Resultado.Ok;
        }

        bool despesaExiste = await db.Despesas.AnyAsync(d => d.Id == despesaId);

        return despesaExiste
            ? Falha.NaoEncontrado($"A despesa {despesaId} não possui comprovante.")
            : DespesaNaoEncontrada(despesaId);
    }

    private static Falha DespesaNaoEncontrada(int despesaId)
    {
        return Falha.NaoEncontrado($"Despesa {despesaId} não encontrada.");
    }

    private static Dictionary<string, string[]> Validar(ComprovanteRequest request)
    {
        Dictionary<string, string[]> erros = [];

        string? url = request.Url?.Trim();
        if (string.IsNullOrEmpty(url))
        {
            erros[nameof(request.Url)] = ["A URL é obrigatória."];
        }
        else if (url.Length > Comprovante.TamanhoMaximoUrl)
        {
            erros[nameof(request.Url)] = [$"A URL deve ter no máximo {Comprovante.TamanhoMaximoUrl} caracteres."];
        }
        else if (!EhUrlHttpAbsoluta(url))
        {
            erros[nameof(request.Url)] = ["A URL deve ser absoluta, com esquema http ou https."];
        }

        if (request.Observacao?.Trim().Length > Comprovante.TamanhoMaximoObservacao)
        {
            erros[nameof(request.Observacao)] = [$"A observação deve ter no máximo {Comprovante.TamanhoMaximoObservacao} caracteres."];
        }

        return erros;
    }

    private static bool EhUrlHttpAbsoluta(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
