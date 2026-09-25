using System.Data;

using Gastos.Api.Data;
using Gastos.Api.Domain;
using Gastos.Api.Dtos;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gastos.Api.Services;

public sealed class RelatorioService(AppDbContext db)
{
    public const int AnoMinimo = 1;
    public const int AnoMaximo = 9998;

    public async Task<Resultado<IReadOnlyList<RelatorioMensalResponse>>> ObterMensalAsync(int? ano, int? mes)
    {
        Dictionary<string, string[]> erros = Validar(ano, mes);
        if (erros.Count > 0 || ano is not int anoInformado || mes is not int mesInformado)
        {
            return Falha.Validacao(erros);
        }

        List<LinhaRelatorioMensal> linhas = await db.RelatorioMensal
            .FromSqlRaw(
                "EXEC dbo.sp_relatorio_mensal @ano, @mes",
                new SqlParameter("@ano", SqlDbType.Int) { Value = anoInformado },
                new SqlParameter("@mes", SqlDbType.Int) { Value = mesInformado })
            .ToListAsync();

        return linhas
            .Select(l => new RelatorioMensalResponse(l.CategoriaId, l.Categoria, l.Cor, l.Total, l.Quantidade, l.Percentual))
            .ToList();
    }

    private static Dictionary<string, string[]> Validar(int? ano, int? mes)
    {
        Dictionary<string, string[]> erros = [];

        if (ano is null)
        {
            erros["Ano"] = ["O ano é obrigatório."];
        }
        else if (ano is < AnoMinimo or > AnoMaximo)
        {
            erros["Ano"] = [$"O ano deve estar entre {AnoMinimo} e {AnoMaximo}."];
        }

        if (mes is null)
        {
            erros["Mes"] = ["O mês é obrigatório."];
        }
        else if (mes is < 1 or > 12)
        {
            erros["Mes"] = ["O mês deve estar entre 1 e 12."];
        }

        return erros;
    }
}
