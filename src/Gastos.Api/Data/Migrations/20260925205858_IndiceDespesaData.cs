using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gastos.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class IndiceDespesaData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Despesas_Data",
                table: "Despesas",
                column: "Data")
                .Annotation("SqlServer:Include", new[] { "CategoriaId", "Valor" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Despesas_Data",
                table: "Despesas");
        }
    }
}
