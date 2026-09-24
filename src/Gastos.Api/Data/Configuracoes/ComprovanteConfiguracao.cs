using Gastos.Api.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gastos.Api.Data.Configuracoes;

public sealed class ComprovanteConfiguracao : IEntityTypeConfiguration<Comprovante>
{
    public void Configure(EntityTypeBuilder<Comprovante> builder)
    {
        builder.ToTable("Comprovantes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Url)
            .HasMaxLength(Comprovante.TamanhoMaximoUrl)
            .IsRequired();

        builder.Property(c => c.Observacao)
            .HasMaxLength(Comprovante.TamanhoMaximoObservacao);

        builder.HasOne(c => c.Despesa)
            .WithOne(d => d.Comprovante)
            .HasForeignKey<Comprovante>(c => c.DespesaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.DespesaId).IsUnique();
    }
}
