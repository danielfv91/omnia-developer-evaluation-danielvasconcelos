
# Documentação de Clean Code e Boas Práticas

## Visão Geral

Este documento descreve as decisões e melhorias implementadas no projeto para garantir a aderência aos princípios de Clean Code e boas práticas de arquitetura de software. Cada seção destaca como os critérios foram atendidos de forma objetiva e com justificativas baseadas no código.

## Princípios SOLID

### S - Single Responsibility Principle (SRP)
- Os handlers (`CreateSaleHandler`, `UpdateSaleHandler`) foram refatorados para delegar a lógica de construção de itens para a classe `SaleItemBuilder`, centralizando o comportamento e respeitando o SRP.
- Separação clara entre camadas: controllers apenas coordenam o fluxo, Application contém a lógica de negócio e Domain define regras.

### O - Open/Closed Principle (OCP)
- A lógica de cálculo e construção de itens pode ser estendida por meio da interface `ISaleItemBuilder`, permitindo novas implementações sem alterar os handlers.

### L - Liskov Substitution Principle (LSP)
- Todas as dependências são injetadas via interfaces, e substituições com mocks nos testes garantem aderência ao princípio.

### I - Interface Segregation Principle (ISP)
- Interfaces como `ISaleItemBuilder` e `ISaleItemCalculator` foram definidas com responsabilidade única.

### D - Dependency Inversion Principle (DIP)
- Handlers e serviços dependem de abstrações, não de implementações concretas.

## DRY

- Extração de lógica repetida de cálculo para `SaleItemBuilder`.
- Criação de `SaleTestData` para centralização de dados em testes com Bogus.
- Separação entre validadores da camada WebApi e Application com responsabilidades distintas, evitando duplicação conceitual.

## YAGNI

- Nenhuma funcionalidade foi implementada de forma antecipada.
- Remoção de arquivos não utilizados como `IUserService.cs`.
- Implementações minimalistas e focadas no escopo do desafio.

## Validações

- Validators separados por responsabilidade:
    UpdateSaleRequestValidator (WebApi): valida estrutura da requisição.
    UpdateSaleValidator (Application): valida regras de negócio como limite de 20 itens.
- Validação de ordenação e datas feita no validator GetSalesValidator.
- Middleware ValidationExceptionMiddleware trata e responde corretamente erros de validação com status 400.

## Eventos

- Eventos criados:
    SaleCreatedEvent, SaleModifiedEvent, SaleCancelledEvent, ItemCancelledEvent.
- Publicação centralizada via IEventPublisher.
- Testes garantem a publicação dos eventos corretos em cada cenário.

## Testes

- Testes de unidade utilizam:
    Bogus para dados realistas.
    NSubstitute para mocks.
    FluentAssertions para asserts claros e legíveis.
- SaleTestData centraliza cenários e dados reutilizáveis.
- Cobertura garantida para handlers, eventos e validações.
- Middleware de exceções e validadores serão cobertos na branch de testes.

## Clean Code

- Responsabilidades bem divididas.
- Nomes expressivos e padronizados.
- Separacão clara de camadas (WebApi, Application, Domain, ORM, Common).
- Sem uso de lógica especulativa ou código comentado.
- Utilização de padronizações REST, AutoMapper, MediatR.

## Conformidade Adicional

- Validações de regra de negócio realizadas em `CreateSaleValidator` e `UpdateSaleValidator` (Application).
- Validações de integridade do request feitas em `CreateSaleRequestValidator` e `UpdateSaleRequestValidator` (WebApi).
- Cobertura de testes unitários com FluentAssertions, Bogus, NSubstitute.
- Publicação de eventos testada e refatorada com clareza de responsabilidade.