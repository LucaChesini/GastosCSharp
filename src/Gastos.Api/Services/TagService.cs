using Gastos.Api.Data;
using Gastos.Api.Domain;
using Gastos.Api.Dtos;

using Microsoft.EntityFrameworkCore;

namespace Gastos.Api.Services;

public sealed class TagService(AppDbContext db)
{
    public async Task<IReadOnlyList<TagResponse>> ListarAsync()
    {
        return await db.Tags
            .OrderBy(t => t.Nome)
            .Select(t => new TagResponse(t.Id, t.Nome))
            .ToListAsync();
    }

    public async Task<Resultado<TagResponse>> CriarAsync(TagRequest request)
    {
        Dictionary<string, string[]> erros = Validar(request);
        if (erros.Count > 0)
        {
            return Falha.Validacao(erros);
        }

        string nome = request.Nome.Trim().ToLowerInvariant();
        if (await db.Tags.AnyAsync(t => t.Nome == nome))
        {
            return Falha.Conflito($"Já existe uma tag com o nome '{nome}'.");
        }

        Tag tag = new() { Nome = nome };
        db.Tags.Add(tag);
        await db.SaveChangesAsync();

        return new TagResponse(tag.Id, tag.Nome);
    }

    public async Task<Resultado> RemoverAsync(int id)
    {
        int removidas = await db.Tags.Where(t => t.Id == id).ExecuteDeleteAsync();

        return removidas == 0 ? Falha.NaoEncontrado($"Tag {id} não encontrada.") : Resultado.Ok;
    }

    private static Dictionary<string, string[]> Validar(TagRequest request)
    {
        Dictionary<string, string[]> erros = [];

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            erros[nameof(request.Nome)] = ["O nome é obrigatório."];
        }
        else if (request.Nome.Trim().Length > Tag.TamanhoMaximoNome)
        {
            erros[nameof(request.Nome)] = [$"O nome deve ter no máximo {Tag.TamanhoMaximoNome} caracteres."];
        }

        return erros;
    }
}
