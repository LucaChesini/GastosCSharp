using Gastos.Api.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gastos.Api.Data.Configuracoes;

public sealed class CategoriaConfiguracao : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .HasMaxLength(Categoria.TamanhoMaximoNome)
            .IsRequired();

        builder.Property(c => c.Cor)
            .HasMaxLength(Categoria.TamanhoCor)
            .IsRequired();

        builder.HasIndex(c => c.Nome).IsUnique();
    }
}
