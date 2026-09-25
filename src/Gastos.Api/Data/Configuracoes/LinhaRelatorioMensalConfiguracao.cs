using Gastos.Api.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gastos.Api.Data.Configuracoes;

public sealed class LinhaRelatorioMensalConfiguracao : IEntityTypeConfiguration<LinhaRelatorioMensal>
{
    public void Configure(EntityTypeBuilder<LinhaRelatorioMensal> builder)
    {
        builder.HasNoKey();
        builder.ToView("vw_relatorio_mensal");

        builder.Property(l => l.Total).HasPrecision(38, 2);
        builder.Property(l => l.Percentual).HasPrecision(5, 2);
    }
}
