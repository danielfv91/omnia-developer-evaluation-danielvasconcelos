# Documentação da Arquitetura do Projeto

## 1. Introdução

Este projeto tem como objetivo disponibilizar uma API RESTful completa para gerenciamento de vendas (Sales), com regras de negócio aplicadas, testes unitários, arquitetura sólida e extensível baseada em princípios de Domain-Driven Design (DDD).

---

## 2. Arquitetura baseada em DDD

O projeto segue a organização por camadas, conforme o padrão DDD:

```
src/
├── Domain            → Entidades e interfaces de repositório
├── Application       → Casos de uso (Handlers, Commands, Validators)
├── ORM               → EF Core, DbContext e implementação de repositórios
├── WebApi            → Controllers, Requests, Responses, Middlewares
├── Common/Crosscut   → Exceções, validações e utilitários
└── IoC               → Registro de dependências (DI)
```

### Responsabilidades das camadas:

- **Domain**: núcleo da regra de negócio (entidades como `Sale`, `SaleItem`, interfaces como `ISaleRepository`).
- **Application**: coordena casos de uso via MediatR. Contém comandos (`CreateSaleCommand`, etc), Handlers, Resultados e Validators.
- **ORM (Infraestrutura)**: persistência com EF Core. Inclui `DefaultContext`, `SaleRepository`, e mapeamentos com Fluent API.
- **WebApi**: expõe os endpoints, recebe os dados via `Request` e envia via `Response`. Valida entradas e executa handlers.
- **Common/Crosscutting**: tratamento de exceções (`BusinessException`), validação e publicação de eventos.
- **IoC**: centraliza a injeção de dependência.

---

## 3. Padrões Utilizados

| Padrão                  | Implementação                                      |
|-------------------------|----------------------------------------------------|
| CQRS                    | MediatR Handlers para separação de comandos        |
| Repository Pattern      | `ISaleRepository` / `SaleRepository`               |
| AutoMapper              | Mapeamento entre DTOs e Entidades                  |
| FluentValidation        | Validação de requests com mensagens em inglês      |
| Domain Events           | Eventos como `SaleCreatedEvent`, publicados via log|
| Middleware              | Tratamento global de exceções e validações         |

---

## 4. Atendimento aos Requisitos

Abaixo a relação direta entre os requisitos da avaliação e a implementação no projeto:

| Requisito                                                     | Implementação                                                   |
|---------------------------------------------------------------|-----------------------------------------------------------------|
| CRUD completo de vendas                                       | Endpoints POST, GET, PUT, DELETE em SalesController             |
| SaleNumber, Customer, Branch, Products, Quantities, etc.      | Presentes em todas as requests/responses                        |
| TotalAmount por item e por venda                              | Calculado automaticamente com aplicação de descontos            |
| Regras de desconto (4–9: 10%, 10–20: 20%)                     | Aplicadas nos Handlers de Create e Update                       |
| Proibição de venda > 20 itens                                 | Validação com BusinessException (HTTP 400)                      |
| Cancelled/Not Cancelled                                       | Campo `IsCancelled` disponível nas entidades                    |
| Validação com mensagens em inglês                             | FluentValidation + Forçamento da cultura en-US                  |
| Testes unitários                                              | xUnit + NSubstitute + Bogus em todos os handlers e eventos      |
| Eventos opcionais                                             | SaleCreated, SaleModified, SaleCancelled implementados e logados|

---

## 5. Validação automática de requisições (Melhoria em relação ao Template)

No template original, a validação era feita manualmente dentro dos controllers com:

```csharp
var validator = new SomeRequestValidator();
var validationResult = await validator.ValidateAsync(request, cancellationToken);
```

Para melhorar a escalabilidade e reduzir a responsabilidade dos controllers, este projeto adotou a **validação automática com FluentValidation integrada ao pipeline**.

**O que foi adicionado:**
- Configuração no `Program.cs`:
  ```csharp
  builder.Services.AddFluentValidationAutoValidation();
  builder.Services.AddValidatorsFromAssembly(typeof(ApplicationLayer).Assembly);
  builder.Services.AddValidatorsFromAssemblyContaining<Program>();
  ```
- Todos os validadores são registrados e executados automaticamente.

**Benefício:** As requisições são validadas antes de chegar aos controllers, e os erros são tratados pelo `ValidationExceptionMiddleware`, com respostas consistentes e claras.

---

## 6. Conclusão

O projeto está estruturado para ser limpo, escalável e de fácil manutenção. Os padrões adotados permitem fácil evolução (ex: integração com Message Brokers), reaproveitamento de lógica de negócio e testes automatizados confiáveis.