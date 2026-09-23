using System.Text.RegularExpressions;

using Gastos.Api.Data;
using Gastos.Api.Domain;
using Gastos.Api.Dtos;

namespace Gastos.Api.Services;

public sealed partial class CategoriaService(DadosEmMemoria dados)
{
    public Task<IReadOnlyList<CategoriaResponse>> ListarAsync()
    {
        return Task.FromResult(Listar());
    }

    public Task<Resultado<CategoriaResponse>> CriarAsync(CategoriaRequest request)
    {
        return Task.FromResult(Criar(request));
    }

    public Task<Resultado> AtualizarAsync(int id, CategoriaRequest request)
    {
        return Task.FromResult(Atualizar(id, request));
    }

    public Task<Resultado> RemoverAsync(int id)
    {
        return Task.FromResult(Remover(id));
    }

    private IReadOnlyList<CategoriaResponse> Listar()
    {
        lock (dados.Trava)
        {
            return dados.Categorias
                .OrderBy(c => c.Nome, StringComparer.OrdinalIgnoreCase)
                .Select(c => new CategoriaResponse(c.Id, c.Nome, c.Cor))
                .ToList();
        }
    }

    private Resultado<CategoriaResponse> Criar(CategoriaRequest request)
    {
        Dictionary<string, string[]> erros = Validar(request);
        if (erros.Count > 0)
        {
            return Falha.Validacao(erros);
        }

        string nome = request.Nome.Trim();

        lock (dados.Trava)
        {
            if (NomeEmUso(nome, idIgnorado: null))
            {
                return NomeDuplicado(nome);
            }

            Categoria categoria = new()
            {
                Id = dados.ProximoIdCategoria(),
                Nome = nome,
                Cor = request.Cor,
            };
            dados.Categorias.Add(categoria);

            return new CategoriaResponse(categoria.Id, categoria.Nome, categoria.Cor);
        }
    }

    private Resultado Atualizar(int id, CategoriaRequest request)
    {
        lock (dados.Trava)
        {
            Categoria? categoria = dados.Categorias.Find(c => c.Id == id);
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
            if (NomeEmUso(nome, idIgnorado: id))
            {
                return NomeDuplicado(nome);
            }

            categoria.Nome = nome;
            categoria.Cor = request.Cor;

            return Resultado.Ok;
        }
    }

    private Resultado Remover(int id)
    {
        lock (dados.Trava)
        {
            Categoria? categoria = dados.Categorias.Find(c => c.Id == id);
            if (categoria is null)
            {
                return NaoEncontrada(id);
            }

            int despesasDependentes = dados.Despesas.Count(d => d.CategoriaId == id);
            if (despesasDependentes > 0)
            {
                return Falha.Conflito(
                    $"A categoria {id} possui {despesasDependentes} despesa(s) vinculada(s) e não pode ser removida.",
                    new Dictionary<string, object?> { ["despesasDependentes"] = despesasDependentes });
            }

            dados.Categorias.Remove(categoria);

            return Resultado.Ok;
        }
    }

    private bool NomeEmUso(string nome, int? idIgnorado)
    {
        return dados.Categorias.Exists(c =>
            c.Id != idIgnorado && string.Equals(c.Nome, nome, StringComparison.OrdinalIgnoreCase));
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

        if (string.IsNullOrWhiteSpace(request.Cor) || !CorHexadecimal().IsMatch(request.Cor))
        {
            erros[nameof(request.Cor)] = ["A cor é obrigatória e deve estar no formato hexadecimal #RRGGBB."];
        }

        return erros;
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex CorHexadecimal();
}
