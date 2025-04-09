# Testes de Integração

Este documento descreve a cobertura, arquitetura e abordagens utilizadas nos testes de integração do projeto.

---

## Objetivo dos testes de integração

Garantir que os componentes principais do domínio `Sales` interajam corretamente com:

- O banco de dados real (PostgreSQL via Testcontainers)
- O pipeline de validação (FluentValidation + MediatR)
- O comportamento de persistência e leitura

---

## Tecnologias utilizadas

- **xUnit**: estrutura de testes
- **FluentAssertions**: para validações legíveis
- **FluentValidation**: para validação de regras
- **MediatR**: para orquestração de comandos e queries
- **AutoMapper**: para mapeamentos entre entidades e DTOs
- **Testcontainers para .NET**: para isolamento com PostgreSQL real
- **Bogus**: geração de dados realistas
- **NSubstitute**: mocks de serviços internos (quando aplicável)

---

## Escopo de cobertura

### Handlers testados

| Handler                | Cobertura                                                            |
|------------------------|----------------------------------------------------------------------|
| `CreateSaleHandler`    | Persistência, eventos, regras de desconto                            |
| `UpdateSaleHandler`    | Atualização, desconto condicional, eventos                           |
| `GetSaleHandler`       | Busca por ID com validação de entrada                                |
| `GetSalesHandler`      | Paginação, ordenação, filtros, validações                            |
| `DeleteSaleHandler`    | Cancelamento lógico + publicação de evento                           |
| `SaleLifecycleHandler` | Criação, leitura, atualização e exclusão sequencial (fluxo completo) |

---

## Validações testadas

As regras de validação do domínio são testadas **através do pipeline**, garantindo que o comportamento seja o mesmo da aplicação real.

Exemplos:
- Campos obrigatórios (ex: `CustomerName`, `SaleNumber`)
- Limites de quantidade (`Quantity` entre 1 e 20)
- Descontos por quantidade aplicados corretamente
- Rejeição de filtros inválidos no `GetSales`

---

## Execução dos testes

- Ao iniciar os testes, um container PostgreSQL é iniciado automaticamente com a porta `5533`
- O banco é migrado com `EF Core Migrations`
- Os dados são inseridos e consultados usando repositórios reais
- Após os testes, o container é encerrado e removido

---

## Organização dos testes

```bash
tests/
└── Ambev.DeveloperEvaluation.Integration/
    ├── Common/
    │   └── IntegrationTestFixture.cs
    ├── Builders/
    │   ├── SaleCommandFakerBuilder.cs
    │   └── SaleUpdateFakerBuilder.cs
    └── Handlers/
        ├── CreateSaleHandlerTests.cs
        ├── UpdateSaleHandlerTests.cs
        ├── GetSaleHandlerTests.cs
        ├── GetSalesHandlerTests.cs
        ├── SaleLifecycleHandlerTests.cs
        └── DeleteSaleHandlerTests.cs
