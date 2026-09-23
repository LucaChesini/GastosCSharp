using Gastos.Api.Data;
using Gastos.Api.Domain;
using Gastos.Api.Dtos;

namespace Gastos.Api.Services;

public sealed class DespesaService(DadosEmMemoria dados, TimeProvider relogio)
{
    public Task<IReadOnlyList<DespesaResponse>> ListarAsync()
    {
        return Task.FromResult(Listar());
    }

    public Task<Resultado<DespesaResponse>> ObterAsync(int id)
    {
        return Task.FromResult(Obter(id));
    }

    public Task<Resultado<DespesaResponse>> CriarAsync(DespesaRequest request)
    {
        return Task.FromResult(Criar(request));
    }

    public Task<Resultado> AtualizarAsync(int id, DespesaRequest request)
    {
        return Task.FromResult(Atualizar(id, request));
    }

    public Task<Resultado> RemoverAsync(int id)
    {
        return Task.FromResult(Remover(id));
    }

    private IReadOnlyList<DespesaResponse> Listar()
    {
        lock (dados.Trava)
        {
            return ProjetarComCategoria(dados.Despesas
                    .OrderByDescending(d => d.Data)
                    .ThenByDescending(d => d.Id))
                .ToList();
        }
    }

    private Resultado<DespesaResponse> Obter(int id)
    {
        lock (dados.Trava)
        {
            DespesaResponse? despesa = ProjetarComCategoria(dados.Despesas.Where(d => d.Id == id)).FirstOrDefault();

            return despesa is null ? NaoEncontrada(id) : despesa;
        }
    }

    private Resultado<DespesaResponse> Criar(DespesaRequest request)
    {
        lock (dados.Trava)
        {
            Dictionary<string, string[]> erros = Validar(request);
            if (erros.Count > 0)
            {
                return Falha.Validacao(erros);
            }

            Despesa despesa = new()
            {
                Id = dados.ProximoIdDespesa(),
                Descricao = request.Descricao.Trim(),
                Valor = request.Valor,
                Data = request.Data,
                CategoriaId = request.CategoriaId,
            };
            dados.Despesas.Add(despesa);

            return ProjetarComCategoria([despesa]).First();
        }
    }

    private Resultado Atualizar(int id, DespesaRequest request)
    {
        lock (dados.Trava)
        {
            Despesa? despesa = dados.Despesas.Find(d => d.Id == id);
            if (despesa is null)
            {
                return NaoEncontrada(id);
            }

            Dictionary<string, string[]> erros = Validar(request);
            if (erros.Count > 0)
            {
                return Falha.Validacao(erros);
            }

            despesa.Descricao = request.Descricao.Trim();
            despesa.Valor = request.Valor;
            despesa.Data = request.Data;
            despesa.CategoriaId = request.CategoriaId;

            return Resultado.Ok;
        }
    }

    private Resultado Remover(int id)
    {
        lock (dados.Trava)
        {
            int removidas = dados.Despesas.RemoveAll(d => d.Id == id);

            return removidas == 0 ? NaoEncontrada(id) : Resultado.Ok;
        }
    }

    private IEnumerable<DespesaResponse> ProjetarComCategoria(IEnumerable<Despesa> despesas)
    {
        return despesas.Join(
            dados.Categorias,
            d => d.CategoriaId,
            c => c.Id,
            (d, c) => new DespesaResponse(d.Id, d.Descricao, d.Valor, d.Data, d.CategoriaId, c.Nome));
    }

    private static Falha NaoEncontrada(int id)
    {
        return Falha.NaoEncontrado($"Despesa {id} não encontrada.");
    }

    private Dictionary<string, string[]> Validar(DespesaRequest request)
    {
        Dictionary<string, string[]> erros = [];

        if (string.IsNullOrWhiteSpace(request.Descricao))
        {
            erros[nameof(request.Descricao)] = ["A descrição é obrigatória."];
        }

        if (request.Valor <= 0)
        {
            erros[nameof(request.Valor)] = ["O valor deve ser maior que zero."];
        }

        if (request.Data == default)
        {
            erros[nameof(request.Data)] = ["A data é obrigatória."];
        }
        else if (request.Data.Date > relogio.GetLocalNow().Date)
        {
            erros[nameof(request.Data)] = ["A data não pode ser futura."];
        }

        if (!dados.Categorias.Exists(c => c.Id == request.CategoriaId))
        {
            erros[nameof(request.CategoriaId)] = [$"A categoria {request.CategoriaId} não existe."];
        }

        return erros;
    }
}
