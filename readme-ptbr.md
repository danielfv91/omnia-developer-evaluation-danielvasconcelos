# Projeto de Avaliação Técnica Ambev

## Visão Geral do Projeto
Este projeto é uma API para gerenciamento de vendas, desenvolvida em .NET 8 utilizando PostgreSQL, MediatR, AutoMapper e arquitetura DDD. Foi criada como parte de uma avaliação técnica para uma vaga de desenvolvedor sênior/especialista .NET.

## Requisitos Atendidos

A API implementa integralmente as funcionalidades requeridas:

- [x] CRUD completo de vendas (Sales)
- [x] Aplicação das regras de desconto conforme quantidade de itens
- [x] Todos os campos solicitados disponíveis nas operações (SaleNumber, Customer, Branch, Produtos, Quantities, etc.)
- [x] Validação de dados com mensagens padronizadas em inglês
- [x] Eventos de domínio implementados e logados no console (SaleCreated, SaleModified, SaleCancelled, ItemCancelled)

## Melhorias Técnicas

Este projeto aprimorou o template original implementando a validação automática de requisições utilizando a integração do FluentValidation com o pipeline de requisições. Com isso, os controllers ficam mais limpos e os erros de validação são tratados de forma centralizada e padronizada. Isso foi aplicada ao CRUD de Sales.

### Autenticação

A autenticação JWT está funcional no projeto.

- O endpoint `/auth` retorna um token após validação das credenciais do usuário
- Para proteger rotas (ex: `/sales`), basta adicionar o atributo `[Authorize]`

> Obs: Foi identificado um problema de configuração do AutoMapper no template original, que foi corrigido para garantir a geração correta do token JWT.

## Funcionalidades
- Criar, consultar, atualizar e excluir registros de vendas
- Regras de negócio aplicadas aos itens da venda:
  - 4–9 itens: 10% de desconto
  - 10–20 itens: 20% de desconto
  - Mais de 20 itens: não permitido
- Validação de entrada com FluentValidation (mensagens em inglês)
- Formato de resposta da API padronizado (ApiResponse)
- Testes unitários cobrindo todos os cenários das regras de negócio

## Tecnologias
- .NET 8.0
- PostgreSQL + EF Core
- MediatR (CQRS)
- AutoMapper
- xUnit + NSubstitute + Bogus (testes)
- FluentValidation
- Arquitetura: Domain-Driven Design (DDD)
- Serilog (Para Logs)

## Configuração do Projeto

Este guia irá ajudá-lo a configurar rapidamente o projeto no seu ambiente local.

---

### Pré-requisitos

Antes de começar, certifique-se de ter as seguintes ferramentas instaladas:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) ou [Visual Studio Code](https://code.visualstudio.com/)
- [PostgreSQL](https://www.postgresql.org/download/) (recomenda-se PgAdmin incluso na instalação)
- [Git](https://git-scm.com/downloads)

---

### Configuração Inicial

#### 1. Clone o Repositório

```bash
git clone https://github.com/danielfv91/omnia-developer-evaluation-danielvasconcelos.git
cd omnia-developer-evaluation-danielvasconcelos
```

#### 2. Configure o Banco de Dados PostgreSQL

- Abra o PgAdmin e crie um novo banco de dados com o nome `DeveloperEvaluation`.

#### 3. Atualize a String de Conexão

No arquivo `appsettings.json` localizado em `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`, atualize a string de conexão com suas credenciais do PostgreSQL:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=DeveloperEvaluation;User Id=postgres;Password=sua_senha;"
}
```

#### 4. Aplicar Migrations do EF Core

No **Package Manager Console** do Visual Studio, execute:

```powershell
Update-Database -Project Ambev.DeveloperEvaluation.ORM -StartupProject Ambev.DeveloperEvaluation.WebApi
```

Isso criará automaticamente as tabelas necessárias no seu banco de dados PostgreSQL.

---

### Executando a Aplicação

- No Visual Studio, defina o projeto `Ambev.DeveloperEvaluation.WebApi` como projeto de inicialização e pressione **F5**.

- A aplicação será iniciada via HTTPS (`https://localhost:<porta>`), abrindo automaticamente a interface Swagger.

### Problema com Certificado HTTPS

Se ocorrer o erro **"Sua conexão não é particular"** (`ERR_CERT_INVALID`), execute os seguintes comandos no prompt (como administrador):

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Reinicie o Visual Studio e execute novamente.

---

## Execução dos Testes

Para ver como rodar cada tipo de teste, consulte a documentação específica de cada um:

- [Estratégia de Testes](docs/pt/tests.md) → Testes Unitários e Estratégia de Testes  
- [Testes Funcionais](docs/pt/tests-functional.md) → Testes Funcionais  
- [Testes de Integração](docs/pt/tests-integration.md) → Testes de Integração  

---

## Documentação

Para ver as documentações do projeto, acesse:

- [Documentação](docs/index.md)

---

## Estrutura do Projeto
```
src/
├── Application       → Regras de negócio (Handlers MediatR, Commands)
├── Domain            → Entidades e Interfaces (DDD)
├── ORM               → Contexto EF Core e Repositórios
├── WebApi            → Controllers, Requests, Responses
├── Common/Crosscut   → Exceptions, Helpers, Validações
tests/
├── Unit              → Testes unitários (xUnit, NSubstitute, Bogus)
├── Functional        → Testes funcionais com chamadas HTTP
└── Integration       → Testes de ponta a ponta com Testcontainers
```


## Padrão de Commits
Commits semânticos em português:
- `feat:` nova funcionalidade
- `fix:` correção de erro
- `test:` testes
- `chore:` ajustes menores

## Link do Repositório
[github.com/danielfv91/omnia-developer-evaluation-danielvasconcelos](https://github.com/danielfv91/omnia-developer-evaluation-danielvasconcelos)

---

**Tudo pronto para começar!**