using Gastos.Api.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gastos.Api.Data.Configuracoes;

public sealed class DespesaConfiguracao : IEntityTypeConfiguration<Despesa>
{
    private const string TabelaDeJuncaoDeTags = "DespesaTag";

    public void Configure(EntityTypeBuilder<Despesa> builder)
    {
        builder.ToTable("Despesas");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Descricao)
            .HasMaxLength(Despesa.TamanhoMaximoDescricao)
            .IsRequired();

        builder.Property(d => d.Valor).HasPrecision(18, 2);

        builder.HasOne(d => d.Categoria)
            .WithMany(c => c.Despesas)
            .HasForeignKey(d => d.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Tags)
            .WithMany(t => t.Despesas)
            .UsingEntity<Dictionary<string, object>>(
                TabelaDeJuncaoDeTags,
                juncao => juncao.HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey("TagId")
                    .OnDelete(DeleteBehavior.Cascade),
                juncao => juncao.HasOne<Despesa>()
                    .WithMany()
                    .HasForeignKey("DespesaId")
                    .OnDelete(DeleteBehavior.Cascade),
                juncao =>
                {
                    juncao.ToTable(TabelaDeJuncaoDeTags);
                    juncao.HasKey("DespesaId", "TagId");
                });
    }
}
