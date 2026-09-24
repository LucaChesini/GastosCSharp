using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gastos.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategoriasETags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: ["Nome", "Cor"],
                values: new object[,]
                {
                    { "Alimentação", "#E53935" },
                    { "Transporte", "#1E88E5" },
                    { "Moradia", "#8E24AA" },
                    { "Saúde", "#43A047" },
                    { "Lazer", "#FB8C00" },
                });

            migrationBuilder.InsertData(
                table: "Tags",
                column: "Nome",
                values: ["mercado", "recorrente", "cartao", "pix"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Nome",
                keyValues: ["mercado", "recorrente", "cartao", "pix"]);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Nome",
                keyValues: ["Alimentação", "Transporte", "Moradia", "Saúde", "Lazer"]);
        }
    }
}
