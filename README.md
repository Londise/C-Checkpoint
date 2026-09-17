# CP4.Catalogo

## Checkpoint 4 — C# Software Development

Projeto desenvolvido para o Checkpoint 4 da disciplina de **C# Software Development — FIAP — 2º semestre de 2026**.

A aplicação consiste em um catálogo de produtos desenvolvido com **ASP.NET Core, C#, Entity Framework Core, SQL Server e Oracle**, utilizando uma API REST como camada de acesso aos dados e uma aplicação MVC como interface web.

O projeto permite alternar entre os bancos **SQL Server** e **Oracle** através do provider informado no header `X-Database-Provider`.

---

# 1. Integrantes

| Nome                    |     RM |
| ----------------------- | -----: |
| Gabriel Guilherme Leste | 558638 |
| Fernando Carlos         | 558095 |
| Gabriel Lacerda         | 558307 |
| Julia Carolina          | 558896 |

**Turma:** FIAP — C# Software Development — 2º semestre de 2026

---

# 2. Sobre o projeto

O sistema foi desenvolvido seguindo uma arquitetura separada em três projetos principais:

- **CP4.Catalogo.Data** — entidades, `DbContext` e infraestrutura de acesso aos bancos.
- **CP4.Catalogo.Api** — API REST responsável pelas operações de categorias e produtos.
- **CP4.Catalogo.Web** — aplicação MVC responsável pela interface do catálogo e comunicação com a API.

A aplicação Web **não acessa diretamente o banco de dados**. Ela realiza as requisições através da API utilizando `HttpClient`.

Fluxo da aplicação:

```text
┌──────────────────────┐
│   CP4.Catalogo.Web   │
│       MVC            │
└──────────┬───────────┘
           │ HTTP
           │ X-Database-Provider
           ▼
┌──────────────────────┐
│   CP4.Catalogo.Api   │
│      REST API        │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ CP4.Catalogo.Data    │
│ Entity Framework     │
└──────────┬───────────┘
           │
      ┌────┴─────┐
      ▼          ▼
┌──────────┐ ┌──────────┐
│SQL Server│ │  Oracle  │
└──────────┘ └──────────┘
```

---

# 3. Estrutura da solução

```text
CP4.Catalogo
│
├── CP4.Catalogo.Api
│   ├── Controllers
│   │   ├── CatalogoControllerBase.cs
│   │   ├── CategoriasController.cs
│   │   └── ProdutosController.cs
│   │
│   ├── DTOs
│   │   ├── CategoriaCreateDto.cs
│   │   ├── CategoriaResponseDto.cs
│   │   ├── ProdutoCreateDto.cs
│   │   ├── ProdutoResponseDto.cs
│   │   └── ProdutoUpdateDto.cs
│   │
│   ├── Properties
│   │   └── launchSettings.json
│   │
│   ├── appsettings.json
│   ├── Program.cs
│   └── CP4.Catalogo.Api.csproj
│
├── CP4.Catalogo.Data
│   ├── Infrastructure
│   │   ├── CatalogoDbContextFactory.cs
│   │   ├── DatabaseProvider.cs
│   │   └── DatabaseProviderResolver.cs
│   │
│   ├── Models
│   │   ├── Categoria.cs
│   │   └── Produto.cs
│   │
│   ├── CatalogoDbContext.cs
│   └── CP4.Catalogo.Data.csproj
│
├── CP4.Catalogo.Web
│   ├── Controllers
│   │   └── ProdutosController.cs
│   │
│   ├── Models
│   │   ├── CategoriaViewModel.cs
│   │   ├── ProdutoCreateViewModel.cs
│   │   └── ProdutoViewModel.cs
│   │
│   ├── Services
│   │   └── ApiService.cs
│   │
│   ├── Views
│   │   ├── Produtos
│   │   │   ├── Create.cshtml
│   │   │   ├── Details.cshtml
│   │   │   └── Index.cshtml
│   │   └── Shared
│   │       └── _Layout.cshtml
│   │
│   ├── wwwroot
│   ├── Program.cs
│   └── CP4.Catalogo.Web.csproj
│
├── scripts
│   ├── CP4_Fernando_RM_558095_SQLServer.sql
│   ├── CP4_Fernando_RM_558095_Oracle_XE_Local_Admin.sql
│   └── CP4_Fernando_RM_558095_Oracle_Institucional.sql
│
├── CP4.Catalogo.sln
├── .gitignore
└── README.md
```

---

# 4. Tecnologias utilizadas

- C#
- .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server / LocalDB
- Oracle Database / Oracle XE
- `Oracle.EntityFrameworkCore`
- Swagger / OpenAPI
- `HttpClient`
- Razor Views
- Bootstrap
- Programação assíncrona com `async`/`await`

---

# 5. Camada de dados — CP4.Catalogo.Data

O projeto `CP4.Catalogo.Data` concentra a estrutura de persistência da aplicação.

As entidades principais são:

### Categoria

```text
Categoria
├── Id
├── Nome
├── Descricao
└── Produtos
```

### Produto

```text
Produto
├── Id
├── Nome
├── Descricao
├── Preco
├── QuantidadeEstoque
└── CategoriaId
```

Existe um relacionamento de **1** entre categorias e produtos:

```text
CATEGORIAS
    │
    │ 1
    │
    │ N
    ▼
PRODUTOS
```

Cada produto pertence obrigatoriamente a uma categoria.

O `CatalogoDbContext` mapeia as entidades para as tabelas `CATEGORIAS` e `PRODUTOS`, incluindo chave primária, chave estrangeira, índices, restrições e precisão do campo de preço.

---

# 6. Suporte a SQL Server e Oracle

Uma das funcionalidades principais do projeto é permitir que a mesma API utilize dois bancos diferentes.

O projeto possui:

```csharp
public enum DatabaseProvider
{
    SqlServer,
    Oracle
}
```

A escolha do banco é resolvida pelo `DatabaseProviderResolver`.

O `CatalogoDbContextFactory` cria o contexto utilizando o provider correspondente:

```text
SqlServer → UseSqlServer()
Oracle    → UseOracle()
```

O pacote do Oracle está presente no projeto `CP4.Catalogo.Data`:

```xml
<PackageReference Include="Oracle.EntityFrameworkCore"
                  Version="8.23.60" />
```

---

# 7. Seleção do banco através do Header

A API utiliza o header:

```http
X-Database-Provider
```

Os valores aceitos são:

```text
SqlServer
Oracle
```

Exemplo:

```http
X-Database-Provider: SqlServer
```

ou:

```http
X-Database-Provider: Oracle
```

Quando nenhum provider é informado, a aplicação utiliza o provider definido em:

```json
"Database": {
    "DefaultProvider": "SqlServer"
}
```

Atualmente, o provider padrão configurado é:

```text
SqlServer
```

A API também retorna no response o provider utilizado através do mesmo header.

---

# 8. Banco de dados SQL Server

O projeto possui um script específico para preparar o SQL Server:

```text
scripts/CP4_Fernando_RM_558095_SQLServer.sql
```

O script:

- cria o banco `CP4_Fernando_RM_558095`, caso ele não exista;
- cria a tabela `CATEGORIAS`;
- cria a tabela `PRODUTOS`;
- cria a chave estrangeira entre produtos e categorias;
- cria índice para `CATEGORIAID`;
- cria restrições para preço e estoque;
- insere categorias de exemplo;
- insere produtos de exemplo.

O script pode ser executado no **SQL Server ou LocalDB**. fileciteturn5file4L240-L280

### Como preparar

1. Abra o SQL Server Management Studio, Azure Data Studio ou ferramenta equivalente.
2. Conecte ao SQL Server/LocalDB.
3. Execute:

```text
scripts/CP4_Fernando_RM_558095_SQLServer.sql
```

4. Confirme a existência do banco:

```text
CP4_Fernando_RM_558095
```

5. Confirme as tabelas:

```text
CATEGORIAS
PRODUTOS
```

### Evidência — SQL Server

> **INSERIR PRINT AQUI**
>
> Evidência mostrando o banco `CP4_Fernando_RM_558095`, as tabelas `CATEGORIAS` e `PRODUTOS` e os dados cadastrados.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

# 9. Banco de dados Oracle

Foram disponibilizados dois scripts para Oracle, considerando diferentes ambientes.

## 9.1 Oracle XE local

Arquivo:

```text
scripts/CP4_Fernando_RM_558095_Oracle_XE_Local_Admin.sql
```

Esse script deve ser executado em uma conexão administrativa no PDB:

```text
XEPDB1
```

Ele cria o usuário/schema:

```text
CP4_FERNANDO_RM_558095
```

e concede as permissões necessárias para a execução do projeto.

O script contém uma senha de exemplo que deve ser alterada antes da utilização. fileciteturn5file7L465-L501

### Atenção

Esse script é destinado ao **Oracle XE local**.

Não executar `CREATE USER` no Oracle institucional caso o usuário/schema já tenha sido fornecido pela instituição.

---

## 9.2 Oracle institucional

Arquivo:

```text
scripts/CP4_Fernando_RM_558095_Oracle_Institucional.sql
```

Esse script foi preparado para o cenário em que o aluno já possui um usuário/schema Oracle fornecido.

Nesse caso:

- conectar utilizando o usuário autorizado;
- não executar `CREATE USER`;
- criar as tabelas no schema conectado.

O script cria:

```text
CATEGORIAS
PRODUTOS
```

e insere os dados de exemplo. fileciteturn5file8L514-L583

### Evidência — Oracle

> **INSERIR PRINT AQUI**
>
> Evidência mostrando o schema Oracle, as tabelas `CATEGORIAS` e `PRODUTOS` e os dados cadastrados.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

# 10. Connection Strings

As connection strings da API ficam em:

```text
CP4.Catalogo.Api/appsettings.json
```

Configuração atual:

```json
"Database": {
    "DefaultProvider": "SqlServer"
},
"ConnectionStrings": {
    "SqlServer": "...",
    "Oracle": "..."
}
```

A conexão do SQL Server utiliza LocalDB:

```text
Server=(localdb)\MSSQLLocalDB
```

A conexão Oracle está configurada para:

```text
localhost:1521/XEPDB1
```

### Segurança

A senha real do Oracle **não deve ser publicada no GitHub, README ou evidências**.

Antes de executar o provider Oracle, configure a senha correta no ambiente local.

Para entrega pública, recomenda-se utilizar User Secrets, variável de ambiente ou outra forma de configuração segura.

---

# 11. API REST

A API está no projeto:

```text
CP4.Catalogo.Api
```

Ela utiliza:

- Controllers;
- DTOs;
- Entity Framework Core;
- validação com Data Annotations;
- `async`/`await`;
- Swagger;
- tratamento de erros;
- seleção dinâmica do banco.

---

# 12. Endpoints

## Categorias

### Listar categorias

```http
GET /api/categorias
```

Header:

```http
X-Database-Provider: SqlServer
```

ou:

```http
X-Database-Provider: Oracle
```

### Criar categoria

```http
POST /api/categorias
```

Exemplo:

```json
{
  "nome": "Informática",
  "descricao": "Equipamentos de informática"
}
```

---

## Produtos

### Listar produtos

```http
GET /api/produtos
```

### Buscar produto por ID

```http
GET /api/produtos/{id}
```

Exemplo:

```http
GET /api/produtos/1
```

### Criar produto

```http
POST /api/produtos
```

Exemplo:

```json
{
  "nome": "Notebook",
  "descricao": "Notebook para desenvolvimento",
  "preco": 5499.90,
  "quantidadeEstoque": 10,
  "categoriaId": 1
}
```

### Atualizar produto

```http
PUT /api/produtos/{id}
```

### Excluir produto

```http
DELETE /api/produtos/{id}
```

Todos os endpoints de produtos utilizam o header:

```http
X-Database-Provider
```

---

# 13. Validações

A API utiliza `DataAnnotations` nos DTOs.

Entre as validações implementadas:

- nome obrigatório;
- limite de caracteres para nome;
- limite de caracteres para descrição;
- preço maior ou igual a zero;
- estoque maior ou igual a zero;
- categoria válida;
- verificação da existência da categoria antes de criar ou atualizar um produto;
- prevenção de categorias duplicadas.

---

# 14. Swagger

O Swagger está habilitado na API e pode ser utilizado para testar os endpoints.

Com a configuração atual, a API HTTPS utiliza:

```text
https://localhost:7030
```

O Swagger fica disponível em:

```text
https://localhost:7030/swagger
```

No Swagger, deve ser informado o header:

```http
X-Database-Provider
```

com um dos valores:

```text
SqlServer
Oracle
```

### Evidência — Swagger

> **INSERIR PRINT AQUI**
>
> Evidência mostrando o Swagger aberto e os endpoints disponíveis.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

# 15. Teste da API com SQL Server

Com a API executando, utilize:

```http
GET /api/produtos
```

Header:

```http
X-Database-Provider: SqlServer
```

Resultado esperado:

```text
HTTP 200 OK
```

com a lista de produtos cadastrados no SQL Server.

### Evidência — GET Produtos / SQL Server

> **INSERIR PRINT AQUI**
>
> Evidência do Swagger executando `GET /api/produtos` com:
>
> `X-Database-Provider: SqlServer`

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

# 16. Teste da API com Oracle

Com a API executando e o banco Oracle preparado, utilize:

```http
GET /api/produtos
```

Header:

```http
X-Database-Provider: Oracle
```

Resultado esperado:

```text
HTTP 200 OK
```

com os produtos cadastrados no Oracle.

### Evidência — GET Produtos / Oracle

> **INSERIR PRINT AQUI**
>
> Evidência do Swagger executando `GET /api/produtos` com:
>
> `X-Database-Provider: Oracle`

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

# 17. Aplicação Web MVC

O projeto:

```text
CP4.Catalogo.Web
```

é responsável pela interface do sistema.

A aplicação utiliza:

```text
ASP.NET Core MVC
Razor Views
HttpClient
async/await
Bootstrap
```

A comunicação com a API é realizada pelo:

```text
Services/ApiService.cs
```

A Web envia o provider selecionado para a API através do header:

```http
X-Database-Provider
```

---

# 18. Funcionalidades da interface

A tela principal apresenta o catálogo de produtos.

É possível:

- visualizar produtos;
- visualizar detalhes de um produto;
- cadastrar produtos;
- selecionar o banco utilizado;
- alternar entre SQL Server e Oracle.

A própria interface mostra o provider atualmente selecionado.

Exemplo:

```text
Banco ativo: SQL Server

Provider enviado à API: SqlServer
```

ou:

```text
Banco ativo: Oracle

Provider enviado à API: Oracle
```

A implementação da tela possui um seletor entre SQL Server e Oracle e envia o provider escolhido para a API. fileciteturn4file4L223-L268

---

# 19. Cadastro de produto pela Web

Na tela:

```text
Produtos → Novo produto
```

é possível informar:

- nome;
- descrição;
- preço;
- estoque;
- categoria.

As categorias são carregadas pela API.

Após o cadastro, a aplicação retorna para o catálogo.

---

# 20. Evidência — Aplicação MVC

> **INSERIR PRINT AQUI**
>
> Evidência mostrando a tela do catálogo funcionando e o provider selecionado.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

### Evidência — MVC com SQL Server

> **INSERIR PRINT AQUI**

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

### Evidência — MVC com Oracle

> **INSERIR PRINT AQUI**

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

# 21. Como executar o projeto

## Pré-requisitos

Instalar:

- .NET 8 SDK;
- Visual Studio 2022 ou IDE compatível;
- SQL Server/LocalDB;
- Oracle XE local ou acesso ao Oracle institucional;
- ferramenta para execução dos scripts SQL.

---

## Passo 1 — Preparar o banco

Escolha o banco que será utilizado.

### SQL Server

Execute:

```text
scripts/CP4_Fernando_RM_558095_SQLServer.sql
```

### Oracle XE local

Execute:

```text
scripts/CP4_Fernando_RM_558095_Oracle_XE_Local_Admin.sql
```

e ajuste a senha utilizada pela aplicação.

### Oracle institucional

Execute:

```text
scripts/CP4_Fernando_RM_558095_Oracle_Institucional.sql
```

utilizando o usuário/schema Oracle autorizado.

---

# 22. Executar a API

Na raiz do projeto:

```bash
dotnet run --project CP4.Catalogo.Api --launch-profile https
```

API:

```text
https://localhost:7030
```

Swagger:

```text
https://localhost:7030/swagger
```

Os perfis atuais do projeto confirmam essas portas e o lançamento do Swagger. fileciteturn4file6L408-L450

---

# 23. Executar a aplicação Web

Em outro terminal:

```bash
dotnet run --project CP4.Catalogo.Web --launch-profile https
```

Aplicação Web:

```text
https://localhost:7113
```

A Web está configurada para utilizar a API em:

```text
https://localhost:7030/
```

fileciteturn4file0L16-L20

---

# 24. Executar pelo Visual Studio

Também é possível abrir:

```text
CP4.Catalogo.sln
```

no Visual Studio.

Configure os projetos:

```text
CP4.Catalogo.Api
CP4.Catalogo.Web
```

para serem executados simultaneamente.

A API ficará disponível em:

```text
https://localhost:7030
```

e a aplicação Web em:

```text
https://localhost:7113
```

---

# 25. Teste completo sugerido

Para demonstrar o funcionamento completo do projeto:

### 1. SQL Server

Execute o script SQL Server.

### 2. API

Execute a API.

### 3. Swagger

Abra:

```text
https://localhost:7030/swagger
```

### 4. Teste SQL Server

Execute:

```http
GET /api/produtos
```

com:

```http
X-Database-Provider: SqlServer
```

### 5. Oracle

Prepare o banco Oracle.

### 6. Teste Oracle

Execute novamente:

```http
GET /api/produtos
```

com:

```http
X-Database-Provider: Oracle
```

### 7. MVC

Execute a aplicação Web e alterne o provider através do seletor disponível na tela.

---

# 26. Evidências da entrega

## 26.1 Estrutura do projeto

> **INSERIR PRINT AQUI**
>
> Mostrar a Solution Explorer com os três projetos:
>
> - `CP4.Catalogo.Api`
> - `CP4.Catalogo.Data`
> - `CP4.Catalogo.Web`

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

## 26.2 Banco SQL Server

> **INSERIR PRINT AQUI**
>
> Mostrar banco, tabelas e dados.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

## 26.3 Banco Oracle

> **INSERIR PRINT AQUI**
>
> Mostrar schema, tabelas e dados.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

## 26.4 Swagger

> **INSERIR PRINT AQUI**
>
> Mostrar Swagger e endpoints.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

## 26.5 GET Produtos — SQL Server

> **INSERIR PRINT AQUI**
>
> Mostrar `GET /api/produtos` com `X-Database-Provider: SqlServer`.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

## 26.6 GET Produtos — Oracle

> **INSERIR PRINT AQUI**
>
> Mostrar `GET /api/produtos` com `X-Database-Provider: Oracle`.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

## 26.7 Aplicação MVC

> **INSERIR PRINT AQUI**
>
> Mostrar catálogo funcionando.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

## 26.8 Alternância de provider

> **INSERIR PRINT AQUI**
>
> Mostrar o seletor da aplicação alternando entre SQL Server e Oracle e o provider enviado à API.

```text
[ ESPAÇO PARA EVIDÊNCIA ]
```

---

# 27. Checklist de entrega

Antes de enviar o projeto, verificar:

- [ ] Os quatro integrantes estão identificados com nome e RM.
- [ ] `CP4.Catalogo.sln` está presente.
- [ ] `CP4.Catalogo.Api` está presente.
- [ ] `CP4.Catalogo.Data` está presente.
- [ ] `CP4.Catalogo.Web` está presente.
- [ ] Pasta `scripts` está presente.
- [ ] Script SQL Server está presente.
- [ ] Script Oracle XE local está presente.
- [ ] Script Oracle institucional está presente.
- [ ] `Oracle.EntityFrameworkCore` está presente no projeto Data.
- [ ] A solução compila.
- [ ] SQL Server foi testado.
- [ ] Oracle foi testado.
- [ ] Swagger foi testado.
- [ ] `GET /api/produtos` funciona com SQL Server.
- [ ] `GET /api/produtos` funciona com Oracle.
- [ ] Aplicação MVC funciona.
- [ ] Alternância de provider funciona.
- [ ] As evidências foram adicionadas neste README.
- [ ] Senhas reais não foram incluídas na entrega.
- [ ] Pastas `bin` e `obj` não estão presentes no ZIP.

---

# 28. Observações técnicas

O projeto utiliza uma única estrutura de entidades e `DbContext`, permitindo que a API escolha em tempo de execução entre:

```text
SQL Server
```

e:

```text
Oracle
```

A escolha é centralizada através de:

```text
DatabaseProvider
DatabaseProviderResolver
CatalogoDbContextFactory
```

A aplicação Web mantém a separação de responsabilidades ao consumir somente a API, enquanto a API concentra o acesso aos bancos.

---

# 29. Resumo da arquitetura

```text
                         USUÁRIO
                            │
                            ▼
                 ┌────────────────────┐
                 │   CP4.Catalogo.Web │
                 │       MVC          │
                 └─────────┬──────────┘
                           │
                     HttpClient
                           │
              X-Database-Provider
                           │
                           ▼
                 ┌────────────────────┐
                 │   CP4.Catalogo.Api │
                 │     REST API       │
                 └─────────┬──────────┘
                           │
                           ▼
                 ┌────────────────────┐
                 │ CP4.Catalogo.Data  │
                 │   EF Core / DbCtx  │
                 └─────────┬──────────┘
                           │
                  ┌────────┴────────┐
                  │                 │
                  ▼                 ▼
             SQL Server          Oracle
```

---

# 30. Considerações finais

O projeto apresenta uma API REST integrada a uma aplicação MVC, utilizando Entity Framework Core para persistência e permitindo a utilização de SQL Server ou Oracle através da seleção de provider.

Os scripts disponibilizados permitem preparar os ambientes de banco de dados e os endpoints podem ser testados através do Swagger.

As evidências solicitadas devem ser adicionadas nas seções correspondentes deste documento antes da entrega final.
