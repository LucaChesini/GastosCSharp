using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gastos.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelatorioMensal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ScriptSql.LerComoExec("vw_relatorio_mensal.sql"));
            migrationBuilder.Sql(ScriptSql.LerComoExec("sp_relatorio_mensal.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_relatorio_mensal;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_relatorio_mensal;");
        }
    }
}
