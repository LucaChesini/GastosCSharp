using Gastos.Api.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gastos.Api.Data.Configuracoes;

public sealed class TagConfiguracao : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nome)
            .HasMaxLength(Tag.TamanhoMaximoNome)
            .IsRequired();

        builder.HasIndex(t => t.Nome).IsUnique();
    }
}
