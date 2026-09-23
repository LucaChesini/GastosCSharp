using Gastos.Api.Domain;

namespace Gastos.Api.Data;

public sealed class DadosEmMemoria
{
    private int _ultimoIdCategoria;
    private int _ultimoIdDespesa;

    public Lock Trava { get; } = new();
    public List<Categoria> Categorias { get; } = [];
    public List<Despesa> Despesas { get; } = [];

    public int ProximoIdCategoria()
    {
        return ++_ultimoIdCategoria;
    }

    public int ProximoIdDespesa()
    {
        return ++_ultimoIdDespesa;
    }
}
