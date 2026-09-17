/*
===============================================================================
FIAP - C# SOFTWARE DEVELOPMENT - CHECKPOINT 4
SQL SERVER - FERNANDO RM 558095
===============================================================================

Execute este script no SQL Server ou LocalDB antes de iniciar a API usando
o provider SqlServer. Ele pode ser executado novamente durante os testes.
===============================================================================
*/

USE master;
GO

IF DB_ID(N'CP4_Fernando_RM_558095') IS NULL
BEGIN
    CREATE DATABASE [CP4_Fernando_RM_558095];
END
GO

USE [CP4_Fernando_RM_558095];
GO

SET NOCOUNT ON;
GO

DROP TABLE IF EXISTS dbo.PRODUTOS;
DROP TABLE IF EXISTS dbo.CATEGORIAS;
GO

CREATE TABLE dbo.CATEGORIAS
(
    ID          INT IDENTITY(1,1) NOT NULL,
    NOME        NVARCHAR(100) NOT NULL,
    DESCRICAO   NVARCHAR(500) NULL,

    CONSTRAINT PK_CATEGORIAS PRIMARY KEY (ID),
    CONSTRAINT UQ_CATEGORIAS_NOME UNIQUE (NOME)
);
GO

CREATE TABLE dbo.PRODUTOS
(
    ID                  INT IDENTITY(1,1) NOT NULL,
    NOME                NVARCHAR(150) NOT NULL,
    DESCRICAO           NVARCHAR(500) NULL,
    PRECO               DECIMAL(12,2) NOT NULL,
    QUANTIDADEESTOQUE   INT NOT NULL,
    CATEGORIAID         INT NOT NULL,

    CONSTRAINT PK_PRODUTOS PRIMARY KEY (ID),
    CONSTRAINT FK_PRODUTOS_CATEGORIAS
        FOREIGN KEY (CATEGORIAID) REFERENCES dbo.CATEGORIAS(ID),
    CONSTRAINT CK_PRODUTOS_PRECO CHECK (PRECO >= 0),
    CONSTRAINT CK_PRODUTOS_QUANTIDADEESTOQUE CHECK (QUANTIDADEESTOQUE >= 0)
);
GO

CREATE INDEX IX_PRODUTOS_CATEGORIAID ON dbo.PRODUTOS(CATEGORIAID);
GO

INSERT INTO dbo.CATEGORIAS (NOME, DESCRICAO)
VALUES
    (N'Informática', N'Equipamentos e acessórios de informática'),
    (N'Escritório', N'Produtos para uso em escritório'),
    (N'Áudio', N'Equipamentos de áudio');
GO

INSERT INTO dbo.PRODUTOS (NOME, DESCRICAO, PRECO, QUANTIDADEESTOQUE, CATEGORIAID)
VALUES
    (N'Notebook', N'Notebook para desenvolvimento', 5499.90, 10, 1),
    (N'Mouse', N'Mouse óptico USB', 89.90, 25, 1),
    (N'Cadeira de Escritório', N'Cadeira ergonômica', 899.00, 8, 2),
    (N'Headset', N'Headset com microfone', 299.90, 15, 3);
GO

SELECT DB_NAME() AS BancoEmUso;
GO

SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_SCHEMA, TABLE_NAME;
GO

SELECT
    p.ID,
    p.NOME,
    p.DESCRICAO,
    p.PRECO,
    p.QUANTIDADEESTOQUE,
    c.ID AS CATEGORIAID,
    c.NOME AS CATEGORIA
FROM dbo.PRODUTOS AS p
INNER JOIN dbo.CATEGORIAS AS c ON c.ID = p.CATEGORIAID
ORDER BY p.ID;
GO
