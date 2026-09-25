-- Massa de dados para comparar planos de execução do relatório mensal.
-- Uso manual, só em banco descartável (ex.: GastosPlano). Não é aplicado por migration.
-- sqlcmd ... -d GastosPlano -i sql/massa_plano_execucao.sql

SET NOCOUNT ON;

IF DB_NAME() = N'Gastos'
    THROW 50000, N'Script de massa: não rode no banco de desenvolvimento.', 1;

DECLARE @despesas int = 200000;
DECLARE @inicio datetime2(7) = '2021-01-01';
DECLARE @minutosNoPeriodo int = DATEDIFF(MINUTE, @inicio, '2026-01-01');

INSERT INTO dbo.Categorias (Nome, Cor)
SELECT novas.Nome, novas.Cor
FROM (VALUES
    (N'Educação', N'#3949AB'),
    (N'Vestuário', N'#D81B60'),
    (N'Assinaturas', N'#00897B'),
    (N'Pets', N'#6D4C41'),
    (N'Viagem', N'#FDD835')
) AS novas (Nome, Cor)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Categorias AS c WHERE c.Nome = novas.Nome);

DROP TABLE IF EXISTS #categorias;
SELECT ROW_NUMBER() OVER (ORDER BY Id) AS Posicao, Id
INTO #categorias
FROM dbo.Categorias;

DECLARE @quantidadeCategorias int = (SELECT COUNT(*) FROM #categorias);

DROP TABLE IF EXISTS #massa;
SELECT TOP (@despesas)
    ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Numero,
    1 + ABS(CHECKSUM(NEWID())) % @quantidadeCategorias AS PosicaoCategoria,
    ABS(CHECKSUM(NEWID())) % @minutosNoPeriodo AS Minutos,
    CAST((1 + ABS(CHECKSUM(NEWID())) % 50000) / 100.0 AS decimal(18, 2)) AS Valor
INTO #massa
FROM sys.all_columns AS a
CROSS JOIN sys.all_columns AS b;

INSERT INTO dbo.Despesas (Descricao, Valor, Data, CategoriaId)
SELECT
    CONCAT(N'Lançamento de teste número ', m.Numero),
    m.Valor,
    DATEADD(MINUTE, m.Minutos, @inicio),
    c.Id
FROM #massa AS m
INNER JOIN #categorias AS c ON c.Posicao = m.PosicaoCategoria;

SELECT COUNT(*) AS Despesas, MIN(Data) AS Primeira, MAX(Data) AS Ultima FROM dbo.Despesas;
