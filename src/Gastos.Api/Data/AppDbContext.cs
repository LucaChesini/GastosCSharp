using Gastos.Api.Domain;

using Microsoft.EntityFrameworkCore;

namespace Gastos.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Despesa> Despesas => Set<Despesa>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Comprovante> Comprovantes => Set<Comprovante>();
    public DbSet<LinhaRelatorioMensal> RelatorioMensal => Set<LinhaRelatorioMensal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
