using System.Text.RegularExpressions;

using Gastos.Api.Data;
using Gastos.Api.Domain;
using Gastos.Api.Dtos;

using Microsoft.EntityFrameworkCore;

namespace Gastos.Api.Services;

public sealed partial class CategoriaService(AppDbContext db)
{
    public async Task<IReadOnlyList<CategoriaResponse>> ListarAsync()
    {
        return await db.Categorias
            .OrderBy(c => c.Nome)
            .Select(c => new CategoriaResponse(c.Id, c.Nome, c.Cor))
            .ToListAsync();
    }

    public async Task<Resultado<CategoriaResponse>> CriarAsync(CategoriaRequest request)
    {
        Dictionary<string, string[]> erros = Validar(request);
        if (erros.Count > 0)
        {
            return Falha.Validacao(erros);
        }

        string nome = request.Nome.Trim();
        if (await NomeEmUsoAsync(nome, idIgnorado: null))
        {
            return NomeDuplicado(nome);
        }

        Categoria categoria = new()
        {
            Nome = nome,
            Cor = request.Cor,
        };
        db.Categorias.Add(categoria);
        await db.SaveChangesAsync();

        return new CategoriaResponse(categoria.Id, categoria.Nome, categoria.Cor);
    }

    public async Task<Resultado> AtualizarAsync(int id, CategoriaRequest request)
    {
        Categoria? categoria = await db.Categorias.FindAsync(id);
        if (categoria is null)
        {
            return NaoEncontrada(id);
        }

        Dictionary<string, string[]> erros = Validar(request);
        if (erros.Count > 0)
        {
            return Falha.Validacao(erros);
        }

        string nome = request.Nome.Trim();
        if (await NomeEmUsoAsync(nome, idIgnorado: id))
        {
            return NomeDuplicado(nome);
        }

        categoria.Nome = nome;
        categoria.Cor = request.Cor;
        await db.SaveChangesAsync();

        return Resultado.Ok;
    }

    public async Task<Resultado> RemoverAsync(int id)
    {
        int? despesasDependentes = await db.Categorias
            .Where(c => c.Id == id)
            .Select(c => (int?)c.Despesas.Count)
            .FirstOrDefaultAsync();

        if (despesasDependentes is null)
        {
            return NaoEncontrada(id);
        }

        if (despesasDependentes > 0)
        {
            return Falha.Conflito(
                $"A categoria {id} possui {despesasDependentes} despesa(s) vinculada(s) e não pode ser removida.",
                new Dictionary<string, object?> { ["despesasDependentes"] = despesasDependentes });
        }

        await db.Categorias.Where(c => c.Id == id).ExecuteDeleteAsync();

        return Resultado.Ok;
    }

    private Task<bool> NomeEmUsoAsync(string nome, int? idIgnorado)
    {
        return db.Categorias.AnyAsync(c => c.Id != idIgnorado && c.Nome == nome);
    }

    private static Falha NaoEncontrada(int id)
    {
        return Falha.NaoEncontrado($"Categoria {id} não encontrada.");
    }

    private static Falha NomeDuplicado(string nome)
    {
        return Falha.Conflito($"Já existe uma categoria com o nome '{nome}'.");
    }

    private static Dictionary<string, string[]> Validar(CategoriaRequest request)
    {
        Dictionary<string, string[]> erros = [];

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            erros[nameof(request.Nome)] = ["O nome é obrigatório."];
        }
        else if (request.Nome.Trim().Length > Categoria.TamanhoMaximoNome)
        {
            erros[nameof(request.Nome)] = [$"O nome deve ter no máximo {Categoria.TamanhoMaximoNome} caracteres."];
        }

        if (string.IsNullOrWhiteSpace(request.Cor) || !CorHexadecimal().IsMatch(request.Cor))
        {
            erros[nameof(request.Cor)] = ["A cor é obrigatória e deve estar no formato hexadecimal #RRGGBB."];
        }

        return erros;
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex CorHexadecimal();
}
