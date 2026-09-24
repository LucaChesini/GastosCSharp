using Gastos.Api.Data;
using Gastos.Api.Domain;
using Gastos.Api.Dtos;

using Microsoft.EntityFrameworkCore;

namespace Gastos.Api.Services;

public sealed class DespesaService(AppDbContext db, TimeProvider relogio)
{
    public async Task<IReadOnlyList<DespesaResponse>> ListarAsync()
    {
        return await ProjetarParaResposta(db.Despesas
                .OrderByDescending(d => d.Data)
                .ThenByDescending(d => d.Id))
            .ToListAsync();
    }

    public async Task<Resultado<DespesaResponse>> ObterAsync(int id)
    {
        DespesaResponse? despesa = await ProjetarParaResposta(db.Despesas.Where(d => d.Id == id))
            .FirstOrDefaultAsync();

        return despesa is null ? NaoEncontrada(id) : despesa;
    }

    public async Task<Resultado<DespesaResponse>> CriarAsync(DespesaRequest request)
    {
        int[] tagIds = IdsDistintos(request.TagIds);
        List<Tag> tags = await CarregarTagsAsync(tagIds);

        Dictionary<string, string[]> erros = await ValidarAsync(request, tagIds, tags);
        if (erros.Count > 0)
        {
            return Falha.Validacao(erros);
        }

        Despesa despesa = new()
        {
            Descricao = request.Descricao.Trim(),
            Valor = request.Valor,
            Data = request.Data,
            CategoriaId = request.CategoriaId,
            Tags = tags,
        };
        db.Despesas.Add(despesa);
        await db.SaveChangesAsync();

        return await ProjetarParaResposta(db.Despesas.Where(d => d.Id == despesa.Id)).FirstAsync();
    }

    public async Task<Resultado> AtualizarAsync(int id, DespesaRequest request)
    {
        Despesa? despesa = await db.Despesas
            .Include(d => d.Tags)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (despesa is null)
        {
            return NaoEncontrada(id);
        }

        int[] tagIds = IdsDistintos(request.TagIds);
        List<Tag> tags = await CarregarTagsAsync(tagIds);

        Dictionary<string, string[]> erros = await ValidarAsync(request, tagIds, tags);
        if (erros.Count > 0)
        {
            return Falha.Validacao(erros);
        }

        despesa.Descricao = request.Descricao.Trim();
        despesa.Valor = request.Valor;
        despesa.Data = request.Data;
        despesa.CategoriaId = request.CategoriaId;
        despesa.Tags.Clear();
        despesa.Tags.AddRange(tags);
        await db.SaveChangesAsync();

        return Resultado.Ok;
    }

    public async Task<Resultado> RemoverAsync(int id)
    {
        int removidas = await db.Despesas.Where(d => d.Id == id).ExecuteDeleteAsync();

        return removidas == 0 ? NaoEncontrada(id) : Resultado.Ok;
    }

    private static IQueryable<DespesaResponse> ProjetarParaResposta(IQueryable<Despesa> despesas)
    {
        return despesas.Select(d => new DespesaResponse(
            d.Id,
            d.Descricao,
            d.Valor,
            d.Data,
            d.CategoriaId,
            d.Categoria.Nome,
            d.Tags
                .OrderBy(t => t.Nome)
                .Select(t => new TagResponse(t.Id, t.Nome))
                .ToList(),
            d.Comprovante == null
                ? null
                : new ComprovanteResponse(d.Comprovante.Url, d.Comprovante.Observacao)));
    }

    private static int[] IdsDistintos(IReadOnlyList<int>? ids)
    {
        return ids is null ? [] : ids.Distinct().ToArray();
    }

    private async Task<List<Tag>> CarregarTagsAsync(int[] tagIds)
    {
        if (tagIds.Length == 0)
        {
            return [];
        }

        return await db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
    }

    private static Falha NaoEncontrada(int id)
    {
        return Falha.NaoEncontrado($"Despesa {id} não encontrada.");
    }

    private async Task<Dictionary<string, string[]>> ValidarAsync(
        DespesaRequest request,
        int[] tagIds,
        List<Tag> tagsEncontradas)
    {
        Dictionary<string, string[]> erros = [];

        if (string.IsNullOrWhiteSpace(request.Descricao))
        {
            erros[nameof(request.Descricao)] = ["A descrição é obrigatória."];
        }
        else if (request.Descricao.Trim().Length > Despesa.TamanhoMaximoDescricao)
        {
            erros[nameof(request.Descricao)] = [$"A descrição deve ter no máximo {Despesa.TamanhoMaximoDescricao} caracteres."];
        }

        if (request.Valor <= 0)
        {
            erros[nameof(request.Valor)] = ["O valor deve ser maior que zero."];
        }
        else if (request.Valor > Despesa.ValorMaximo)
        {
            erros[nameof(request.Valor)] = [$"O valor deve ser no máximo {Despesa.ValorMaximo}."];
        }

        if (request.Data == default)
        {
            erros[nameof(request.Data)] = ["A data é obrigatória."];
        }
        else if (request.Data.Date > relogio.GetLocalNow().Date)
        {
            erros[nameof(request.Data)] = ["A data não pode ser futura."];
        }

        if (!await db.Categorias.AnyAsync(c => c.Id == request.CategoriaId))
        {
            erros[nameof(request.CategoriaId)] = [$"A categoria {request.CategoriaId} não existe."];
        }

        int[] tagsInexistentes = tagIds.Except(tagsEncontradas.Select(t => t.Id)).ToArray();
        if (tagsInexistentes.Length > 0)
        {
            erros[nameof(request.TagIds)] = tagsInexistentes.Select(tagId => $"A tag {tagId} não existe.").ToArray();
        }

        return erros;
    }
}
