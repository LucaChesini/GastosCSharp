CREATE OR ALTER PROCEDURE dbo.sp_relatorio_mensal
    @ano int,
    @mes int
AS
BEGIN
    SET NOCOUNT ON;

    IF @ano IS NULL OR @ano NOT BETWEEN 1 AND 9998
        THROW 50001, N'Parâmetro @ano deve estar entre 1 e 9998.', 1;

    IF @mes IS NULL OR @mes NOT BETWEEN 1 AND 12
        THROW 50002, N'Parâmetro @mes deve estar entre 1 e 12.', 1;

    DECLARE @inicio datetime2(7) = DATEFROMPARTS(@ano, @mes, 1);
    DECLARE @fim datetime2(7) = DATEADD(MONTH, 1, @inicio);

    SELECT
        @ano AS Ano,
        @mes AS Mes,
        c.Id AS CategoriaId,
        c.Nome AS Categoria,
        c.Cor,
        SUM(d.Valor) AS Total,
        COUNT(*) AS Quantidade,
        CAST(ROUND(100.0 * SUM(d.Valor) / SUM(SUM(d.Valor)) OVER (), 2) AS decimal(5, 2)) AS Percentual
    FROM dbo.Despesas AS d
    INNER JOIN dbo.Categorias AS c ON c.Id = d.CategoriaId
    WHERE d.Data >= @inicio
      AND d.Data < @fim
    GROUP BY c.Id, c.Nome, c.Cor
    ORDER BY Total DESC, c.Nome;
END;
