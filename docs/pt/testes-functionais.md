# Testes Funcionais - Developer Evaluation Project

Este documento descreve como executar e entender os testes funcionais (de integração) do projeto `Developer Evaluation`.

## Pré-requisitos
- Docker Desktop em execução
- Imagem `postgres:latest` disponível
- .NET 8 SDK instalado (`dotnet --version`)
- Projeto compilando com sucesso (`dotnet build`)

## Executando os testes
```bash
dotnet test tests/Ambev.DeveloperEvaluation.Functional
```

## Como funciona
- Cada teste inicia um **container PostgreSQL isolado** com `Testcontainers`
- `CustomWebApplicationFactory` aponta o `DbContext` para o container
- App executado com autenticação, Serilog, AutoMapper, etc.
- Testes usam `HttpClient` real via `TestServer`

## Estrutura
- `SalesEndpointTests.cs`: API de vendas
- `PostgreSqlContainerFactory.cs`: cria container
- `CustomWebApplicationFactory.cs`: injeta dependências reais

## Dicas
| Erro                 | Causa Provável      | Solução                 |
|----------------------|---------------------|-------------------------|
| Container não pronto | Docker não iniciado | Inicie o Docker         |
| pg_isready falha     | Porta 5433 ocupada  | Liberar ou trocar porta |

## Testes Cobertos
- `GET /api/sales`
- `GET /api/sales/{id}`
- `POST /api/sales`
- `PUT /api/sales/{id}`
- `DELETE /api/sales/{id}`

## Notas
- Sem dependência de banco local