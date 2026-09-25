CREATE OR ALTER VIEW dbo.vw_relatorio_mensal
AS
SELECT
    YEAR(d.Data) AS Ano,
    MONTH(d.Data) AS Mes,
    c.Id AS CategoriaId,
    c.Nome AS Categoria,
    c.Cor,
    SUM(d.Valor) AS Total,
    COUNT(*) AS Quantidade,
    CAST(ROUND(100.0 * SUM(d.Valor) / SUM(SUM(d.Valor)) OVER (PARTITION BY YEAR(d.Data), MONTH(d.Data)), 2) AS decimal(5, 2)) AS Percentual
FROM dbo.Despesas AS d
INNER JOIN dbo.Categorias AS c ON c.Id = d.CategoriaId
GROUP BY YEAR(d.Data), MONTH(d.Data), c.Id, c.Nome, c.Cor;
